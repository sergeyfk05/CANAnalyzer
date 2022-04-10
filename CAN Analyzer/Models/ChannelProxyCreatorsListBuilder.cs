/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerChannelProxyInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CANAnalyzer.Models
{
    public static class ChannelProxyCreatorsListBuilder
    {
        public static List<IChannelProxyCreator> GenerateTraceDataTypeProviders()
        {
            var proxiesCreators = new List<IChannelProxyCreator>();

            if(File.Exists("Resources/Configs/Proxies.ini"))
            {
                string[] ddls = File.ReadAllLines("Resources/Configs/Proxies.ini");
                foreach(var ddlName in ddls)
                {
                    try
                    {
                        Assembly asm = Assembly.LoadFrom(ddlName);
                        foreach (Type type in asm.GetTypes())
                        {
                            var absraction = type.GetInterface("IChannelProxyCreator");
                            if(absraction != null)
                            {
                                var creator = System.Activator.CreateInstance(type);
                                proxiesCreators.Add((IChannelProxyCreator)creator);
                            }

                        }
                    }
                    catch(Exception e)
                    {

                    }
                }
            }

            return proxiesCreators;
        }
    }
}
