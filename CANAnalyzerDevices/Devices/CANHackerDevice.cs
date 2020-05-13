/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

            lock (port)
            {
                port.Open();
            }


            OnIsConnectedChanged();
        }

        public void Disconnect()
        {
            lock(port)
                port.Close();

            OnIsConnectedChanged();
        }


        public IEnumerable<IChannel> Channels
        {
            get { return _channels; }
        }
        protected List<IChannel> _channels;

        public int ChannelCount => _channels.Count;

        public bool IsConnected => port.IsOpen;

        public override string ToString()
        {
            return $"CanHacker v2.0 : {port.PortName}";
        }

        public event EventHandler IsConnectedChanged;
        private void OnIsConnectedChanged()
        {
            IsConnectedChanged?.Invoke(this, null);
        }

        private SerialPort port;
    }
}
