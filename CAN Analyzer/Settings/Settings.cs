using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace CANAnalyzer
{
    public class Settings
    {
        public Settings()
        {
        }


        public string LanguagesXmlPath { get; set; } = "Languages.xml";
        public string ThemesXmlPath { get; set; } = "Themes.xml";
        public string SettingsPath { get; set; } = "Settings.xml";
        public string ThemeCulture { get; set; } = "dark";
        public string LanguageCulture { get; set; } = "EN";








        private static Settings _settings;
        public static Settings Instance => _settings ?? (_settings = new Settings());

        public static async void ImportFromJson(string path)
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

        public static async void SaveToJson(string path)
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
