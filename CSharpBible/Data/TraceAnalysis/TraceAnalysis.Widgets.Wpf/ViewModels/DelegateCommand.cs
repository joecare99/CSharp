using System;
using System.Windows.Input;

namespace TraceAnalysis.Widgets.Wpf.ViewModels;

/// <summary>
/// Minimal delegate command for reusable WPF widgets.
/// </summary>
public sealed class DelegateCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        _execute();
    }

    /// <summary>
    /// Raises the command availability changed event.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
