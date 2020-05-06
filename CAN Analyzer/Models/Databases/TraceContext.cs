/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
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
        ///    Constructs a new context instance using the existing connection to connect to
        ///    a database. The connection will be disposed when the context is disposed.
        /// </summary>
        /// <param name="sqliteFile">path to SQLite DB file</param>
        public TraceContext(string sqliteFile)
        {
            _path = sqliteFile;
        }

        private string _path;
        public DbSet<TraceModel> Traces { get; set; }
        public DbSet<CanHeaderModel> CanHeaders { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={_path}");
    }
}
