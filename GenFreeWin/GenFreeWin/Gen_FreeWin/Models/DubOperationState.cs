// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="DubOperationState.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>State and context for duplicate handling operations</summary>
// ***********************************************************************

using System.Collections.Generic;

namespace Gen_FreeWin.Models
{
    /// <summary>
    /// Represents the operational state for duplicate person/family resolution.
    /// Encapsulates the two candidate records and current operation context.
    /// </summary>
    public class DubOperationState
    {
        /// <summary>
        /// Gets or sets the first person ID in duplicate pair.
        /// </summary>
        public int Person1Id { get; set; }

        /// <summary>
        /// Gets or sets the second person ID in duplicate pair.
        /// </summary>
        public int Person2Id { get; set; }

        /// <summary>
        /// Gets or sets the first family ID in duplicate pair (if family duplicates).
        /// </summary>
        public int Family1Id { get; set; }

        /// <summary>
        /// Gets or sets the second family ID in duplicate pair (if family duplicates).
        /// </summary>
        public int Family2Id { get; set; }

        /// <summary>
        /// Gets or sets the event types that differ between duplicates.
        /// </summary>
        public List<string> DiffingEvents { get; set; } = new();

        /// <summary>
        /// Gets or sets the name data for Person1.
        /// </summary>
        public string Person1Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name data for Person2.
        /// </summary>
        public string Person2Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this is a family (vs. person) duplicate resolution.
        /// </summary>
        public bool IsFamilyDuplicate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether transaction is active during merge.
        /// </summary>
        public bool IsTransactionActive { get; set; }

        /// <summary>
        /// Gets or sets the merge target (0 = Person1, 1 = Person2).
        /// </summary>
        public int MergeTargetIndex { get; set; }
    }
}
