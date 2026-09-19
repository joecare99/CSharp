// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 03-22-2025
//
// Last Modified By : Mir
// Last Modified On : 03-22-2025
// ***********************************************************************
// <copyright file="EDateModifier.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Data namespace.
/// </summary>
namespace GenInterfaces.Data;

/// <summary>
/// Enum EDateModifier
/// </summary>
public enum EDateModifier
{
    /// <summary>
    /// No modifier, the Date is exact.   
    /// </summary>
    None,
    /// <summary>
    /// before Modifier, the Event is before the given Date.
    /// </summary>
    Before,
    /// <summary>
    /// after Modifier, the Event is after the given Date (Date2).
    /// </summary>
    After,
    /// <summary>
    /// between Modifier, the Event is between the given Dates.
    /// </summary>
    Between,
    /// <summary>
    /// The about Modifier, the Event is about the given Date. 
    /// </summary>
    About,
    /// <summary>
    /// The estimated Modifier, the Event is estimated.
    /// </summary>
    Estimated,
    /// <summary>
    /// The calculated Modifier, the Event is calculated.
    /// </summary>
    Calculated,
    /// <summary>
    /// From to Modifier, the Event lasted is between the given Dates.
    /// </summary>
    FromTo,
    /// <summary>
    /// From Modifier, the Event started at the given Date.
    /// </summary>
    From,
    /// <summary>
    /// To Modifier, the Event ended at the given Date (Date2).
    /// </summary>
    To,
    /// <summary>
    /// The text Modifier, the Date is given as text.
    /// </summary>
    Text,
}