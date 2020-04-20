/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CANAnalyzer.Models.Converters
{
    public class MillisecondsToHumanFriendlyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int ms && targetType == typeof(string))
            {
                if (ms < 0)
                    return null;

                if(ms < 1000)
                {
                    return $"{ms}ms";
                }
                else if(ms < 60000)
                {
                    return $"{(ms / 1000.0).ToString("G4")}s";
                }
                else
                {
                    return $"{(ms / 60000.0).ToString("G4")}min";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && targetType == typeof(int))
            {

                if ((str.Length >= 3) && (str.Substring(str.Length - 3) == "ms"))
                {
                    int res;
                    if (!int.TryParse(str.Substring(0, str.Length - 2), out res))
                    {
                        return null;
                    }
                    return res;
                }
                else if ((str.Length >= 2) && (str.Substring(str.Length - 2) == "s"))
                {
                    int res;
                    if (!int.TryParse(str.Substring(0, str.Length - 1), out res))
                    {
                        return null;
                    }
                    return res * 1000;
                }
                if ((str.Length >= 4) && (str.Substring(str.Length - 4) == "min"))
                {
                    int res;
                    if (!int.TryParse(str.Substring(0, str.Length - 3), out res))
                    {
                        return null;
                    }
                    return res * 60000;
                }

                return null;
            }

            return null;
        }
    }
}
