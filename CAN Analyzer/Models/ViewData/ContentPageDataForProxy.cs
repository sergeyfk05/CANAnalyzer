/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using CANAnalyzerChannelProxyInterfaces;
using HamburgerMenu;
using System;
using System.Windows.Controls;

namespace CANAnalyzer.Models.ViewData
{
    public class ContentPageDataForProxy : ContentPageData
    {
        public ContentPageDataForProxy(NavMenuItemData nd, string imageKey, IChannelProxy proxy, PageKind kind = PageKind.Undefined, Action<ContentPageData> clickAction = null) 
            : base(nd, "", imageKey, kind, clickAction)
        {
            _proxy = proxy;
            _proxy.NameChanged += _proxy_NameChanged;
            this.LocalizedKey = _proxy.Name;
        }

        public ContentPageDataForProxy(NavMenuItemData nd, string imageKey, IChannelProxy proxy, PageKind kind, UserControl page, Action<ContentPageData> clickAction = null)
            : base(nd, "", imageKey, kind, page, clickAction)
        {
            _proxy = proxy;
            _proxy.NameChanged += _proxy_NameChanged;
            this.LocalizedKey = _proxy.Name;
        }

        public IChannelProxy Proxy => _proxy;
        private IChannelProxy _proxy;


        private void _proxy_NameChanged(object sender, EventArgs e)
        {
            this.LocalizedKey = _proxy.Name;
        } 
        ~ContentPageDataForProxy()
        {
            if(_proxy != null)
                _proxy.NameChanged -= _proxy_NameChanged;
        }
    }
}
