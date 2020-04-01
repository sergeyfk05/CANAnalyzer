/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System.Windows.Data;

namespace DynamicResource
{
    /// <summary>
    /// Слушатель изменения культуры при локализации по привязке
    /// </summary>
    internal class BindingDynamicResourceListener<T> : DynamicResourceListener<T> where T: Manager
    {
        public BindingDynamicResourceListener(IEventManager eventManager, Manager manager)
            :base (eventManager, manager)
        {

        }
        private BindingExpressionBase BindingExpression { get; set; }

        public void SetBinding(BindingExpressionBase bindingExpression)
        {
            BindingExpression = bindingExpression;
        }

        protected override void OnCultureChanged()
        {
            try
            {
                // Обновляем результат выражения привязки
                // При этом конвертер вызывается повторно уже для новой культуры
                BindingExpression?.UpdateTarget();
            }
            catch
            {
                // ignored
            }
        }
    }
}
