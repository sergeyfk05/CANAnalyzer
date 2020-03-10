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

            //реализовать выбор культуры из сохранений
            Manager<ThemeCultureInfo>.StaticInstance.Provider = new XMLThemeChangerProvider("Themes.xml", "dark");
            Manager<LanguageCultureInfo>.StaticInstance.Provider = new XMLLanguageChangerProvider("Languages.xml", "EN");
        }
    }
}
