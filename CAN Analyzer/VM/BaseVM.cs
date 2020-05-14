/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CANAnalyzer.VM
{
    public abstract class BaseVM : INotifyPropertyChanged
    {
        public event EventHandler<PropertyChangedEventArgs> WeakPropertyChangedEventArgs
        {
            add
            {
                WeakEventManager<BaseVM, PropertyChangedEventArgs>.AddHandler(this, "PropertyChanged", value);
            }
            remove
            {
                WeakEventManager<BaseVM, PropertyChangedEventArgs>.RemoveHandler(this, "PropertyChanged", value);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
