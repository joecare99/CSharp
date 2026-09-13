using System;
using System.Collections.Generic;
using System.IO;

namespace Project.Explorer;

/// <summary>Contains the immutable result of loading or refreshing an explorer root.</summary>
public sealed record ProjectExplorerSnapshot
{
    /// <summary>Initializes a snapshot for one normalized explorer root.</summary>
    public ProjectExplorerSnapshot(string rootPath, IReadOnlyList<ProjectExplorerItem>? rootItems = null)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            throw new ArgumentException("An explorer root path is required.", nameof(rootPath));
        }

        RootPath = Path.GetFullPath(rootPath);
        RootItems = rootItems ?? Array.Empty<ProjectExplorerItem>();
    }

    /// <summary>Gets the normalized root path that was requested.</summary>
    public string RootPath { get; }

    /// <summary>Gets the deterministically ordered root items.</summary>
    public IReadOnlyList<ProjectExplorerItem> RootItems { get; }
}
