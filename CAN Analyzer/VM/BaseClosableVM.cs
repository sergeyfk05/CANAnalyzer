/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Windows;
using CANAnalyzer.Models;

namespace CANAnalyzer.VM
{
    public abstract class BaseClosableVM : BaseVM
    {
        FastSmartWeakEvent<EventHandler> _closed = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler Closed
        {
            add { _closed.Add(value); }
            remove { _closed.Remove(value); }
        }



        protected void RaiseClosedEvent()
        {
            _closed.Raise(this, EventArgs.Empty);
        }
    }
}
