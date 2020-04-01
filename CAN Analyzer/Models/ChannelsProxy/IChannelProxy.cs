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
