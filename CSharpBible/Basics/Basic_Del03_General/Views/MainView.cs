using System;
using System.Windows.Input;
using Basic_Del03_General.ViewModels;
using BaseLib.Interfaces;
using BaseLib.Models;

namespace Basic_Del03_General.Views;

public class MainView : ICommand
{
    public MainViewModel DataContext { get; set; } = new();
    public IConsole console { get; set; } = new ConsoleProxy();

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        console.WriteLine(DataContext.Greeting);
        console.WriteLine($"Result: {DataContext.CalcResult}");
    }
}
