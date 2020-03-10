using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CANAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Settings.ImportFromJson("1.json");

            //Settings.Instance.SettingsPath = "1.json";

            //Settings.SaveToJson(Settings.Instance.SettingsPath);

            Manager<ThemeCultureInfo>.StaticInstance.Provider = new XMLThemeChangerProvider(Settings.Instance.ThemesXmlPath, "dark");
            Manager<LanguageCultureInfo>.StaticInstance.Provider = new XMLLanguageChangerProvider(Settings.Instance.LanguagesXmlPath, "EN");

            
        }
    }
}
