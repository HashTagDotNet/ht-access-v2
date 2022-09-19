﻿using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient.Interfaces
{
    /// <summary>
    /// Generate Read/Write or Read/Only connections to the base
    /// </summary>
    public interface IDbIntent
    {
        /// <summary>
        /// <inheritdoc cref="IDbCommandFactory"/>
        /// </summary>
        IDbCommandFactory Command { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory"/>
        /// </summary>
        IDbConnectionFactory Connection { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbCommandFactory.Sproc"/> (Convenience method)
        /// </summary>
        SqlCommand SprocCommand(string procName, int? commandTimeOutSecs = null) =>
            Command?.Sproc(procName, commandTimeOutSecs);

        /// <summary>
        /// <inheritdoc cref="IDbCommandFactory.Text"/> (Convenience method)
        /// </summary>
        SqlCommand TextCommand(string nativeSql, int? commandTimeOutSecs = null) =>
            Command?.Text(nativeSql, commandTimeOutSecs);

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.NewConnection"/> (Convenience method)
        /// </summary>
        public SqlConnection NewConnection();

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.Open(CancellationToken)"/>
        /// </summary>
        Task<SqlConnection> OpenConnection(CancellationToken cancellationToken = default);
    }
}
