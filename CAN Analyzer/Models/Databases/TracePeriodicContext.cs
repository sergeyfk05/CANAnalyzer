using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
        public TracePeriodicContext(DbConnection connection, bool contextOwnsConnection)// : base(connection, contextOwnsConnection)
        {
        }

        /// <summary>
        ///    Constructs a new context instance using the existing connection to connect to
        ///    a database. The connection will be disposed when the context is disposed.
        /// </summary>
        /// <param name="sqliteFile">path to SQLite DB file</param>
        public TracePeriodicContext(string sqliteFile)// : base(new SqliteConnection($"Data Source={sqliteFile}"), true)
        {
            _path = sqliteFile;
        }

        private string _path;
        public DbSet<TracePeriodicModel> Traces { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
=> options.UseSqlite($"Data Source={_path}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TracePeriodicModel>()
                .Property(e => e.Payload)
                .HasConversion(
                    v => v.ToArray(),
                    v => new ObservableCollection<byte>(new byte[] { 1,2,3,4,5,6,7,8}));
        }
    }
}
