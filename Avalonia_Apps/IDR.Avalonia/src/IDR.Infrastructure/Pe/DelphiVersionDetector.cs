using IDR.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace IDR.Infrastructure.Pe;

public sealed class DelphiVersionDetector : IDelphiVersionDetector
{
    private static readonly DelphiVersion[] SupportedVersions =
    [
        DelphiVersion.Delphi2,
        DelphiVersion.Delphi3,
        DelphiVersion.Delphi4,
        DelphiVersion.Delphi5,
        DelphiVersion.Delphi6,
        DelphiVersion.Delphi7,
        DelphiVersion.Delphi8,
        DelphiVersion.Delphi2005,
        DelphiVersion.Delphi2006,
        DelphiVersion.Delphi2007,
        DelphiVersion.Delphi2009,
        DelphiVersion.Delphi2010,
        DelphiVersion.DelphiXE1,
        DelphiVersion.DelphiXE2,
        DelphiVersion.DelphiXE3,
        DelphiVersion.DelphiXE4
    ];

    public DelphiVersionDetection Detect(PeImage image)
    {
        ArgumentNullException.ThrowIfNull(image);

        HashSet<DelphiVersion> importedVersions = [];
        foreach (PeImportModule import in image.Imports)
        {
            if (TryGetRuntimeVersion(import.Name, out DelphiVersion version))
            {
                importedVersions.Add(version);
            }
        }

        if (importedVersions.Count > 0)
        {
            return CreateDetection(importedVersions);
        }

        string text = Encoding.ASCII.GetString(image.Image.Span);
        HashSet<DelphiVersion> candidates = [];

        foreach ((string marker, DelphiVersion version) in Markers)
        {
            if (text.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                candidates.Add(version);
            }
        }

        return candidates.Count > 0
            ? CreateDetection(candidates)
            : new DelphiVersionDetection(DelphiVersion.Unknown, SupportedVersions, true);
    }

    private static DelphiVersionDetection CreateDetection(HashSet<DelphiVersion> candidates)
    {
        DelphiVersion[] ordered = SupportedVersions
            .Where(candidates.Contains)
            .ToArray();
        if (ordered.Length == 1)
        {
            return new DelphiVersionDetection(ordered[0], ordered, false);
        }

        return new DelphiVersionDetection(DelphiVersion.Unknown, ordered, true);
    }

    private static bool TryGetRuntimeVersion(string moduleName, out DelphiVersion version)
    {
        version = DelphiVersion.Unknown;
        if (moduleName.Contains('\\') || moduleName.Contains('/'))
        {
            return false;
        }

        string extension;
        if (moduleName.EndsWith(".bpl", StringComparison.OrdinalIgnoreCase))
        {
            extension = ".bpl";
        }
        else if (moduleName.EndsWith(".dpl", StringComparison.OrdinalIgnoreCase))
        {
            extension = ".dpl";
        }
        else
        {
            return false;
        }

        string libraryName = moduleName[..^extension.Length];
        string versionText;
        if (libraryName.StartsWith("vcl", StringComparison.OrdinalIgnoreCase)
            || libraryName.StartsWith("rtl", StringComparison.OrdinalIgnoreCase))
        {
            versionText = libraryName[3..];
        }
        else
        {
            return false;
        }

        if (!int.TryParse(
            versionText,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out int runtimeVersion))
        {
            return false;
        }

        version = runtimeVersion switch
        {
            30 => DelphiVersion.Delphi3,
            40 => DelphiVersion.Delphi4,
            50 => DelphiVersion.Delphi5,
            60 => DelphiVersion.Delphi6,
            70 => DelphiVersion.Delphi7,
            80 => DelphiVersion.Delphi8,
            90 => DelphiVersion.Delphi2005,
            100 => DelphiVersion.Delphi2006,
            110 => DelphiVersion.Delphi2007,
            120 => DelphiVersion.Delphi2009,
            140 => DelphiVersion.Delphi2010,
            150 => DelphiVersion.DelphiXE1,
            160 => DelphiVersion.DelphiXE2,
            170 => DelphiVersion.DelphiXE3,
            180 => DelphiVersion.DelphiXE4,
            _ => DelphiVersion.Unknown
        };
        return version != DelphiVersion.Unknown;
    }

    private static IReadOnlyList<(string Marker, DelphiVersion Version)> Markers =>
    [
        ("vcl30.dpl", DelphiVersion.Delphi3),
        ("vcl30.bpl", DelphiVersion.Delphi3),
        ("vcl40.dpl", DelphiVersion.Delphi4),
        ("vcl40.bpl", DelphiVersion.Delphi4),
        ("vcl50.dpl", DelphiVersion.Delphi5),
        ("vcl50.bpl", DelphiVersion.Delphi5),
        ("vcl60.dpl", DelphiVersion.Delphi6),
        ("vcl60.bpl", DelphiVersion.Delphi6),
        ("vcl70.dpl", DelphiVersion.Delphi7),
        ("vcl70.bpl", DelphiVersion.Delphi7),
        ("vcl80.dpl", DelphiVersion.Delphi8),
        ("vcl80.bpl", DelphiVersion.Delphi8),
        ("vcl90.dpl", DelphiVersion.Delphi2005),
        ("vcl90.bpl", DelphiVersion.Delphi2005),
        ("vcl100.dpl", DelphiVersion.Delphi2006),
        ("vcl100.bpl", DelphiVersion.Delphi2006),
        ("vcl110.dpl", DelphiVersion.Delphi2007),
        ("vcl110.bpl", DelphiVersion.Delphi2007),
        ("vcl120.dpl", DelphiVersion.Delphi2009),
        ("vcl120.bpl", DelphiVersion.Delphi2009),
        ("vcl140.dpl", DelphiVersion.Delphi2010),
        ("vcl140.bpl", DelphiVersion.Delphi2010),
        ("vcl150.dpl", DelphiVersion.DelphiXE1),
        ("vcl150.bpl", DelphiVersion.DelphiXE1),
        ("vcl160.dpl", DelphiVersion.DelphiXE2),
        ("vcl160.bpl", DelphiVersion.DelphiXE2),
        ("vcl170.dpl", DelphiVersion.DelphiXE3),
        ("vcl170.bpl", DelphiVersion.DelphiXE3),
        ("vcl180.dpl", DelphiVersion.DelphiXE4),
        ("vcl180.bpl", DelphiVersion.DelphiXE4),
        ("rtl30.dpl", DelphiVersion.Delphi3),
        ("rtl30.bpl", DelphiVersion.Delphi3),
        ("rtl40.dpl", DelphiVersion.Delphi4),
        ("rtl40.bpl", DelphiVersion.Delphi4),
        ("rtl50.dpl", DelphiVersion.Delphi5),
        ("rtl50.bpl", DelphiVersion.Delphi5),
        ("rtl80.dpl", DelphiVersion.Delphi8),
        ("rtl80.bpl", DelphiVersion.Delphi8),
        ("rtl110.dpl", DelphiVersion.Delphi2007),
        ("rtl110.bpl", DelphiVersion.Delphi2007),
        ("rtl140.dpl", DelphiVersion.Delphi2010),
        ("rtl140.bpl", DelphiVersion.Delphi2010),
        ("rtl150.dpl", DelphiVersion.DelphiXE1),
        ("rtl150.bpl", DelphiVersion.DelphiXE1),
        ("rtl160.dpl", DelphiVersion.DelphiXE2),
        ("rtl160.bpl", DelphiVersion.DelphiXE2),
        ("rtl170.dpl", DelphiVersion.DelphiXE3),
        ("rtl170.bpl", DelphiVersion.DelphiXE3),
        ("rtl180.dpl", DelphiVersion.DelphiXE4),
        ("rtl180.bpl", DelphiVersion.DelphiXE4),
        ("rtl60.bpl", DelphiVersion.Delphi6),
        ("rtl60.dpl", DelphiVersion.Delphi6),
        ("rtl70.bpl", DelphiVersion.Delphi7),
        ("rtl70.dpl", DelphiVersion.Delphi7),
        ("rtl90.dpl", DelphiVersion.Delphi2005),
        ("rtl90.bpl", DelphiVersion.Delphi2005),
        ("rtl100.dpl", DelphiVersion.Delphi2006),
        ("rtl100.bpl", DelphiVersion.Delphi2006),
        ("rtl120.dpl", DelphiVersion.Delphi2009),
        ("rtl120.bpl", DelphiVersion.Delphi2009)
    ];
}
