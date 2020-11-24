/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerChannelProxyInterfaces;
using ConsoleChannelProxy;
using DirtRallyV2ToRenaultDashboardProxy;
using InfluxDBChannelProxy;
using System.Collections.Generic;

namespace CANAnalyzer.Models
{
    public static class ChannelProxyCreatorsListBuilder
    {
        public static List<IChannelProxyCreator> GenerateTraceDataTypeProviders()
        {
            var proxiesCreators = new List<IChannelProxyCreator>();
            proxiesCreators.Add(new ConsoleChannelProxyCreator());
            proxiesCreators.Add(new DirtRallyV2ToRenaultDashboadrProxyCreator());
            proxiesCreators.Add(new InfluxDBChannelProxyCreator());

            return proxiesCreators;
        }
    }
}
