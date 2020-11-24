/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public class SQLiteTraceDataTypeProvider : ITraceDataTypeProvider, IDisposable
    {

        public async void SaveChanges()
        {
            if (!File.Exists(TargetFile))
                throw new ArgumentException("TargetFile does not exist.");

            await context.SaveChangesAsync();
        }

        public string TargetFile
        {
            get { return _targetFile; }
            set
            {
                if (value == _targetFile)
                    return;

                if (!File.Exists(value))
                    throw new ArgumentException("File does not exist.");

                if (!CanWorkWithIt(value))
                    throw new ArgumentException("Invalid file type.");

                _targetFile = value;
                TargetFileChanged();
            }
        }
        private string _targetFile;

        private async void TargetFileChanged()
        {
            using (SqliteConnection dbConnection = new SqliteConnection($"Data Source={TargetFile}"))
            {
                dbConnection.Open();
                string sql = "DELETE FROM CanHeaders WHERE((SELECT count(*) from Traces Where Traces.CanHeaderId = CanHeaders.Id) = 0); ";

                SqliteCommand command = new SqliteCommand(sql, dbConnection);
                await command.ExecuteNonQueryAsync();
            }

            context = new TraceContext(TargetFile);
        }

        public void CloseConnection()
        {
            context?.Dispose();
            context = null;
        }

        /// <summary>
        /// Determines if the provider can work with this type of file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Return true if this provider can work with this type of file</returns>
        public bool CanWorkWithIt(string filePath)
        {
            var splittedPath = filePath.ToLower().Split('.');
            var extension = splittedPath[splittedPath.Length - 1].ToLower();

            return extension == "traceSQLite3".ToLower() || extension == "traceDB".ToLower();
        }

        /// <summary>
        /// Traces dataset
        /// </summary>
        public IQueryable<TraceModel> Traces => context?.Traces.OrderBy(x => x.Time);

        /// <summary>
        /// CanHeaders dataset
        /// </summary>
        public IQueryable<CanHeaderModel> CanHeaders => context?.CanHeaders;


        /// <summary>
        /// Supported files
        /// </summary>
        public string SupportedFiles => "*.traceDB;*.traceSQLite3";


        private TraceContext context;

        public ITraceDataTypeProvider SaveAs(string path, IEnumerable<TraceModel> traces, IEnumerable<CanHeaderModel> canHeaders)
        {
            if (File.Exists(path))
                File.Delete(path);

            //SqliteConnection.CreateFile(path);

            using (SqliteConnection dbConnection = new SqliteConnection($"Data Source={path}"))
            {
                dbConnection.Open();
                string sql = "CREATE TABLE \"CanHeaders\"(" +
                    "\"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                    "\"CanId\" INTEGER NOT NULL," +
                    "\"IsExtId\" INTEGER NOT NULL DEFAULT 0 CHECK(IsExtId == 0 OR IsExtId == 1)," +
                    "\"DLC\" INTEGER NOT NULL DEFAULT 8 CHECK(DLC >= 0 AND DLC <= 8)," +
                    "\"Comment\" TEXT NOT NULL DEFAULT \"\");" +
                    "CREATE TABLE \"Traces\" (" +
                    "\"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                    "\"Time\" REAL NOT NULL," +
                    "\"Payload\" BLOB NOT NULL," +
                    "\"CanHeaderId\" INTEGER NOT NULL," +
                    "FOREIGN KEY(\"CanHeaderId\") REFERENCES \"CanHeaders\"(\"Id\"));";

                SqliteCommand command = new SqliteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }

            var result = new SQLiteTraceDataTypeProvider();
            result.TargetFile = path;

            if (canHeaders != null)
                result.AddRange(canHeaders);

            if (traces != null)
                result.AddRange(traces);
            result.SaveChanges();

            return result;
        }
        public async Task<ITraceDataTypeProvider> SaveAsAsync(string path, IEnumerable<TraceModel> traces, IEnumerable<CanHeaderModel> canHeaders)
        {
            if (File.Exists(path))
                File.Delete(path);

            //SqliteConnection.CreateFile(path);

            using (SqliteConnection dbConnection = new SqliteConnection($"Data Source={path}"))
            {
                dbConnection.Open();
                string sql = "CREATE TABLE \"CanHeaders\"(" +
                    "\"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                    "\"CanId\" INTEGER NOT NULL," +
                    "\"IsExtId\" INTEGER NOT NULL DEFAULT 0 CHECK(IsExtId == 0 OR IsExtId == 1)," +
                    "\"DLC\" INTEGER NOT NULL DEFAULT 8 CHECK(DLC >= 0 AND DLC <= 8)," +
                    "\"Comment\" TEXT NOT NULL DEFAULT \"\");" +
                    "CREATE TABLE \"Traces\" (" +
                    "\"Id\" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                    "\"Time\" REAL NOT NULL," +
                    "\"Payload\" BLOB NOT NULL," +
                    "\"CanHeaderId\" INTEGER NOT NULL," +
                    "FOREIGN KEY(\"CanHeaderId\") REFERENCES \"CanHeaders\"(\"Id\"));";

                SqliteCommand command = new SqliteCommand(sql, dbConnection);
                await command.ExecuteNonQueryAsync();
            }

            var result = new SQLiteTraceDataTypeProvider();
            result.TargetFile = path;

            result.AddRange(canHeaders);
            result.AddRange(traces);
            result.SaveChanges();

            return result;
        }

        public void Add(TraceModel entity)
        {
            lock (context)
            {
                context?.Traces.Add(entity);
            }
        }
        public void Add(CanHeaderModel entity)
        {
            lock (context)
            {
                context?.CanHeaders.Add(entity);
            }
        }

        public void Remove(TraceModel entity)
        {
            lock (context)
            {
                context?.Traces.Remove(entity);
            }
        }
        public void Remove(CanHeaderModel entity)
        {
            lock (context)
            {
                context?.CanHeaders.Remove(entity);
            }
        }


        public void AddRange(IEnumerable<TraceModel> entities)
        {
            lock (context)
            {
                context?.Traces.AddRange(entities);
            }
        }
        public void AddRange(IEnumerable<CanHeaderModel> entities)
        {
            lock (context)
            {
                context?.CanHeaders.AddRange(entities);
            }
        }

        public void RemoveRange(IEnumerable<TraceModel> entities)
        {
            lock (context)
            {
                context?.Traces.RemoveRange(entities);
            }
        }
        public void RemoveRange(IEnumerable<CanHeaderModel> entities)
        {
            lock (context)
            {
                context?.CanHeaders.RemoveRange(entities);
            }
        }

        public void Dispose()
        {
            context?.Dispose();
        }

        public void RemoveAll()
        {
            context?.Traces.RemoveRange(context.Traces);
            context?.CanHeaders.RemoveRange(context.CanHeaders);
            //await context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM CanHeaders");
            //await context.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM Traces");
        }

        public override bool Equals(object obj)
        {
            return obj is SQLiteTraceDataTypeProvider provider &&
                   TargetFile == provider.TargetFile &&
                   _targetFile == provider._targetFile &&
                   EqualityComparer<IQueryable<TraceModel>>.Default.Equals(Traces, provider.Traces) &&
                   EqualityComparer<IQueryable<CanHeaderModel>>.Default.Equals(CanHeaders, provider.CanHeaders) &&
                   SupportedFiles == provider.SupportedFiles &&
                   EqualityComparer<TraceContext>.Default.Equals(context, provider.context);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetFile, _targetFile, Traces, CanHeaders, SupportedFiles, context);
        }

        ~SQLiteTraceDataTypeProvider()
        {
            this.Dispose();
        }
    }
}
