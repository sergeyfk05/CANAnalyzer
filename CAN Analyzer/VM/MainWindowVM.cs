/*
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

namespace CANAnalyzer.VM
{
    public class MainWindowVM : BaseVM
    {
        public MainWindowVM()
        {
            ContentPageData buf = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                "appSettingsMenu",
                "AppSettingsPageIcon",
                PageKind.Settings,
                new AppSettingsPage(),
                ChangePage);
            PagesData.Add(buf);
            BottomItemSource.Add(buf.NavData);


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


            List<NavMenuItemData> channels = new List<NavMenuItemData>();

            foreach (var el in Settings.Instance.Device.Channels.Reverse())
            {
                //recievePage
                var recievePage = new RecieveChannelPage();
                recievePage.DataContext = new RecieveChannelPageVM();

                ContentPageData recievePageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                    "NavMenuRecieveChannelPage",
                    "RecievedChannelPageIcon",
                    PageKind.Channel,
                    recievePage,
                    ChangePage);
                PagesData.Add(recievePageData);


                //monitorPage
                var monitorPage = new MonitorChannelPage();
                monitorPage.DataContext = new MonitorChannelPageVM();

                ContentPageData monitorPageData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = false, IsSelected = false },
                    "NavMenuMonitorChannelPage",
                    "MonitorChannelPageIcon",
                    PageKind.Channel,
                    monitorPage,
                    ChangePage);
                PagesData.Add(monitorPageData);


                ContentPageData channelViewData = new ContentPageData(new NavMenuItemData() { IsDropdownItem = true, IsSelected = false },
                    el.ToString() + "NavMenu",
                    el.ToString() + "Icon",
                    PageKind.Channel);
                channelViewData.NavData.AddDropdownItem(recievePageData.NavData);
                channelViewData.NavData.AddDropdownItem(monitorPageData.NavData);

                PagesData.Add(channelViewData);

                _context.Post((s) =>
                {
                    TopItemSource.Add(channelViewData.NavData);
                }, null);
                //channels.Add(channelViewData.NavData);

            }

            



            //create views
        }

        private void OnProxiesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }


        private void ClearChannelsAndProxies()
        {
            //clear channels and proxies
            var deleteObjects = PagesData.Where(x => x.Kind == PageKind.Channel || x.Kind == PageKind.Proxy).Select(x=> x.NavData);
            _context.Send((s) => 
            {
                TopItemSource.RemoveAll(x => deleteObjects.Contains(x));
            }, null);   

            PagesData.RemoveAll(x => x.Kind == PageKind.Channel || x.Kind == PageKind.Proxy);
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
            if ((pageData == null) || (pageData.Page == null))
                return;

            _context.Post((s) =>
            {
                pageData.ClickAction(pageData);
            }, null);
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
