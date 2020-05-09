/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Collections.Generic;

namespace CANAnalyzer.Models.DataTypesProviders.Builders
{
    public static class TransmitPeriodicDataTypeProvidedsBuilder
    {
        public static List<ITransmitPeriodicDataTypeProvider> GenerateTransmitPeriodicDataTypeProviders()
        {
            var result = new List<ITransmitPeriodicDataTypeProvider>();
            result.Add(new SQLiteTransmitPeriodicDataTypeProvider());

            return result;
        }
    }
}
