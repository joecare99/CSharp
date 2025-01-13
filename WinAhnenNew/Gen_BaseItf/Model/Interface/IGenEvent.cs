// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 03-30-2024
//
// Last Modified By : Mir
// Last Modified On : 03-30-2024
// ***********************************************************************
// <copyright file="IGenEvent.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Gen_BaseItf.Model.Data;

/// <summary>
/// The Interface namespace.
/// </summary>
namespace Gen_BaseItf.Model.Interface;

/// <summary>
/// Interface IGenEvent
/// Extends the <see cref="Gen_BaseItf.Model.Interface.IGenFact" />
/// </summary>
/// <seealso cref="Gen_BaseItf.Model.Interface.IGenFact" />
public interface IGenEvent : IGenFact
{
    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    /// <value>The date.</value>
    string Date { get; set; }
    /// <summary>
    /// Gets or sets the place.
    /// </summary>
    /// <value>The place.</value>
    string Place { get; set; }
    /// <summary>
    /// Gets or sets the type of the event.
    /// </summary>
    /// <value>The type of the event.</value>
    EEventType EventType { get; set; }
}
