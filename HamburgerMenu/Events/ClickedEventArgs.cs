/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HamburgerMenu.Events
{
    public class ClickedEventArgs : RoutedEventArgs
    {
        public ClickedEventArgs(RoutedEvent routedEvent, NavMenuItemData item) 
            : base(routedEvent)
        {
            ClickedItem = item;
        }

        public NavMenuItemData ClickedItem { get; private set; }
    }
}
