/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Collections.Generic;

namespace CANAnalyzerDevices.Devices.DeviceCreaters
{
    internal static class DeviceCreatorsBuilder
    {
        internal static List<IDeviceCreator> BuildDeviceCreatorsList()
        {
            var l = new List<IDeviceCreator>();
            l.Add(new CanHackerCreator());
            return l;
        }

    }
}
