using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models.Gedcom;

internal sealed class GedcomFamily : GedcomEntity, IGenFamily
{
    public override EGenType eGenType => EGenType.GenFamily;
    public IGenPerson? Husband { get; set; }
    public IGenPerson? Wife { get; set; }
    public int ChildCount => Children.Count;
    public IIndexedList<IGenPerson> Children { get; } = new GedcomIndexedList<IGenPerson>();
    public IGenDate? MarriageDate { get; set; }
    public IGenPlace? MarriagePlace { get; set; }
    public IGenFact? Marriage { get; set; }
    public string? FamilyRefID { get; set; }
    public string? FamilyName { get; set; }
}
