/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
*/
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CANAnalyzer.Models
{
    public class RelayCommandWithParameterAsync<T> : BaseCommand, ICommand
    {
        private readonly Action<T> execute;

        /// <summary>
        /// Создает новую команду, которая всегда может выполняться.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        public RelayCommandWithParameterAsync(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Создает новую команду.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <param name="canExecute">Логика состояния выполнения.</param>
        public RelayCommandWithParameterAsync(Action<T> execute, Func<bool> canExecute)
            :base(canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.execute = execute;
        }

        public override bool Equals(object obj)
        {
            return obj is RelayCommandWithParameterAsync<T> async &&
                   base.Equals(obj) &&
                   EqualityComparer<Action<T>>.Default.Equals(execute, async.execute);
        }

        /// <summary>
        /// Выполняет <see cref="RelayCommandWithParameterAsync{T}"/> текущей цели команды.
        /// </summary>
        /// <param name="parameter">
        /// Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.
        /// </param>
        public void Execute(object parameter)
        {
            if ((parameter != null) && (CanExecute(parameter)) && (execute != null))
                Task.Run(() => execute.Invoke((T)parameter));
            //execute?.BeginInvoke((T)parameter, null, null);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), execute);
        }
    }
}
