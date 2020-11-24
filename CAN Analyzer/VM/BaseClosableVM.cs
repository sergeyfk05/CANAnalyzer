/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Windows;
using CANAnalyzer.Models;

namespace CANAnalyzer.VM
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public abstract class BaseClosableVM : BaseVM
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        FastSmartWeakEvent<EventHandler> _closed = new FastSmartWeakEvent<EventHandler>();
        public event EventHandler Closed
        {
            add { _closed.Add(value); }
            remove { _closed.Remove(value); }
        }

        public override bool Equals(object obj)
        {
            return obj is BaseClosableVM vM &&
                   EqualityComparer<FastSmartWeakEvent<EventHandler>>.Default.Equals(_closed, vM._closed);
        }

        protected void RaiseClosedEvent()
        {
            _closed.Raise(this, EventArgs.Empty);
        }
    }
}
