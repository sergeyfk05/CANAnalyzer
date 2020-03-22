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
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ChannelDataReceivedEventHandler(object sender, ChannelDataReceivedEventArgs e);
}
