/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicResource
{
    public abstract class Manager
    {
        protected Manager() { }
        public abstract object GetResource(string key);

        public abstract Manager InstanceStock { get; }

        public event EventHandler CultureChanged = (sender, e) => { };

        protected void OnCultureChanged()
        {
            CultureChanged(this, EventArgs.Empty);
        }

    }
    public class Manager<T> : Manager where T: BaseCultureInfo
    {
        private Manager() { }

        private static Manager<T> _manager;

        public Manager<T> Instance => StaticInstance;
        public override Manager InstanceStock => Instance;

        public static Manager<T> StaticInstance => _manager ?? (_manager = new Manager<T>());

        public T CurrentCulture
        {
            get
            {
                if (Provider == null)
                    throw new NullReferenceException("Provider is null");

                return Provider.CurrentCulture;
            }
            set
            {
                if (Provider == null)
                    throw new NullReferenceException("Provider is null");

                if (Equals(value, Provider.CurrentCulture))
                    return;

                Provider.CurrentCulture = value;
                OnCultureChanged();
            }
        }

        public IEnumerable<T> Cultures => Provider?.Cultures ?? Enumerable.Empty<T>();

        public BaseProvider<T> Provider
        { get;
            set; }

        public override object GetResource(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "[NULL]";
            var localizedValue = Provider?.GetResource(key);
            return localizedValue ?? $"[{key}]";
        }

    }
}
