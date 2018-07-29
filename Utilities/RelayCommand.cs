using System;
using System.Windows.Input;

namespace Utilities
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _predicate;

        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            _action = action;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter = null)
        {
            return _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            if (_action != null && CanExecute())
                _action(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(this, new EventArgs());
        }
    }
}
