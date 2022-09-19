using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;

namespace HT.Extensions.SqlClient
{
    /// <summary>
    /// Generate a new, closed, <see cref="SqlConnection"/> based on <see cref="ConnectionString"/>. Caller is responsible for disposing returned connection.
    /// </summary>
    public class DbConnectionSet : IDbConnectionSet
    {
        public SqlConnection Connection => new SqlConnection(ConnectionString);
        public string ConnectionString { get; set; }
    }
}
