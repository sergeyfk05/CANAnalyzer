/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ChannelsProxy
{
    public interface IChannelProxy : IChannel, IDisposable
    {
        string Path { get; }
        IChannel Channel { get; }

        void SetChannel(IChannel newCH);
    }
}
