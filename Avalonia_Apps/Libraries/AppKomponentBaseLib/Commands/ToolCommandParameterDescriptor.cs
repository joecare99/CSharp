using System;

namespace AppKomponentBaseLib.Commands;

/// <summary>
/// Describes a parameter that can be supplied to a tool-capable command.
/// </summary>
public sealed class ToolCommandParameterDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToolCommandParameterDescriptor"/> class.
    /// </summary>
    /// <param name="parameterName">The stable parameter identifier.</param>
    /// <param name="displayName">The host-facing display name for the parameter.</param>
    /// <param name="description">The optional localized parameter description.</param>
    /// <param name="isRequired">Indicates whether the parameter must be present.</param>
    /// <param name="defaultValue">The optional default value that should be used when omitted.</param>
    /// <param name="valueKind">The optional value kind or schema hint.</param>
    public ToolCommandParameterDescriptor(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        string? defaultValue = null,
        string? valueKind = null)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException("A stable parameter identifier is required.", nameof(parameterName));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A display name is required.", nameof(displayName));
        }

        ParameterName = parameterName.Trim();
        DisplayName = displayName.Trim();
        Description = description;
        IsRequired = isRequired;
        DefaultValue = string.IsNullOrWhiteSpace(defaultValue) ? null : defaultValue.Trim();
        ValueKind = string.IsNullOrWhiteSpace(valueKind) ? null : valueKind.Trim();
    }

    /// <summary>
    /// Gets the stable parameter identifier.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Gets the host-facing display name for the parameter.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Gets the optional parameter description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets a value indicating whether the parameter is required.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// Gets the optional default value associated with the parameter.
    /// </summary>
    public string? DefaultValue { get; }

    /// <summary>
    /// Gets the optional value-kind or schema hint for the parameter.
    /// </summary>
    public string? ValueKind { get; }
}
