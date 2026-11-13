using System;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

/// <summary>
/// Reports an unavailable profiling tool, failed launch, or unsuccessful diagnostic operation.
/// </summary>
public sealed class DotNetProfilingException : InvalidOperationException
{
    public DotNetProfilingException(
        string message,
        string toolName,
        int? exitCode = null,
        string? standardError = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ToolName = toolName;
        ExitCode = exitCode;
        StandardError = standardError;
    }

    public string ToolName { get; }

    public int? ExitCode { get; }

    public string? StandardError { get; }
}
