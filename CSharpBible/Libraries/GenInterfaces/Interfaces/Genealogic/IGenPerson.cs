using GenFree2Base.Interfaces;

namespace GenInterfaces.Interfaces.Genealogic;

public interface IGenPerson : IGenEntity
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; set; }
    /// <summary>
    /// Gets or sets the name of the given.
    /// </summary>
    /// <value>The name of the given.</value>
    string GivenName { get; set; }
    /// <summary>
    /// Gets or sets the surname.
    /// </summary>
    /// <value>The surname.</value>
    string Surname { get; set; }
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    string Title { get; set; }
    /// <summary>
    /// Gets or sets the sex.
    /// </summary>
    /// <value>The sex.</value>
    string Sex { get; set; }
    /// <summary>
    /// Gets or sets the ind reference identifier.
    /// </summary>
    /// <value>The ind reference identifier.</value>
    string IndRefID { get; set; }

    // Relationship-Properties
    /// <summary>
    /// Gets or sets the father.
    /// </summary>
    /// <value>The father.</value>
    IGenPerson Father { get; set; }
    /// <summary>
    /// Gets or sets the mother.
    /// </summary>
    /// <value>The mother.</value>
    IGenPerson Mother { get; set; }
    /// <summary>
    /// Gets the child count.
    /// </summary>
    /// <value>The child count.</value>
    int ChildCount { get; }
    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    IIndexedList<IGenPerson> Children { get; }
    /// <summary>
    /// Gets or sets the parent family.
    /// </summary>
    /// <value>The parent family.</value>
    IGenFamily ParentFamily { get; set; }
    /// <summary>
    /// Gets the family count.
    /// </summary>
    /// <value>The family count.</value>
    int FamilyCount { get; }
    /// <summary>
    /// Gets the families.
    /// </summary>
    /// <value>The families.</value>
    IIndexedList<IGenFamily> Families { get; }
    /// <summary>
    /// Gets the spouse count.
    /// </summary>
    /// <value>The spouse count.</value>
    int SpouseCount { get; }
    /// <summary>
    /// Gets the spouses.
    /// </summary>
    /// <value>The spouses.</value>
    IIndexedList<IGenPerson> Spouses { get; }

    // Vital-Properties
    /// <summary>
    /// Gets or sets the birth date.
    /// </summary>
    /// <value>The birth date.</value>
    IGenDate BirthDate { get; set; }
    /// <summary>
    /// Gets or sets the birth place.
    /// </summary>
    /// <value>The birth place.</value>
    IGenPlace BirthPlace { get; set; }
    /// <summary>
    /// Gets or sets the birth.
    /// </summary>
    /// <value>The birth.</value>
    IGenFact Birth { get; set; }
    /// <summary>
    /// Gets or sets the bapt date.
    /// </summary>
    /// <value>The bapt date.</value>
    IGenDate BaptDate { get; set; }
    /// <summary>
    /// Gets or sets the bapt place.
    /// </summary>
    /// <value>The bapt place.</value>
    IGenPlace BaptPlace { get; set; }
    /// <summary>
    /// Gets or sets the baptism.
    /// </summary>
    /// <value>The baptism.</value>
    IGenFact Baptism { get; set; }
    /// <summary>
    /// Gets or sets the death date.
    /// </summary>
    /// <value>The death date.</value>
    IGenDate DeathDate { get; set; }
    /// <summary>
    /// Gets or sets the death place.
    /// </summary>
    /// <value>The death place.</value>
    IGenPlace DeathPlace { get; set; }
    /// <summary>
    /// Gets or sets the death.
    /// </summary>
    /// <value>The death.</value>
    IGenFact Death { get; set; }
    /// <summary>
    /// Gets or sets the burial date.
    /// </summary>
    /// <value>The burial date.</value>
    IGenDate BurialDate { get; set; }
    /// <summary>
    /// Gets or sets the burial place.
    /// </summary>
    /// <value>The burial place.</value>
    IGenPlace BurialPlace { get; set; }
    /// <summary>
    /// Gets or sets the burial.
    /// </summary>
    /// <value>The burial.</value>
    IGenFact Burial { get; set; }
    /// <summary>
    /// Gets or sets the religion.
    /// </summary>
    /// <value>The religion.</value>
    string Religion { get; set; }
    /// <summary>
    /// Gets or sets the occupation.
    /// </summary>
    /// <value>The occupation.</value>
    string Occupation { get; set; }
    /// <summary>
    /// Gets or sets the occu place.
    /// </summary>
    /// <value>The occu place.</value>
    IGenPlace OccuPlace { get; set; }
    /// <summary>
    /// Gets or sets the residence.
    /// </summary>
    /// <value>The residence.</value>
    IGenPlace Residence { get; set; }
}