/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ChannelsProxy.Creators
{
    public static class ChannelProxyCreatorsListBuilder
    {
        public static List<IChannelProxyCreator> GenerateTraceDataTypeProviders()
        {
            var proxiesCreators = new List<IChannelProxyCreator>();
            proxiesCreators.Add(new ConsoleChannelProxyCreator());

            return proxiesCreators;
        }
    }
}
