/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Events
{
    public class SerialPortDataRecievedEventArgs : EventArgs
    {
        public SerialPortDataRecievedEventArgs(SerialDataReceivedEventArgs data, byte[] payload)
        {
            this.Data = data;
            this.Payload = payload;
        }
        public SerialDataReceivedEventArgs Data { get; private set; }
        public byte[] Payload { get; private set; }
    }
}
