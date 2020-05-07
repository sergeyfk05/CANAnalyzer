/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.ComponentModel;
using CANAnalyzerDevices.Devices;
using System.Runtime.CompilerServices;
using CANAnalyzer.Models.ChannelsProxy;
using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using System.Threading;
using CANAnalyzer.Models.Extensions;

namespace CANAnalyzer
{
    public class Settings : INotifyPropertyChanged, IDisposable
    {
        public Settings()
        {
            PropertyChanged += OnDevice_PropertyChanged;
            _context = SynchronizationContext.Current;
        }

        SynchronizationContext _context;

        private void OnDevice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Device")
                return;

            if(Device != null)
                Device.IsConnectedChanged += Device_IsConnectedChanged;

            _proxies.RemoveAll((x) => true);
        }

        private void Device_IsConnectedChanged(object sender, EventArgs e)
        {
            if(sender is IDevice dev && dev == Device && !Device.IsConnected)
                Proxies.RemoveAll((x) => true);
        }

        public string LanguagesXmlPath
        {
            get { return _languagesXmlPath; }
            set
            {
                if (value == _languagesXmlPath)
                    return;

                _languagesXmlPath = value;
                RaisePropertyChanged();
            }
        }
        private string _languagesXmlPath = @"Resources\DynamicResources\Languages.xml";

        public string ThemesXmlPath
        {
            get { return _themesXmlPath; }
            set
            {
                if (value == _themesXmlPath)
                    return;

                _themesXmlPath = value;
                RaisePropertyChanged();
            }
        }
        private string _themesXmlPath = @"Resources\DynamicResources\Themes.xml";

        public string SettingsPath
        {
            get { return _settingsPath; }
            set
            {
                if (value == _settingsPath)
                    return;

                _settingsPath = value;
                RaisePropertyChanged();
            }
        }
        private string _settingsPath = "Settings.json";

        public string ThemeCulture
        {
            get { return _themeCulture; }
            set
            {
                if (value == _themeCulture)
                    return;

                _themeCulture = value;
                RaisePropertyChanged();
            }
        }
        private string _themeCulture = "dark";

        public string LanguageCulture
        {
            get { return _languageCulture; }
            set
            {
                if (value == _languageCulture)
                    return;

                _languageCulture = value;
                RaisePropertyChanged();
            }
        }
        private string _languageCulture = "EN";


        [JsonIgnore]
        public IDevice Device
        {
            get { return _device; }
            set
            {
                if (value == _device)
                    return;

                _device = value;
                RaisePropertyChanged();
            }
        }
        private IDevice _device = null;

        [JsonIgnore]
        public ObservableCollection<IChannelProxy> Proxies
        {
            get { return _proxies ?? (_proxies = new ObservableCollection<IChannelProxy>()); }
        }
        private ObservableCollection<IChannelProxy> _proxies;


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            _context.Post((s) => 
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }, null);
        }



        private static Settings _settings;
        public static Settings Instance => _settings ?? (_settings = new Settings());

        public static async void ImportFromJsonAsync(string path)
        {
            if (!File.Exists(path))
            {
                _settings = new Settings();
                return;
            }

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                _settings = await JsonSerializer.DeserializeAsync<Settings>(fs);
            }
        }
        public static void ImportFromJson(string path)
        {
            if (!File.Exists(path))
            {
                _settings = new Settings();
                return;
            }

            string fs = File.ReadAllText(path);
            _settings = JsonSerializer.Deserialize<Settings>(fs);
        }

        public static void SaveToJsonAsync(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }


            _ = Task.Run(() => { File.WriteAllText(path, JsonSerializer.Serialize<Settings>(_settings)); });
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            if (Proxies == null)
                return;

            foreach (var el in Proxies)
            {
                try
                {
                    el?.Dispose();
                }
                catch { continue; }
            }

            isDisposed = true;
        }
        private bool isDisposed = false;
    }
}
