using System.Threading;
using System.Threading.Tasks;
using HT.Access.Admin.Service.Schema.Interfaces;
using HT.Access.Admin.Service.Schema.Models;
using HT.Extensions.SqlClient;
using HT.Extensions.SqlClient.Interfaces;
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

        public async Task<AttributeModel> GetAttributeByName(string attributeName, CancellationToken cancellationToken = default)
        {
            const string sql = "Access.Attribute_GetByName";
            using var cmd = _db.RO.SprocCommand(sql)
                .AddNVarchar("@Name", attributeName);

            return await _db.Execute.Single(cmd, dr => new AttributeModel()
            {
                AllowMultipleValues = dr.ReadBoolean("AllowMultipleValues", true),
                IsSystemEntry = dr.ReadBoolean("AllowUserModification", true),
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
                .AddBit("@AllowUserModification", attribute.IsSystemEntry);

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
                .AddBit("@AllowUserModification", attribute.IsSystemEntry);

            await _db.Execute.NonQuery(cmd, cancellationToken: cancellationToken).ConfigureAwait(false);
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
