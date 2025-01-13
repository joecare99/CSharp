// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 03-30-2024
//
// Last Modified By : Mir
// Last Modified On : 03-30-2024
// ***********************************************************************
// <copyright file="IGenFact.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Gen_BaseItf.Model.Interface;

/// <summary>
/// Interface IGenFact
/// Extends the <see cref="Gen_BaseItf.Model.Interface.IGenData" /></summary>
/// <seealso cref="Gen_BaseItf.Model.Interface.IGenData" />
public interface IGenFact : IGenData
{
    /// <summary>
    /// internal Interface _ILinks
    /// </summary>
    public interface _ILinks
    {
        /// <summary>
        /// Entities linked to this fact
        /// </summary>
        IGenEntity this[object Idx] { get; set; }
    }

    /// <summary>
    /// internal Interface _ISources
    /// </summary>
    public interface _ISources
    {
        /// <summary>
        /// Sources linked to this fact
        /// </summary>
        IGenData this[object Idx] { get; set; }
    }

    /// <summary>
    /// Number of Sources linked to this fact
    /// </summary>
    int SourceCount { get; }
    /// <summary>
    /// Sources linked to this fact
    /// </summary>
    _ISources Sources { get; set; }
    int FType { get; set; }

    /// <summary>
    /// Number of entities linked to this fact
    /// </summary>
    int LinksCount { get; }

    /// <summary>
    /// Entities linked to this fact
    /// </summary>
    _ILinks Links { get; }
}
