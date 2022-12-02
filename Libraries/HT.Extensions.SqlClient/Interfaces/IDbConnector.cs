using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace HT.Extensions.SqlClient.Interfaces
{
    /// <summary>
    /// Returns connections or contexts to a database.  Connection might be open or closed depending on connection type or connection reusable state.
    /// </summary>
    public interface IDbConnector
    {
        /// <summary>
        /// Connections to Read-only instance of a database. <inheritdoc cref="IDbIntent"/>
        /// </summary>
        IDbIntent RO { get; set; }
        
        /// <summary>
        /// Connections to Read/Write instance of a database. <inheritdoc cref="IDbIntent"/>
        /// </summary>
        IDbIntent RW { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbExecute"/>
        /// </summary>
        IDbExecute Execute { get; set; }

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.Close(SqlCommand,CancellationToken)"/> (convenience method)
        ///</summary>
        Task CloseAsync(SqlCommand command,CancellationToken cancellationToken=default);
        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.Open(SqlCommand,CancellationToken)"/> (convenience method)
        ///</summary>
        Task<SqlConnection> OpenAsync(SqlCommand command, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.Close(SqlConnection,CancellationToken)"/> (convenience method)
        ///</summary>
        Task CloseAsync(SqlConnection connection, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="IDbConnectionFactory.Open(SqlConnection,CancellationToken)"/> (convenience method)
        ///</summary>
        Task<SqlConnection> OpenAsync(SqlConnection connection, CancellationToken cancellationToken = default);


    }



}

