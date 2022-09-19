using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient
{
    /// <summary>
    /// <inheritdoc cref="IDbConnector"/>
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Simple class")]
    public class DbConnector : IDbConnector
    {
        /// <summary>
        /// Partial connection to support wrapper methods
        /// </summary>
        private readonly IDbConnectionFactory _connection;

        public DbConnector(string defaultConnectionString, DbConnectorOptions options, IDbConnectionFactory connectionFactory)
        {
            RO = new DbIntent(defaultConnectionString, options, true);
            RW = new DbIntent(defaultConnectionString, options, false);
            _connection = connectionFactory;
            Execute = new DbExecute(options, _connection);
        }
        public DbConnector(string defaultConnectionString, DbConnectorOptions options):this(defaultConnectionString,options,new DbConnectionFactory(options))
        {
        
        }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.RO"/>
        /// </summary>
        public IDbIntent RO { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.RW"/>
        /// </summary>
        public IDbIntent RW { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.Execute"/>
        /// </summary>
        public IDbExecute Execute { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.Close(SqlCommand,CancellationToken)"/>
        /// </summary>
        public async Task Close(SqlCommand command,CancellationToken cancellationToken=default)
        {
            await _connection.Close(command,cancellationToken);
        }
        /// <summary>
        /// <inheritdoc cref="IDbConnector.Close(SqlConnection,CancellationToken)"/> (convenience method)
        /// </summary>
        public async Task Close(SqlConnection connection,CancellationToken cancellationToken = default)
        {
            await _connection.Close(connection, cancellationToken);
        }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.Open(SqlCommand,CancellationToken)"/> (convenience method)
        /// </summary>
        public async Task<SqlConnection> Open(SqlCommand command,CancellationToken cancellationToken=default)
        {
            return await _connection.Open(command, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// <inheritdoc cref="IDbConnector.Open(SqlConnection,CancellationToken)"/> (convenience method)
        /// </summary>
        public async Task<SqlConnection> Open(SqlConnection connection,CancellationToken cancellationToken=default)
        {
            return await _connection.Open(connection, cancellationToken).ConfigureAwait(false);
        }
    }


}
