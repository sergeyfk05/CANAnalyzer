/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Models.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Data.Entity;

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

        private void TargetFileChanged()
        {
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
            var extension = splittedPath[splittedPath.Length - 1];

            return extension == "sqlite3" || extension == "db";
        }

        /// <summary>
        /// Traces dataset
        /// </summary>
        public IQueryable<TraceModel> Traces => context?.Traces;

        /// <summary>
        /// CanHeaders dataset
        /// </summary>
        public IQueryable<CanHeaderModel> CanHeaders => context?.CanHeaders;


        /// <summary>
        /// Supported files
        /// </summary>
        public string SupportedFiles => "*.db;*.sqlite3";


        private TraceContext context;

        public ITraceDataTypeProvider SaveAs(string path, IEnumerable<TraceModel> traces, IEnumerable<CanHeaderModel> canHeaders)
        {
            if (File.Exists(path))
                File.Delete(path);

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={path}"))
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

                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }

            var result = new SQLiteTraceDataTypeProvider();
            result.TargetFile = path;

            if(canHeaders != null)
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

            SQLiteConnection.CreateFile(path);

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={path}"))
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

                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                await command.ExecuteNonQueryAsync();
            }

            var result = new SQLiteTraceDataTypeProvider();
            result.TargetFile = path;

            result.AddRange(canHeaders);
            result.AddRange(traces);
            result.SaveChanges();

            return result;
        }

        public void Add(TraceModel entity) => context?.Traces.Add(entity);
        public void Add(CanHeaderModel entity)
        {
            context?.CanHeaders.Add(entity);
        }

        public void Remove(TraceModel entity) => context?.Traces.Remove(entity);
        public void Remove(CanHeaderModel entity) => context?.CanHeaders.Remove(entity);


        public void AddRange(IEnumerable<TraceModel> entities) => context?.Traces.AddRange(entities);
        public void AddRange(IEnumerable<CanHeaderModel> entities) => context?.CanHeaders.AddRange(entities);

        public void RemoveRange(IEnumerable<TraceModel> entities) => context?.Traces.RemoveRange(entities);
        public void RemoveRange(IEnumerable<CanHeaderModel> entities) => context?.CanHeaders.RemoveRange(entities);

        public void Dispose()
        {
            context?.Dispose();
        }

        public async void RemoveAll()
        {
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM CanHeaders");
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM Traces");
        }

        ~SQLiteTraceDataTypeProvider()
        {
            this.Dispose();
        }
    }
}
