using System;

namespace SharpHack.Persist;

/// <summary>
/// Indicates that a save file could not be accessed for reading or writing.
/// </summary>
public sealed class SaveFileAccessException : SavePersistenceException
{
    public SaveFileAccessException(string message)
        : base(message)
    {
    }

    public SaveFileAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
