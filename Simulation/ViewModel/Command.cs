using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Simulation.ViewModel
{
    /// <summary>
    /// Command used with GUI buttons.
    /// </summary>
    class Command : ICommand
    {
        /// <summary>
        /// Handles event when execute ability of command changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Delegate to function which should be executed upon button click.
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// Creates new instance of Command.
        /// </summary>
        /// <param name="command">Delegate to function which Command wraps around.</param>
        public Command(Action<object> command)
        {
            _execute = command;
        }

        /// <summary>
        /// Tells if command can be executed.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True if command can be executed, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return _execute != null;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Parameter for the command.</param>
        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
    }
}
