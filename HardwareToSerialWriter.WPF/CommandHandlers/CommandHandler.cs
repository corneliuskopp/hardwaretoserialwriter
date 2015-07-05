namespace HardwareToSerialWriter.CommandHandlers
{
    using System;
    using System.Windows.Input;

    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecuteCallback;

        public CommandHandler(Action action, Func<bool> canExecuteCallback)
        {
            _action = action;
            _canExecuteCallback = canExecuteCallback;
        }

        public CommandHandler(Action action)
        {
            _action = action;
            _canExecuteCallback = () => true;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteCallback();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}