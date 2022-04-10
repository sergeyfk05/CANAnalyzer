/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDataProvidersInterfaces;
using CANAnalyzerSQLiteDataProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CANAnalyzer.Models.DataTypesProvidersBuilders
{
    public static class TraceDataTypeProvidersListBuilder
    {
        internal static List<ITraceDataTypeProvider> _providers = null;
        public static List<ITraceDataTypeProvider> GenerateTraceDataTypeProviders()
        {
            if (_providers != null)
            {
                return _providers;
            }

            var dataTypeProviders = new List<ITraceDataTypeProvider>();

            if (File.Exists("Resources/Configs/DataProviders.ini"))
            {
                string[] ddls = File.ReadAllLines("Resources/Configs/DataProviders.ini");
                foreach (var ddlName in ddls)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(ddlName);
                        foreach (Type type in asm.GetTypes())
                        {
                            var absraction = type.GetInterface("ITraceDataTypeProvider");
                            if (absraction != null)
                            {
                                var creator = System.Activator.CreateInstance(type);
                                dataTypeProviders.Add((ITraceDataTypeProvider)creator);
                            }

                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            _providers = dataTypeProviders;
            return _providers;
        }

    }
}
