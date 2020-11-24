/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RelayCommands
{
    public class RelayCommandAsync : BaseCommand, ICommand
    {
        private readonly Action execute;


        /// <summary>
        /// Создает новую команду, которая всегда может выполняться.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        public RelayCommandAsync(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Создает новую команду.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <param name="canExecute">Логика состояния выполнения.</param>
        public RelayCommandAsync(Action execute, Func<bool> canExecute)
            :base(canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.execute = execute;
        }

        public override bool Equals(object obj)
        {
            return obj is RelayCommandAsync async &&
                   base.Equals(obj) &&
                   EqualityComparer<Action>.Default.Equals(execute, async.execute);
        }

        /// <summary>
        /// Выполняет <see cref="RelayCommandAsync"/> текущей цели команды.
        /// </summary>
        /// <param name="parameter">
        /// Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.
        /// </param>
        public void Execute(object parameter = null)
        {
            if (CanExecute())
                Task.Run(() => execute.Invoke());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), execute);
        }
    }
}
