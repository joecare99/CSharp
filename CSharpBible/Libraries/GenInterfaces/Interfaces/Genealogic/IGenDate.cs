// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 03-22-2025
// ***********************************************************************
// <copyright file="IGenDate.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using System;
using System.Text.Json.Serialization;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic;

/// <summary>
/// Interface IGenDate
/// Extends the <see cref="T:GenInterfaces.Interfaces.Genealogic.IGenObject" />, A genealogical date
/// </summary>
public interface IGenDate : IGenObject
{
    /// <summary>
    /// Gets or sets the general date modifier.
    /// </summary>
    /// <value>The e date modifier.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    EDateModifier eDateModifier { get; set; }
    /// <summary>
    /// Gets or sets the type of Date1.
    /// </summary>
    /// <value>The e date type1.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    EDateType eDateType1 { get; set; }
    /// <summary>
    /// Gets or sets the date1.<br/>
    /// This Date is mostly set. and marks the date or the beginning of the event.
    /// </summary>
    /// <value>The date1.</value>
    DateTime Date1 { get; set; }
    /// <summary>
    /// Gets or sets the type of date2.
    /// </summary>
    /// <value>The e date type2.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    EDateType? eDateType2 { get; set; }
    /// <summary>
    /// Gets or sets the date2.<br/>
    /// This Date is optional and marks the end or the last possibility of the event.
    /// </summary>
    /// <value>The date2.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    DateTime? Date2 { get; set; }
    /// <summary>
    /// Gets or sets the date text.<br/>
    /// This text is used if the date is not a real date but a text.
    /// </summary>
    /// <value>The date text.</value>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? DateText { get; set; }
}