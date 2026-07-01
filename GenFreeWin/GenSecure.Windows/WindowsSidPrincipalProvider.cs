using System;
using System.Security.Principal;
using GenSecure.Contracts;

namespace GenSecure.Windows;

/// <summary>
/// Provides the current Windows SID as the principal identifier.
/// </summary>
public sealed class WindowsSidPrincipalProvider : ICurrentPrincipalProvider
{
    /// <inheritdoc />
    public string GetCurrentPrincipalId()
    {
        using WindowsIdentity? identity = WindowsIdentity.GetCurrent();
        if (identity is null || !identity.IsAuthenticated || identity.User is null)
        {
            throw new InvalidOperationException("The current Windows user is not authenticated.");
        }

        return identity.User.Value;
    }
}
