using System;
using Avln_TestConsole.Controls;
using BaseLib.Interfaces;

namespace Avln_TestConsole.Interfaces;

/// <summary>
/// Extends <see cref="IConsole"/> with Avalonia-specific access needed by host code and tests.
/// </summary>
public interface IAvaloniaConsole : IConsole
{
    /// <summary>
    /// Gets the visual control that renders the console content.
    /// </summary>
    AvaloniaConsoleControl Control { get; }

    /// <summary>
    /// Gets the exported encoded content.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// Enqueues a key that will later be returned by <see cref="IConsole.ReadKey"/>.
    /// </summary>
    /// <param name="keyInfo">The key information to enqueue.</param>
    void EnqueueKey(ConsoleKeyInfo keyInfo);

    /// <summary>
    /// Enqueues a complete line for scripted input.
    /// </summary>
    /// <param name="line">The line to enqueue.</param>
    void EnqueueLine(string line);

    /// <summary>
    /// Gets or sets an optional timeout for <see cref="IConsole.ReadLine"/>.
    /// </summary>
    TimeSpan? ReadLineTimeout { get; set; }

    /// <summary>
    /// Writes a line using the specified colors and restores the previous colors afterwards.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="foregroundColor">The foreground color to apply.</param>
    /// <param name="backgroundColor">The background color to apply.</param>
    void WriteLine(string? text, ConsoleColor foregroundColor, ConsoleColor backgroundColor);
}
