using System;

namespace SharpHack.Persist;

/// <summary>
/// Indicates that a requested save file does not exist.
/// </summary>
public sealed class SaveFileMissingException : SavePersistenceException
{
    public SaveFileMissingException(string message)
        : base(message)
    {
    }

    public SaveFileMissingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
