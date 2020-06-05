using CANAnalyzer.Models.Databases;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.DataTypesProviders.SQLiteADOTraceDataTypeProvider
{
    public class CanHeaderRepository : IRepositorySQLiteADOTraceDataTypeProvider<CanHeaderModel>
    {
        public CanHeaderRepository(SqliteConnection conn, ref SqliteTransaction tran)
        {
            if (conn == null)
                throw new ArgumentNullException();

            _connection = conn;
            _transaction = tran;
        }
        private SqliteConnection _connection;
        private SqliteTransaction _transaction;


        public void Add(CanHeaderModel item)
        {
            SqliteCommand command = new SqliteCommand($"INSERT INTO CanHeaders (CanId, IsExtId, DLC, Comment) VALUES({item.CanId},{item.IsExtId}, {item.DLC},{item.Comment});", _connection, _transaction);
            command.ExecuteNonQuery();
        }

        public CanHeaderModel Get(int id)
        {
            SqliteCommand command = new SqliteCommand($"SELECT Id,CanId,IsExtId,DLC,Comment FROM CanHeaders WHERE Id = {id};", _connection, _transaction);
            SqliteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return null;

            reader.Read();



            CanHeaderModel canHeader = new CanHeaderModel()
            {
                Id = reader.GetInt32(0),
                CanId = reader.GetInt32(1),
                IsExtId = reader.GetInt32(2) == 0 ? false : true,
                DLC = reader.GetInt32(3),
                Comment = reader.GetString(4)
            };


            return canHeader;

        }

        public IEnumerable<CanHeaderModel> GetAll()
        {
            List<CanHeaderModel> result = new List<CanHeaderModel>();

            SqliteCommand command = new SqliteCommand("SELECT Id,CanId,IsExtId,DLC,Comment FROM CanHeaders;", _connection, _transaction);
            SqliteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return result;

            while (reader.Read())
            {
                CanHeaderModel canHeader = new CanHeaderModel()
                {
                    Id = reader.GetInt32(0),
                    CanId = reader.GetInt32(1),
                    IsExtId = reader.GetInt32(2) == 0 ? false : true,
                    DLC = reader.GetInt32(3),
                    Comment = reader.GetString(4)
                };
                result.Add(canHeader);
            }

            return result;
        }

        public void Remove(int id)
        {
            SqliteCommand command = new SqliteCommand($"DELETE FROM CanHeaders WHERE Id={id};", _connection, _transaction);
            command.ExecuteNonQuery();
        }

        public void Update(CanHeaderModel item)
        {
            SqliteCommand command = new SqliteCommand($"UPDATE CanHeaders SET CanId={item.CanId},IsExtId={item.IsExtId},DLC={item.DLC},Comment={item.Comment} WHERE Id={item.Id}", _connection, _transaction);
            command.ExecuteNonQuery();
        }
    }
}
