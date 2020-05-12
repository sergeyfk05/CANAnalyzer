using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace CANAnalyzer.Models.Converters
{
    public class StringToByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && targetType == typeof(byte))
            {
                byte res;
                if (byte.TryParse(str, out res))
                    return res;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte data && targetType == typeof(string))
            {
                return data.ToString();
            }

            return null;
        }
    }
}
