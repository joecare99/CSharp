using System;
using System.Collections.Generic;
using System.Linq;

namespace AppKomponentBaseLib.Context;

/// <summary>
/// Describes a normalized context target that can participate in context-sensitive command evaluation.
/// </summary>
public sealed class AppContextTarget
{
    private readonly string[] _roles;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppContextTarget"/> class.
    /// </summary>
    /// <param name="targetType">The stable target type identifier.</param>
    /// <param name="targetId">The optional stable target identifier.</param>
    /// <param name="roles">The optional normalized roles exposed by the target.</param>
    public AppContextTarget(string targetType, string? targetId = null, IEnumerable<string>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(targetType))
        {
            throw new ArgumentException("A target type is required.", nameof(targetType));
        }

        var normalizedRoles = roles?
            .Where(static role => !string.IsNullOrWhiteSpace(role))
            .Select(static role => role.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        TargetType = targetType.Trim();
        TargetId = string.IsNullOrWhiteSpace(targetId) ? null : targetId.Trim();
        _roles = normalizedRoles;
    }

    /// <summary>
    /// Gets the stable target type identifier.
    /// </summary>
    public string TargetType { get; }

    /// <summary>
    /// Gets the optional stable target identifier.
    /// </summary>
    public string? TargetId { get; }

    /// <summary>
    /// Gets the normalized target roles.
    /// </summary>
    public IReadOnlyList<string> Roles => _roles;
}
