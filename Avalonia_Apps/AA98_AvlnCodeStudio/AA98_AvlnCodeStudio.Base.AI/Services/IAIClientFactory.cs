namespace AA98_AvlnCodeStudio.Base.AI.Services;

/// <summary>
/// Creates provider-agnostic AI client instances.
/// </summary>
public interface IAIClientFactory
{
    /// <summary>
    /// Creates an AI client instance.
    /// </summary>
    /// <returns>The created client.</returns>
    IAIClient Create();
}