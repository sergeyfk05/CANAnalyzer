﻿/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using HamburgerMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CANAnalyzer.Models;
using System.Windows.Input;
using DynamicResource;
using CANAnalyzer.Resources.DynamicResources;
using System.Windows.Media;
using System.Windows.Controls;
using CANAnalyzer.Pages;
using CANAnalyzer.Models.ViewData;
using System.Collections.ObjectModel;

namespace CANAnalyzer.VM
{
    public class MainWindowsVM : BaseVM
    {
        public MainWindowsVM()
        {
            ContentPageData buf = new ContentPageData(new NavMenuItemData() { IsDropdownItem = true, IsSelected = false },
                "appSettingsMenu",
                "appSettingsIcon",
                new AppSettingsPage(),
                ChangePage);
            PagesData.Add(buf);
            BottomItemSource.Add(buf.NavData);

            Source.Add(buf.NavData);

            ContentPageData buf1 = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
    "appSettingsMenu",
    "appSettingsIcon",
    new AppSettingsPage(),
    ChangePage);
            buf.NavData.AddDropdownItem(buf1.NavData);
            buf.NavData.AddDropdownItem(buf1.NavData);

            ContentPageData buf2 = new ContentPageData(new NavMenuItemData() { IsDropdownItem = true, IsSelected = false },
"appSettingsMenu",
"appSettingsIcon",
new AppSettingsPage(),
ChangePage);
            buf2.NavData.AddDropdownItem(buf1.NavData);
            buf2.NavData.AddDropdownItem(buf1.NavData);
            buf2.NavData.AddDropdownItem(buf1.NavData);

            buf.NavData.AddDropdownItem(buf2.NavData);


            buf.NavData.AddDropdownItem(buf1.NavData);
            buf.NavData.AddDropdownItem(buf1.NavData);

            Manager<LanguageCultureInfo>.StaticInstance.CultureChanged += LanguageManager_CultureChanged;
            Manager<ThemeCultureInfo>.StaticInstance.CultureChanged += ThemeManager_CultureChanged;
        }

        public ObservableCollection<NavMenuItemData> Source
        {
            get
            {
                return _source ?? (_source = new ObservableCollection<NavMenuItemData>());
            }
            set
            {
                if (_source == value)
                    return;

                _source = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<NavMenuItemData> _source;

        private void ThemeManager_CultureChanged(object sender, EventArgs e)
        {
            if (PagesData == null)
                return;

            foreach (var el in PagesData)
            {
                el.UpdateTheme();
            }
        }

        private void LanguageManager_CultureChanged(object sender, EventArgs e)
        {
            if (PagesData == null)
                return;

            foreach (var el in PagesData)
            {
                el.UpdateLocalization();
            }
        }


        private void ChangePage(ContentPageData data)
        {
            MainContent = data.Page;
            ResetSelectedItems();
            data.NavData.IsSelected = true;
        }





        private List<ContentPageData> PagesData = new List<ContentPageData>();
        private void ResetSelectedItems()
        {
            foreach (var el in PagesData)
                el.NavData.ResetIsSelectedFlag();
        }

        public ObservableCollection<NavMenuItemData> TopItemSource
        {
            get
            {
                return _topItemSource ?? (_topItemSource = new ObservableCollection<NavMenuItemData>());
            }
            set
            {
                if (_topItemSource == value)
                    return;

                _topItemSource = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<NavMenuItemData> _topItemSource;


        public ObservableCollection<NavMenuItemData> BottomItemSource
        {
            get
            {
                return _bottomItemSource ?? (_bottomItemSource = new ObservableCollection<NavMenuItemData>());
            }
            set
            {
                if (_bottomItemSource == value)
                    return;

                _bottomItemSource = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<NavMenuItemData> _bottomItemSource;


        public bool MenuIsCollapsed
        {
            get { return _menuIsCollapsed; }
            set
            {
                if (_menuIsCollapsed == value)
                    return;

                _menuIsCollapsed = value;
                RaisePropertyChanged();
            }
        }
        private bool _menuIsCollapsed = true;


        public UserControl MainContent
        {
            get { return _mainContent; }
            set
            {
                if (value == _mainContent)
                    return;

                _mainContent = value;
                RaisePropertyChanged();
            }
        }
        private UserControl _mainContent;

        private ICommand _navMenuClicked;
        public ICommand NavMenuClicked
        {
            get
            {
                if (_navMenuClicked == null)
                    _navMenuClicked = new RelayCommandWithParameterAsync<NavMenuItemData>(this.NavMenuClicked_Execute);

                return _navMenuClicked;
            }
        }
        private void NavMenuClicked_Execute(NavMenuItemData arg)
        {
            ContentPageData pageData = PagesData.FirstOrDefault(x => x.NavData == arg);
            if ((pageData == null) || (pageData.Page == null))
                return;

            pageData.ClickAction(pageData);
        }


        private ICommand _clickContent;
        public ICommand ClickContent
        {
            get
            {
                if (_clickContent == null)
                    _clickContent = new RelayCommand(this.ClickContent_Execute, () => !MenuIsCollapsed);

                return _clickContent;
            }
        }
        private void ClickContent_Execute()
        {
            MenuIsCollapsed = true;
        }
    }
}
