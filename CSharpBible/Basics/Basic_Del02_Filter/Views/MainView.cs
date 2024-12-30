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

        bool IsGreaterThan5(int parameter) => parameter > 5;
        bool IsEven(int parameter)=> parameter % 2 == 0;

        Predicate<int> IsGreaterThan(int parameter2) => (i)=>i > parameter2;

        public void Execute(object? parameter)
        {
            console.WriteLine(DataContext.Greeting);
            console.WriteLine($"Original: {String.Join(", ", DataContext.GetList)}");
            // using the function IsGreaterThan5 as delegate 
            console.WriteLine($"Filtered(IsGreaterThan5): {String.Join(", ", DataContext.GetFilteredData(IsGreaterThan5))}");
            // using the function IsEven as delegate 
            console.WriteLine($"Filtered(IsEven): {String.Join(", ", DataContext.GetFilteredData(IsEven))}");
            // using the function IsGreaterThan with 1 as delegate 
            console.WriteLine($"Filtered(IsGreaterThan(1)): {String.Join(", ", DataContext.GetFilteredData(IsGreaterThan(1)))}");
            // using an anonymous function as delegate 
            console.WriteLine($"Filtered(anon odd): {String.Join(", ", DataContext.GetFilteredData((i)=>i%2!=0))}");
        }
    }
}
