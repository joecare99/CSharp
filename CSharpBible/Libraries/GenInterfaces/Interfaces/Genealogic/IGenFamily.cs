using GenFree2Base.Interfaces;

namespace GenInterfaces.Interfaces.Genealogic;

public interface IGenFamily : IGenEntity
{
    // Relationship
    IGenPerson Husband { get; set; }
    IGenPerson Wife { get; set; }
    int ChildCount { get; }
    IIndexedList<IGenPerson> Children { get; }

    // Vital
    IGenDate MarriageDate { get; set; }
    IGenPlace MarriagePlace { get; set; }
    IGenFact Marriage { get; set; }

    // Management
    string FamilyRefID { get; set; }
    string FamilyName { get; set; }
}