using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using CANAnalyzer.Models;

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
        }

        private void LanguageSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentLanguage"))
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
        private LanguageCultureInfo _currentLanguage = Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture;




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
        private ThemeCultureInfo _themeLanguage = Manager<ThemeCultureInfo>.StaticInstance.CurrentCulture;

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
        }
    }
}
