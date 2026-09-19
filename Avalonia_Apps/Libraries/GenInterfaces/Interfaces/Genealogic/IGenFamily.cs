// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 03-08-2025
// ***********************************************************************
// <copyright file="IGenFamily.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenFamily
/// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenEntity" />
/// </summary>
/// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenEntity" />
public interface IGenFamily : IGenEntity
{
    // Relationship
    /// <summary>
    /// Gets or sets the husband.
    /// </summary>
    /// <value>The husband.</value>
    IGenPerson? Husband { get; set; }
    /// <summary>
    /// Gets or sets the wife.
    /// </summary>
    /// <value>The wife.</value>
    IGenPerson? Wife { get; set; }
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

    // Vital
    /// <summary>
    /// Gets or sets the marriage date.
    /// </summary>
    /// <value>The marriage date.</value>
    IGenDate? MarriageDate { get; set; }
    /// <summary>
    /// Gets or sets the marriage place.
    /// </summary>
    /// <value>The marriage place.</value>
    IGenPlace? MarriagePlace { get; set; }
    /// <summary>
    /// Gets or sets the marriage.
    /// </summary>
    /// <value>The marriage.</value>
    IGenFact? Marriage { get; set; }

    // Management
    /// <summary>
    /// Gets or sets the family reference identifier.
    /// </summary>
    /// <value>The family reference identifier.</value>
    string? FamilyRefID { get; set; }
    /// <summary>
    /// Gets or sets the name of the family.
    /// </summary>
    /// <value>The name of the family.</value>
    string? FamilyName { get; set; }
}