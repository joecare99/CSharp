using System;

namespace ConsoleLib.Showcase.Apps;

/// <summary>Portable options for an open, save, or folder selection request.</summary>
public sealed class FileDialogRequest
{
    public FileDialogRequest(string title, string? initialPath = null, string? filter = null)
    {
        Title = string.IsNullOrWhiteSpace(title) ? throw new ArgumentException("A title is required.", nameof(title)) : title;
        InitialPath = initialPath;
        Filter = filter;
    }

    public string Title { get; }
    public string? InitialPath { get; }
    public string? Filter { get; }
}
