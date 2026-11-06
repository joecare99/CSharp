using System;

namespace Property.Editor;

/// <summary>
/// Describes which provider and persisted target supply a set of properties.
/// </summary>
public sealed class PropertyProviderContext
{
    /// <summary>Creates display and storage metadata for a property provider.</summary>
    public PropertyProviderContext(
        PropertyScope scope,
        string providerDisplayName,
        string targetDisplayName,
        string? storageLocation = null,
        string? readOnlyReason = null)
    {
        if (string.IsNullOrWhiteSpace(providerDisplayName))
        {
            throw new ArgumentException("A provider display name is required.", nameof(providerDisplayName));
        }

        if (string.IsNullOrWhiteSpace(targetDisplayName))
        {
            throw new ArgumentException("A target display name is required.", nameof(targetDisplayName));
        }

        Scope = scope;
        ProviderDisplayName = providerDisplayName;
        TargetDisplayName = targetDisplayName;
        StorageLocation = storageLocation;
        ReadOnlyReason = readOnlyReason;
    }

    /// <summary>Gets the ownership scope for values supplied by this provider.</summary>
    public PropertyScope Scope { get; }

    /// <summary>Gets the provider name intended for display.</summary>
    public string ProviderDisplayName { get; }

    /// <summary>Gets the selected target, such as a project or user profile.</summary>
    public string TargetDisplayName { get; }

    /// <summary>Gets the actual storage location when it is safe and useful to display.</summary>
    public string? StorageLocation { get; }

    /// <summary>Gets why values from this provider cannot be edited.</summary>
    public string? ReadOnlyReason { get; }
}
