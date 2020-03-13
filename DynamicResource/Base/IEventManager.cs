using System.Windows;

namespace DynamicResource
{
    internal interface IEventManager
    {
        void AddListener(Manager source, IWeakEventListener listener);

        void RemoveListener(Manager source, IWeakEventListener listener);
    }
}
