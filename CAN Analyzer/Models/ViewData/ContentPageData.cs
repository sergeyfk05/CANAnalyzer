/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzer.Resources.DynamicResources;
using DynamicResource;
using HamburgerMenu;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CANAnalyzer.Models.ViewData
{

    public class ContentPageData
    {
        private ContentPageData()
        {
            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += OnLanguageCultureChanged;
            Manager<ThemeCultureInfo>.StaticInstance.CultureChanged += OnThemeCultureChanged;
        }

        private void OnThemeCultureChanged(object sender, EventArgs e)
        {
            this.UpdateTheme();
        }

        private void OnLanguageCultureChanged(object sender, EventArgs e)
        {
            this.UpdateLocalization();
        }

        public override bool Equals(object obj)
        {
            return obj is ContentPageData data &&
                   EqualityComparer<NavMenuItemData>.Default.Equals(NavData, data.NavData) &&
                   Kind == data.Kind &&
                   LocalizedKey == data.LocalizedKey &&
                   ImageKey == data.ImageKey &&
                   EqualityComparer<UserControl>.Default.Equals(Page, data.Page);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NavData, Kind, LocalizedKey, ImageKey, Page);
        }

        public ContentPageData(NavMenuItemData nd, string locKey, string imageKey, PageKind kind = PageKind.Undefined, Action<ContentPageData> clickAction = null) : this()
        {
            LocalizedKey = locKey;
            NavData = nd;
            ClickAction = clickAction;
            ImageKey = imageKey;
            Kind = kind;

            this.UpdateLocalization();
            this.UpdateTheme();
        }
        public ContentPageData(NavMenuItemData nd, string locKey, string imageKey, PageKind kind, UserControl page, Action<ContentPageData> clickAction = null)
            : this(nd, locKey, imageKey, kind, clickAction)
        {
            Page = page;
        }

        public NavMenuItemData NavData { get; private set; }

        public PageKind Kind { get; private set; }

        public string LocalizedKey
        {
            get { return _localizedKey; }
            protected set
            {
                if (value == _localizedKey)
                    return;

                _localizedKey = value;
                this.UpdateLocalization();
            }
        }
        private string _localizedKey = "";

        public string ImageKey
        {
            get { return _imageKey; }
            protected set
            {
                if (value == _imageKey)
                    return;

                _imageKey = value;
                this.UpdateTheme();
            }
        }
        private string _imageKey = "";

        public UserControl Page { get; private set; }

        public Action<ContentPageData> ClickAction { get; private set; }
    }

}
