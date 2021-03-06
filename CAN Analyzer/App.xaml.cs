﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
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

            Settings.ImportFromJson(Settings.Instance.SettingsPath);

            Manager<ThemeCultureInfo>.StaticInstance.Provider = new XMLThemeChangerProvider(Settings.Instance.ThemesXmlPath, Settings.Instance.ThemeCulture);
            Manager<LanguageCultureInfo>.StaticInstance.Provider = new XMLLanguageChangerProvider(Settings.Instance.LanguagesXmlPath, Settings.Instance.LanguageCulture);
            

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Instance.LanguageCulture = Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture.Name;
            Settings.Instance.ThemeCulture = Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture.Name;

            Settings.SaveToJsonAsync(Settings.Instance.SettingsPath);

            Settings.Instance.Dispose();

        }
    }
}
