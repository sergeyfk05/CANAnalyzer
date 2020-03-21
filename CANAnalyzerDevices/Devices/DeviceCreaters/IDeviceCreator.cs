using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzerDevices.Finder;

namespace CANAnalyzerDevices.Devices.DeviceCreaters
{
    internal interface IDeviceCreator
    {
        bool IsCanWorkWith(HardwareDeviceInfo info);
    }
}
