// ***********************************************************************
// Assembly         : BaseLib & MVVM_BaseLib
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 06-18-2024
// ***********************************************************************
// <copyright file="IPersistence.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace BaseLib.Interfaces;

/// <summary>
/// Interface IPersistence 
/// </summary>
public interface IPersistence
{
    /// <summary>
    /// Enumerates the property-types.
    /// </summary>
    /// <returns>IEnumerable&lt;System.ValueTuple&lt;System.String, System.Type&gt;&gt;.</returns>
    IEnumerable<(string, Type)> PropTypes { get; }

    /// <summary>
    /// Enumerates the property.
    /// </summary>
    /// <returns>IEnumerable&lt;System.ValueTuple&lt;System.String, System.Object&gt;&gt;.</returns>
    IEnumerable<(string, object)> EnumerateProp();
    /// <summary>
    /// Reads from enumerable.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool ReadFromEnumerable(IEnumerable<(string, object)> enumerable);


}