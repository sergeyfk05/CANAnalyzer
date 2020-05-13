using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CANAnalyzerDevices.Devices.DeviceChannels
{
    public class FakeChannel : IChannel
    {
        public int Bitrate => 0;

        public bool IsListenOnly => true;

        public bool IsOpen => false;

        public IDevice Owner => null;

        public event ChannelDataReceivedEventHandler ReceivedData;
        public event EventHandler IsOpenChanged;

        public void Close()
        {
            
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            
        }

        public void Transmit(TransmitData data)
        {
            
        }

        public override string ToString()
        {
            return "Fake channel";
        }
    }
}
