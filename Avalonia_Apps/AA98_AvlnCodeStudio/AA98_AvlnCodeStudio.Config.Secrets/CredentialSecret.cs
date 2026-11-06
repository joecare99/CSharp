using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>
/// Holds a credential in mutable character memory that is cleared when disposed.
/// This limits this instance's lifetime only; callers and providers must avoid creating
/// unnecessary immutable string copies, must dispose every instance, and must not share it across threads.
/// </summary>
public sealed class CredentialSecret : IDisposable
{
    private readonly char[] _characters;
    private bool _disposed;

    /// <summary>Copies the supplied characters into this secret's owned buffer.</summary>
    public CredentialSecret(ReadOnlySpan<char> value)
    {
        _characters = value.ToArray();
    }

    /// <summary>Gets a read-only view of the secret while it is alive.</summary>
    public ReadOnlySpan<char> Reveal()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _characters;
    }

    /// <summary>Clears the owned character buffer.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        CryptographicOperations.ZeroMemory(MemoryMarshal.AsBytes(_characters.AsSpan()));
        _disposed = true;
    }

    /// <summary>Returns a redacted representation.</summary>
    public override string ToString() => "[REDACTED]";
}
