// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="DubSearchResult.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Search result for duplicate candidates</summary>
// ***********************************************************************

namespace GenFreeWin.Models
{
    /// <summary>
    /// Represents a search result record for potential duplicates.
    /// </summary>
    public class DubSearchResult
    {
        /// <summary>
        /// Gets or sets the person ID.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Gets or sets the person's full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the birth year or relevant date year.
        /// </summary>
        public int DateYear { get; set; }

        /// <summary>
        /// Gets or sets the sex indicator (M/F).
        /// </summary>
        public string Sex { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets parent names if available (for CheckBox1 option).
        /// </summary>
        public string ParentNames { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets child names if available (for CheckBox2 option).
        /// </summary>
        public string ChildNames { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the formatted display text for list binding.
        /// </summary>
        public string DisplayText { get; set; } = string.Empty;
    }
}
