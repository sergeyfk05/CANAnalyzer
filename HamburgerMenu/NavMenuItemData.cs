using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HamburgerMenu
{
    public class NavMenuItemData
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

        public Uri ImageSource { get; set; }

        public string Text { get; set; }

        public bool IsSelected { get; set; }

    }
}
