// ***********************************************************************
// Assembly         : BaseGenClasses
// Author           : Mir
// Created          : 03-13-2025
//
// Last Modified By : Mir
// Last Modified On : 03-23-2025
// ***********************************************************************
// <copyright file="GenObject.cs" company="JC-Soft">
//     Copyright © JC-Soft 2024
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/// <summary>
/// The Model namespace.
/// </summary>
namespace BaseGenClasses.Model
{
    /// <summary>
    /// Class GenObject.
    /// Implements the <see cref="IGenObject" />
    /// </summary>
    /// <seealso cref="IGenObject" />
    [JsonDerivedType(typeof(GenObject), typeDiscriminator: "base")]

    public abstract class GenObject : IGenObject
    {
        /// <summary>
        /// The uid
        /// </summary>
        private Guid? _uid;

        /// <summary>
        /// Gets the u identifier.
        /// </summary>
        /// <value>The u identifier.</value>
        [DataMember]
        public Guid UId { get => _uid ??= Guid.NewGuid(); init => _uid = value; }
        /// <summary>
        /// Gets the type of the genealogy-object.
        /// </summary>
        /// <value>The type of the e gen.</value>
        [DataMember]
        public abstract EGenType eGenType { get ; }

        /// <summary>
        /// Gets or sets the last change.
        /// </summary>
        /// <value>The last change.</value>
        public DateTime? LastChange { get; protected set; }
    }
}