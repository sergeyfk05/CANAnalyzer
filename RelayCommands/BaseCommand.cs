/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Threading;

namespace RelayCommands
{
    public abstract class BaseCommand
    {
        private readonly Func<bool> canExecute;

        public BaseCommand(Func<bool> canExecute)
        {
            this.canExecute = canExecute;
            _context = SynchronizationContext.Current;
        }

        /// <summary>
        /// Создано при вызове RaiseCanExecuteChanged.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        private SynchronizationContext _context;

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
            if (CanExecuteChanged != null)
            {
                _context?.Post(s =>
                {
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }, null);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BaseCommand command &&
                   EqualityComparer<Func<bool>>.Default.Equals(canExecute, command.canExecute) &&
                   EqualityComparer<SynchronizationContext>.Default.Equals(_context, command._context);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(canExecute, _context);
        }
    }
}
