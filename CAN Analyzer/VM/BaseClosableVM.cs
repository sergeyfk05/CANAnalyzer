/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Windows;

namespace CANAnalyzer.VM
{
    public abstract class BaseClosableVM : BaseVM
    {
        public event EventHandler<EventArgs> Closed { 
            add 
            {
                WeakEventManager<BaseClosableVM, EventArgs>.AddHandler(this, "WeakedClosedEvent", value);
            } 
            remove 
            {
                WeakEventManager<BaseClosableVM, EventArgs>.RemoveHandler(this, "WeakedClosedEvent", value);
            } 
        }
        public event EventHandler WeakedClosedEvent;


        protected void RaiseClosedEvent()
        {
            WeakedClosedEvent?.Invoke(this, null);
        }
    }
}
