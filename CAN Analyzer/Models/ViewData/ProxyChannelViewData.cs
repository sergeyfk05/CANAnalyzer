using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Models.ChannelsProxy;

namespace CANAnalyzer.Models.ViewData
{
    public class ProxyChannelViewData : INotifyPropertyChanged
    {
        public ProxyChannelViewData(IChannelProxy chProxy)
        {
            if (chProxy == null)
                throw new ArgumentNullException("chProxy must be not null.");

            IsOpen = false;
            ChannelProxy = chProxy;
            Path = ChannelProxy.Path;
        }
        public IChannelProxy ChannelProxy { get; private set; }



        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (value == _isOpen)
                    return;

                _isOpen = value;
                RaisePropertyChanged();
            }
        }
        private bool _isOpen;

        public string Path { get; private set; }

        public IEnumerable<IChannel> channels => Settings.Instance.Device.Channels;

        public IChannel OwnerChannel
        {
            get { return _ownerChannel; }
            set
            {
                if (_ownerChannel == value)
                    return;

                _ownerChannel = value;
                RaisePropertyChanged();
            }
        }
        private IChannel _ownerChannel;


        private RelayCommandAsync _proxyOpenCommand;
        public RelayCommandAsync ProxyOpenCommand
        {
            get
            {
                if (_proxyOpenCommand == null)
                    _proxyOpenCommand = new RelayCommandAsync(this.ProxyOpenCommand_Execute);

                return _proxyOpenCommand;
            }
        }
        private void ProxyOpenCommand_Execute()
        {
            if(IsOpen)
            {
                ChannelProxy.Close();
            }
            else
            {
                throw new NotImplementedException("добавить смену канала");
                ChannelProxy.Open();
            }
        }


        public override string ToString()
        {
            return ChannelProxy.ToString();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
