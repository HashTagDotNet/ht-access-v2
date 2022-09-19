using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using HT.Access.Admin.Service.Cryptography.Interfaces;
using HT.Access.Admin.Service.LDAP.Interfaces;
using HT.Access.Admin.Service.LDAP.Models;
using HT.Extensions.SqlClient;
using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;

namespace HT.Access.Admin.Service.LDAP.Runners
{
    public class LdifSqlRunner:ILdifRunner
    {
        private readonly IDbConnector _db;
        private readonly ICryptographyService _crypto;


        public LdifSqlRunner(IDbConnector db,ICryptographyService cryptoService)
        {
            _db = db;
            _crypto = cryptoService;
        }

        public async Task Run(List<LdifCommandBase> commands,CancellationToken cancellationToken=default)
        {
            if (commands == null || commands.Count == 0) return;

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await using var cn = await _db.RW.OpenConnection(cancellationToken);

            LdifCommandBase activeCommand=null;
            try
            {
                
                foreach (var ldifCommand in commands)
                {
                    activeCommand = ldifCommand;
                    switch (ldifCommand.ChangeType)
                    {
                        case DnChangeType.Add:
                            await runAddCommand(activeCommand, cn, cancellationToken).ConfigureAwait(false);
                            break;
                    }
                    activeCommand.ExecuteStatus = LdifStatusCode.Success;
                }

                ts.Complete();
            }
            catch (Exception ex)
            {
                if (activeCommand != null)
                {
                    activeCommand.ExecuteStatus = LdifStatusCode.Other;
                    activeCommand.ExecuteStatusMessage = ex.Message;
                }
            }
            finally
            {
                await LogCommands(commands,cn,cancellationToken).ConfigureAwait(false);
            }
            
        }

        private async Task LogCommands(List<LdifCommandBase> commands, SqlConnection cn, CancellationToken cancellationToken)
        {
            
            foreach (var cmd in commands)
            {
                await logCommands(cmd, cn, cancellationToken);
            }
        }

        private async Task logCommands(LdifCommandBase ldifCmd, SqlConnection cn, CancellationToken cancellationToken)
        {
            var sql = @"
INSERT INTO Access.ChangeLog (
    DN,
    ChangeType,
    StatusCode,
    Message,
    ChangeDate
) VALUES (
    @Dn,
    @ChangeType,
    @StatusCode,
    @Message,
    getutcdate()
)

SELECT cast(@@Identity as int)
";
            using var sqlCmd = cn.CreateCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sql;
            sqlCmd.AddVarchar("DN", ldifCmd.Dn.ToString())
                .AddVarchar("ChangeType",ldifCmd.ChangeType.ToString().ToLower())
                .AddVarchar("StatusCode",ldifCmd.ExecuteStatus.ToString())
                .AddNVarchar("Message",ldifCmd.ExecuteStatusMessage);

            var logId = await _db.Execute.Scalar<int>(sqlCmd, cancellationToken: cancellationToken).ConfigureAwait(false);
            sql = @"
INSERT INTO Access.ChangeLogDetails (
    ChangeLogId,
    CommandKey,
    CommandValue
) VALUES (
    @ChangeLogId,
    @CommandKey,
    @CommandValue
)
";
            sqlCmd.CommandText = sql;
            foreach (var line in ldifCmd.CommandLines.Skip(1)) //always skip the dn: xxx line since we already have it.
            {
                sqlCmd.Parameters.Clear();
                sqlCmd.AddInteger("ChangeLogId", logId)
                    .AddVarchar("CommandKey", line.Key)
                    .AddNVarchar("CommandValue", line.Value);
                await _db.Execute.NonQuery(sqlCmd, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task runAddCommand(LdifCommandBase ldifCommand, SqlConnection cn, CancellationToken cancellationToken)
        {
            var sql = @"
INSERT INTO Access.Objects (
    ObjectUid,
    
    DN,
    DN_Hash,
    
    RDN,
    RDN_Hash,
    RDN_Attribute,
    RDN_Value,

    Parent_DN_Id,
    Parent_DN,
    Parent_DN_Hash,

    UpdatedOn
) VALUES (
    @ObjectUid,
    
    @DN,
    @DN_Hash,

    @RDN,
    @RDN_Hash,
    @RDN_Attribute,
    @RDN_Value,

    @Parent_DN_Id,
    @Parent_DN,
    @Parent_DN_Hash,

    getutcdate()
)

SELECT CAST(@@identity AS INT)
";
            using var cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            cmd.AddVarchar("ObjectUid", Guid.NewGuid().ToString().Replace("-", ""));
            cmd.AddNVarchar("DN", ldifCommand.Dn.ToString());
            cmd.AddVarBinary("DN_Hash", ldifCommand.DnHash);

            cmd.AddVarchar("RDN", ldifCommand.Dn.Rdn.ToString());
            cmd.AddBinary("RDN_Hash", ldifCommand.RdnHash);
            cmd.AddNVarchar("RDN_Attribute", ldifCommand.Dn.Rdn.Attribute);
            cmd.AddNVarchar("RDN_Value", ldifCommand.Dn.Rdn.Value);

            cmd.AddNullable("Parent_DN_Id", null);
            cmd.AddNVarchar("Parent_DN", ldifCommand.Dn.ParentDn);
            cmd.AddBinary("Parent_DN_Hash", ldifCommand.ParentDnHash);

            var objectId =await _db.Execute.Scalar<int>(cmd, -1, cancellationToken: cancellationToken).ConfigureAwait(false);

            sql = @"
INSERT INTO Access.ObjectAttributes (
    ObjectId,
    [Attribute],
    [Value],
    [Value_Hash]
) VALUES (
    @ObjectId,
    @Name,
    @Value,
    @ValueHash
)
";
            cmd.CommandText = sql;
            foreach (var line in ldifCommand.CommandLines.Skip(2))
            {
                cmd.Parameters.Clear();
                cmd.AddInteger("ObjectId", objectId);
                cmd.AddNVarchar("Name", line.Key.ToLowerInvariant());
                cmd.AddNVarchar("Value", line.Value);
                cmd.AddVarBinary("ValueHash", _crypto.Hash(line.Value.ToLower()));

                await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
