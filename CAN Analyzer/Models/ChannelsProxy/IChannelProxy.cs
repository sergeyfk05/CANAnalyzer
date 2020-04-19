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
        /// <summary>
        /// target file
        /// </summary>
        string Path { get; }
        /// <summary>
        /// owner channel
        /// </summary>
        IChannel Channel { get; }

        /// <summary>
        /// human friendly name
        /// </summary>
        string Name { get; set; }
        event EventHandler NameChanged;

        /// <summary>
        /// set another owner channel
        /// </summary>
        /// <param name="newCH"></param>
        void SetChannel(IChannel newCH);
    }
}
