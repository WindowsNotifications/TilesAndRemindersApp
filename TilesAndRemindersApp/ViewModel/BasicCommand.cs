using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TilesAndRemindersApp.ViewModel
{
    public class BasicCommand : ICommand
    {
        private Action<object> _action;
        
        public BasicCommand(Action action, bool defaultCanExecute = true)
            : this(new Action<object>(delegate { action.Invoke(); }), defaultCanExecute)
        {
            // Nothing, this is just a wrapper for the other constructor
        }

        public BasicCommand(Action<object> action, bool defaultCanExecute = true)
        {
            _action = action;
            _canExecute = defaultCanExecute;
        }

        public event EventHandler CanExecuteChanged;

        private bool _canExecute;

        public void SetCanExecute(bool value)
        {
            if (_canExecute != value)
            {
                _canExecute = value;

                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, new EventArgs());
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
    }
}
