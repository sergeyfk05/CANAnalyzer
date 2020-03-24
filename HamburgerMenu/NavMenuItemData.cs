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
