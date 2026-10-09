using System.Collections.Generic;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomPerson : GedcomEntity, IGenPerson
{
    public override EGenType eGenType => EGenType.GenPerson;
    public string Name { get; set; } = string.Empty;
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? BirthSurname { get; set; }
    public List<string> AliasNames { get; } = new();
    public List<GedcomNameEvent> NameEvents { get; } = new();
    public string? Title { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string? IndRefID { get; set; }
    public string? ReferenceNumber { get; set; }
    public IGenPerson? Father { get; set; }
    public IGenPerson? Mother { get; set; }
    public int ChildCount => Children.Count;
    public IIndexedList<IGenPerson> Children { get; } = new GedcomIndexedList<IGenPerson>();
    public IGenFamily? ParentFamily { get; set; }
    public int FamilyCount => Families.Count;
    public IIndexedList<IGenFamily> Families { get; } = new GedcomIndexedList<IGenFamily>();
    public IIndexedList<IGenFamily> Marriages => Families;
    public int SpouseCount => Spouses.Count;
    public IIndexedList<IGenPerson> Spouses { get; } = new GedcomIndexedList<IGenPerson>();
    public IGenDate? BirthDate { get; set; }
    public IGenPlace? BirthPlace { get; set; }
    public IGenFact? Birth { get; set; }
    public IGenDate? BaptDate { get; set; }
    public IGenPlace? BaptPlace { get; set; }
    public IGenFact? Baptism { get; set; }
    public IGenDate? DeathDate { get; set; }
    public IGenPlace? DeathPlace { get; set; }
    public IGenFact? Death { get; set; }
    public IGenDate? BurialDate { get; set; }
    public IGenPlace? BurialPlace { get; set; }
    public IGenFact? Burial { get; set; }
    public string? Religion { get; set; }
    public string? Occupation { get; set; }
    public IGenPlace? OccuPlace { get; set; }
    public IGenPlace? Residence { get; set; }
}
