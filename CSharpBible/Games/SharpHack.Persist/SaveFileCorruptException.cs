using System;

namespace SharpHack.Persist;

/// <summary>
/// Indicates that a save file exists but could not be parsed or restored safely.
/// </summary>
public sealed class SaveFileCorruptException : SavePersistenceException
{
    public SaveFileCorruptException(string message)
        : base(message)
    {
    }

    public SaveFileCorruptException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
