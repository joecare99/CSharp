using System;
using System.Windows.Input;
using ConsoleDisplay.View;
using Basic_Del01_Action.ViewModels;

namespace Basic_Del01_Action.Views
{
    public class MainView : ICommand
    {
        public MainViewModel DataContext { get; set; } = new();
        public IConsole console { get; set; } = new MyConsole();

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            console.WriteLine(DataContext.Greeting);
        }
    }
}
