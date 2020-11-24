/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CANAnalyzer.Models.ViewData
{

    public class MonitorByteViewData : INotifyPropertyChanged, IReadOnlyMonitorByteViewData
    {

        public bool IsChanged
        {
            get { return _isChanged; }
            private set
            {
                if (value == IsChanged)
                    return;

                _isChanged = value;
                RaisePropertyChanged();
            }
        }
        private bool _isChanged = false;

        public byte Data
        {
            get { return _data; }
            set
            {
                if (_data == value)
                {
                    IsChanged = false;
                    return;
                }

                _data = value;
                RaisePropertyChanged();
                IsChanged = true;
            }
        }
        private byte _data = 0;

        FastSmartWeakEvent<PropertyChangedEventHandler> _propertyChanged = new FastSmartWeakEvent<PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged.Add(value); }
            remove { _propertyChanged.Remove(value); }
        }

        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            _propertyChanged.Raise(this, new PropertyChangedEventArgs(prop));
        }

        public override bool Equals(object obj)
        {
            return obj is MonitorByteViewData data &&
                   IsChanged == data.IsChanged &&
                   Data == data.Data;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsChanged, Data);
        }
    }
}
