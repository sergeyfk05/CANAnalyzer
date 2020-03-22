using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.ComponentModel;
using System.Runtime.Remoting.Channels;
using IChannel = CANAnalyzerDevices.Devices.DeviceChannels.IChannel;

namespace CANAnalyzerDevices.Devices
{
    public interface IDevice
    {
        void Connect();
        void Disconnect();
        IEnumerable<IChannel> Channels { get; }

        int ChannelCount { get; }

        bool IsConnected { get; }
    }
}
