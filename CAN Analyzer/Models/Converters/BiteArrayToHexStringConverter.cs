using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CANAnalyzer.Models.Converters
{
    public class BiteArrayToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is byte[] bytes && targetType == typeof(string))
            {
                string result = "";
                foreach(var b in bytes)
                {
                    result += b.ToString("X2");
                    result += " ";
                }

                return result.Trim();
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
