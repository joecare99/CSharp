using System;

namespace SharpHack.Persist;

/// <summary>
/// Base exception for SharpHack save persistence failures.
/// </summary>
public class SavePersistenceException : Exception
{
    public SavePersistenceException(string message)
        : base(message)
    {
    }

    public SavePersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
