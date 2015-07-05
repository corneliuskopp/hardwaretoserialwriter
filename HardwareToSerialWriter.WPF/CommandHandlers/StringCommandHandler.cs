namespace HardwareToSerialWriter.CommandHandlers
{
    using System;
    using System.Windows.Input;

    internal class StringCommandHandler : ICommand
    {
        private readonly Action<string> _executeCallback;
        private readonly Func<bool> _canExecuteCallback;

        public StringCommandHandler(Action<string> executeCallback, Func<bool> canExecuteCallback)
        {
            _executeCallback = executeCallback;
            _canExecuteCallback = canExecuteCallback;
        }

        public StringCommandHandler(Action<string> executeCallback)
        {
            _executeCallback = executeCallback;
            _canExecuteCallback = () => true;
        }

        public void Execute(object parameter)
        {
            var parm = parameter as string;
            if (parm == null)
            {
                throw new ArgumentException(string.Format("Expected string parameter, but got '{0}' with ToString '{1}'.", parameter.GetType(), parameter.ToString()));
            }

            _executeCallback(parm);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteCallback();
        }

        public event EventHandler CanExecuteChanged;
    }
}