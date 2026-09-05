using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Mirrors the deterministic Pascal media planning rule without performing
/// file access, downloads, OCR, or UI actions.
/// </summary>
public static class NoticeMediaPolicy
{
    public static NoticeMediaPlan Evaluate(NoticeMediaInput input)
    {
        if (!input.DirectoryAvailable)
        {
            return new NoticeMediaPlan(false, false, false, false, false, false, false, false);
        }

        var pdfFound = input.PdfAvailable && input.PdfSize > 0;
        var xmlFound = input.XmlAvailable;
        var pngFound = input.PngAvailable && input.PngSize > 0;
        var useAlternatePng = false;

        if (!pngFound && input.AlternatePngAvailable && input.AlternatePngSize > 0)
        {
            pngFound = true;
            useAlternatePng = true;
        }

        var autoParseText =
            input.AutoText &&
            !input.TextIsNull &&
            ((input.PlaceIsNull && input.BurialIsNull) ||
             (input.DeathIsNull && input.BirthIsNull && input.BurialIsNull) ||
             (input.GenderIsNull && (input.Rubrik < (int)AdvertisementCategory.Announcement)) ||
             (input.BurialIsNull && input.Rubrik == (int)AdvertisementCategory.DeathNotice) ||
             (input.PlaceIsNull && input.Rubrik == (int)AdvertisementCategory.DeathNotice) ||
             (input.LinkIsNull && !input.RubrikIsNull &&
              (input.Rubrik % 100 is 55 or 60 or 70 or 80)));

        var autoLoadXml = !autoParseText && xmlFound && input.AutoText && input.TextIsNull;
        var autoProcessPdf = !autoParseText && !autoLoadXml &&
            pdfFound && input.AutoText && input.TextIsNull;

        return new NoticeMediaPlan(
            pdfFound,
            xmlFound,
            pngFound,
            useAlternatePng,
            autoParseText,
            autoLoadXml,
            autoProcessPdf,
            input.AutoImage && pngFound);
    }
}
