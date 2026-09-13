using System;
using System.Collections.Generic;
using System.IO;

namespace Project.Explorer;

/// <summary>
/// Represents an immutable project, folder, or file in an explorer hierarchy.
/// </summary>
public sealed record ProjectExplorerItem
{
    /// <summary>Initializes an explorer item with a stable identity and full path.</summary>
    public ProjectExplorerItem(
        string id,
        string name,
        string path,
        ProjectExplorerItemKind kind,
        bool isAvailable = true,
        IReadOnlyList<ProjectExplorerItem>? children = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("An explorer item identifier is required.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("An explorer item name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("An explorer item path is required.", nameof(path));
        }

        Id = id;
        Name = name;
        Path = System.IO.Path.GetFullPath(path);
        Kind = kind;
        IsAvailable = isAvailable;
        Children = children ?? Array.Empty<ProjectExplorerItem>();
    }

    /// <summary>Gets the stable identity used for selection and expansion state.</summary>
    public string Id { get; }

    /// <summary>Gets the display name supplied by the source adapter.</summary>
    public string Name { get; }

    /// <summary>Gets the normalized full path represented by this item.</summary>
    public string Path { get; }

    /// <summary>Gets the project, folder, or file role.</summary>
    public ProjectExplorerItemKind Kind { get; }

    /// <summary>Gets whether the backing path was accessible to the source adapter.</summary>
    public bool IsAvailable { get; }

    /// <summary>Gets deterministically ordered child items.</summary>
    public IReadOnlyList<ProjectExplorerItem> Children { get; }
}
