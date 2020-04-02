﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using CANAnalyzer.Models;
using DynamicResource;
using CANAnalyzer.Resources.DynamicResources;

namespace CANAnalyzer.Models.ViewData
{
    public class DeviceChannelViewData : INotifyPropertyChanged
    {
        public DeviceChannelViewData(IChannel channel)
        {
            if (channel == null)
                throw new ArgumentException("argument must not be null");

            this.Channel = channel;
            this.Channel.IsOpenChanged += Channel_IsOpenChanged;

            if(IsOpen)
            {
                BitrateText = this.Channel.Bitrate.ToString();
                IsListenOnlyViewable = this.Channel.IsListenOnly;
            }
            else
            {
                BitrateText = "500";
                IsListenOnlyViewable = true;
            }

            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += Language_CultureChanged;
        }

        private void Language_CultureChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Name");
        }

        private void Channel_IsOpenChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("IsOpen");
        }

        public string BitrateText
        {
            get { return _bitrateText; }
            set
            {
                if (value == _bitrateText)
                    return;

                _bitrateText = value;
                RaisePropertyChanged();
            }
        }
        private string _bitrateText;

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
                Channel?.Open(Convert.ToInt32(BitrateText), IsListenOnlyViewable);
            }
        }

        public string Name => (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource(this.ToString() + "NavMenu");


        public override string ToString()
        {
            return Channel.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
