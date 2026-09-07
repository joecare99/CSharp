using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib.Showcase.Models;

namespace ConsoleLib.Showcase.Desktop.ViewModels;

/// <summary>Calculator page adapter around the pure calculator engine.</summary>
public partial class CalculatorViewModel : ObservableObject
{
    private readonly CalculatorEngine _engine = new();

    [ObservableProperty]
    private string display = "0";

    [ObservableProperty]
    private string indicator = string.Empty;

    public void Refresh()
    {
        Display = _engine.Display;
        Indicator = _engine.Indicator;
    }

    [RelayCommand]
    private void Clear()
    {
        _engine.Clear();
        Refresh();
    }

    [RelayCommand]
    private void Seven() => EnterDigit(7);

    [RelayCommand]
    private void Eight() => EnterDigit(8);

    [RelayCommand]
    private void Nine() => EnterDigit(9);

    [RelayCommand]
    private void Zero() => EnterDigit(0);

    [RelayCommand]
    private void One() => EnterDigit(1);

    [RelayCommand]
    private void Two() => EnterDigit(2);

    [RelayCommand]
    private void Three() => EnterDigit(3);

    [RelayCommand]
    private void Four() => EnterDigit(4);

    [RelayCommand]
    private void Five() => EnterDigit(5);

    [RelayCommand]
    private void Six() => EnterDigit(6);

    [RelayCommand]
    private void Decimal() { _engine.DecimalPoint(); Refresh(); }

    [RelayCommand]
    private void Add()
    {
        _engine.SetOperation(CalculatorOperation.Add);
        Refresh();
    }

    [RelayCommand]
    private void Subtract() => SetOperation(CalculatorOperation.Subtract);

    [RelayCommand]
    private void Multiply() => SetOperation(CalculatorOperation.Multiply);

    [RelayCommand]
    private void Divide() => SetOperation(CalculatorOperation.Divide);

    [RelayCommand]
    private void Backspace() { _engine.Backspace(); Refresh(); }

    [RelayCommand]
    private void ToggleSign() { _engine.ToggleSign(); Refresh(); }

    [RelayCommand]
    private void Equals()
    {
        _engine.Equals();
        Refresh();
    }

    private void EnterDigit(int digit)
    {
        _engine.Digit(digit);
        Refresh();
    }

    private void SetOperation(CalculatorOperation operation)
    {
        _engine.SetOperation(operation);
        Refresh();
    }
}
