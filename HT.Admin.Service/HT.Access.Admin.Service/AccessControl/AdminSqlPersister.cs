using System;
using System.Data;
using System.Threading.Tasks;
using HT.Access.Admin.Service.AccessControl.Interfaces;
using HT.Access.Admin.Service.AccessControl.Models;
using HT.Access.Admin.Service.Cryptography.Interfaces;
using HT.Extensions.SqlClient;
using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;

namespace HT.Access.Admin.Service.AccessControl
{
    public class AdminSqlPersister:IAdminPersister
    {
        private readonly IDbConnector _db;
        private readonly ICryptographyService _cryptoUtils;

        public AdminSqlPersister(IDbConnector db,ICryptographyService cryptoUtils)
        {
            _db = db;
            _cryptoUtils = cryptoUtils;
        }
        public async Task AddDomain(Domain domain)
        {
            string domainKey = $"do={domain.Code}";

            string sql = "Access.Entries_Add";
            using SqlCommand cmd = _db.RW.SprocCommand(sql)
                .AddNVarchar("@DN", domainKey)
                .AddVarBinary("@DN_Hash", _cryptoUtils.Hash(domainKey))
                .AddBigInt("EntryId", 0, ParameterDirection.Output);

            await _db.Execute.NonQuery(cmd).ConfigureAwait(false);

            var id = cmd.Parameters["@EntryId"].Value;

        }


    }
}
