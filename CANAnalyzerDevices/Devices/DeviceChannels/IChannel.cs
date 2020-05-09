/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    /// <summary>
    /// Channel interface for CANAnalyzerDevices.Devices.IDevice
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Open channel. Need first connected to owner. 
        /// If Channel already opened and params isn't equals then channel will be close and reopen.
        /// </summary>
        /// <param name="bitrate">Bitrate CAN bus in kbps.</param>
        /// <param name="isListenOnly">Flag which indicate CAN transiever mode.</param>
        void Open(int bitrate = 500, bool isListenOnly = true);

        /// <summary>
        /// Close channel. Need first connected to owner. 
        /// </summary>
        void Close();

        /// <summary>
        /// Trasmit data to CAN channel.
        /// </summary>
        /// <param name="data">Data to be transmited.</param>
        void Transmit(TransmitData data);


        /// <summary>
        /// Indicates that data has been received through a port represented by the CANAnalyzerDevices.Devices.DeviceChannels.IChannel object.
        /// </summary>
        event ChannelDataReceivedEventHandler ReceivedData;

        /// <summary>
        /// Current CAN bus bitrate.
        /// </summary>
        int Bitrate { get; }

        /// <summary>
        /// Current CAN transiever mode.
        /// </summary>
        bool IsListenOnly { get; }

        /// <summary>
        /// CAN bus status
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Channel owner.
        /// </summary>
        IDevice Owner { get; }

        event EventHandler IsOpenChanged;
    }
}
