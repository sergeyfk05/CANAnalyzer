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

namespace CANAnalyzer.VM
{
    public class AppSettingPageVM : BaseVM
    {
        public AppSettingPageVM()
        {
            Languages = Manager<LanguageCultureInfo>.StaticInstance.Cultures;
            Themes = Manager<ThemeCultureInfo>.StaticInstance.Cultures;

            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += Language_CultureChanged;
            Manager<ThemeCultureInfo>.StaticInstance.CultureChanged += Theme_CultureChanged;
            PropertyChanged += LanguageSelectorChanged;
            PropertyChanged += ThemeSelectorChanged;

            Settings.Instance.PropertyChanged += Device_PropertyChanged;
        }

        private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
        private void LanguageSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentLanguage"))
            {
                if (vm.CurrentLanguage == null)
                    return;

                Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture = vm.CurrentLanguage;
            }
        }
        private void Language_CultureChanged(object sender, EventArgs e)
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        private LanguageCultureInfo _currentLanguage;




        private void ThemeSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentTheme"))
            {
                if (vm.CurrentTheme == null)
                    return;

                Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture = vm.CurrentTheme;
            }
        }
        private void Theme_CultureChanged(object sender, EventArgs e)
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
        private IEnumerable<DeviceChannelViewData> _channelsData;

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
                    IsConnected = false;
                }
                catch (Exception e)
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
                catch (Exception e)
                {
                    IsConnected = false;
                    Settings.Instance.Device = null;
                    MessageBox.Show("Не получилось подключиться", (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource("ErrorMsgBoxTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }
    }
}
