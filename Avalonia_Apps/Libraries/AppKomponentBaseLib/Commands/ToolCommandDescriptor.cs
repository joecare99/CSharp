using System;
using System.Collections.Generic;
using System.Linq;

namespace AppKomponentBaseLib.Commands;

/// <summary>
/// Describes a command that can be surfaced as a tool in a host-neutral workflow.
/// </summary>
public sealed class ToolCommandDescriptor
{
    private readonly ToolCommandParameterDescriptor[] _parameters;
    private readonly ToolCommandResultDescriptor[] _results;
    private readonly string[] _requiredContextKinds;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolCommandDescriptor"/> class.
    /// </summary>
    /// <param name="commandId">The stable command identifier.</param>
    /// <param name="displayTitle">The host-facing display title.</param>
    /// <param name="parameters">The optional tool parameters.</param>
    /// <param name="results">The optional tool results.</param>
    /// <param name="requiredContextKinds">The optional context kinds required for execution.</param>
    /// <param name="description">The optional localized description.</param>
    /// <param name="requiresConsent">Indicates whether tool execution requires consent.</param>
    /// <param name="safetyLevel">The safety level associated with the tool.</param>
    public ToolCommandDescriptor(
        string commandId,
        string displayTitle,
        IEnumerable<ToolCommandParameterDescriptor>? parameters = null,
        IEnumerable<ToolCommandResultDescriptor>? results = null,
        IEnumerable<string>? requiredContextKinds = null,
        string? description = null,
        bool requiresConsent = false,
        ToolCommandSafetyLevel safetyLevel = ToolCommandSafetyLevel.Unspecified)
    {
        if (string.IsNullOrWhiteSpace(commandId))
        {
            throw new ArgumentException("A stable command identifier is required.", nameof(commandId));
        }

        if (string.IsNullOrWhiteSpace(displayTitle))
        {
            throw new ArgumentException("A display title is required.", nameof(displayTitle));
        }

        var normalizedParameters = parameters?.ToArray() ?? Array.Empty<ToolCommandParameterDescriptor>();
        if (normalizedParameters.Any(static parameter => parameter is null))
        {
            throw new ArgumentException("Parameters cannot contain null entries.", nameof(parameters));
        }

        var normalizedResults = results?.ToArray() ?? Array.Empty<ToolCommandResultDescriptor>();
        if (normalizedResults.Any(static result => result is null))
        {
            throw new ArgumentException("Results cannot contain null entries.", nameof(results));
        }

        var normalizedContextKinds = requiredContextKinds?
            .Where(static contextKind => !string.IsNullOrWhiteSpace(contextKind))
            .Select(static contextKind => contextKind.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? Array.Empty<string>();

        CommandId = commandId.Trim();
        DisplayTitle = displayTitle.Trim();
        Description = description;
        RequiresConsent = requiresConsent;
        SafetyLevel = safetyLevel;
        _parameters = normalizedParameters;
        _results = normalizedResults;
        _requiredContextKinds = normalizedContextKinds;
    }

    /// <summary>
    /// Gets the stable command identifier.
    /// </summary>
    public string CommandId { get; }

    /// <summary>
    /// Gets the host-facing display title.
    /// </summary>
    public string DisplayTitle { get; }

    /// <summary>
    /// Gets the optional localized description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets a value indicating whether tool execution requires explicit consent.
    /// </summary>
    public bool RequiresConsent { get; }

    /// <summary>
    /// Gets the safety level associated with the tool.
    /// </summary>
    public ToolCommandSafetyLevel SafetyLevel { get; }

    /// <summary>
    /// Gets the tool parameters.
    /// </summary>
    public IReadOnlyList<ToolCommandParameterDescriptor> Parameters => _parameters;

    /// <summary>
    /// Gets the tool results.
    /// </summary>
    public IReadOnlyList<ToolCommandResultDescriptor> Results => _results;

    /// <summary>
    /// Gets the optional context kinds required for execution.
    /// </summary>
    public IReadOnlyList<string> RequiredContextKinds => _requiredContextKinds;
}

/// <summary>
/// Identifies the sensitivity level for a tool-capable command.
/// </summary>
public enum ToolCommandSafetyLevel
{
    /// <summary>
    /// No explicit safety level was provided.
    /// </summary>
    Unspecified,

    /// <summary>
    /// Low risk tool behavior.
    /// </summary>
    Low,

    /// <summary>
    /// Moderate risk tool behavior.
    /// </summary>
    Medium,

    /// <summary>
    /// High risk tool behavior.
    /// </summary>
    High
}
