using System;

namespace AA98_AvlnCodeStudio.Base.Profiling.Models;

/// <summary>
/// A line emitted by a running .NET profiling tool.
/// </summary>
public sealed class DotNetProfilingOutputEventArgs : EventArgs
{
    public DotNetProfilingOutputEventArgs(string text, bool isError)
    {
        Text = text;
        IsError = isError;
    }

    public string Text { get; }

    public bool IsError { get; }
}
