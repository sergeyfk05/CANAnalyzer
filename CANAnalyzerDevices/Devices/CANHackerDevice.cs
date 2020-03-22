﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using System.Runtime.Remoting.Channels;
using IChannel = CANAnalyzerDevices.Devices.DeviceChannels.IChannel;

namespace CANAnalyzerDevices.Devices
{
    public class CANHackerDevice : IDevice
    {
        public CANHackerDevice(string portName, bool IsConnectNow = false)
        {
            port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);

            _channels = new List<IChannel>();
            _channels.Add(new CanHackerChannel(this, port));

            if (IsConnectNow)
                Connect();
                
        }

        public void Connect()
        {
            if (IsConnected)
                return;

            port.Open();
        }

        public void Disconnect()
        {
            if (!IsConnected)
                return;

            port.Close();
        }


        public IEnumerable<IChannel> Channels
        {
            get { return _channels; }
        }
        private List<IChannel> _channels;

        public int ChannelCount => _channels.Count;

        public bool IsConnected => port.IsOpen;


        public override string ToString()
        {
            return $"CanHacker v2.0 : {port.PortName}";
        }

        private SerialPort port;
    }
}
