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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using CANAnalyzer.Models.Extensions;
using System.Threading;
using CANAnalyzer.Models.ChannelsProxy;
using System.Collections;

namespace CANAnalyzer.VM
{
    public class MainWindowVM : BaseVM
    {
        public MainWindowVM()
        {
            ContentPageData buf = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                "#appSettingsMenu",
                "AppSettingsPageIcon",
                PageKind.Settings,
                new AppSettingsPage(),
                ChangePage);
            PagesData.Add(buf);
            BottomItemSource.Add(buf.NavData);


            buf = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                "#addBomberPageMenu",
                "AddBomberPageeIcon",
                PageKind.Settings,
                (data) => { AddUserPage(new BomberPage(), new BomberPageVM(), "#NavMenuBomberPage", "BomberPageIcon"); });
            PagesData.Add(buf);
            BottomItemSource.Insert(0, buf.NavData);

            buf = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                "#addTransmitPageMenu",
                "AddTransmitPageeIcon",
                PageKind.Settings,
                (data) => { AddUserPage(new TransmitPage(), new TransmitFilePageVM(), "#NavMenuTransmitFilePage", "TransmitFilePageIcon"); });
            PagesData.Add(buf);
            BottomItemSource.Insert(0, buf.NavData);


            Settings.Instance.Proxies.CollectionChanged += OnProxiesCollectionChanged;
            Settings.Instance.PropertyChanged += OnDevicePropertyChanged;

            Manager<ThemeCultureInfo>.StaticInstance.CultureChanged += Theme_CultureChanged;
        }

        private SynchronizationContext _context = SynchronizationContext.Current;

        private void Theme_CultureChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("NavMenuDropdownIcon");
        }

        private void OnDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e != null && e.PropertyName != "Device")
                return;


            ClearChannelsAndProxies();


            if (Settings.Instance.Device == null)
                return;


            Settings.Instance.Device.IsConnectedChanged += (object s, EventArgs args) =>
            {
                if (s == Settings.Instance.Device)
                    if (Settings.Instance.Device.IsConnected)
                        OnDevicePropertyChanged(null, null);
                    else
                        ClearChannelsAndProxies();
            };

            if (!Settings.Instance.Device.IsConnected)
                return;


            //create views
            foreach (var el in Settings.Instance.Device.Channels.Reverse())
            {
                //recievePage
                var recievePage = new RecieveChannelPage();
                recievePage.DataContext = new RecieveChannelPageVM(el);

                ContentPageData recievePageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                    "#NavMenuRecieveChannelPage",
                    "RecievedChannelPageIcon",
                    PageKind.Channel,
                    recievePage,
                    ChangePage);
                PagesData.Add(recievePageData);


                //monitorPage
                var monitorPage = new MonitorChannelPage();
                monitorPage.DataContext = new MonitorChannelPageVM(el);

                ContentPageData monitorPageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                    "#NavMenuMonitorChannelPage",
                    "MonitorChannelPageIcon",
                    PageKind.Channel,
                    monitorPage,
                    ChangePage);
                PagesData.Add(monitorPageData);


                ContentPageData channelViewData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = true, IsSelected = false },
                    "#" + el.ToString() + "NavMenu",
                    el.ToString() + "Icon",
                    PageKind.Channel);
                channelViewData.NavData.AddDropdownItem(recievePageData.NavData);
                channelViewData.NavData.AddDropdownItem(monitorPageData.NavData);

                PagesData.Add(channelViewData);

                _context.Post((s) =>
                {
                    TopItemSource.Add(channelViewData.NavData);
                }, null);
            }
        }

        private void OnProxiesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(var el in e.NewItems)
                {
                    if(el is IChannelProxy proxy)
                    {
                        //recievePage
                        RecieveChannelPage recievePage = null;
                        _context.Send((s) =>
                        {
                            recievePage = new RecieveChannelPage();
                            recievePage.DataContext = new RecieveChannelPageVM(proxy);
                        }, null);                        

                        ContentPageData recievePageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                            "#NavMenuRecieveProxyPage",
                            "RecievedProxyPageIcon",
                            PageKind.Proxy,
                            recievePage,
                            ChangePage);
                        PagesData.Add(recievePageData);


                        //monitorPage
                        MonitorChannelPage monitorPage = null;
                        _context.Send((s) =>
                        {
                            monitorPage = new MonitorChannelPage();
                            monitorPage.DataContext = new MonitorChannelPageVM(proxy);
                        }, null);

                        ContentPageData monitorPageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                            "#NavMenuMonitorProxyPage",
                            "MonitorProxyPageIcon",
                            PageKind.Proxy,
                            monitorPage,
                            ChangePage);
                        PagesData.Add(monitorPageData);


                        ContentPageDataForProxy channelViewData = new ContentPageDataForProxy(new NavMenuItemData() { IsDropdownItem = true, IsSelected = false },
                            proxy.ToString() + "Icon",
                            proxy,
                            PageKind.Proxy);
                        channelViewData.NavData.AddDropdownItem(recievePageData.NavData);
                        channelViewData.NavData.AddDropdownItem(monitorPageData.NavData);

                        PagesData.Add(channelViewData);

                        _context.Post((s) =>
                        {
                            TopItemSource.Add(channelViewData.NavData);
                        }, null);
                    }
                }

            }


            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                ClearProxies();
            }


            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var objectsToRemove = PagesData.Where(x => IsContentPageDataContainsProxies(x, e.OldItems)).Select(x => x.NavData);

                _context.Send((s) =>
                {
                    TopItemSource.RemoveAll(x => objectsToRemove.Contains(x));
                }, null);
            }
        }

        /// <summary>
        /// Метода, который нужен для удаления прокси из меню
        /// </summary>
        /// <param name="data">ContentPageData который мы проверим на то отвечает ли он за прокси и содержится ли прокси в листе</param>
        /// <param name="proxies">Лист по которому мы будет смотреть содержаться ли прокси</param>
        /// <returns></returns>
        private bool IsContentPageDataContainsProxies(ContentPageData data, IList proxies)
        {
            if(data is ContentPageDataForProxy proxyData)
            {
                return proxies.Contains(proxyData.Proxy);
            }
            return false;
        }


        /// <summary>
        /// Remove all pages with PageKind == PageKind.Proxy | PageKind.Channel.
        /// </summary>
        private void ClearChannelsAndProxies()
        {
            //clear channels and proxies
            ClearChannels();
            ClearProxies();
        }

        /// <summary>
        /// Remove all pages with PageKind == PageKind.Channel.
        /// </summary>
        private void ClearChannels()
        {
            //clear channels
            var deleteObjects = PagesData.Where(x => x.Kind == PageKind.Channel).Select(x => x.NavData);
            _context.Send((s) =>
            {
                TopItemSource.RemoveAll(x => deleteObjects.Contains(x));
            }, null);

            PagesData.RemoveAll(x => x.Kind == PageKind.Channel);
        }

        /// <summary>
        /// Remove all pages with PageKind == PageKind.Proxy.
        /// </summary>
        private void ClearProxies()
        {
            //clear proxies
            var deleteObjects = PagesData.Where(x => x.Kind == PageKind.Proxy).Select(x => x.NavData);
            _context.Send((s) =>
            {
                TopItemSource.RemoveAll(x => deleteObjects.Contains(x));
            }, null);

            PagesData.RemoveAll(x => x.Kind == PageKind.Proxy);
        }


        /// <summary>
        /// Changing active page.
        /// </summary>
        /// <param name="data">Page which should be active.</param>
        private void ChangePage(ContentPageData data)
        {
            if (data.Page == null || data.NavData == null)
                return;

            MainContent = data.Page;
            ResetSelectedItems();
            data.NavData.IsSelected = true;
        }

        /// <summary>
        /// Adds user page into top navigation menu.
        /// </summary>
        /// <param name="page">Page which should be added to top navigation menu.</param>
        /// <param name="vm">ViewModel for page.</param>
        /// <param name="navMenuLabelKey">Localization key for label in navigation menu.</param>
        /// <param name="navMenuIconKey">Localization key for icon in navigation menu.</param>
        private void AddUserPage(UserControl page, BaseClosableVM vm, string navMenuLabelKey = "", string navMenuIconKey = "")
        {
            page.DataContext = vm;

            ContentPageData pageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                navMenuLabelKey,
                navMenuIconKey,
                PageKind.UserPages,
                page,
                ChangePage);

            vm.Closed += (object sender, EventArgs e) => { DelPageFromMenu(pageData); };

            PagesData.Add(pageData);
            TopItemSource.Add(pageData.NavData);

            ResetSelectedItems();
            MainContent = page;
            pageData.NavData.IsSelected = true;
        }

        /// <summary>
        /// Delete page from navigation menu.
        /// </summary>
        /// <param name="page">Page which should be deleted</param>
        private void DelPageFromMenu(ContentPageData page)
        {
            MainContent = null;
            ResetSelectedItems();

            PagesData.Remove(page);
            TopItemSource.Remove(page.NavData);
            BottomItemSource.Remove(page.NavData);
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


        public ImageSource NavMenuDropdownIconSource
        {
            get
            {
                try
                {
                    return new BitmapImage(new Uri(
                                        new Uri(Assembly.GetExecutingAssembly().Location),
                                        (string)Manager<ThemeCultureInfo>.StaticInstance.GetResource("NavMenuDropdownIcon")));
                }
                catch
                {
                    return null;
                }
            }
        }


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
            if ((pageData == null))
                return;

            _context.Post((s) =>
            {
                pageData?.ClickAction?.Invoke(pageData);
            }, null);
        }


        private ICommand _clickContent;
        public ICommand ClickContent
        {
            get
            {
                if (_clickContent == null)
                    _clickContent = new RelayCommand(this.ClickContent_Execute);

                return _clickContent;
            }
        }
        private void ClickContent_Execute()
        {
            MenuIsCollapsed = true;
        }
    }
}
