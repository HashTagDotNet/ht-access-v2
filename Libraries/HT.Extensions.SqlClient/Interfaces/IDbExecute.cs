using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient.Interfaces
{
    /// <summary>
    /// Executes a <see cref="SqlCommand"/> on the database using configured retry parameters.
    /// </summary>
    public interface IDbExecute
    {
        /// <summary>
        /// Executes <paramref name="command"/> returning a <see cref="SqlDataReader"/>.  Opens <see cref="SqlCommand.Connection"/> if connection is closed.  Caller is responsible for closing returned connection and disposing of this instance.   Call performs retry using a simple linear backoff strategy.
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific should run before timing out.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SqlDataReader> Reader(SqlCommand command, int? commandTimeOutSecs=null,CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes <paramref name="command"/> returning a list of records.  If <see cref="SqlCommand.Connection" /> is not open, call will auto-open and auto-close connection.  Call performs retry using a simple linear backoff strategy.
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="mapper">Code to translate <see cref="SqlDataReader"/> into <typeparamref name="T"/></param>
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific command should run before timing out.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Type of db record to return</typeparam>
        Task<List<T>> Query<T>(SqlCommand command, Func<SqlDataReader, T> mapper, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes <paramref name="command"/>. If <see cref="SqlCommand.Connection"></see> is not open, call will auto-open and auto-close connection.  Call performs retry using a simple linear backoff strategy.
        /// </summary>
        /// <param name="command">SQL command to execute</param>       
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific command should run before timing out.</param>
        /// <param name="cancellationToken"></param>        
        Task NonQuery(SqlCommand command, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes <param name="command"/> returning first record found or default(T) if not found.  If <see cref="SqlCommand.Connection" /> is not open, call will auto-open and auto-close connection.  Call performs retry using a simple linear backoff strategy.
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="mapper">Code to translate <see cref="SqlDataReader"/> into <typeparamref name="T"/></param>
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific should run before timing out.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Type of db record to return</typeparam>
        Task<T> Single<T>(SqlCommand command, Func<SqlDataReader, T> mapper, int? commandTimeOutSecs = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes scalar <paramref name="command"></paramref> returning first column of first row or <paramref name="nullValue"></paramref> if not found.
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="nullValue">Value to return if not found. (default: default(T))</param>
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific should run before timing out.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Type of return value from scalar call (frequently int)</typeparam>
        Task<T> Scalar<T>(SqlCommand command, T nullValue = default(T), int? commandTimeOutSecs = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes scalar <paramref name="command"></paramref> returning first column of first row or <paramref name="nullValue"></paramref> if not found.
        /// </summary>
        /// <param name="command">SQL command to execute</param>
        /// <param name="identityParameterName">Name of output parameter name (default: 'Identity')</param>
        /// <param name="commandTimeOutSecs">Per-call timeout.  Use when you need to tune <inheritdoc cref="DbConnectorOptions.CommandTimeOutSecs"/> how long or short a specific should run before timing out.</param>
        /// <param name="cancellationToken"></param>
        Task<int> Identity(SqlCommand command, string identityParameterName = "Identity",
            int? commandTimeOutSecs = null,
            CancellationToken cancellationToken = default);
    }
}
