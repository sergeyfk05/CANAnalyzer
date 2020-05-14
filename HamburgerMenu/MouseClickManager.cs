/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using HamburgerMenu.Events;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace HamburgerMenu
{
    public class MouseClickManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDelay">max delay for click events in ms</param>
        public MouseClickManager(int maxDelay)
        {
            watch = new Stopwatch();
            MaxDelay = maxDelay;
        }

        public MouseClickManager(Func<bool> canExecute) : this(200)
        {
            _canExecute = canExecute;
        }

        public MouseClickManager(int maxDelay, Func<bool> canExecute) : this(maxDelay)
        {
            _canExecute = canExecute;
        }

        private Func<bool> _canExecute;

        /// <summary>
        /// MouseLeftButtonDown handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_canExecute == null || _canExecute())
            {
                watch.Reset();
                watch.Start();
            }
        }

        /// <summary>
        /// MouseLeftButtonIp handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_canExecute == null || _canExecute())
            {
                watch.Stop();
                if (watch.ElapsedMilliseconds <= MaxDelay)
                {
                    RaiseClick(e, sender);
                }
            }
        }

        FastSmartWeakEvent<MouseButtonEventHandler> _click = new FastSmartWeakEvent<MouseButtonEventHandler>();
        public event MouseButtonEventHandler Click
        {
            add { _click.Add(value); }
            remove { _click.Remove(value); }
        }
        private void RaiseClick(MouseButtonEventArgs e, object sender = null)
        {
            _click.Raise(this, e);
        }

        private Stopwatch watch;

        /// <summary>
        /// max delay for click events in ms
        /// </summary>
        public int MaxDelay { get; private set; }
        
    }
}
