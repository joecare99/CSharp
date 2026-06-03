namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Identifies the role of a message in an AI interaction.
/// </summary>
public enum AIMessageRole
{
    /// <summary>
    /// Represents a system-level instruction.
    /// </summary>
    System,

    /// <summary>
    /// Represents a user message.
    /// </summary>
    User,

    /// <summary>
    /// Represents an assistant message.
    /// </summary>
    Assistant,
}