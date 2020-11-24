/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;

namespace CANAnalyzerDataModels
{
    /// <summary>
    /// Constructs a new context instance using the existing connection to connect to
    ///     a database. The connection will not be disposed when the context is disposed
    ///     if contextOwnsConnection is false.
    /// </summary>
    public class TracePeriodicContext : DbContext
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

        public override bool Equals(object obj)
        {
            return obj is TracePeriodicContext context &&
                   base.Equals(obj) &&
                   EqualityComparer<DatabaseFacade>.Default.Equals(Database, context.Database) &&
                   EqualityComparer<ChangeTracker>.Default.Equals(ChangeTracker, context.ChangeTracker) &&
                   EqualityComparer<IModel>.Default.Equals(Model, context.Model) &&
                   EqualityComparer<DbContextId>.Default.Equals(ContextId, context.ContextId) &&
                   _path == context._path &&
                   EqualityComparer<DbSet<TracePeriodicModel>>.Default.Equals(TransmitModels, context.TransmitModels);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Database, ChangeTracker, Model, ContextId, _path, TransmitModels);
        }
    }
}
