using ConsoleLib.Interfaces;
using System;

namespace ConsoleLib;

/// <summary>
/// Provides access to the active widget set used by existing ConsoleLib controls.
/// </summary>
public static class WidgetSetContext
{
    private static IWidgetSet? _current;

    /// <summary>
    /// Gets or sets the active widget set.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the widget set is requested before initialization.</exception>
    public static IWidgetSet Current
    {
        get => _current ?? throw new InvalidOperationException("No widget set is configured.");
        set => _current = value ?? throw new ArgumentNullException(nameof(value));
    }
}
