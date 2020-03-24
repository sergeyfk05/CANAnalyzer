using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HamburgerMenu
{
    public class NavMenuItemData : INotifyPropertyChanged
    {
        public void AddDropdownItem(NavMenuItemData item)
        {
            if (_dropdownItems == null)
                _dropdownItems = new List<NavMenuItemData>();

            _dropdownItems.Add(item);
        }
        public bool IsDropdownItem { get; set; }

        public IEnumerable<NavMenuItemData> DropdownItems { get { return _dropdownItems; } }

        private List<NavMenuItemData> _dropdownItems;

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


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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
