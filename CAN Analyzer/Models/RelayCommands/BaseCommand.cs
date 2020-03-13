using System;

namespace CANAnalyzer.Models
{
    public abstract class BaseCommand
    {
        private readonly Func<bool> canExecute;

        public BaseCommand(Func<bool> canExecute)
        {
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Создано при вызове RaiseCanExecuteChanged.
        /// </summary>
        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Определяет, можно ли выполнить эту команду <see cref="RelayCommandAsync"/> в текущем состоянии.
        /// </summary>
        /// <param name="parameter">
        /// Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.
        /// </param>
        /// <returns>true, если команда может быть выполнена; в противном случае - false.</returns>
        public bool CanExecute(object parameter = null)
        {
            return canExecute == null ? true : canExecute();
        }

        /// <summary>
        /// Метод, используемый для создания события <see cref="CanExecuteChanged"/>
        /// чтобы показать, что возвращаемое значение <see cref="CanExecute"/>
        /// метод изменился.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
