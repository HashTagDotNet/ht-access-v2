using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using HT.Access.Admin.Service.Schema.Interfaces;
using HT.Access.Admin.Service.Schema.Models;
using HT.Extensions.SqlClient;
using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace HT.Access.Admin.Service.Schema
{
    internal class SchemaSqlPersister : ISchemaPersister
    {
        private readonly IDbConnector _db;

        public SchemaSqlPersister(IDbConnector db)
        {
            _db = db;
        }

        public async Task<AttributeModel> GetAttributeByName(string attributeName,
            CancellationToken cancellationToken = default)
        {
            const string sql = "Access.Attribute_GetByName";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", attributeName);

            return await _db.Execute.Single(cmd, dr => new AttributeModel()
                {
                    AllowMultipleValues = dr.ReadBoolean("AllowMultipleValues", true),
                    IsSystemEntry = !dr.ReadBoolean("AllowUserModification", true),
                    Description = dr.ReadString("Description"),
                    Name = dr.ReadString("Name"),
                    Obsolete = dr.ReadBoolean("IsObsolete", false),
                    Type = dr.ReadEnum("Type", AttributeType.Unknown)
                }
                , cancellationToken: cancellationToken);
        }

        public async Task InsertAttribute(AttributeModel attribute, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].[Attribute_Insert]";
            using var cmd = _db.RW.SprocCommand(sql)
                .AddNVarchar("ValueTypeName", attribute.Type.ToString())
                .AddBit("@IsObsolete", attribute.Obsolete)
                .AddNVarchar("@Name", attribute.Name)
                .AddNVarchar("@Description", attribute.Description)
                .AddBit("@AllowMultipleValues", attribute.AllowMultipleValues)
                .AddBit("@AllowUserModification", !attribute.IsSystemEntry);

            await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateAttribute(AttributeModel attribute, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].[Attribute_Update]";
            using var cmd = _db.RW.SprocCommand(sql)
                .AddNVarchar("ValueTypeName", attribute.Type.ToString())
                .AddBit("@IsObsolete", attribute.Obsolete)
                .AddNVarchar("@Name", attribute.Name)
                .AddNVarchar("@Description", attribute.Description)
                .AddBit("@AllowMultipleValues", attribute.AllowMultipleValues)
                .AddBit("@AllowUserModification", !attribute.IsSystemEntry);

            await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DoesObjectClassExist(string className, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].[ObjectClass_ExistsByName]";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", className);

            return await _db.Execute
                .Single(cmd, dr => dr.ReadBoolean("ObjectExists", false), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> DoesAttributeExist(string attributeName, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].[Attribute_ExistsByName]";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", attributeName);

            return await _db.Execute
                .Single(cmd, dr => dr.ReadBoolean("AttributeExists", false), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task InsertObjectClass(ObjectClassModel model, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].[ObjectClass_Insert]";

            using var cmd = _db.RW.SprocCommand(sql)
                .AddNVarchar("@Name", model.Name)
                .AddNVarchar("@Description", model.Description)
                .AddBit("@IsObsolete", model.IsObsolete)
                .AddBit("@IsAbstract", model.IsAbstract)
                .AddBit("@IsStructural", model.IsStructural)
                .AddBit("@IsAuxiliary", model.IsAuxiliary)
                .AddNVarchar("@ParentClassName", model.ParentObjectClassName);

            SqlConnection cn = null;
            try
            {
                using TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                cn = await _db.OpenAsync(cmd, cancellationToken).ConfigureAwait(false);
                var objectClassId = await _db.Execute.Scalar<int>(cmd, -1, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                cmd.CommandText = "Access.[ObjectClassAttributes_Insert]";
                if (model.Attributes is { Count: > 0 })
                {
                    foreach (var attr in model.Attributes)
                    {
                        cmd.Parameters.Clear();
                        cmd.AddInteger("@ObjectClassId", objectClassId)
                            .AddNVarchar("@Name", attr.Name)
                            .AddBit("IsRequired", attr.IsRequired);

                        await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken);
                    }
                }

                transactionScope.Complete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (cn is { State: ConnectionState.Open })
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        public async Task<bool> IsAttributeUsed(string attributeName, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].Attribute_IsUsed";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", attributeName);

            return await _db.Execute
                .Single(cmd, dr => dr.ReadNullableInt("UsageFlag", 0) > 0, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteAttributeByName(string attributeName, CancellationToken cancellationToken = default)
        {
            const string sql = "[Access].Attribute_DeleteByName";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", attributeName);

            await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}