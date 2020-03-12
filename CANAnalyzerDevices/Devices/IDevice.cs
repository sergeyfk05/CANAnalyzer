using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace CANAnalyzerDevices.Devices
{
    public interface IDevice
    {
        bool IsSupportWorkWith(ManagementObject obj);
    }
}
