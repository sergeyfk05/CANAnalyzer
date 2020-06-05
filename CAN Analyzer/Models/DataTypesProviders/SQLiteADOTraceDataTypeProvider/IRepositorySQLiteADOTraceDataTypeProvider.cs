using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzer.Models.DataTypesProviders.SQLiteADOTraceDataTypeProvider
{
    public interface IRepositorySQLiteADOTraceDataTypeProvider<T>
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T item);
        void Update(T item);
        void Remove(int id);
    }
}
