using CANAnalyzer.Models.Databases;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public class SQLiteTransmitPeriodicDataTypeProvider : ITransmitPeriodicDataTypeProvider, IDisposable
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
            context = new TracePeriodicContext(TargetFile);
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

            return extension == "tracePeriodicDB".ToLower() || extension == "tracePeriodicSQLite3".ToLower();
        }

        /// <summary>
        /// Traces dataset
        /// </summary>
        public IQueryable<TracePeriodicModel> Traces => context?.Traces;



        /// <summary>
        /// Supported files
        /// </summary>
        public string SupportedFiles => "*.tracePeriodicDB;*.tracePeriodicSQLite3";


        private TracePeriodicContext context;

        public ITransmitPeriodicDataTypeProvider SaveAs(string path, IEnumerable<TracePeriodicModel> traces)
        {
            if (File.Exists(path))
                File.Delete(path);

            SQLiteConnection.CreateFile(path);

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={path}"))
            {
                dbConnection.Open();
                string sql = "CREATE TABLE \"CanHeaders\"(" +
                    "\"CanId\" INTEGER NOT NULL," +
                    "\"IsExtId\" INTEGER NOT NULL DEFAULT 0 CHECK(IsExtId == 0 OR IsExtId == 1)," +
                    "\"DLC\" INTEGER NOT NULL DEFAULT 8 CHECK(DLC >= 0 AND DLC <= 8)," +
                    "\"Payload\" BLOB NOT NULL," +
                    "\"Period\" INTEGER NOT NULL" +
                    "\"Comment\" TEXT NOT NULL DEFAULT \"\");";

                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
            }

            var result = new SQLiteTransmitPeriodicDataTypeProvider();
            result.TargetFile = path;

            if (traces != null)
                result.AddRange(traces);
            result.SaveChanges();

            return result;
        }
        public async Task<ITransmitPeriodicDataTypeProvider> SaveAsAsync(string path, IEnumerable<TracePeriodicModel> traces)
        {
            if (File.Exists(path))
                File.Delete(path);

            SQLiteConnection.CreateFile(path);

            using (SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={path}"))
            {
                dbConnection.Open();
                string sql = "CREATE TABLE \"CanHeaders\"(" +
                    "\"CanId\" INTEGER NOT NULL," +
                    "\"IsExtId\" INTEGER NOT NULL DEFAULT 0 CHECK(IsExtId == 0 OR IsExtId == 1)," +
                    "\"DLC\" INTEGER NOT NULL DEFAULT 8 CHECK(DLC >= 0 AND DLC <= 8)," +
                    "\"Payload\" BLOB NOT NULL," +
                    "\"Period\" INTEGER NOT NULL" +
                    "\"Comment\" TEXT NOT NULL DEFAULT \"\");";

                SQLiteCommand command = new SQLiteCommand(sql, dbConnection);
                await command.ExecuteNonQueryAsync();
            }

            var result = new SQLiteTransmitPeriodicDataTypeProvider();
            result.TargetFile = path;

            result.AddRange(traces);
            result.SaveChanges();

            return result;
        }

        public void Add(TracePeriodicModel entity)
        {
            lock (context)
            {
                context?.Traces.Add(entity);
            }
        }
        public void AddRange(IEnumerable<TracePeriodicModel> entities)
        {
            lock (context)
            {
                context?.Traces.AddRange(entities);
            }
        }

        public void Remove(TracePeriodicModel entity)
        {
            lock (context)
            {
                context?.Traces.Remove(entity);
            }
        }
        public void RemoveRange(IEnumerable<TracePeriodicModel> entities)
        {
            lock (context)
            {
                context?.Traces.RemoveRange(entities);
            }
        }


        public void Dispose()
        {
            context?.Dispose();
        }

        public async void RemoveAll()
        {
            await context.Database.ExecuteSqlCommandAsync("DELETE FROM Traces");
        }

        ~SQLiteTransmitPeriodicDataTypeProvider()
        {
            this.Dispose();
        }
    }
}
