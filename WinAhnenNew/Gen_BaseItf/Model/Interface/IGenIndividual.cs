// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 03-30-2024
//
// Last Modified By : Mir
// Last Modified On : 03-30-2024
// ***********************************************************************
// <copyright file="IGenIndividual.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Interface namespace.
/// </summary>
namespace Gen_BaseItf.Model.Interface;

/// <summary>
/// Interface IGenIndividual
/// Extends the <see cref="Gen_BaseItf.Model.Interface.IGenEntity" />
/// </summary>
/// <seealso cref="Gen_BaseItf.Model.Interface.IGenEntity" />
public interface IGenIndividual : IGenEntity
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
    IGenIndividual Father { get; set; }
    /// <summary>
    /// Gets or sets the mother.
    /// </summary>
    /// <value>The mother.</value>
    IGenIndividual Mother { get; set; }
    /// <summary>
    /// Gets the child count.
    /// </summary>
    /// <value>The child count.</value>
    int ChildCount { get; }
    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    IIndexedList<IGenIndividual> Children { get; }
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
    IIndexedList<IGenIndividual> Spouses { get; }

    // Vital-Properties
    /// <summary>
    /// Gets or sets the birth date.
    /// </summary>
    /// <value>The birth date.</value>
    string BirthDate { get; set; }
    /// <summary>
    /// Gets or sets the birth place.
    /// </summary>
    /// <value>The birth place.</value>
    string BirthPlace { get; set; }
    /// <summary>
    /// Gets or sets the birth.
    /// </summary>
    /// <value>The birth.</value>
    IGenEvent Birth { get; set; }
    /// <summary>
    /// Gets or sets the bapt date.
    /// </summary>
    /// <value>The bapt date.</value>
    string BaptDate { get; set; }
    /// <summary>
    /// Gets or sets the bapt place.
    /// </summary>
    /// <value>The bapt place.</value>
    string BaptPlace { get; set; }
    /// <summary>
    /// Gets or sets the baptism.
    /// </summary>
    /// <value>The baptism.</value>
    IGenEvent Baptism { get; set; }
    /// <summary>
    /// Gets or sets the death date.
    /// </summary>
    /// <value>The death date.</value>
    string DeathDate { get; set; }
    /// <summary>
    /// Gets or sets the death place.
    /// </summary>
    /// <value>The death place.</value>
    string DeathPlace { get; set; }
    /// <summary>
    /// Gets or sets the death.
    /// </summary>
    /// <value>The death.</value>
    IGenEvent Death { get; set; }
    /// <summary>
    /// Gets or sets the burial date.
    /// </summary>
    /// <value>The burial date.</value>
    string BurialDate { get; set; }
    /// <summary>
    /// Gets or sets the burial place.
    /// </summary>
    /// <value>The burial place.</value>
    string BurialPlace { get; set; }
    /// <summary>
    /// Gets or sets the burial.
    /// </summary>
    /// <value>The burial.</value>
    IGenEvent Burial { get; set; }
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
    string OccuPlace { get; set; }
    /// <summary>
    /// Gets or sets the residence.
    /// </summary>
    /// <value>The residence.</value>
    string Residence { get; set; }
}
