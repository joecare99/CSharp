using System;
using System.Windows.Input;
using ConsoleDisplay.View;
using Basic_Del02_Filter.ViewModels;

namespace Basic_Del02_Filter.Views
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
            console.WriteLine($"Orginal: {String.Join(", ", DataContext.GetList)}");
            console.WriteLine($"Filtered1: {String.Join(", ", DataContext.GetFilteredData((i) => i % 2 != 0))}");
            console.WriteLine($"Filtered2: {String.Join(", ", DataContext.GetFilteredData((i)=>i%2==0))}");
        }
    }
}
