using System;

namespace AA98_AvlnCodeStudio.Config.Secrets;

/// <summary>Identifies one credential without containing its secret value.</summary>
public sealed record CredentialId
{
    /// <summary>Creates a stable service/name identifier and optional provider-defined scope.</summary>
    public CredentialId(string serviceId, string name, string? scopeId = null)
    {
        ServiceId = ValidateSegment(serviceId, nameof(serviceId));
        Name = ValidateSegment(name, nameof(name));
        ScopeId = scopeId is null ? null : ValidateSegment(scopeId, nameof(scopeId));
    }

    /// <summary>Gets the stable service or application identifier.</summary>
    public string ServiceId { get; }

    /// <summary>Gets the credential name within the service.</summary>
    public string Name { get; }

    /// <summary>Gets the optional provider-defined scope.</summary>
    public string? ScopeId { get; }

    private static string ValidateSegment(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (!string.Equals(value, value.Trim(), StringComparison.Ordinal)
            || value.IndexOfAny(['\r', '\n', '\0']) >= 0)
        {
            throw new ArgumentException("Credential identifiers must be trimmed and contain no control delimiters.", parameterName);
        }

        return value;
    }
}
