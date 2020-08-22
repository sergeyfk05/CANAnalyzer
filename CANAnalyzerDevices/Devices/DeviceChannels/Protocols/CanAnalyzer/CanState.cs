using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer
{
    internal enum CanState
    {
        Closed = 0,
        OpenedNormal = 1,
        OpenedListenOnly = 2
    }
}
