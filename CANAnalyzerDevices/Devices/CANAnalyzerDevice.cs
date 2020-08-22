/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/

using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Protocols.CanAnalyzer;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace CANAnalyzerDevices.Devices
{
    public class CANAnalyzerDevice : IDevice
    {
        public CANAnalyzerDevice(string portName, bool IsConnectNow = false)
        {
            port = new SerialPort(portName, 512000, Parity.None, 8, StopBits.One);

            _channels = new List<IChannel>();

            byte[] buf = new byte[15];

            lock (port)
            {
                port.Open();
            }

            port.Write(new byte[] { 1 }, 0, 1);

            //timeout readlock (port)
            int size = 0;
            lock (port)
            {
                size = port.SafeRead(50, ref buf);
            }

            //check size and command id
            if ((size != 5) || (buf[0] != 1))
            {
                port.Close();
                throw new Exception("Invalid Device");
            }


            DeviceInfo deviceInfo = (DeviceInfo)buf;

            for (byte i = 0; i < deviceInfo.channels; i++)
            {
                _channels.Add(new CanAnalyzerChannel(this, port, i));
            }

            port.Close();

            if (IsConnectNow)
                Connect();

        }

        public IEnumerable<IChannel> Channels => _channels;
        protected List<IChannel> _channels;

        public int ChannelCount => _channels.Count;

        public bool IsConnected => port.IsOpen;

        FastSmartWeakEvent<EventHandler> _isConnectedChanged = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler IsConnectedChanged
        {
            add { _isConnectedChanged.Add(value); }
            remove { _isConnectedChanged.Remove(value); }
        }
        private void OnIsConnectedChanged()
        {
            _isConnectedChanged.Raise(this, EventArgs.Empty);
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
            lock (port)
                port.Close();

            OnIsConnectedChanged();
        }

        public override string ToString()
        {
            return $"CanAnalyzer : {port.PortName}";
        }

        private SerialPort port;
    }
}
