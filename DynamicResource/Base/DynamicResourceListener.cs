/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Windows;

namespace DynamicResource
{
    /// <summary>
    /// Базовый класс для слушателей изменения культуры
    /// </summary>
    internal abstract class DynamicResourceListener<T> : IWeakEventListener, IDisposable where T: Manager
    {
        protected DynamicResourceListener(IEventManager eventManager, Manager manager)
        {
            _eventManager = eventManager;
            _manager = manager;
            _eventManager.AddListener(_manager.InstanceStock, this);
        }
        private IEventManager _eventManager;
        private Manager _manager;

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CultureChangedDynamicResourceEventManager<T>))
            {
                OnCultureChanged();
                return true;
            }
            return false;
        }

        protected abstract void OnCultureChanged();

        public void Dispose()
        {
            _eventManager.RemoveListener(_manager.InstanceStock, this);
            GC.SuppressFinalize(this);
        }
    }
}
