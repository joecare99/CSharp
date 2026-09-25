using System;
using System.Collections.Generic;
using System.Linq;

namespace Osb.Core.Selection;

public enum SelectionInclusionMode
{
    IncludeOnly,
    ExcludeSelected
}

public sealed record FamilySelectionScope
{
    public FamilySelectionScope(
        int initialPersonId,
        int initialFamilyId,
        IEnumerable<int> selectedPersonIds,
        IEnumerable<int> selectedFamilyIds,
        SelectionInclusionMode inclusionMode)
    {
        if (initialPersonId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialPersonId));
        }

        if (initialFamilyId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialFamilyId));
        }

        if (selectedPersonIds == null)
        {
            throw new ArgumentNullException(nameof(selectedPersonIds));
        }

        if (selectedFamilyIds == null)
        {
            throw new ArgumentNullException(nameof(selectedFamilyIds));
        }

        InitialPersonId = initialPersonId;
        InitialFamilyId = initialFamilyId;
        SelectedPersonIds = selectedPersonIds.Distinct().OrderBy(id => id).ToArray();
        SelectedFamilyIds = selectedFamilyIds.Distinct().OrderBy(id => id).ToArray();
        InclusionMode = inclusionMode;
    }

    public int InitialPersonId { get; }

    public int InitialFamilyId { get; }

    public IReadOnlyList<int> SelectedPersonIds { get; }

    public IReadOnlyList<int> SelectedFamilyIds { get; }

    public SelectionInclusionMode InclusionMode { get; }

    public bool IsPersonSelected(int personId)
    {
        if (personId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(personId));
        }

        return InclusionMode switch
        {
            SelectionInclusionMode.IncludeOnly => SelectedPersonIds.Contains(personId),
            SelectionInclusionMode.ExcludeSelected => !SelectedPersonIds.Contains(personId),
            _ => throw new ArgumentOutOfRangeException(nameof(InclusionMode))
        };
    }

    public bool IsFamilySelected(int familyId)
    {
        if (familyId < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(familyId));
        }

        return InclusionMode switch
        {
            SelectionInclusionMode.IncludeOnly => SelectedFamilyIds.Contains(familyId),
            SelectionInclusionMode.ExcludeSelected => !SelectedFamilyIds.Contains(familyId),
            _ => throw new ArgumentOutOfRangeException(nameof(InclusionMode))
        };
    }
}
