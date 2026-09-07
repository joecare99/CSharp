using System;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Immutable read-only projection exposed to search and detail clients.</summary>
public sealed record NoticeProjection(
    long Id,
    string OrderNumber,
    string? Keyword,
    string? FamilyName,
    string? GivenName,
    string? MaidenName,
    string? Title,
    string? Sex,
    DateTime? PublishedOn,
    DateTime? BirthDate,
    DateQualification BirthQualification,
    DateTime? DeathDate,
    DateQualification DeathQualification,
    DateTime? BurialDate,
    string? Place,
    AdvertisementCategory Category,
    string? Text,
    string? Path,
    string? PdfFile,
    string? PngFile,
    long? LinkedNoticeId,
    string? ProfileImage,
    int ProfileImageCount,
    DateTime? TimeStamp)
{
    public string Description =>
        $"{FamilyName ?? "< >"}, {GivenName ?? "< >"}" +
        (string.IsNullOrWhiteSpace(MaidenName) ? string.Empty : " geb. " + MaidenName);

    public static NoticeProjection FromDomain(DeathNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);
        return new NoticeProjection(
            notice.Id, notice.OrderNumber, notice.Keyword, notice.FamilyName,
            notice.GivenName, notice.MaidenName, notice.Title, notice.Sex,
            notice.PublishedOn, notice.BirthDate, notice.BirthQualification,
            notice.DeathDate, notice.DeathQualification, notice.BurialDate,
            notice.Place, notice.Category, notice.Text, notice.Path, notice.PdfFile,
            notice.PngFile, notice.LinkedNoticeId, notice.ProfileImage,
            notice.ProfileImageCount, notice.TimeStamp);
    }

    public DeathNotice ToDomain() => new()
    {
        Id = Id,
        OrderNumber = OrderNumber,
        Keyword = Keyword,
        FamilyName = FamilyName,
        GivenName = GivenName,
        MaidenName = MaidenName,
        Title = Title,
        Sex = Sex,
        PublishedOn = PublishedOn,
        BirthDate = BirthDate,
        BirthQualification = BirthQualification,
        DeathDate = DeathDate,
        DeathQualification = DeathQualification,
        BurialDate = BurialDate,
        Place = Place,
        Category = Category,
        Text = Text,
        Path = Path,
        PdfFile = PdfFile,
        PngFile = PngFile,
        LinkedNoticeId = LinkedNoticeId,
        ProfileImage = ProfileImage,
        ProfileImageCount = ProfileImageCount,
        TimeStamp = TimeStamp,
    };
}
