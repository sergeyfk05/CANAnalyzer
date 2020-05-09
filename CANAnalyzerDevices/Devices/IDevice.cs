/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
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
