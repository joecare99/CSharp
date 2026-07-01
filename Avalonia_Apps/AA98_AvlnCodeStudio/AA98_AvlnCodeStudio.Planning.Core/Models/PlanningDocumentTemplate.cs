namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Represents a planning document template exposed by a provider.
/// </summary>
public sealed class PlanningDocumentTemplate
{
    public string TemplateId { get; set; } = string.Empty;

    public PlanningItemKind Kind { get; set; } = PlanningItemKind.Unknown;

    public string DisplayName { get; set; } = string.Empty;

    public string SuggestedFileNamePattern { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
}
