/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
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
