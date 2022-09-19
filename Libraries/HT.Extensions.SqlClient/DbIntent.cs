using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient
{
    /// <summary>
    /// <inheritdoc cref="IDbIntent"/>
    /// </summary>
    public class DbIntent : IDbIntent
    {
        public DbIntent(string defaultConnectionString, DbConnectorOptions options, bool isReadOnly)
        {
            Connection = new DbConnectionFactory(defaultConnectionString, options, isReadOnly);
            Command = new DbCommandFactory(Connection, options);

        }

        /// <summary>
        /// <inheritdoc cref="IDbIntent.Command"/>
        /// </summary>
        public IDbCommandFactory Command { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbIntent.Command"/>
        /// </summary>
        public IDbConnectionFactory Connection { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbIntent.SprocCommand"/>
        /// </summary>
        public SqlCommand SprocCommand(string procName, int? commandTimeOutSecs = null) =>
            Command?.Sproc(procName, commandTimeOutSecs);

        /// <summary>
        /// <inheritdoc cref="IDbIntent.TextCommand"/>
        /// </summary>
        public SqlCommand TextCommand(string nativeSql, int? commandTimeOutSecs = null) =>
            Command?.Text(nativeSql, commandTimeOutSecs);

        /// <summary>
        /// <inheritdoc cref="IDbIntent.NewConnection"/>
        /// </summary>
        /// <returns></returns>
        public SqlConnection NewConnection() => Connection?.NewConnection();

        /// <summary>
        /// <inheritdoc cref="IDbIntent.OpenConnection"/>
        /// </summary>
        /// <returns></returns>
        public async Task<SqlConnection> OpenConnection(CancellationToken cancellationToken=default) => await Connection.Open(cancellationToken).ConfigureAwait(false);

    }
}
