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

    public enum ClickedType
    {
        Item,
        Data
    }

    public class HamburgerMenuClickedEventArgs : RoutedEventArgs
    {
        public HamburgerMenuClickedEventArgs(RoutedEvent routedEvent, ClickedType type, NewNavMenuItem item, NavMenuItemData data)
            : base(routedEvent)
        {
            ClickedItem = item;
            ClickedData = data;
            Type = type;
        }
        
        public ClickedType Type { get; private set; }
        
        public NewNavMenuItem ClickedItem { get; private set; }
        public NavMenuItemData ClickedData { get; private set; }
    }
}
