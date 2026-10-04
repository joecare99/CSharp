namespace OFBCreator.Projects.Models;

/// <summary>A persisted editorial choice for a merge candidate identified by stable family records.</summary>
public sealed class OFBGroupingDecision
{
    /// <summary>Stable project-local identifier for editor selection and diagnostics.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("D");

    /// <summary>Project-wide ordering used to resolve multiple manual group labels.</summary>
    public int Order { get; set; }

    /// <summary>Provider-qualified stable family target anchoring the left surname group.</summary>
    public string LeftFamilyTargetId { get; set; } = string.Empty;

    /// <summary>Provider-qualified stable family target anchoring the right surname group.</summary>
    public string RightFamilyTargetId { get; set; } = string.Empty;

    /// <summary>Editorial choice: acceptMerge, rejectMerge, or manualMerge.</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Required display group label for a manualMerge decision.</summary>
    public string? GroupName { get; set; }
}
