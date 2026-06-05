namespace AA98_AvlnCodeStudio.Base.AI.Models;

/// <summary>
/// Provides optional provider-agnostic settings for text generation requests.
/// </summary>
public sealed class AICompletionOptions
{
    /// <summary>
    /// Gets or sets the optional model identifier.
    /// </summary>
    public string? ModelId { get; set; }

    /// <summary>
    /// Gets or sets the optional temperature.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or sets the optional maximum token count.
    /// </summary>
    public int? MaxTokens { get; set; }
}