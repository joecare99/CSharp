// ***********************************************************************
// Assembly         : Avln_BaseLib
// Author           : Mir
// Created          : 07-01-2024
//
// Last Modified By : Mir
// Last Modified On : 07-01-2024
// ***********************************************************************
// <copyright file="IRandom.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Interfaces namespace.
/// </summary>
namespace BaseLib.Models.Interfaces;

/// <summary>
/// Interface IRandom
/// </summary>
public interface IRandom
{
    /// <summary>
    /// Gets the next random number between v1 and v2 if v2 is set, between 0 and v1 if v2 is not set.
    /// </summary>
    /// <param name="v1">The first boarder</param>
    /// <param name="v2">The second boarder</param>
    /// <returns>System.Int32.</returns>
    int Next(int v1, int v2 = -1);
    /// <summary>
    /// Gets the next random number as double between ]0 and 1[.
    /// </summary>
    /// <returns>System.Double.</returns>
    double NextDouble();
    /// <summary>
    /// Gets the next random number as int.
    /// </summary>
    /// <returns>System.Int32.</returns>
    int NextInt();
    /// <summary>
    /// Specifies the seed-value.
    /// </summary>
    /// <param name="value">The value.</param>
    void Seed(int value);
}
