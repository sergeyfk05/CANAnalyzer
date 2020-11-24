/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Resources.DynamicResources;
using CANAnalyzerDevices.Devices.DeviceChannels;
using DynamicResource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using RelayCommands;

namespace CANAnalyzer.Models.ViewData
{
    public class DeviceChannelViewData : INotifyPropertyChanged
    {
        public DeviceChannelViewData(IChannel channel)
        {
            if (channel == null)
                throw new ArgumentException("argument must not be null");

            this.Channel = channel;
            this.Channel.IsOpenChanged += OnChannel_IsOpenChanged;

            if(IsOpen)
            {
                Bitrate = (uint)this.Channel.Bitrate;
                IsListenOnlyViewable = this.Channel.IsListenOnly;
            }
            else
            {
                Bitrate = 500;
                IsListenOnlyViewable = true;
            }

        }


        private void OnChannel_IsOpenChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("IsOpen");
        }

        public uint Bitrate
        {
            get { return _bitrate; }
            set
            {
                if (value == _bitrate)
                    return;

                _bitrate = value;
                RaisePropertyChanged();
            }
        }
        private uint _bitrate;

        public bool IsListenOnlyViewable
        {
            get { return _isListenOnlyViewable; }
            set
            {
                if (value == _isListenOnlyViewable)
                    return;

                _isListenOnlyViewable = value;
                RaisePropertyChanged();
            }
        }
        private bool _isListenOnlyViewable;


        public bool IsOpen => Channel.IsOpen;

        public IChannel Channel { get; private set; }

        public void BitrateTextBoxInputIsAllowed(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Match match = Regex.Match(e.Text, @"^[0-9]$");

            if (sender is TextBox tb)
            {
                if (tb.Text.Length >= 4 || !match.Success)
                {
                    e.Handled = true;
                    return;
                }

                if (Convert.ToInt32(tb.Text + e.Text) > 1000)
                {
                    e.Handled = true;
                    tb.Text = "1000";
                    tb.CaretIndex = tb.Text.Length;
                    return;
                }

            }
            else
            {
                e.Handled = true;
                return;
            }

        }


        private RelayCommandAsync _channelConnectCommand;
        public RelayCommandAsync ChannelConnectCommand
        {
            get
            {
                if (_channelConnectCommand == null)
                    _channelConnectCommand = new RelayCommandAsync(this.ChannelConnectCommand_Execute);

                return _channelConnectCommand;
            }
        }
        private void ChannelConnectCommand_Execute()
        {
            if(IsOpen)
            {
                Channel?.Close();
            }
            else
            {
                Channel?.Open((int)Bitrate, IsListenOnlyViewable);
            }
        }

        public string Name => $"#{this.ToString()}NavMenu";


        public override string ToString()
        {
            return Channel.ToString();
        }

        FastSmartWeakEvent<PropertyChangedEventHandler> _propertyChanged = new FastSmartWeakEvent<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged.Add(value); }
            remove { _propertyChanged.Remove(value); }
        }

        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            _propertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return obj is DeviceChannelViewData data &&
                   Bitrate == data.Bitrate &&
                   IsListenOnlyViewable == data.IsListenOnlyViewable &&
                   IsOpen == data.IsOpen &&
                   EqualityComparer<IChannel>.Default.Equals(Channel, data.Channel) &&
                   EqualityComparer<RelayCommandAsync>.Default.Equals(ChannelConnectCommand, data.ChannelConnectCommand) &&
                   Name == data.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Bitrate, IsListenOnlyViewable, IsOpen, Channel, ChannelConnectCommand, Name);
        }

        ~DeviceChannelViewData()
        {
            if(Channel!=null)
                Channel.IsOpenChanged -= OnChannel_IsOpenChanged;
        }
    }
}
