using System;

namespace SharpHack.Persist;

/// <summary>
/// Indicates that a save file uses an unsupported format or save-model version.
/// </summary>
public sealed class SaveFileIncompatibleException : SavePersistenceException
{
    public SaveFileIncompatibleException(string message)
        : base(message)
    {
    }

    public SaveFileIncompatibleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
