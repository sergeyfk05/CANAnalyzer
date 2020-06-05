using CANAnalyzer.Models.Databases;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.DataTypesProviders.SQLiteADOTraceDataTypeProvider
{
    public class TraceModelRepository : IRepositorySQLiteADOTraceDataTypeProvider<TraceModel>
    {
        public TraceModelRepository(SqliteConnection conn, ref SqliteTransaction tran)
        {
            if (conn == null)
                throw new ArgumentNullException();

            _connection = conn;
            _transaction = tran;
        }
        private SqliteConnection _connection;
        private SqliteTransaction _transaction;


        public void Add(TraceModel item)
        {
            SqliteCommand command = new SqliteCommand($"INSERT INTO TraceModels (Time, Payload, CanHeaderId) VALUES({item.Time}, @p,{item.CanHeaderId});", _connection, _transaction);
            command.Parameters.Add("@p", SqliteType.Blob, item.Payload.Length).Value = item.Payload;
            command.ExecuteNonQuery();
        }

        public TraceModel Get(int id)
        {
            SqliteCommand command = new SqliteCommand($"SELECT Traces.Id,Traces.Time,Traces.Payload,CanHeaders.Id,CanHeaders.CanId,CanHeaders.IsExtId,CanHeaders.DLC,CanHeaders.Comment FROM Traces INNER JOIN CanHeaders ON CanHeaders.Id = Traces.CanHeaderId WHERE Traces.Id = {id};", _connection, _transaction);
            SqliteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return null;

            reader.Read();



            CanHeaderModel canHeader = new CanHeaderModel()
            {
                Id = reader.GetInt32(3),
                CanId = reader.GetInt32(4),
                IsExtId = reader.GetInt32(5) == 0 ? false : true,
                DLC = reader.GetInt32(6),
                Comment = reader.GetString(7)
            };

            byte[] payload = new byte[canHeader.DLC];
            reader.GetBytes(2, 0, payload, 0, canHeader.DLC);
            TraceModel trace = new TraceModel()
            {
                Id = reader.GetInt32(0),
                Time = reader.GetDouble(1),
                Payload = payload,
                CanHeaderId = canHeader.Id,
                CanHeader = canHeader
            };

            return trace;

        }

        public IEnumerable<TraceModel> GetAll()
        {
            List<TraceModel> result = new List<TraceModel>();

            SqliteCommand command = new SqliteCommand("SELECT Traces.Id,Traces.Time,Traces.Payload,CanHeaders.Id,CanHeaders.CanId,CanHeaders.IsExtId,CanHeaders.DLC,CanHeaders.Comment FROM Traces INNER JOIN CanHeaders ON CanHeaders.Id = Traces.CanHeaderId;", _connection, _transaction);
            SqliteDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
                return result;

            while (reader.Read())
            {
                CanHeaderModel canHeader = new CanHeaderModel()
                {
                    Id = reader.GetInt32(3),
                    CanId = reader.GetInt32(4),
                    IsExtId = reader.GetInt32(5) == 0 ? false : true,
                    DLC = reader.GetInt32(6),
                    Comment = reader.GetString(7)
                };

                byte[] payload = new byte[canHeader.DLC];
                reader.GetBytes(2, 0, payload, 0, canHeader.DLC);
                TraceModel trace = new TraceModel()
                {
                    Id = reader.GetInt32(0),
                    Time = reader.GetDouble(1),
                    Payload = payload,
                    CanHeaderId = canHeader.Id,
                    CanHeader = canHeader
                };
                result.Add(trace);
            }

            return result;
        }

        public void Remove(int id)
        {
            SqliteCommand command = new SqliteCommand($"DELETE FROM Traces WHERE Id={id};", _connection, _transaction);
            command.ExecuteNonQuery();
        }

        public void Update(TraceModel item)
        {
            SqliteCommand command = new SqliteCommand($"UPDATE Traces SET Time={item.Time}, Payload=@p, CanHeaderId={item.CanHeaderId}  WHERE Id={item.Id}", _connection, _transaction);
            command.Parameters.Add("@p", SqliteType.Blob, item.Payload.Length).Value = item.Payload;
            command.ExecuteNonQuery();
        }
    }
}
