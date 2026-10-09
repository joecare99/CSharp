using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomConnect : IGenConnects
{
    public Guid UId { get; init; } = Guid.NewGuid();

    public EGenType eGenType => EGenType.GenConnect;

    public IGenEntity? Entity { get; init; }

    public EGenConnectionType eGenConnectionType { get; init; }
}
