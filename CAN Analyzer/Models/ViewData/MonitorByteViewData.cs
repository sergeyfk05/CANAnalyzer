﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CANAnalyzer.Models.ViewData
{
    public interface ReadOnlyMonitorByteViewData
    {
        bool IsChanged { get; }
        byte Data { get; }
    }
    public class MonitorByteViewData : INotifyPropertyChanged, ReadOnlyMonitorByteViewData
    {

        public bool IsChanged
        {
            get { return _isChanged; }
            private set
            {
                if (value == IsChanged)
                    return;

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


                RaisePropertyChanged();
                IsChanged = true;
            }
        }
        private byte _data;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}