using System;
using System.Windows.Input;
using ConsoleDisplay.View;
using Pattern_01_Singleton.Models;
using Pattern_01_Singleton.ViewModels;

namespace Pattern_01_Singleton.Views
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
            if (DataContext.MyUserContext == UserContext.Instance)
                console.WriteLine(DataContext.EqualityMsg);
        }
    }
}
