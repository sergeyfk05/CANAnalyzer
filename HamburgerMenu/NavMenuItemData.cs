/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using HamburgerMenu.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HamburgerMenu
{
    public class NavMenuItemData : INotifyPropertyChanged
    {
        public void AddDropdownItem(NavMenuItemData item)
        {
            if (_dropdownItems == null)
                _dropdownItems = new ObservableCollection<NavMenuItemData>();

            _dropdownItems.Add(item);
        }
        public bool IsDropdownItem { get; set; }

        public ObservableCollection<NavMenuItemData> DropdownItems { get { return _dropdownItems; } }

        private ObservableCollection<NavMenuItemData> _dropdownItems;

        public Uri ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (value == _imageSource)
                    return;

                _imageSource = value;
                OnPropertyChanged();
            }
        }
        private Uri _imageSource;

        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text)
                    return;

                _text = value;
                OnPropertyChanged();
            }
        }
        private string _text;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelected;


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

    }

    public static class NavMenuItemDataExtensions
    {
        public static void ResetIsSelectedFlag(this NavMenuItemData data)
        {
            data.IsSelected = false;

            if (data.DropdownItems == null)
                return;

            foreach (var el in data.DropdownItems)
                el.ResetIsSelectedFlag();
        }
    }
}
