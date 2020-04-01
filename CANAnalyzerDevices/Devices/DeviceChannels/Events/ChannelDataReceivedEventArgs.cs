/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzerDevices.Devices.DeviceChannels.Events
{

    /// <summary>
    /// Provides data for the CANAnalyzerDevices.Devices.DeviceChannels.IChannel.DataRecieved event.
    /// </summary>
    public class ChannelDataReceivedEventArgs : EventArgs
    {
        public ChannelDataReceivedEventArgs(ReceivedData data)
        {
            this.Data = data;
        }
        public ReceivedData Data { get; private set; }
    }
}
