using System.Collections.Generic;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Workflows;

public sealed class HejInspection
{
    internal HejInspection(HejDocument document)
    {
        Document = document;
        RecordCounts = new Dictionary<HejSection, int>
        {
            [HejSection.Individuals] = document.Individuals.Count,
            [HejSection.Marriages] = document.Marriages.Count,
            [HejSection.Adoptions] = document.Adoptions.Count,
            [HejSection.Places] = document.Places.Count,
            [HejSection.Sources] = document.Sources.Count
        };
    }

    public HejDocument Document { get; }

    public IReadOnlyDictionary<HejSection, int> RecordCounts { get; }
}
