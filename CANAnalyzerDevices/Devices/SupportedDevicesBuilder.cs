using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzerDevices.Devices
{
    internal static class SupportedDevicesBuilder
    {
        internal static List<DeviceBase> GetSupportedDevices()
        {
            var l = new List<DeviceBase>();
            //l.Add(new CANHackerDevice());
            return l;
        }
    }
}
