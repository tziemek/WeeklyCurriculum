using System;
using System.Windows.Input;

namespace WeeklyCurriculum.Wpf
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> onExecute;
        private readonly Predicate<object> onCanExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> onExecute)
        {
            this.onExecute = onExecute;
        }

        public RelayCommand(Action<object> onExecute, Predicate<object> onCanExecute)
        {
            this.onExecute = onExecute;
            this.onCanExecute = onCanExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this.onCanExecute != null)
            {
                return this.onCanExecute.Invoke(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.onExecute?.Invoke(parameter);
            }
        }
    }
}
