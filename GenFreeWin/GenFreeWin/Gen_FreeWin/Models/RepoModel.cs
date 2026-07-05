// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="RepoModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for genealogy Repository data</summary>
// ***********************************************************************

namespace Gen_FreeWin.Models
{
    /// <summary>
    /// Represents a genealogy Repository with contact information and metadata.
    /// </summary>
    public class RepoModel
    {
        /// <summary>
        /// Gets or sets the unique repository identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the repository name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city/town name.
        /// </summary>
        public string Place { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the website URL.
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets remarks/comments.
        /// </summary>
        public string Remarks { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the search name (combined name + place for indexing).
        /// </summary>
        public string SearchName { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoModel"/> class.
        /// </summary>
        public RepoModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoModel"/> class with display data.
        /// </summary>
        /// <param name="id">The repository ID.</param>
        /// <param name="name">The repository name.</param>
        /// <param name="place">The city/town.</param>
        public RepoModel(int id, string name, string place)
        {
            Id = id;
            Name = name;
            Place = place;
            SearchName = $"{name} {place}".Trim();
        }

        /// <summary>
        /// Gets the display text for list items (Name + Place).
        /// </summary>
        public string DisplayText => $"{Name} {Place}".Trim();

        /// <summary>
        /// Validates that required fields are not empty.
        /// </summary>
        /// <returns>True if the model is valid; otherwise false.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
    }

    /// <summary>
    /// Represents a Repository source/document reference link.
    /// </summary>
    public class RepoSourceModel
    {
        /// <summary>
        /// Gets or sets the source/document ID.
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Gets or sets the repository ID.
        /// </summary>
        public int RepositoryId { get; set; }

        /// <summary>
        /// Gets or sets the source description.
        /// </summary>
        public string SourceDescription { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoSourceModel"/> class.
        /// </summary>
        public RepoSourceModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoSourceModel"/> class.
        /// </summary>
        /// <param name="sourceId">The source ID.</param>
        /// <param name="repositoryId">The repository ID.</param>
        /// <param name="sourceDescription">The source description.</param>
        public RepoSourceModel(int sourceId, int repositoryId, string sourceDescription)
        {
            SourceId = sourceId;
            RepositoryId = repositoryId;
            SourceDescription = sourceDescription;
        }
    }
}
