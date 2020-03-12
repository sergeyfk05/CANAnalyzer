using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using HamburgerMenu;

namespace CANAnalyzer.Models
{
    public class ContentPageData
    {
        public ContentPageData(NavMenuItemData nd, UserControl p)
        {
            NavData = nd;
            Page = p;
        }

        public NavMenuItemData NavData { get; private set; }
        public UserControl Page { get; private set; }
    }
}
