namespace RnzTrauer.Core.Services;

/// <summary>Deterministic media facts supplied by an infrastructure adapter.</summary>
public sealed record NoticeMediaInput(
    bool DirectoryAvailable,
    bool PdfAvailable,
    bool XmlAvailable,
    bool PngAvailable,
    bool AlternatePngAvailable,
    bool TextIsNull,
    bool AutoText,
    bool AutoImage,
    bool PlaceIsNull,
    bool BurialIsNull,
    bool DeathIsNull,
    bool BirthIsNull,
    bool GenderIsNull,
    bool RubrikIsNull,
    bool LinkIsNull,
    long PdfSize,
    long PngSize,
    long AlternatePngSize,
    int Rubrik);
