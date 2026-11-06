using System;

namespace Property.Editor;

/// <summary>Describes the effective source and policy state of a property value.</summary>
public sealed class PropertyValueContext
{
    /// <summary>Creates source and policy metadata for one effective property value.</summary>
    public PropertyValueContext(
        PropertyScope scope,
        string? origin = null,
        bool isInherited = false,
        bool isPolicyLocked = false,
        string? readOnlyReason = null)
    {
        if (scope == PropertyScope.Unspecified && (origin is not null || isInherited || isPolicyLocked))
        {
            throw new ArgumentException("Value source metadata requires a declared scope.", nameof(scope));
        }

        if (isPolicyLocked && string.IsNullOrWhiteSpace(readOnlyReason))
        {
            throw new ArgumentException("A policy-locked value requires an explanation.", nameof(readOnlyReason));
        }

        Scope = scope;
        Origin = origin;
        IsInherited = isInherited;
        IsPolicyLocked = isPolicyLocked;
        ReadOnlyReason = readOnlyReason;
    }

    /// <summary>Gets the default metadata for values without a known source.</summary>
    public static PropertyValueContext Unspecified { get; } = new(PropertyScope.Unspecified);

    /// <summary>Gets the scope from which the effective value was resolved.</summary>
    public PropertyScope Scope { get; }

    /// <summary>Gets a human-readable source such as a file, profile, or policy.</summary>
    public string? Origin { get; }

    /// <summary>Gets whether the effective value is inherited from another scope.</summary>
    public bool IsInherited { get; }

    /// <summary>Gets whether an authoritative policy prevents editing the value.</summary>
    public bool IsPolicyLocked { get; }

    /// <summary>Gets the explanation for a policy lock or other source-level restriction.</summary>
    public string? ReadOnlyReason { get; }
}
