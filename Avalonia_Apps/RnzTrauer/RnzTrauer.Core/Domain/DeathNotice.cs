using System;

namespace RnzTrauer.Core.Domain;

/// <summary>Editable projection of one row of the legacy <c>Anzeigen</c> table.</summary>
public sealed class DeathNotice
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? Keyword { get; set; }
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public string? MaidenName { get; set; }
    public string? Title { get; set; }
    public string? Sex { get; set; }
    public DateTime? PublishedOn { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateQualification BirthQualification { get; set; }
    public DateTime? DeathDate { get; set; }
    public DateQualification DeathQualification { get; set; }
    public DateTime? BurialDate { get; set; }
    public string? Place { get; set; }
    public AdvertisementCategory Category { get; set; }
    public string? Text { get; set; }
    public string? Path { get; set; }
    public string? PdfFile { get; set; }
    public string? PngFile { get; set; }
    public long? LinkedNoticeId { get; set; }
    public string? ProfileImage { get; set; }
    public int ProfileImageCount { get; set; }
    public DateTime? TimeStamp { get; set; }

    /// <summary>Returns the display format used by the legacy linked-record selector.</summary>
    public string Description => $"{FamilyName ?? "< >"}, {GivenName ?? "< >"}{(string.IsNullOrWhiteSpace(MaidenName) ? string.Empty : " geb. " + MaidenName)}";
}
