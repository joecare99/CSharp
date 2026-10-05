using System;
using System.IO;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Workflows;

public sealed class HejInspectionService : IHejInspectionService
{
    private readonly IHejReader _hejReader;

    public HejInspectionService(IHejReader hejReader)
    {
        ArgumentNullException.ThrowIfNull(hejReader);
        _hejReader = hejReader;
    }

    public HejInspection Inspect(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new HejInspection(_hejReader.Read(source));
    }
}
