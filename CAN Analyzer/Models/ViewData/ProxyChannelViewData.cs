/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Resources.DynamicResources;
using CANAnalyzerChannelProxyInterfaces;
using DynamicResource;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CANAnalyzer.Models.ViewData
{
    public class ProxyChannelViewData : INotifyPropertyChanged
    {
        public ProxyChannelViewData(IChannelProxy chProxy)
        {
            IsOpen = false;
            ChannelProxy = chProxy ?? throw new ArgumentNullException("chProxy must be not null.");
            Path = ChannelProxy.Path;
            Name = this.ToString();
            PropertyChanged += ProxyChannelViewData_PropertyChanged;
            chProxy.NameChanged += ChProxy_NameChanged;
        }

        private void ChProxy_NameChanged(object sender, EventArgs e)
        {
            Name = ChannelProxy.Name;
        }

        private void ProxyChannelViewData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Name")
            {
                ChannelProxy.Name = Name;
            }
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

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                RaisePropertyChanged();
            }
        }
        private string _name;

        public DeviceChannelViewData OwnerChannel
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
        private DeviceChannelViewData _ownerChannel;


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
                IsOpen = false;
            }
            else
            {
                try
                {
                    ChannelProxy.SetChannel(OwnerChannel != null ? OwnerChannel.Channel : null);
                    ChannelProxy.Open();
                    IsOpen = true;
                }
                catch
                {
                    MessageBox.Show((string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#InvaliseParrentChannelError"), (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("#ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }
        }


        private RelayCommand _removeProxyCommand;
        public RelayCommand RemoveProxyCommand
        {
            get
            {
                if (_removeProxyCommand == null)
                    _removeProxyCommand = new RelayCommand(this.RemoveProxyCommand_Execute);

                return _removeProxyCommand;
            }
        }
        private void RemoveProxyCommand_Execute()
        {
            RaiseDeletedEvent();
        }

        public event EventHandler DeletedEvent;
        private void RaiseDeletedEvent()
        {
            DeletedEvent?.Invoke(this, new EventArgs());
        }

        public override string ToString()
        {
            return ChannelProxy.ToString();
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

        ~ProxyChannelViewData()
        {
            if(ChannelProxy != null)
                ChannelProxy.NameChanged -= ChProxy_NameChanged;
        }
    }
}
