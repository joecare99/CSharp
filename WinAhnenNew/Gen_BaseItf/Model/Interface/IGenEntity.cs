// ***********************************************************************
// Assembly         : 
// Author           : Mir
// Created          : 03-30-2024
//
// Last Modified By : Mir
// Last Modified On : 03-30-2024
// ***********************************************************************
// <copyright file="IGenEntity.cs" company="JC-Soft">
//     Copyright (c) JC-Soft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Gen_BaseItf.Model.Interface;

/// <summary>
/// Interface IGenEntity
/// Extends the <see cref="Gen_BaseItf.Model.Interface.IGenData" /></summary>
/// <seealso cref="Gen_BaseItf.Model.Interface.IGenData" />
public interface IGenEntity : IGenData
{
    /// <summary>
    /// Interface _IEvents
    /// </summary>
    public interface _IEvents
    {
        IGenEvent this[object Idx] { get; set; }
    }
    
    int EventCount { get; }
    _IEvents Events { get; }
}
