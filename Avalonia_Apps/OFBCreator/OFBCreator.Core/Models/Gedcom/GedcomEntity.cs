using System;
using System.Collections.Generic;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal abstract class GedcomEntity : IGenEntity
{
    private IGenealogy? _owner;

    public Guid UId { get; init; } = Guid.NewGuid();
    public abstract EGenType eGenType { get; }
    public int ID { get; init; }
    public DateTime? LastChange { get; private set; } = DateTime.UtcNow;
    public IGenealogy? Owner => _owner;
    public IList<IGenFact?> Facts { get; init; } = new List<IGenFact?>();
    public IList<IGenConnects?> Connects { get; init; } = new List<IGenConnects?>();
    public IGenFact? Start => null;
    public IGenFact? End => null;
    public IList<IGenSource?> Sources { get; init; } = new List<IGenSource?>();
    public IList<IGenMedia?> Media { get; init; } = new List<IGenMedia?>();

    public void SetOwner(IGenealogy owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        LastChange = DateTime.UtcNow;
    }
}
