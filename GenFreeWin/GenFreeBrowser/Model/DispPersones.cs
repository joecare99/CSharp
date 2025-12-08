using System;

namespace GenFreeBrowser.Model;

public record DispPersones(int Id = 0, string Vollname = "", DateTime? GeburtsDatum = null, DateTime? SterbeDatum = null)
{
    public int LebensAlter => (GeburtsDatum.HasValue ? (SterbeDatum ?? DateTime.Today).Year - GeburtsDatum.Value.Year : 0);
}
