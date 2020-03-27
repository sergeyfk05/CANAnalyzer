﻿using CANAnalyzerDevices.Devices.DeviceChannels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CANAnalyzer.Models
{
    public class DeviceChannelViewData : INotifyPropertyChanged
    {
        public DeviceChannelViewData(IChannel channel)
        {
            if (channel == null)
                throw new ArgumentException("argument must not be null");

            this.Channel = channel;
            this.Channel.IsOpenChanged += Channel_IsOpenChanged;
            IsOpen = this.Channel.IsOpen;

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
        }

        private void Channel_IsOpenChanged(object sender, EventArgs e)
        {
            IsOpen = this.Channel.IsOpen;
        }

        public string BitrateText
        {
            get { return _bitrateText; }
            set
            {
                if (value == _bitrateText)
                    return;

                _bitrateText = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        private bool _isListenOnlyViewable;

        public bool IsOpen
        {
            get { return _isOpen; }
            private set
            {
                if (value == _isOpen)
                    return;

                _isOpen = value;
                OnPropertyChanged();
            }
        }
        private bool _isOpen;


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

        public override string ToString()
        {
            return Channel.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
