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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CANAnalyzer
{
    public class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
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


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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

        public static async void SaveToJsonAsync(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                await JsonSerializer.SerializeAsync<Settings>(fs, _settings);
            }
        }
    }
}
