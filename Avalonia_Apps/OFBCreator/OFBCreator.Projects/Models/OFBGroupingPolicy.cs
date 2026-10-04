namespace OFBCreator.Projects.Models;

/// <summary>Controls when evidence-scored surname merge suggestions are accepted automatically.</summary>
public sealed class OFBGroupingPolicy
{
    /// <summary>Minimum evidence score from 0 through 100 required for automatic acceptance.</summary>
    public int AutoAcceptThreshold { get; set; } = 85;
}
