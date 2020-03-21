using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
