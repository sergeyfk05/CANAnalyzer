using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using HamburgerMenu;

namespace CANAnalyzer.Models.ViewData
{
    public class ContentPageData
    {
        public ContentPageData(NavMenuItemData nd, string locKey, string imageKey, Action<ContentPageData> clickAction = null)
        {
            LocalizedKey = locKey;
            NavData = nd;
            ClickAction = clickAction;
            ImageKey = imageKey;

            this.UpdateLocalization();
            this.UpdateTheme();
        }
        public ContentPageData(NavMenuItemData nd, string locKey, string imageKey, UserControl page, Action<ContentPageData> clickAction = null)
            : this(nd, locKey, imageKey, clickAction)
        {
            Page = page;
        }

        public NavMenuItemData NavData { get; private set; }

        public string LocalizedKey { get; private set; }
        public string ImageKey { get; private set; }

        public UserControl Page { get; private set; }

        public Action<ContentPageData> ClickAction { get; private set; }
    }

    public static class ContentPageDataExtensions
    {
        public static void UpdateLocalization(this ContentPageData data)
        {
            data.NavData.Text = (string)Manager<LanguageCultureInfo>.StaticInstance.GetResource(data.LocalizedKey);
        }
        public static void UpdateTheme(this ContentPageData data)
        {
            data.NavData.ImageSource = new Uri(
                new Uri(Assembly.GetExecutingAssembly().Location), 
                (string)Manager<ThemeCultureInfo>.StaticInstance.GetResource(data.ImageKey));
        }
    }
}
