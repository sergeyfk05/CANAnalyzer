using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CANAnalyzerDevices;

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

            Settings.ImportFromJson(Settings.Instance.SettingsPath);
            

            Manager<ThemeCultureInfo>.StaticInstance.Provider = new XMLThemeChangerProvider(Settings.Instance.ThemesXmlPath, Settings.Instance.ThemeCulture);
            Manager<LanguageCultureInfo>.StaticInstance.Provider = new XMLLanguageChangerProvider(Settings.Instance.LanguagesXmlPath, Settings.Instance.LanguageCulture);

            DevicesFinder.find();

            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.SaveToJson(Settings.Instance.SettingsPath);
        }
    }
}
