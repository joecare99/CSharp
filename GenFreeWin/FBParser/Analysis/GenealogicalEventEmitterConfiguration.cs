using System;

namespace FBParser.Analysis;

/// <summary>
/// Stores validation callbacks and event sinks used by <see cref="GenealogicalEventEmitter"/>.
/// </summary>
internal sealed class GenealogicalEventEmitterConfiguration
{
    /// <summary>
    /// Gets or initializes the callback that validates date fragments.
    /// </summary>
    public required Func<string, bool> IsValidDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that validates place fragments.
    /// </summary>
    public required Func<string, bool> IsValidPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that validates family-book references.
    /// </summary>
    public required Func<string, bool> IsValidReference { get; init; }

    /// <summary>
    /// Gets or initializes the callback that reports parser errors.
    /// </summary>
    public required Action<string> Error { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits the family-start event.
    /// </summary>
    public required Action<string> OnStartFamily { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits the entry-end event.
    /// </summary>
    public required Action<string> OnEntryEnd { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family-date events.
    /// </summary>
    public required Action<string, string, int> OnFamilyDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family-type events.
    /// </summary>
    public required Action<string, string, int> OnFamilyType { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family-data events.
    /// </summary>
    public required Action<string, string, int> OnFamilyData { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family-place events.
    /// </summary>
    public required Action<string, string, int> OnFamilyPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits family-member events.
    /// </summary>
    public required Action<string, string, int> OnFamilyIndiv { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-name events.
    /// </summary>
    public required Action<string, string, int> OnIndiName { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-date events.
    /// </summary>
    public required Action<string, string, int> OnIndiDate { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-place events.
    /// </summary>
    public required Action<string, string, int> OnIndiPlace { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-occupation events.
    /// </summary>
    public required Action<string, string, int> OnIndiOccu { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-relation events.
    /// </summary>
    public required Action<string, string, int> OnIndiRel { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-reference events.
    /// </summary>
    public required Action<string, string, int> OnIndiRef { get; init; }

    /// <summary>
    /// Gets or initializes the callback that emits individual-data events.
    /// </summary>
    public required Action<string, string, int> OnIndiData { get; init; }
}
