using System;
using System.Windows.Input;
using BaseLib.Interfaces;
using ConsoleDisplay.View;
using Pattern_02_Observer.Models;
using Pattern_02_Observer.ViewModels;

namespace Pattern_02_Observer.Views
{
    public class MainView : ICommand
    {
        public MainViewModel DataContext { get; set; } = new();
        public IConsole console { get; set; } = new MyConsole();

        public event EventHandler? CanExecuteChanged;

        public MainView()
        {
            DataContext.PropertyChangedAdv += OnPropertyChangedAdv;
        }

        private void OnPropertyChangedAdv(object sender, PropertyChangedAdvEventArgs e)
        {
            if (e.PropertyName == nameof(DataContext.Greeting))
                console.WriteLine((string?)e.NewVal);
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            DataContext.SetGreeting((EGreetings)new Random().Next(5)); 
        }
    }
}
