/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Windows;

namespace DynamicResource
{
    internal class CultureChangedDynamicResourceEventManager<T> : WeakEventManager, IEventManager
    {
        private static CultureChangedDynamicResourceEventManager<T> _eventmanager;
        public static CultureChangedDynamicResourceEventManager<T> Instance => _eventmanager ?? (_eventmanager = new CultureChangedDynamicResourceEventManager<T>());

        private static CultureChangedDynamicResourceEventManager<T> CurrentManager
        {
            get
            {
                var managerType = typeof(CultureChangedDynamicResourceEventManager<T>);
                var manager = (CultureChangedDynamicResourceEventManager<T>)GetCurrentManager(managerType);
                if (manager == null)
                {
                    manager = new CultureChangedDynamicResourceEventManager<T>();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }

        public static void AddListenerSt(Manager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }
        public void AddListener(Manager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListenerSt(Manager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }
        public void RemoveListener(Manager source, IWeakEventListener listener)
        {
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnCultureChanged(object sender, EventArgs e)
        {
            DeliverEvent(sender, e);
        }

        protected override void StartListening(object source)
        {
            var manager = (Manager)source;
            manager.CultureChanged += OnCultureChanged;
        }

        protected override void StopListening(object source)
        {
            var manager = (Manager)source;
            manager.CultureChanged -= OnCultureChanged;
        }
    }
}
