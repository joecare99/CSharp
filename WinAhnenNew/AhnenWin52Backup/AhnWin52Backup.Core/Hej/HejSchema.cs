using System;

namespace AhnWin52Backup.Core.Hej;

public static class HejSchema
{
    public static int GetFieldCount(HejSection section) => section switch
    {
        HejSection.Individuals => 51,
        HejSection.Marriages => 22,
        HejSection.Adoptions => 3,
        HejSection.Places => 13,
        HejSection.Sources => 11,
        _ => throw new ArgumentOutOfRangeException(nameof(section), section, "Unknown HEJ section.")
    };

    public static int GetMinimumLegacyFieldCount(HejSection section) => section switch
    {
        HejSection.Individuals => 50,
        _ => GetFieldCount(section)
    };

    public static string? GetHeader(HejSection section) => section switch
    {
        HejSection.Individuals => null,
        HejSection.Marriages => "mrg",
        HejSection.Adoptions => "adop",
        HejSection.Places => "ortv",
        HejSection.Sources => "quellv",
        _ => throw new ArgumentOutOfRangeException(nameof(section), section, "Unknown HEJ section.")
    };
}
