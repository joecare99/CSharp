using IDR.Core.Services;
using System;
using System.Collections.Generic;
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

        string text = Encoding.ASCII.GetString(image.Image.Span);
        HashSet<DelphiVersion> candidates = [];

        foreach ((string marker, DelphiVersion version) in Markers)
        {
            if (text.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                candidates.Add(version);
            }
        }

        if (candidates.Count == 1)
        {
            DelphiVersion selected = GetOnly(candidates);
            return new DelphiVersionDetection(selected, [selected], false);
        }

        if (candidates.Count > 1)
        {
            DelphiVersion[] ordered = SupportedVersions
                .Where(candidates.Contains)
                .ToArray();
            return new DelphiVersionDetection(DelphiVersion.Unknown, ordered, true);
        }

        return new DelphiVersionDetection(DelphiVersion.Unknown, SupportedVersions, true);
    }

    private static DelphiVersion GetOnly(HashSet<DelphiVersion> values)
    {
        foreach (DelphiVersion value in values)
        {
            return value;
        }

        throw new InvalidOperationException("The candidate set is empty.");
    }

    private static IReadOnlyList<(string Marker, DelphiVersion Version)> Markers =>
    [
        ("vcl30.dpl", DelphiVersion.Delphi3),
        ("vcl40.bpl", DelphiVersion.Delphi4),
        ("vcl60.bpl", DelphiVersion.Delphi6),
        ("vcl70.bpl", DelphiVersion.Delphi7),
        ("vcl90.bpl", DelphiVersion.Delphi2005),
        ("vcl100.bpl", DelphiVersion.Delphi2006),
        ("vcl100.bpl", DelphiVersion.Delphi2007),
        ("vcl120.bpl", DelphiVersion.Delphi2009),
        ("rtl60.bpl", DelphiVersion.Delphi6),
        ("rtl70.bpl", DelphiVersion.Delphi7),
        ("rtl90.bpl", DelphiVersion.Delphi2005),
        ("rtl100.bpl", DelphiVersion.Delphi2006),
        ("rtl100.bpl", DelphiVersion.Delphi2007),
        ("rtl120.bpl", DelphiVersion.Delphi2009)
    ];
}
