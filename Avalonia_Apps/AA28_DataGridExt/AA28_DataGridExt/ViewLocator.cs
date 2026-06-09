using System;
using AA28_DataGridExt.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AA28_DataGridExt;

/// <summary>
/// Maps view models to their matching Avalonia views.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <inheritdoc />
    public Control? Build(object? param)
    {
        if (param is null)
        {
            return null;
        }

        var name = param.GetType().FullName?.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = name is null ? null : Type.GetType(name);

        if (type is not null && typeof(Control).IsAssignableFrom(type))
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock
        {
            Text = $"Not Found: {name}",
        };
    }

    /// <inheritdoc />
    public bool Match(object? data)
        => data is MainWindowViewModel or DataGridViewModel;
}
