/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.DataTypesProviders
{
    public static class TraceDataTypeProvidersListBuilder
    {
        public static List<ITraceDataTypeProvider> GenerateTraceDataTypeProviders()
        {
            var traceProviders = new List<ITraceDataTypeProvider>();
            traceProviders.Add(new SQLiteTraceDataTypeProvider());

            return traceProviders;
        }

    }
}
