﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDataProvidersInterfaces;
using CANAnalyzerSQLiteDataProvider;
using System.Collections.Generic;

namespace CANAnalyzer.Models.DataTypesProvidersBuilders
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
