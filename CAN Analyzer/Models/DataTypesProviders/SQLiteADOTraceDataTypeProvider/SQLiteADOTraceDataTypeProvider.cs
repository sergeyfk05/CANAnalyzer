using CANAnalyzer.Models.Databases;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.DataTypesProviders.SQLiteADOTraceDataTypeProvider
{
    public class SQLiteADOTraceDataTypeProvider : ITraceDataTypeProvider, IDisposable
    {

        private UnitOfWorkSQLiteADOTraceDataTypeProvider unitOfWork = null;

        public string TargetFile
        {
            get
            {
                return _targetFile;
            }
            set
            {
                if (_targetFile == value)
                    return;

                _targetFile = value;
                OnTargetFileChanged();
            }
        }
        private string _targetFile = "";
        private void OnTargetFileChanged()
        {
            unitOfWork?.CloseConnection();

            unitOfWork = new UnitOfWorkSQLiteADOTraceDataTypeProvider(TargetFile);
        }

        public IQueryable<TraceModel> Traces => unitOfWork.TraceModelRepository.GetAll().AsQueryable();

        public IQueryable<CanHeaderModel> CanHeaders => unitOfWork.CanHeaderModelRepository.GetAll().AsQueryable();

        public string SupportedFiles => "*.traceDB";

        public void Add(TraceModel entity)
        {
            unitOfWork?.TraceModelRepository?.Add(entity);
        }

        public void Add(CanHeaderModel entity)
        {
            unitOfWork?.CanHeaderModelRepository?.Add(entity);
        }

        public void AddRange(IEnumerable<TraceModel> entities)
        {
            foreach (var el in entities)
                Add(el);
        }

        public void AddRange(IEnumerable<CanHeaderModel> entities)
        {
            foreach (var el in entities)
                Add(el);
        }

        public bool CanWorkWithIt(string filePath)
        {
            var splittedPath = filePath.ToLower().Split('.');
            var extension = splittedPath[splittedPath.Length - 1].ToLower();

            return extension == "traceDB1".ToLower();
        }

        public void CloseConnection()
        {
            unitOfWork?.CloseConnection();
        }

        public void Dispose()
        {
            unitOfWork?.CloseConnection();
        }

        public void Remove(TraceModel entity)
        {
            unitOfWork?.TraceModelRepository?.Remove(entity.Id);
        }

        public void Remove(CanHeaderModel entity)
        {
            unitOfWork?.CanHeaderModelRepository?.Remove(entity.Id);
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TraceModel> entities)
        {
            foreach (var el in entities)
                Remove(el);
        }

        public void RemoveRange(IEnumerable<CanHeaderModel> entities)
        {
            foreach (var el in entities)
                Remove(el);
        }

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

        public void SaveChanges()
        {
            unitOfWork?.SaveChanges();
        }
    }
}
