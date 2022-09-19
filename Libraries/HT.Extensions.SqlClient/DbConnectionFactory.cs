using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient
{
    /// <summary>
    /// <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _defaultConnectionString;
        private string _constructedConnectionString;
        private readonly DbConnectorOptions _options;
        private readonly bool _isReadOnly;

        internal DbConnectionFactory(DbConnectorOptions options)
        {
            _options = options;
        }

        public DbConnectionFactory(string defaultConnectionString, DbConnectorOptions options, bool isReadOnly = true)
        {

            _defaultConnectionString = defaultConnectionString;
            _options = options;
            _isReadOnly = isReadOnly;
        }
        /// <summary>
        /// <inheritdoc cref="ConnectionString"/>
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_constructedConnectionString))
                    return _constructedConnectionString;

                if (string.IsNullOrWhiteSpace(_defaultConnectionString))
                    throw new ArgumentNullException($"Cannot access property {nameof(ConnectionString)} when connection string is not initialized");

                SqlConnectionStringBuilder builder = new(_defaultConnectionString);

                if (!string.IsNullOrWhiteSpace(_options.ApplicationName))
                {
                    builder.ApplicationName = _options.ApplicationName;
                    if (builder.ApplicationName != null && _isReadOnly)
                    {
                        builder.ApplicationName += " (R/O)";
                        if (builder.ApplicationName.Length > 122)
                        {
                            throw new ArgumentException($"ApplicationName must not be greater than 122 characters. Found {builder.ApplicationName.Length} characters");
                        }
                    }
                }

                builder.MultipleActiveResultSets = true;
                builder.ConnectRetryCount = _options.ConnectRetryCount ?? builder.ConnectRetryCount;
                builder.ConnectRetryInterval = _options.ConnectRetryIntervalSecs ?? builder.ConnectRetryInterval;
                builder.ConnectTimeout = _options.ConnectTimeOutSecs ?? builder.ConnectTimeout;
                builder.MinPoolSize = _options.MinPoolSize ?? builder.MinPoolSize;
                builder.MaxPoolSize = _options.MaxPoolSize ?? builder.MaxPoolSize;
                builder.Pooling = _options.EnablePooling ?? builder.Pooling;
                
                builder.CommandTimeout = _options.CommandTimeOutSecs ?? builder.CommandTimeout;
                _constructedConnectionString = builder.ToString();
                return _constructedConnectionString;
            }
        }


        /// <summary>
        /// <inheritdoc cref="Open(System.Threading.CancellationToken)"/>
        /// </summary>
        public async Task<SqlConnection> Open(CancellationToken cancellationToken = default)
        {
            var cn = NewConnection();
            return await Open(cn, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.NewConnection"/>
        /// </summary>
        public SqlConnection NewConnection()
        {
            return new SqlConnection(ConnectionString);

        }

        /// <summary>
        /// <inheritdoc cref="Open(SqlConnection,CancellationToken)"/>
        /// </summary>
        public async Task<SqlConnection> Open(SqlConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection == null) throw new ArgumentNullException($"{nameof(connection)}", "Connection is null.  Ensure connection is set before calling this method");
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false); // depend on SqlClient connection retry capability
                return connection;

                //for (int currentAttemptCount = 0; currentAttemptCount < _options.MaxConnectionAttempts; currentAttemptCount++)
                //{
                //    try
                //    {
                //        await connection.OpenAsync().ConfigureAwait(false);
                //        return connection;
                //    }
                //    catch (SqlException ex) when ((ex.Number == DbExecute.SQL_TIMEOUT_ERROR || ex.Number == DbExecute.SQL_GENERALNETWORK_ERROR) && currentAttemptCount < _options.MaxConnectionAttempts)
                //    {
                //        await Task.Delay(_options.ConnectPauseMs).ConfigureAwait(false);
                //        continue;
                //    }
                //}
            }
            return connection;
        }

        /// <inheritdoc />
        public async Task<SqlConnection> Open(SqlCommand command, CancellationToken cancellationToken = default)
        {
            return await Open(command?.Connection, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<SqlConnection> Close(SqlConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                for (int currentAttemptCount = 0; currentAttemptCount < _options.ConnectRetryCount; currentAttemptCount++)
                {
                    try
                    {
                        await connection.CloseAsync().ConfigureAwait(false);
                        break;
                    }
                    catch (SqlException ex) when ((ex.Number == DbExecute.SQL_TIMEOUT_ERROR || ex.Number == DbExecute.SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.ConnectRetryCount ?? DbConnectorOptions.DEFAULT_CONNECT_RETRYCOUNT))
                    {
                        await Task.Delay((_options.ConnectRetryIntervalSecs ?? DbConnectorOptions.DEFAULT_CONNECT_RETRYINTERVAL_SECS) * 1000, cancellationToken).ConfigureAwait(false);
                        continue;
                    }
                }
            }
            return connection;
        }

        /// <inheritdoc />
        public async Task<SqlConnection> Close(SqlCommand command, CancellationToken cancellationToken = default)
        {
            return await Close(command?.Connection, cancellationToken).ConfigureAwait(false);
        }
    }
}
