using System.Collections.Generic;

namespace OFBCreator.Core.Models;

/// <summary>Describes a person retained by the export filters and the privacy changes applied.</summary>
/// <param name="TargetId">The provider-qualified person target identifier.</param>
/// <param name="DisplayName">The person's name in the export view.</param>
/// <param name="Changes">The changed fields and their source/export values.</param>
public sealed record OFBPersonPrivacyPreview(
    string TargetId,
    string DisplayName,
    IReadOnlyList<OFBPersonPrivacyChange> Changes)
{
    /// <summary>Indicates whether any field differs from the imported source record.</summary>
    public bool HasChanges => Changes.Count > 0;
}
