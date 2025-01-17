// ***********************************************************************
// Assembly         : Avln_BaseLib
// Author           : Mir
// Created          : 01-17-2025
//
// Last Modified By : Mir
// Last Modified On : 01-17-2025
// ***********************************************************************
// <copyright file="CRandom.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Models.Interfaces;
using System;

/// <summary>
/// The Models namespace.
/// </summary>
namespace BaseLib.Models;

/// <summary>
/// Class CRandom.
/// Implements the <see cref="IRandom" />
/// </summary>
/// <seealso cref="IRandom" />
public class CRandom : IRandom
{
    /// <summary>
    /// The random
    /// </summary>
    private Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="CRandom"/> class.
    /// </summary>
    public CRandom()
    {
        _random = new Random();
    }

    /// <summary>
    /// Gets the next random number between v1 and v2 if v2 is set, between 0 and v1 if v2 is not set.
    /// </summary>
    /// <param name="v1">The first boarder</param>
    /// <param name="v2">The second boarder</param>
    /// <returns>System.Int32.</returns>
    public int Next(int v1, int v2) => v2 != -1 || v1 < v2 ? _random.Next(v1, v2) : _random.Next(v1);

    /// <summary>
    /// Gets the next random number as double between ]0 and 1[.
    /// </summary>
    /// <returns>System.Double.</returns>
    public double NextDouble() => _random.NextDouble();

    /// <summary>
    /// Gets the next random number as int.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int NextInt() => _random.Next();

    /// <summary>
    /// Specifies the seed-value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Seed(int value) => _random = new Random(value);
}
