// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchCriteria.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for name search criteria</summary>
// ***********************************************************************

namespace GenFreeWin.ViewModels.Models
{
    /// <summary>
    /// Encapsulates search criteria for name search operations.
    /// Replaces scattered UI-state properties in NamenSuchViewModel.
    /// </summary>
    public class SearchCriteria
    {
        /// <summary>
        /// Search text from input field (e.g., "Mueller*" with wildcard support).
        /// </summary>
        public string SearchText { get; set; } = "";

        /// <summary>
        /// Include males in search results.
        /// </summary>
        public bool IncludeMales { get; set; }

        /// <summary>
        /// Include females in search results.
        /// </summary>
        public bool IncludeFemales { get; set; }

        /// <summary>
        /// Search only family members (when IncludeFamily is true).
        /// </summary>
        public bool FamilyOnly { get; set; }

        /// <summary>
        /// Include non-spouse-related persons (opposite when OmitSpouse is true).
        /// </summary>
        public bool OmitSpouse { get; set; }

        /// <summary>
        /// Selection criteria active (secondary gender filters).
        /// </summary>
        public bool Selection { get; set; }

        /// <summary>
        /// For family-based searches: include males (family context).
        /// </summary>
        public bool MaleFamily { get; set; }

        /// <summary>
        /// For family-based searches: include females (family context).
        /// </summary>
        public bool FemaleFamily { get; set; }

        /// <summary>
        /// Additional search text from second input field if applicable.
        /// </summary>
        public string AdditionalText { get; set; } = "";

        /// <summary>
        /// Person ID being searched (context for filtering).
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Family ID being searched (context for filtering).
        /// </summary>
        public int FamilyId { get; set; }

        /// <summary>
        /// Determines if this is a family-based vs person-based search.
        /// </summary>
        public SearchMode Mode { get; set; } = SearchMode.PersonBased;

        /// <summary>
        /// Validates the criteria for minimum requirements.
        /// </summary>
        /// <returns>True if search can proceed with current criteria.</returns>
        public bool IsValid()
        {
            // At least one gender filter must be selected (or family-based search)
            if (!FamilyOnly && !OmitSpouse && !Selection)
            {
                if (!IncludeMales && !IncludeFemales)
                    return false;
            }

            // Search text should not be empty
            if (string.IsNullOrWhiteSpace(SearchText))
                return false;

            return true;
        }

        /// <summary>
        /// Creates a copy of the criteria.
        /// </summary>
        public SearchCriteria Clone() => new()
        {
            SearchText = SearchText,
            IncludeMales = IncludeMales,
            IncludeFemales = IncludeFemales,
            FamilyOnly = FamilyOnly,
            OmitSpouse = OmitSpouse,
            Selection = Selection,
            MaleFamily = MaleFamily,
            FemaleFamily = FemaleFamily,
            AdditionalText = AdditionalText,
            PersonId = PersonId,
            FamilyId = FamilyId,
            Mode = Mode
        };
    }

    /// <summary>
    /// Enum to distinguish search contexts.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>Person-based search with gender filters.</summary>
        PersonBased,
        /// <summary>Family-based search with family filters.</summary>
        FamilyBased,
        /// <summary>Spouse-filtered search (omit spouse mode).</summary>
        SpouseFiltered
    }
}
