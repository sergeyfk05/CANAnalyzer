﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

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

        public event MouseButtonEventHandler Click;
        private void RaiseClick(MouseButtonEventArgs e, object sender = null)
        {
            Click?.Invoke(sender, e);
        }

        private Stopwatch watch;

        /// <summary>
        /// max delay for click events in ms
        /// </summary>
        public int MaxDelay { get; private set; }
        
    }
}
