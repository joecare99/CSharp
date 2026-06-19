namespace Workbench.Builder.Core.Models.References;

/// <summary>
/// Represents a reference after the builder has attempted to resolve it to a concrete artifact.
/// </summary>
public sealed class ResolvedReferenceInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="ResolvedReferenceInfo"/>.
    /// </summary>
    /// <param name="kind">The resolved reference category.</param>
    /// <param name="displayName">The display name used in reports and diagnostics.</param>
    /// <param name="source">The original source expression, include value, or provider name.</param>
    /// <param name="resolvedPath">The resolved file system path, when available.</param>
    /// <param name="exists">A value indicating whether the resolved artifact exists on disk.</param>
    public ResolvedReferenceInfo(
        ReferenceKind kind,
        string displayName,
        string? source,
        string? resolvedPath,
        bool exists)
    {
        Kind = kind;
        DisplayName = displayName;
        Source = source;
        ResolvedPath = resolvedPath;
        Exists = exists;
    }

    /// <summary>
    /// Gets the resolved reference category.
    /// </summary>
    public ReferenceKind Kind { get; }

    /// <summary>
    /// Gets the display name used in reports and diagnostics.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the original source expression, include value, or provider name.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Gets the resolved file system path, when available.
    /// </summary>
    public string? ResolvedPath { get; }

    /// <summary>
    /// Gets a value indicating whether the resolved artifact exists on disk.
    /// </summary>
    public bool Exists { get; }
}
