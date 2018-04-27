using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FastSQL.App.Interfaces
{
    public class BaseCommand : ICommand
    {
        protected readonly Func<object, bool> CanExecuteDelegate;
        protected readonly Action<object> ExecuteDelegate;

        public virtual event EventHandler CanExecuteChanged;
        public BaseCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            CanExecuteDelegate = canExecute;
            ExecuteDelegate = execute;
        }

        public virtual bool CanExecute(object parameter)
        {
            return CanExecuteDelegate?.Invoke(parameter) == true;
        }

        public virtual void Execute(object parameter)
        {
            ExecuteDelegate?.Invoke(parameter);
        }
    }
}
