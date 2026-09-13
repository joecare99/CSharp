using System;

namespace ConsoleLib.Interfaces;

/// <summary>UI-independent request for selecting a registered page.</summary>
public sealed record NavigationRequest
{
    public NavigationRequest(string pageName, object? selection = null, object? dataContext = null)
    {
        PageName = pageName ?? throw new ArgumentNullException(nameof(pageName));
        if (string.IsNullOrWhiteSpace(pageName))
            throw new ArgumentException("A page name is required.", nameof(pageName));
        Selection = selection;
        DataContext = dataContext;
    }

    public string PageName { get; }
    public object? Selection { get; }
    public object? DataContext { get; }
}
