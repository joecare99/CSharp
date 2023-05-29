using ConsoleDisplay.View;
using System;
using System.Windows.Input;

namespace Pattern_00_Template.Views
{
    public class MainView : ICommand
    {
        MainViewModel DataContext { get; set; } = new();
        IConsole console = new MyConsole();

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
