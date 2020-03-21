using System;
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

            channel = new CanHackerChannel(this, port);

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


        public IChannel this[int index]
        {
            get
            {
                if (index >= ChannelCount)
                    throw new ArgumentOutOfRangeException();

                return channel;
            }
        }

        public int ChannelCount { get; private set; } = 1;
        private CanHackerChannel channel;

        public bool IsConnected => port.IsOpen;

        private SerialPort port;
    }
}
