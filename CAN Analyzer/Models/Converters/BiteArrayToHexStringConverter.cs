/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.ObjectModel;
using System.Globalization;
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
                string result = "0x";
                foreach(var b in bytes)
                {
                    result += b.ToString("X2");
                    result += " ";
                }

                return result.Trim();
            }


            if (value is ObservableCollection<byte> collection && targetType == typeof(string))
            {
                string result = "0x";
                foreach (var b in collection)
                {
                    result += b.ToString("X2");
                    result += " ";
                }

                return result.Trim();
            }


            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
