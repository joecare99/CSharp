// ***********************************************************************
// Assembly         : BaseGenClasses
// Author           : Mir
// Created          : 03-22-2025
//
// Last Modified By : Mir
// Last Modified On : 03-22-2025
// ***********************************************************************
// <copyright file="GenDate.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// The Model namespace.
/// </summary>
namespace BaseGenClasses.Model;

/// <summary>
/// Class GenDate.
/// Implements the <see cref="BaseGenClasses.Model.GenObject" />
/// Implements the <see cref="IGenDate" />
/// </summary>
/// <seealso cref="BaseGenClasses.Model.GenObject" />
/// <seealso cref="IGenDate" />
public class GenDate : GenObject, IGenDate
{
    /// <summary>
    /// Gets the type of the e gen.
    /// </summary>
    /// <value>The type of the e gen.</value>
    public override EGenType eGenType => EGenType.GenDate;

    /// <summary>
    /// Gets or sets the general date modifier.
    /// </summary>
    /// <value>The e date modifier.</value>
    public EDateModifier eDateModifier { get; set; }
    /// <summary>
    /// Gets or sets the type of Date1.
    /// </summary>
    /// <value>The e date type1.</value>
    public EDateType eDateType1 { get; set; }
    /// <summary>
    /// Gets or sets the date1.<br />
    /// This Date is mostly set. and marks the date or the beginning of the event.
    /// </summary>
    /// <value>The date1.</value>
    public DateTime Date1 { get; set; }
    /// <summary>
    /// Gets or sets the type of date2.
    /// </summary>
    /// <value>The e date type2.</value>
    public EDateType? eDateType2 { get; set; }
    /// <summary>
    /// Gets or sets the date2.<br />
    /// This Date is optional and marks the end or the last possibility of the event.
    /// </summary>
    /// <value>The date2.</value>
    public DateTime? Date2 { get; set; }
    /// <summary>
    /// Gets or sets the date text.<br />
    /// This text is used if the date is not a real date but a text.
    /// </summary>
    /// <value>The date text.</value>
    public string? DateText { get; set; }

    public GenDate()
    {
        UId = Guid.Empty;
        eDateModifier = EDateModifier.None;
        eDateType1 = EDateType.Full;
        Date1 = DateTime.MinValue;
        eDateType2 = null;
        Date2 = null;
        DateText = null;
    }

    public GenDate(EDateModifier modifier, EDateType type1, DateTime date1, EDateType? type2 = null, DateTime? date2 = null, string? dateText = null) : this()
    {
        UId = Guid.Empty;
        eDateModifier = modifier;
        eDateType1 = type1;
        Date1 = date1;
        eDateType2 = type2;
        Date2 = date2;
        DateText = dateText;
    }

    public GenDate(DateTime date1, EDateType? type2 = null, DateTime? date2 = null, string? dateText = null):this(EDateModifier.None,EDateType.Full,date1,type2,date2,dateText)
    {
    }
}