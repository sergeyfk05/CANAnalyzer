/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Extensions;
using Microsoft.EntityFrameworkCore;

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
        ///    Constructs a new context instance using the existing connection to connect to
        ///    a database. The connection will be disposed when the context is disposed.
        /// </summary>
        /// <param name="sqliteFile">path to SQLite DB file</param>
        public TracePeriodicContext(string sqliteFile)
        {
            _path = sqliteFile;
        }

        private string _path;
        public DbSet<TracePeriodicModel> TransmitModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={_path}");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TracePeriodicModel>().Property(e => e.Payload)
                .HasConversion(
                    v => v.ToByteArray(),
                    v => v.ToObservableCollection());
        }
    }
}
