using System;
using System.Security.Principal;

namespace GenSecure.Core;

internal static class WindowsIdentityUtilities
{
    public static string GetCurrentUserSid()
    {
        using WindowsIdentity? identity = WindowsIdentity.GetCurrent();
        if (identity is null || !identity.IsAuthenticated || identity.User is null)
        {
            throw new InvalidOperationException("The current Windows user is not authenticated.");
        }

        return identity.User.Value;
    }
}
