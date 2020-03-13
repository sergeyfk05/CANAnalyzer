using System.ComponentModel;

namespace DynamicResource
{
    /// <summary>
    /// Слушатель изменения культуры при локализации по ключу
    /// </summary>
    internal class KeyDynamicResourceListener<T> : DynamicResourceListener<T>, INotifyPropertyChanged where T : Manager
    {
        public KeyDynamicResourceListener(string key, object[] args, IEventManager eventManager, Manager manager)
            : base(eventManager, manager)
        {
            _manager = manager;
            Key = key;
            Args = args;
        }

        private Manager _manager;
        private string Key { get; }

        private object[] Args { get; }

        public object Value
        {
            get
            {
                var value = _manager.InstanceStock.GetResource(Key);
                if (value is string && Args != null)
                    value = string.Format((string)value, Args);
                return value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => {};

        protected override void OnCultureChanged()
        {
            // Уведомляем привязку об изменении строки
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
}
