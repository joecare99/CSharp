using System;
using System.Collections.Generic;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomFact : IGenFact
{
    private IGenEntity? _owner;

    public Guid UId { get; init; } = Guid.NewGuid();
    public EGenType eGenType => EGenType.GenFact;
    public int ID { get; init; }
    public DateTime? LastChange => null;
    public EFactType eFactType { get; init; }
    public IGenDate? Date { get; set; }
    public IGenPlace? Place { get; set; }
    public string? Data { get; set; }
    public IList<IGenSource?> Sources { get; init; } = new List<IGenSource?>();
    public IGenEntity? MainEntity => _owner;
    public IList<IGenConnects?> Entities { get; init; } = new List<IGenConnects?>();
    public IList<IGenMedia?> Medias { get; init; } = new List<IGenMedia?>();
    public IGenEntity? Owner => _owner;

    public void SetOwner(IGenEntity owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }
}
