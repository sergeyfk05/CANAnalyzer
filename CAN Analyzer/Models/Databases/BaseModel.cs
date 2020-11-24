/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CANAnalyzer.Models.Databases
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        FastSmartWeakEvent<PropertyChangedEventHandler> _propertyChanged = new FastSmartWeakEvent<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged.Add(value); }
            remove { _propertyChanged.Remove(value); }
        }

        public override bool Equals(object obj)
        {
            return obj is BaseModel model &&
                   EqualityComparer<FastSmartWeakEvent<PropertyChangedEventHandler>>.Default.Equals(_propertyChanged, model._propertyChanged);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_propertyChanged);
        }

        protected void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            _propertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }
    }
}
