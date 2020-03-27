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
        /// <summary>
        /// Try connect to hardware device.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect hardware device.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Available CAN channels.
        /// </summary>
        IEnumerable<IChannel> Channels { get; }

        /// <summary>
        /// Channels count
        /// </summary>
        int ChannelCount { get; }

        /// <summary>
        /// Hardware device status.
        /// </summary>
        bool IsConnected { get; }

        event EventHandler IsConnectedChanged;
    }
}
