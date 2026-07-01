using CommunityToolkit.Mvvm.Input;
using System;

namespace AA98.DevOpsPlanning.Host.ViewModels;

/// <summary>
/// Represents a bindable file-menu command entry.
/// </summary>
public sealed class FileMenuCommandViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileMenuCommandViewModel"/> class.
    /// </summary>
    /// <param name="header">The display header.</param>
    /// <param name="command">The executable command.</param>
    public FileMenuCommandViewModel(string header, IRelayCommand command)
    {
        Header = string.IsNullOrWhiteSpace(header) ? throw new ArgumentException("A command header is required.", nameof(header)) : header;
        Command = command ?? throw new ArgumentNullException(nameof(command));
    }

    /// <summary>
    /// Gets the menu header text.
    /// </summary>
    public string Header { get; }

    /// <summary>
    /// Gets the menu command binding.
    /// </summary>
    public IRelayCommand Command { get; }
}
