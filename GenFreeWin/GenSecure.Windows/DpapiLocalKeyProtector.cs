using System;
using System.Security.Cryptography;
using GenSecure.Contracts;

namespace GenSecure.Windows;

/// <summary>
/// Protects the local master key by using Windows DPAPI for the current user.
/// </summary>
public sealed class DpapiLocalKeyProtector : ILocalKeyProtector
{
    /// <inheritdoc />
    public byte[] Protect(byte[] arrPlaintext)
    {
        ArgumentNullException.ThrowIfNull(arrPlaintext);
        return ProtectedData.Protect(arrPlaintext, optionalEntropy: null, DataProtectionScope.CurrentUser);
    }

    /// <inheritdoc />
    public byte[] Unprotect(byte[] arrProtectedData)
    {
        ArgumentNullException.ThrowIfNull(arrProtectedData);
        return ProtectedData.Unprotect(arrProtectedData, optionalEntropy: null, DataProtectionScope.CurrentUser);
    }
}
