using System;
using System.Windows.Input;

namespace AudioConverter.MVVM
{
    internal class RelayCommand : ICommand
    {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        public RelayCommand()
        {
        }

        public RelayCommand(Action exec)
            : this(exec, null)
        {
        }

        public RelayCommand(Action exec, Func<bool> canExec)
        {
            _execute = exec;
            _canExecute = canExec;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual bool CanExecute()
        {
            return _canExecute == null || _canExecute();
        }

        public virtual void Execute()
        {
            _execute?.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public void Execute(object parameter)
        {
            Execute();
        }
    }
}
