// ***********************************************************************
// Assembly         : BaseGenClasses
// Author           : Mir
// Created          : 03-26-2025
//
// Last Modified By : Mir
// Last Modified On : 03-26-2025
// ***********************************************************************
// <copyright file="GenFamily.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseGenClasses.Helper;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Model namespace.
/// </summary>
namespace BaseGenClasses.Model;

/// <summary>
/// Class GenFamily.
/// Implements the <see cref="BaseGenClasses.Model.GenEntity" />
/// Implements the <see cref="IGenFamily" />
/// </summary>
/// <seealso cref="BaseGenClasses.Model.GenEntity" />
/// <seealso cref="IGenFamily" />
public class GenFamily : GenEntity, IGenFamily
{
    /// <summary>
    /// Gets the type of the e gen.
    /// </summary>
    /// <value>The type of the e gen.</value>
    public override EGenType eGenType => EGenType.GenFamily;

    /// <summary>
    /// Gets or sets the husband.
    /// </summary>
    /// <value>The husband.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IGenPerson? Husband { 
        get => Connects.Where(c => c?.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "M").Select(p => p!.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent, value); }
    /// <summary>
    /// Gets or sets the wife.
    /// </summary>
    /// <value>The wife.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IGenPerson? Wife { 
        get => Connects.Where(c => c?.eGenConnectionType == EGenConnectionType.Parent && c.Entity is IGenPerson p && p.Sex == "F").Select(p => p!.Entity as IGenPerson).FirstOrDefault(); 
        set => Connects.AddPerson(EGenConnectionType.Parent, value); }

    /// <summary>
    /// Gets the child count.
    /// </summary>
    /// <value>The child count.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public int ChildCount => Connects.Where(c => c?.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Count();

    /// <summary>
    /// Gets the children.
    /// </summary>
    /// <value>The children.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IIndexedList<IGenPerson> Children => Connects.Where(c => c?.eGenConnectionType == EGenConnectionType.Child && c.Entity is IGenPerson).Select(f => f!.Entity as IGenPerson).ToIndexedList(i => i.IndRefID??"");

    /// <summary>
    /// Gets or sets the marriage date.
    /// </summary>
    /// <value>The marriage date.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IGenDate? MarriageDate { get => Facts.GetFact(EFactType.Mariage, f => f.Date); set => throw new NotImplementedException(); }
    /// <summary>
    /// Gets or sets the marriage place.
    /// </summary>
    /// <value>The marriage place.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IGenPlace? MarriagePlace { get => Facts.GetFact(EFactType.Mariage, f => f.Place); set => throw new NotImplementedException(); }
    /// <summary>
    /// Gets or sets the marriage.
    /// </summary>
    /// <value>The marriage.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public IGenFact? Marriage { get => Facts.GetFact(EFactType.Mariage); set => throw new NotImplementedException(); }
    /// <summary>
    /// Gets or sets the family reference identifier.
    /// </summary>
    /// <value>The family reference identifier.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public string? FamilyRefID { get => Facts.GetFact(EFactType.Reference, f => f.Data); set => Facts.SetFact(EFactType.Reference,this,value); }
    /// <summary>
    /// Gets or sets the name of the family.
    /// </summary>
    /// <value>The name of the family.</value>
    /// <exception cref="System.NotImplementedException"></exception>
    public string? FamilyName { get => Facts.GetFact(EFactType.Surname,f=>f.Data); set => Facts.SetFact(EFactType.Surname,this, value); }

    /// <summary>
    /// Gets the end fact of entity.
    /// </summary>
    /// <returns>System.Nullable&lt;IGenFact&gt;.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    protected override IGenFact? GetEndFactOfEntity() 
        => Facts.FirstOrDefault(f => f?.eGenType == EGenType.GenFact && f.eFactType is EFactType.Divorce or EFactType.Separation or EFactType.Anull);

    /// <summary>
    /// Gets the start fact of entity.
    /// </summary>
    /// <returns>System.Nullable&lt;IGenFact&gt;.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    protected override IGenFact? GetStartFactOfEntity() 
        => Facts.FirstOrDefault(f => f?.eGenType == EGenType.GenFact && f.eFactType is EFactType.Mariage);
}
