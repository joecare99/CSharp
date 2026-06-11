namespace AA40_Wizzard.Model;

/// <summary>
/// Represents a selectable item with an identifier and localized text.
/// </summary>
public sealed class ListEntry(int value, string text)
{
    /// <summary>
    /// Gets the numeric identifier.
    /// </summary>
    public int ID { get; } = value;

    /// <summary>
    /// Gets the display text.
    /// </summary>
    public string Text { get; } = text;

    /// <inheritdoc />
    public override string ToString() => Text;
}
