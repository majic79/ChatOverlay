using System;
using System.Windows.Input;

namespace MaJiCSoft.ChatOverlay.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> action;
        private readonly Func<object, bool> canExecuteAction;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.action = executeAction;
            this.canExecuteAction = canExecute;
        }

        private EventHandler canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter) => canExecuteAction?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => action(parameter);

        public void InvokeCanExecuteChanged() => canExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
