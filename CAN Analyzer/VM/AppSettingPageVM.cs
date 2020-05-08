/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using CANAnalyzer.Models;
using CANAnalyzerDevices.Finder;
using CANAnalyzerDevices.Devices;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using CANAnalyzer.Models.ViewData;
using CANAnalyzer.Models.ChannelsProxy;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Threading;
using CANAnalyzer.Models.ChannelsProxy.Creators;

namespace CANAnalyzer.VM
{
    public class AppSettingPageVM : BaseVM
    {
        public AppSettingPageVM()
        {
            Languages = Manager<LanguageCultureInfo>.StaticInstance.Cultures;
            Themes = Manager<ThemeCultureInfo>.StaticInstance.Cultures;

            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += OnLanguageCultureChanged;
            Manager<ThemeCultureInfo>.StaticInstance.CultureChanged += OnThemeCultureChanged;
            PropertyChanged += OnLanguageSelectorChanged;
            PropertyChanged += OnThemeSelectorChanged;

            Settings.Instance.PropertyChanged += OnDevicePropertyChanged;
            Settings.Instance.Proxies.CollectionChanged += OnProxiesAddCollectionChanged;
            Settings.Instance.Proxies.CollectionChanged += OnProxiesRemoveCollectionChanged;
            Settings.Instance.Proxies.CollectionChanged += OnProxiesResetCollectionChanged;
            _context = SynchronizationContext.Current;
        }

        private SynchronizationContext _context;

        private void OnProxiesResetCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                return;

            _context.Post((s) => { ProxiesData.Clear(); }, null);
        }
        private void OnProxiesRemoveCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                return;

            foreach (var el in e.OldItems)
            {
                try
                {
                    if (el is IChannelProxy proxy)
                    {
                        _context.Post((s) => 
                        {
                            ProxiesData.Remove(ProxiesData.First(x => x.ChannelProxy == el));
                        }, null);
                    }
                }
                catch
                {
                    return;
                }

            }
        }
        private void OnProxiesAddCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                return;

            foreach(var el in e.NewItems)
            {
                if(el is IChannelProxy proxy)
                {
                    var buf = new ProxyChannelViewData(proxy);
                    buf.DeletedEvent += ProxyRemoved;

                    _context.Post((s) =>
                    {
                        ProxiesData.Add(buf);
                    }, null);
                }
            }
        }
        private void ProxyRemoved(object sender, EventArgs args)
        {
            if(sender is ProxyChannelViewData viewData)
            {
                Settings.Instance.Proxies.Remove(viewData.ChannelProxy);
            }
        }


        private void OnDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Device")
            {
                if (Settings.Instance.Device == null || !Settings.Instance.Device.IsConnected)
                    return;

                var buf = new List<DeviceChannelViewData>();
                foreach (var ch in Settings.Instance.Device.Channels)
                {
                    buf.Add(new DeviceChannelViewData(ch));
                }
                ChannelsData = buf;
            }
        }
        private void OnLanguageSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentLanguage"))
            {
                if (vm.CurrentLanguage == null)
                    return;

                Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture = vm.CurrentLanguage;
            }
        }
        private void OnLanguageCultureChanged(object sender, EventArgs e)
        {
            //уловка, чтобы КомбоБокс заного запросит у Themes и Languages ToString()
            CurrentLanguage = null;
            Languages = null;
            CurrentTheme = null;
            Themes = null;

            Languages = Manager<LanguageCultureInfo>.StaticInstance.Cultures;
            CurrentLanguage = Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture;
            Themes = Manager<ThemeCultureInfo>.StaticInstance.Cultures;
            CurrentTheme = Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture;

        }



        public IEnumerable<LanguageCultureInfo> Languages
        {
            get { return _languages; }
            set
            {
                if (value == _languages)
                    return;

                _languages = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<LanguageCultureInfo> _languages;
        public LanguageCultureInfo CurrentLanguage
        {
            get { return _currentLanguage; }
            set
            {
                if (value == _currentLanguage)
                    return;

                _currentLanguage = value;
                RaisePropertyChanged();
            }
        }
        private LanguageCultureInfo _currentLanguage;




        private void OnThemeSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentTheme"))
            {
                if (vm.CurrentTheme == null)
                    return;

                Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture = vm.CurrentTheme;
            }
        }
        private void OnThemeCultureChanged(object sender, EventArgs e)
        {
            CurrentTheme = Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture;
        }
        public IEnumerable<ThemeCultureInfo> Themes
        {
            get { return _themes; }
            set
            {
                if (value == _themes)
                    return;

                _themes = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<ThemeCultureInfo> _themes;
        public ThemeCultureInfo CurrentTheme
        {
            get { return _themeLanguage; }
            set
            {
                if (value == _themeLanguage)
                    return;

                _themeLanguage = value;
                RaisePropertyChanged();
            }
        }
        private ThemeCultureInfo _themeLanguage;


        public IEnumerable<IDevice> Devices
        {
            get { return _devices; }
            set
            {
                if (value == _devices)
                    return;

                _devices = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<IDevice> _devices;

        public IDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (value == _selectedDevice)
                    return;

                _selectedDevice = value;
                RaisePropertyChanged();
            }
        }
        private IDevice _selectedDevice;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value == _isConnected)
                    return;

                _isConnected = value;
                RaisePropertyChanged();
            }
        }
        private bool _isConnected;


        public IEnumerable<DeviceChannelViewData> ChannelsData
        {
            get { return _channelsData; }
            set
            {
                if (value == _channelsData)
                    return;

                _channelsData = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<DeviceChannelViewData> _channelsData;


        public BindingList<ProxyChannelViewData> ProxiesData
        {
            get { return _proxiesData ?? (_proxiesData = new BindingList<ProxyChannelViewData>()); }
            private set
            {
                if (value == _proxiesData)
                    return;

                _proxiesData = value;
                RaisePropertyChanged();
            }
        }
        private BindingList<ProxyChannelViewData> _proxiesData;

        private RelayCommandAsync _loadedCommand;
        public RelayCommandAsync LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommandAsync(this.LoadedCommand_Execute);

                return _loadedCommand;
            }
        }
        private void LoadedCommand_Execute()
        {
            Languages = Manager<LanguageCultureInfo>.StaticInstance.Cultures;
            CurrentLanguage = Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture;
            Themes = Manager<ThemeCultureInfo>.StaticInstance.Cultures;
            CurrentTheme = Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture;


            UpdateDevicesInfoCommand.Execute();
        }


        private RelayCommandAsync _updateDevicesInfoCommand;
        public RelayCommandAsync UpdateDevicesInfoCommand
        {
            get
            {
                if (_updateDevicesInfoCommand == null)
                    _updateDevicesInfoCommand = new RelayCommandAsync(this.UpdateDeviceInfoCommand_Execute);

                return _updateDevicesInfoCommand;
            }
        }
        private void UpdateDeviceInfoCommand_Execute()
        {
            var newDevices = new List<IDevice>(DevicesFinder.FindAvailableDevices());

            if ((Settings.Instance.Device != null) && (Settings.Instance.Device.IsConnected))
            {
                newDevices.Add(Settings.Instance.Device);

                if (!CompareDevicesList(newDevices, Devices))
                {
                    Devices = newDevices;
                    SelectedDevice = Settings.Instance.Device;
                }
            }
            else
            {
                var buf = SelectedDevice?.ToString();

                if (!CompareDevicesList(newDevices, Devices))
                {
                    Devices = newDevices;

                    if (!string.IsNullOrEmpty(buf))
                    {
                        SelectedDevice = Devices.FirstOrDefault(x => x.ToString() == buf);
                    }
                }
            }

            IsConnected = Settings.Instance.Device == null ? false : Settings.Instance.Device.IsConnected;
        }
        private bool CompareDevicesList(IEnumerable<IDevice> first, IEnumerable<IDevice> second)
        {
            if (first == null || second == null)
                return false;

            if (first.Count() != second.Count())
                return false;

            foreach(var el in first)
            {
                if (second.FirstOrDefault(x => x.ToString() == el.ToString()) == null)
                    return false;
            }

            return true;

        }


        private RelayCommandAsync _deviceConnectCommand;
        public RelayCommandAsync DeviceConnectCommand
        {
            get
            {
                if (_deviceConnectCommand == null)
                    _deviceConnectCommand = new RelayCommandAsync(this.DeviceConnectCommand_Execute);

                return _deviceConnectCommand;
            }
        }
        private void DeviceConnectCommand_Execute()
        {

            if (IsConnected)
            {
                try
                {
                    Settings.Instance.Device?.Disconnect();
                    UpdateDevicesInfoCommand.Execute();
                    IsConnected = false;
                }
                catch
                {
                    IsConnected = false;
                    Settings.Instance.Device = null;
                    MessageBox.Show("Чо-то не то с устройством", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                if (SelectedDevice == null)
                {
                    MessageBox.Show("Ничего не выбрано", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    SelectedDevice.Connect();
                    Settings.Instance.Device = SelectedDevice;
                    IsConnected = true;
                }
                catch
                {
                    IsConnected = false;
                    Settings.Instance.Device = null;
                    MessageBox.Show("Не получилось подключиться", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }

        private RelayCommandAsync _addProxyCommand;
        public RelayCommandAsync AddProxyCommand
        {
            get
            {
                if (_addProxyCommand == null)
                    _addProxyCommand = new RelayCommandAsync(this.AddProxyCommand_Execute);

                return _addProxyCommand;
            }
        }
        private void AddProxyCommand_Execute()
        {
            List<IChannelProxyCreator> creators = ChannelProxyCreatorsListBuilder.GenerateTraceDataTypeProviders();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = GenerateFilterForDialog(creators);
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    foreach(var el in creators)
                    {
                        if(el.IsCanWorkWith(openFileDialog.FileName))
                        {
                            var buf = el.CreateInstanceDefault(openFileDialog.FileName);

                            if (buf == null)
                                throw new ArgumentException("invalid file");

                            Settings.Instance.Proxies.Add(buf);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("не верный файл", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private string GenerateFilterForDialog(IEnumerable<IChannelProxyCreator> source)
        {
            string result = "";

            foreach (var el in source)
            {
                if (result == "")
                    result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#" + el.GetType().ToString() + "_ProxyFileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
                else
                    result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#" + el.GetType().ToString() + "_ProxyFileGroup")}({el.SupportedFiles})|{el.SupportedFiles}";
            }

            if (result == "")
                result += $"{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#AllFiles_FileGroup")} (*.*)|*.* ";
            else
                result += $"|{Manager<LanguageCultureInfo>.StaticInstance.GetResource("#AllFiles_FileGroup")} (*.*)|*.* ";

            return result;
        }
    }
}
