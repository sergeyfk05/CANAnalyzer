using CANAnalyzerDevice;
using CANAnalyzerDevices.Devices;
using CANHackerDevice;
using System.Collections.Generic;

namespace CANAnalyzer.Models
{
    internal static class DeviceCreatorsListBuilder
    {
        internal static List<IDeviceCreator> BuildDeviceCreatorsList()
        {
            var l = new List<IDeviceCreator>();
            l.Add(new CanHackerCreator());
            l.Add(new CanAnalyzerCreator());
            //l.Add(new FakeCanHackerCreator());
            return l;
        }

    }
}
