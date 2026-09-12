using System;

namespace ConsoleLib.Showcase.Apps;

public enum FileDialogStatus
{
    Accepted,
    Cancelled,
    Unavailable,
    Failed
}

/// <summary>Explicit result from a host file-dialog capability.</summary>
public sealed class FileDialogResult
{
    private FileDialogResult(FileDialogStatus status, string? path, string? message)
    {
        Status = status;
        IsAvailable = status is not FileDialogStatus.Unavailable;
        IsAccepted = status is FileDialogStatus.Accepted;
        Path = path;
        Message = message;
    }

    public FileDialogStatus Status { get; }
    public bool IsAvailable { get; }
    public bool IsAccepted { get; }
    public bool IsFailed => Status is FileDialogStatus.Failed;
    public string? Path { get; }
    public string? Message { get; }

    public static FileDialogResult Accepted(string path) =>
        string.IsNullOrWhiteSpace(path) ? throw new ArgumentException("A path is required.", nameof(path)) :
        new(FileDialogStatus.Accepted, path, null);
    public static FileDialogResult Cancelled() => new(FileDialogStatus.Cancelled, null, null);
    public static FileDialogResult Unavailable(string message) => CreateMessage(FileDialogStatus.Unavailable, message);
    public static FileDialogResult Failed(string message) => CreateMessage(FileDialogStatus.Failed, message);

    private static FileDialogResult CreateMessage(FileDialogStatus status, string message) =>
        string.IsNullOrWhiteSpace(message) ? throw new ArgumentException("A message is required.", nameof(message)) :
        new(status, null, message);
}
