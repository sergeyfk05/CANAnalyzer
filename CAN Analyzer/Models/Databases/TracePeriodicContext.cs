using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.Databases
{
    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to
    ///     a database. The connection will not be disposed when the context is disposed
    ///     if contextOwnsConnection is false.
    /// </summary>
    class TracePeriodicContext : DbContext
    {
        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to
        ///a database.The connection will not be disposed when the context is disposed
        ///if contextOwnsConnection is false.
        /// </summary>
        /// <param name="connection"> An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection"> If set to true the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
        /// /// </param>
        public TracePeriodicContext(DbConnection connection, bool contextOwnsConnection) : base(connection, contextOwnsConnection)
        {
        }

        /// <summary>
        ///    Constructs a new context instance using the existing connection to connect to
        ///    a database. The connection will be disposed when the context is disposed.
        /// </summary>
        /// <param name="sqliteFile">path to SQLite DB file</param>
        public TracePeriodicContext(string sqliteFile) : base(new SQLiteConnection($"Data Source={sqliteFile}"), true)
        {
        }
        public DbSet<TracePeriodicModel> Traces { get; set; }
    }
}
