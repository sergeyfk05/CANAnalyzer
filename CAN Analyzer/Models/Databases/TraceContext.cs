/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
    public class TraceContext : DbContext
    {
        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to
        ///a database.The connection will not be disposed when the context is disposed
        ///if contextOwnsConnection is false.
        /// </summary>
        /// <param name="connection"> An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection"> If set to true the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
        /// /// </param>
        public TraceContext(DbConnection connection, bool contextOwnsConnection) : base(connection, contextOwnsConnection)
        {
        }

        /// <summary>
        ///    Constructs a new context instance using the existing connection to connect to
        ///    a database. The connection will be disposed when the context is disposed.
        /// </summary>
        /// <param name="sqliteFile">path to SQLite DB file</param>
        public TraceContext(string sqliteFile) : base(new SQLiteConnection($"Data Source={sqliteFile}"), true)
        {
        }
        public DbSet<TraceModel> Traces { get; set; }
        public DbSet<CanHeaderModel> CanHeaders { get; set; }
    }
}
