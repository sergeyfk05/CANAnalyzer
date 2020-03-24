using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;

namespace CANAnalyzer.VM
{
    public class AppSettingPageVM : BaseVM
    {
        public AppSettingPageVM()
        {
            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += Language_CultureChanged;
            PropertyChanged += LanguageSelectorChanged;
        }

        private void LanguageSelectorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if((sender is AppSettingPageVM vm) && (e.PropertyName == "CurrentLanguage"))
            {
                Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture = vm.CurrentLanguage;
            }
        }

        private void Language_CultureChanged(object sender, EventArgs e)
        {
            CurrentLanguage = Manager<LanguageCultureInfo>.StaticInstance.CurrentCulture;
        }

        public IEnumerable<LanguageCultureInfo> Languages => Manager<LanguageCultureInfo>.StaticInstance.Cultures;

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
    }
}
