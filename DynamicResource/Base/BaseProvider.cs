/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicResource
{
    public abstract class BaseProvider<T> where T: BaseCultureInfo
    {
        public abstract object GetResource(string key);
        public abstract IEnumerable<T> Cultures { get; }

        public T CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                if (Equals(value, _currentCulture))
                    return;

                if (!Cultures.Contains(value))
                    throw new ArgumentException("There is no such culture in the list of cultures.");

                _currentCulture = value;
            }
        }
        private T _currentCulture;
    }
}
