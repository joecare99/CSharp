// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 03-13-2025
// ***********************************************************************
// <copyright file="IGenFact.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using GenInterfaces.Data;
using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic
{
    /// <summary>
    /// Interface IGenFact
    /// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
    /// </summary>
    /// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
    public interface IGenFact : IGenObject, IHasOwner<IGenEntity>
    {
        /// <summary>
        /// Gets the type of the e fact.
        /// </summary>
        /// <value>The type of the e fact.</value>
        EFactType eFactType { get; init; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        IGenDate? Date { get; set; }
        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>The place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        IGenPlace? Place { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string? Data { get; set; }
        /// <summary>
        /// Gets the sources.
        /// </summary>
        /// <value>The sources.</value>
        [JsonIgnore]
        IList<IGenSources?> Sources { get; init; }
        /// <summary>
        /// Gets the main entity.
        /// </summary>
        /// <value>The main entity.</value>
        [JsonIgnore]
        IGenEntity? MainEntity { get; }
        /// <summary>
        /// Gets the entities.
        /// </summary>
        /// <value>The entities.</value>
        IList<IGenConnects?> Entities { get; init; }
        /// <summary>Gets the medias.</summary>
        /// <value>The list of media of this fact.</value>
        [JsonIgnore]
        IList<IGenMedia?> Medias { get; init; }
    }
}