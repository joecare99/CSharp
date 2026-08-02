using System;

namespace AppKomponentBaseLib.Commands;

/// <summary>
/// Describes a result produced by a tool-capable command.
/// </summary>
public sealed class ToolCommandResultDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolCommandResultDescriptor"/> class.
    /// </summary>
    /// <param name="resultName">The stable result identifier.</param>
    /// <param name="displayName">The host-facing display name for the result.</param>
    /// <param name="description">The optional localized result description.</param>
    /// <param name="valueKind">The optional value-kind or schema hint for the result.</param>
    public ToolCommandResultDescriptor(
        string resultName,
        string displayName,
        string? description = null,
        string? valueKind = null)
    {
        if (string.IsNullOrWhiteSpace(resultName))
        {
            throw new ArgumentException("A stable result identifier is required.", nameof(resultName));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A display name is required.", nameof(displayName));
        }

        ResultName = resultName.Trim();
        DisplayName = displayName.Trim();
        Description = description;
        ValueKind = string.IsNullOrWhiteSpace(valueKind) ? null : valueKind.Trim();
    }

    /// <summary>
    /// Gets the stable result identifier.
    /// </summary>
    public string ResultName { get; }

    /// <summary>
    /// Gets the host-facing display name for the result.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the optional result description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the optional value-kind or schema hint for the result.
    /// </summary>
    public string? ValueKind { get; }
}
