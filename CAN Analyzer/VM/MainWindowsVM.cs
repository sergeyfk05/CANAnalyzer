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

namespace CANAnalyzer.VM
{
    public class MainWindowsVM : BaseVM
    {
        public MainWindowsVM()
        {
            PagesData.Add(new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false, Text = "app", ImageSource = new Uri(new Uri(Assembly.GetExecutingAssembly().Location), @"Resources\Icons\1.png") }, new TransmitPage()));
            PagesData.Add(new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false, Text = "device", ImageSource = new Uri(new Uri(Assembly.GetExecutingAssembly().Location), @"Resources\Icons\1.png") }, new DeviceSettingsPage()));

            foreach(var el in PagesData)
            {
                BottomItemSource.Add(el.NavData);
            }
        }

        private List<ContentPageData> PagesData = new List<ContentPageData>();


        public List<NavMenuItemData> TopItemSource
        {
            get
            {
                return _topItemSource ?? (_topItemSource = new List<NavMenuItemData>());
            }
            set
            {
                if (_topItemSource == value)
                    return;

                _topItemSource = value;
                OnPropertyChanged();
            }
        }
        private List<NavMenuItemData> _topItemSource;


        public List<NavMenuItemData> BottomItemSource
        {
            get
            {
                return _bottomItemSource ?? (_bottomItemSource = new List<NavMenuItemData>());
            }
            set
            {
                if (_bottomItemSource == value)
                    return;

                _bottomItemSource = value;
                OnPropertyChanged();
            }
        }
        private List<NavMenuItemData> _bottomItemSource;


        public bool MenuIsCollapsed
        {
            get { return _menuIsCollapsed; }
            set
            {
                if (_menuIsCollapsed == value)
                    return;

                _menuIsCollapsed = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
            if (pageData.Page == null)
                return;

            MainContent = pageData.Page;
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
