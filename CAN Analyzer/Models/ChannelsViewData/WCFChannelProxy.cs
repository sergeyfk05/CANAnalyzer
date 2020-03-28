using CANAnalyzerDevices.Devices;
using CANAnalyzerDevices.Devices.DeviceChannels;
using CANAnalyzerDevices.Devices.DeviceChannels.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CANAnalyzer.Models.ChannelsViewData
{
    public class WCFChannelProxy : IChannel
    {
        public WCFChannelProxy(IChannel ch, string path)
        {
            _ch = ch;
        }

        private IChannel _ch;

        public int Bitrate => _ch.Bitrate;

        public bool IsListenOnly
        {
            get { return _isListenOnly; }
            set
            {
                if (value == _isListenOnly)
                    return;

                _isListenOnly = value;
            }
        }
        private bool _isListenOnly = true;

        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (value == _isOpen)
                    return;
                
                _isOpen = value;
                OnIsOpenChanged();
            }
        }
        private bool _isOpen = false;

        public IDevice Owner => _ch.Owner;

        public event ChannelDataReceivedEventHandler ReceivedData;
        public event EventHandler IsOpenChanged;
        private void OnIsOpenChanged()
        {
            IsOpenChanged?.Invoke(this, null);
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open(int bitrate = 500, bool isListenOnly = true)
        {
            throw new NotImplementedException();
        }

        public void Transmit(TransmitData data)
        {
            throw new NotImplementedException();
        }
    }
}
