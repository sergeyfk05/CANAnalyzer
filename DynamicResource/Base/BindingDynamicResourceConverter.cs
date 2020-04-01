/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DynamicResource
{
    /// <summary>
    /// Конвертер для получения значения выражения привязки в локализации
    /// </summary>
    internal class BindingDynamicResourceConverter : IMultiValueConverter
    {
        public BindingDynamicResourceConverter(Manager manager)
        {
            _manager = manager;
        }
        private Manager _manager;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return null;
            var key = System.Convert.ToString(values[1] ?? "");
            var value = _manager?.InstanceStock.GetResource(key);
            if (value is string)
            {
                var args = (parameter as IEnumerable<object> ?? values.Skip(2)).ToArray();
                if (args.Length == 1 && !(args[0] is string) && args[0] is IEnumerable)
                    args = ((IEnumerable) args[0]).Cast<object>().ToArray();
                if (args.Any())
                    return string.Format(value.ToString(), args);
            }
            return value;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
