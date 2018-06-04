using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace De.Markellus.HybridInjector.Misc
{
    public class WpfCommands
    {
        public static readonly ICommand CloseCommand = new RelayCommand(o => ((Window)o).Close());
        public static readonly ICommand ToggleMusicCommand = new RelayCommand(o => MusicPlayer.ToggleMusicPlayback());
        public static readonly ICommand DragWindowCommand = new RelayCommand(o => ((Window)o).DragMove());
    }

    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 

        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute)); _canExecute = canExecute;
        }
        #endregion // Constructors 

        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }
}
