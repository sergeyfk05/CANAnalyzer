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
    public static class TransmitPeriodicDataTypeProvidedsBuilder
    {
        internal static List<ITransmitPeriodicDataTypeProvider> _providers = null;
        public static List<ITransmitPeriodicDataTypeProvider> GenerateTransmitPeriodicDataTypeProviders()
        {
            if (_providers != null)
            {
                return _providers;
            }

            var dataTypeProviders = new List<ITransmitPeriodicDataTypeProvider>();

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
                            var absraction = type.GetInterface("ITransmitPeriodicDataTypeProvider");
                            if (absraction != null)
                            {
                                var provider = System.Activator.CreateInstance(type);
                                dataTypeProviders.Add((ITransmitPeriodicDataTypeProvider)provider);
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
