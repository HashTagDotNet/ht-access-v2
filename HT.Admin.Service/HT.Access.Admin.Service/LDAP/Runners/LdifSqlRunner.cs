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
    public class LdifSqlRunner : ILdifRunner
    {
        private readonly IDbConnector _db;
        private readonly ICryptographyService _crypto;


        public LdifSqlRunner(IDbConnector db, ICryptographyService cryptoService)
        {
            _db = db;
            _crypto = cryptoService;
        }

        public async Task Run(List<LdifCommandBase> commands, CancellationToken cancellationToken = default)
        {
            if (commands == null || commands.Count == 0) return;

            using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await using var cn = await _db.RW.OpenConnection(cancellationToken);

            LdifCommandBase activeCommand = null;
            int successfulExecutionCount = 0;
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

                }
                successfulExecutionCount = commands.Count(l => l.ExecuteStatus == LdifStatusCode.Success);

                if (successfulExecutionCount == commands.Count) // only complete transaction when the entire command set completed successfully
                {
                    ts.Complete();
                }
                else
                {
                    ts.Dispose();
                }
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
                if (successfulExecutionCount != commands.Count)
                {
                    foreach (var cmd in commands.Where(c => c.ExecuteStatus == LdifStatusCode.Success)) // change any successful commands to 'BatchRollBack' so we know it completed successfully but batch was rolled back
                    {
                        cmd.ExecuteStatus = LdifStatusCode.BatchRollback;
                        cmd.ExecuteStatusMessage = "Successful completion. Batch rolled back";
                    }
                }
                await LogCommands(commands, cn, cancellationToken).ConfigureAwait(false);
                if (cn != null && cn.State == ConnectionState.Open)
                {
                    await cn.CloseAsync().ConfigureAwait(false);
                    await cn.DisposeAsync().ConfigureAwait(false);
                }
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
                .AddVarchar("ChangeType", ldifCmd.ChangeType.ToString().ToLower())
                .AddVarchar("StatusCode", ldifCmd.ExecuteStatus.ToString())
                .AddNVarchar("Message", ldifCmd.ExecuteStatusMessage);

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
            byte[] dnHash = _crypto.Hash(ldifCommand.Dn.Dn);
            byte[] rdnHash = _crypto.Hash(ldifCommand.Dn.Rdn.Value);
            byte[] parentDnHash = _crypto.Hash(ldifCommand.ParentDn.Dn);

            var objectAllReadyExists = await objectExistsInDirectory(cn,dnHash, cancellationToken)
                .ConfigureAwait(false);
            if (objectAllReadyExists)
            {
                ldifCommand.ExecuteStatus = LdifStatusCode.EntryAlreadyExists;
                return;
            }
            var sql = @"
INSERT INTO Access.Entries (
    EntryUid,
    
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
            cmd.AddVarBinary("DN_Hash",dnHash);

            cmd.AddVarchar("RDN", ldifCommand.Dn.Rdn.ToString());
            cmd.AddBinary("RDN_Hash", rdnHash);
            cmd.AddNVarchar("RDN_Attribute", ldifCommand.Dn.Rdn.Attribute);
            cmd.AddNVarchar("RDN_Value", ldifCommand.Dn.Rdn.Value);

            cmd.AddNullable("Parent_DN_Id", null);
            cmd.AddNVarchar("Parent_DN", ldifCommand.Dn.ParentDn);
            cmd.AddBinary("Parent_DN_Hash", parentDnHash);

            var objectId = await _db.Execute.Scalar<int>(cmd, -1, cancellationToken: cancellationToken).ConfigureAwait(false);

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

            ldifCommand.ExecuteStatus = LdifStatusCode.Success;
        }

        private async Task<bool> objectExistsInDirectory(SqlConnection cn, byte[] objectHash, CancellationToken cancellationToken = default)
        {
            var sql = @"
SELECT TOP(1)
	ObjectId
FROM	
	Access.[Objects] WITH (NOLOCK)
WHERE
	DN_Hash = @ObjectHash

SELECT cast(@@ROWCOUNT as int) 'ObjectsFound'
";
            using var cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.AddBinary("ObjectHash", objectHash);

            var rowCount = await _db.Execute.Scalar<int>(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);

            return rowCount >= 1;
        }
    }
}
