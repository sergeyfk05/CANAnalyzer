using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevices.Devices
{
    public class FakeCanHackerDevice : CANHackerDevice
    {
        public FakeCanHackerDevice(string portName, bool IsConnectNow = false) : base(portName, IsConnectNow)
        {
            _channels.Add(new FakeChannel());
        }
        public override string ToString()
        {
            return "FAKE " + base.ToString();
        }
    }
}
