namespace Gen_BaseItf.Model.Interface;

public interface IGenIndividual : IGenEntity
{
    string Name { get; set; }
    string GivenName { get; set; }
    string Surname { get; set; }
    string Title { get; set; }
    string Sex { get; set; }
    string IndRefID { get; set; }

    // Relationship-Properties
    IGenIndividual Father { get; set; }
    IGenIndividual Mother { get; set; }
    int ChildCount { get; }
    IGenList<IGenIndividual> Children { get; }
    IGenFamily ParentFamily { get; set; }
    int FamilyCount { get; }
    IGenList<IGenFamily> Families { get; }
    int SpouseCount { get; }
    IGenList<IGenIndividual> Spouses { get; }

    // Vital-Properties
    string BirthDate { get; set; }
    string BirthPlace { get; set; }
    IGenEvent Birth { get; set; }
    string BaptDate { get; set; }
    string BaptPlace { get; set; }
    IGenEvent Baptism { get; set; }
    string DeathDate { get; set; }
    string DeathPlace { get; set; }
    IGenEvent Death { get; set; }
    string BurialDate { get; set; }
    string BurialPlace { get; set; }
    IGenEvent Burial { get; set; }
    string Religion { get; set; }
    string Occupation { get; set; }
    string OccuPlace { get; set; }
    string Residence { get; set; }
}
