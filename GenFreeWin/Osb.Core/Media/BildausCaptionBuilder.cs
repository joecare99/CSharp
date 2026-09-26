namespace Osb.Core.Media;

/// <summary>
/// Builds the legacy RTB caption and remark strings used by the UI adapter.
/// The legacy implementation prepended a newline before description and remark when present.
/// </summary>
public static class BildausCaptionBuilder
{
    /// <summary>
    /// Builds caption (description) and remark texts using the legacy convention (leading newline).
    /// Returns a tuple: (caption, remark).
    /// </summary>
    public static (string Caption, string Remark) Build(BildausEntry entry)
    {
        if (entry is null) return (string.Empty, string.Empty);

        var caption = string.Empty;
        var remark = string.Empty;

        if (!string.IsNullOrWhiteSpace(entry.Description))
        {
            caption = "\n" + entry.Description!.Trim();
        }

        if (!string.IsNullOrWhiteSpace(entry.Remark))
        {
            remark = "\n" + entry.Remark!.Trim();
        }

        return (caption, remark);
    }
}
