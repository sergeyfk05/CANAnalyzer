using System;
using System.Collections.Generic;
using System.Text;
using CANAnalyzer.Models.Databases;
using Microsoft.Data.Sqlite;

namespace CANAnalyzer.Models.DataTypesProviders.SQLiteADOTraceDataTypeProvider
{
    public class UnitOfWorkSQLiteADOTraceDataTypeProvider
    {
        public UnitOfWorkSQLiteADOTraceDataTypeProvider(string TargetFile)
        {
            _connection = new SqliteConnection($"Data Source={TargetFile}");
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            TraceModelRepository = new TraceModelRepository(_connection, ref _transaction);
            CanHeaderModelRepository = new CanHeaderRepository(_connection, ref _transaction);
        }

        public IRepositorySQLiteADOTraceDataTypeProvider<TraceModel> TraceModelRepository { get; private set; }
        public IRepositorySQLiteADOTraceDataTypeProvider<CanHeaderModel> CanHeaderModelRepository { get; private set; }


        public void SaveChanges()
        {
            _transaction.Commit();
            _transaction = _connection.BeginTransaction();
        }

        public void CloseConnection()
        {
            _connection.Close();
        }

        private SqliteConnection _connection;
        private SqliteTransaction _transaction;
    }
}
