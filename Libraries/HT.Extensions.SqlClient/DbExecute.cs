using HT.Extensions.SqlClient.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace HT.Extensions.SqlClient
{
    /// <summary>
    /// <inheritdoc cref="IDbExecute"/>
    /// </summary>
    public class DbExecute : IDbExecute
    {
        // https://stackoverflow.com/questions/29664/how-to-catch-sqlserver-timeout-exceptions
        public const int SQL_TIMEOUT_ERROR = -2;
        public const int SQL_GENERALNETWORK_ERROR = 11;
        public const int SQL_DEADLOCK = 1205;

        private readonly IDbConnectionFactory _cnFactory;
        private readonly DbConnectorOptions _options;

        internal DbExecute(DbConnectorOptions options, IDbConnectionFactory connectionFactory)
        {
            _cnFactory = connectionFactory;
            _options = options;
        }

        public async Task<int> Identity(SqlCommand command, string identityParameterName = "Identity",
            int? commandTimeOutSecs = null,
            CancellationToken cancellationToken = default)
        {
            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;
            var isOriginalConnectionOpen = command.Connection.State == ConnectionState.Open;
            if (command.Parameters != null)
            {
                if (!command.Parameters.Contains(identityParameterName))
                {
                    var param = command.Parameters.Add(identityParameterName, SqlDbType.Int);
                    param.Direction = ParameterDirection.Output;
                }
            }
            if (!isOriginalConnectionOpen)
            {
                await _cnFactory.Open(command, cancellationToken).ConfigureAwait(false);
            }
            var currentDelayMs = 0;
            try
            {
                for (int currentAttemptCount = 0; currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT); currentAttemptCount++)
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync(cancellationToken);
                        return (int)command.Parameters[identityParameterName].Value;
                    }
                    catch (SqlException ex) when ((ex.Number == SQL_TIMEOUT_ERROR || ex.Number == SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT))
                    {
                        currentDelayMs = getNextDelayMs(currentDelayMs, currentAttemptCount, _options);
                        await Task.Delay(currentDelayMs, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                if (!isOriginalConnectionOpen) await _cnFactory.Close(command, cancellationToken).ConfigureAwait(false);
            }

            return -1;

        }
        /// <summary>
        /// <inheritdoc cref="IDbExecute.Scalar"/>
        /// </summary>
        public async Task<T> Scalar<T>(SqlCommand command, T nullValue = default(T),int? commandTimeOutSecs=null,CancellationToken cancellationToken=default)
        {
            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;
            var isOriginalConnectionOpen = command.Connection.State == ConnectionState.Open;

            if (!isOriginalConnectionOpen)
            {
                await _cnFactory.Open(command, cancellationToken).ConfigureAwait(false);
            }
            var currentDelayMs = 0;
            try
            {
                for (int currentAttemptCount = 0; currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT); currentAttemptCount++)
                {
                    try
                    {
                        var dbVal = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        if (dbVal == DBNull.Value)
                        {
                            return nullValue;
                        }

                        return (T) dbVal;
                    }
                    catch (SqlException ex) when ((ex.Number == SQL_TIMEOUT_ERROR || ex.Number == SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT))
                    {
                        currentDelayMs = getNextDelayMs(currentDelayMs, currentAttemptCount, _options);
                        await Task.Delay(currentDelayMs, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                if (!isOriginalConnectionOpen) await _cnFactory.Close(command, cancellationToken).ConfigureAwait(false);
            }
            return default(T);
        }
        /// <inheritdoc />
        public async Task<SqlDataReader> Reader(SqlCommand command, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default)
        {
            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;

            if (command.Connection.State == ConnectionState.Closed)
            {
                if (_cnFactory != null) await _cnFactory.Open(command, cancellationToken).ConfigureAwait(false);
            }

            int currentDelayMs = 0;
            for (int currentAttemptCount = 0; currentAttemptCount < (_options.CommandRetryCount??DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT); currentAttemptCount++)
            {

                try
                {
                    return await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (SqlException ex) when ((ex.Number == SQL_TIMEOUT_ERROR || ex.Number == SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT))
                {
                    currentDelayMs = getNextDelayMs(currentDelayMs, currentAttemptCount, _options);
                    await Task.Delay(currentDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }
            return default;
        }

        private int getNextDelayMs(int currentDelayMs, int currentAttemptCount, DbConnectorOptions options)
        {
            double nextDelaySeconds = (double)currentDelayMs *
                                 (options?.CommandRetryIntervalSecs ??
                                  DbConnectorOptions.DEFAULT_COMMAND_RETRYINTERVAL);

            return Convert.ToInt32(nextDelaySeconds) * 1000;

        }

        /// <inheritdoc/>
        public async Task<List<T>> Query<T>(SqlCommand command, Func<SqlDataReader, T> mapper, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default)
        {
            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            var isOriginalConnectionOpen = command.Connection.State == ConnectionState.Open;
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;

            try
            {
                using var dr = await Reader(command, commandTimeOutSecs, cancellationToken).ConfigureAwait(false);

                var retList = new List<T>();
                while (await executeReader(dr,cancellationToken).ConfigureAwait(false))
                {
                    retList.Add(mapper(dr));
                }

                return retList;
            }
            finally
            {
                if (isOriginalConnectionOpen)
                {
                    await _cnFactory.Close(command, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> executeReader(SqlDataReader dr, CancellationToken cancellationToken = default)
        {
            var currentDelayMs = 0;
            for (int currentAttemptCount = 0; currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT); currentAttemptCount++)
            {

                try
                {
                    return await dr.ReadAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (SqlException ex) when ((ex.Number == SQL_TIMEOUT_ERROR || ex.Number == SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT))
                {
                    currentDelayMs = getNextDelayMs(currentDelayMs, currentAttemptCount, _options);
                    await Task.Delay(currentDelayMs, cancellationToken).ConfigureAwait(false);
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public async Task<T> Single<T>(SqlCommand command, Func<SqlDataReader, T> mapper, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default)
        {
            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;
            var isOriginalConnectionOpen = command.Connection.State == ConnectionState.Open;

            try
            {
                using var dr = await Reader(command, commandTimeOutSecs, cancellationToken).ConfigureAwait(false);
                while (await executeReader(dr, cancellationToken).ConfigureAwait(false))
                {
                    return mapper(dr);
                }
                return default(T);
            }
            finally
            {
                if (isOriginalConnectionOpen)
                {
                    await _cnFactory.Close(command, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        /// <inheritdoc />
        public async Task NonQuery(SqlCommand command, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default)
        {

            if (command?.Connection == null) throw new ArgumentException($"{nameof(SqlCommand.Connection)}", "Command or Command.Connection is null");
            command.CommandTimeout = commandTimeOutSecs ?? command.CommandTimeout;
            bool isOriginalStateOpen = command.Connection.State == ConnectionState.Open;
            if (!isOriginalStateOpen)
            {
                await _cnFactory.Open(command, cancellationToken).ConfigureAwait(false);
            }
            var currentDelayMs = 0;
            try
            {
                for (int currentAttemptCount = 0; currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT); currentAttemptCount++)
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        break;
                    }
                    catch (SqlException ex) when ((ex.Number == SQL_TIMEOUT_ERROR || ex.Number == SQL_GENERALNETWORK_ERROR) && currentAttemptCount < (_options.CommandRetryCount ?? DbConnectorOptions.DEFAULT_COMMAND_RETRYCOUNT))
                    {
                        currentDelayMs = getNextDelayMs(currentDelayMs, currentAttemptCount, _options);
                        await Task.Delay(currentDelayMs, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                if (!isOriginalStateOpen) await _cnFactory.Close(command, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
