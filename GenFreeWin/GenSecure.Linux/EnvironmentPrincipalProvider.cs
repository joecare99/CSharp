using GenSecure.Contracts;
using System;

namespace GenSecure.Linux;

/// <summary>
/// Provides a generic environment-based principal identifier for non-Windows platforms.
/// </summary>
public sealed class EnvironmentPrincipalProvider : ICurrentPrincipalProvider
{
    /// <inheritdoc />
    public string GetCurrentPrincipalId()
    {
        string sUserName = Environment.UserName;
        if (string.IsNullOrWhiteSpace(sUserName))
        {
            throw new InvalidOperationException("The current environment user name is not available.");
        }

        string sDomainName = Environment.UserDomainName;
        if (!string.IsNullOrWhiteSpace(sDomainName)
            && !string.Equals(sDomainName, Environment.MachineName, StringComparison.OrdinalIgnoreCase))
        {
            return $"user:{sDomainName}\\{sUserName}";
        }

        return $"user:{sUserName}";
    }
}
