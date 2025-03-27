// ***********************************************************************
// Assembly         : GenInterfaces
// Author           : Mir
// Created          : 01-28-2025
//
// Last Modified By : Mir
// Last Modified On : 03-13-2025
// ***********************************************************************
// <copyright file="IGenPlace.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Text.Json.Serialization;

/// <summary>
/// The Genealogic namespace.
/// </summary>
namespace GenInterfaces.Interfaces.Genealogic
{
    /// <summary>
    /// Interface IGenPlace
    /// Extends the <see cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
    /// </summary>
    /// <seealso cref="GenInterfaces.Interfaces.Genealogic.IGenObject" />
    public interface IGenPlace : IGenObject
    {
        /// <summary>
        /// Gets or sets the name of the place.
        /// </summary>
        /// <value>The name of the place.</value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the type of the place.
        /// </summary>
        /// <value>The type of the place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string? Type { get; set; }
        /// <summary>
        /// Gets or sets the GOF-ID of the place.
        /// </summary>
        /// <value>The GOF-ID of the place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string? GOV_ID { get; set; }
        /// <summary>
        /// Gets or sets the latitude of the place.
        /// </summary>
        /// <value>The latitude of the place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        double Latitude { get; set; }
        /// <summary>
        /// Gets or sets the longitude of the place.
        /// </summary>
        /// <value>The longitude of the place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        double Longitude { get; set; }
        /// <summary>
        /// Gets or sets the altitude of the place.
        /// </summary>
        /// <value>The altitude of the place.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string? Notes { get; set; }
        /// <summary>
        /// Gets or sets the parent-place.
        /// </summary>
        /// <value>The parent.</value>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        IGenPlace? Parent { get; set; }
    }
}