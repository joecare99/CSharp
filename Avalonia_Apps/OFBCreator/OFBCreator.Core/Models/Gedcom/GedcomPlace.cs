using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomPlace : IGenPlace
{
    private IGenealogy? _owner;

    public Guid UId { get; init; } = Guid.NewGuid();
    public EGenType eGenType => EGenType.GenPlace;
    public int ID { get; init; }
    public DateTime? LastChange { get; private set; } = DateTime.UtcNow;
    public IGenealogy? Owner => _owner;
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? GOV_ID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Notes { get; set; }
    public IGenPlace? Parent { get; set; }

    public void SetOwner(IGenealogy owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        LastChange = DateTime.UtcNow;
    }
}
