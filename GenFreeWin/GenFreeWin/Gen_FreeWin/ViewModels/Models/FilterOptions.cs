// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="FilterOptions.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Aggregate for search filter UI state</summary>
// ***********************************************************************

using System;

namespace Gen_FreeWin.ViewModels.Models
{
    /// <summary>
    /// Encapsulates all UI filter state for the name search dialog.
    /// Provides a snapshot/restore pattern for complex multi-option filtering.
    /// Replaces scattered boolean/string properties in NamenSuchViewModel.
    /// </summary>
    public class FilterOptions
    {
        // ==================================================================
        // Gender Filters (Primary)
        // ==================================================================

        /// <summary>Include males (primary filter).</summary>
        public bool MaleChecked { get; set; }

        /// <summary>Include females (primary filter).</summary>
        public bool FemalesChecked { get; set; }

        /// <summary>Filter by family only.</summary>
        public bool FamOnlyChecked { get; set; }

        /// <summary>Apply selection mode filters.</summary>
        public bool SelectionChecked { get; set; }

        // ==================================================================
        // Gender Filters (Secondary / Family Context)
        // ==================================================================

        /// <summary>Include males (secondary/family filter).</summary>
        public bool Male2Checked { get; set; }

        /// <summary>Include females (secondary/family filter).</summary>
        public bool Female2Checked { get; set; }

        /// <summary>Omit spouse from results.</summary>
        public bool OmitSpouseChecked { get; set; }

        // ==================================================================
        // UI Visibility & Enable State
        // ==================================================================

        /// <summary>Show male checkbox in primary section.</summary>
        public bool MaleVisible { get; set; }

        /// <summary>Show females checkbox in primary section.</summary>
        public bool FemalesVisible { get; set; }

        /// <summary>Show family-only checkbox.</summary>
        public bool FamOnlyVisible { get; set; }

        /// <summary>Show male2 checkbox in secondary section.</summary>
        public bool Male2Visible { get; set; }

        /// <summary>Show female2 checkbox in secondary section.</summary>
        public bool Female2Visible { get; set; }

        /// <summary>Show omit-spouse checkbox.</summary>
        public bool OmitSpouseVisible { get; set; }

        // ==================================================================
        // Enable State (interaction-dependent)
        // ==================================================================

        /// <summary>Enable/disable male checkbox based on context.</summary>
        public bool MaleEnabled { get; set; } = true;

        /// <summary>Enable/disable female checkbox based on context.</summary>
        public bool FemaleEnabled { get; set; } = true;

        /// <summary>Enable/disable family-only checkbox based on context.</summary>
        public bool FamOnlyEnabled { get; set; } = true;

        /// <summary>Enable/disable omit-spouse checkbox based on context.</summary>
        public bool OmitSpouseEnabled { get; set; } = true;

        // ==================================================================
        // Labels & Display
        // ==================================================================

        /// <summary>Dynamic label text for result list header (changes based on filters).</summary>
        public string Label3Text { get; set; } = "Name,Vorname                        Datum JJJJ  Personennr.";

        /// <summary>Label for search results (e.g., search criteria display).</summary>
        public string ResultsLabel { get; set; } = "";

        // ==================================================================
        // Search Input
        // ==================================================================

        /// <summary>Primary search text input.</summary>
        public string Text1 { get; set; } = "";

        /// <summary>Secondary search text input (if applicable).</summary>
        public string Text2 { get; set; } = "";

        /// <summary>Show second text input field.</summary>
        public bool Text2Visible { get; set; }

        // ==================================================================
        // UI Sections Visibility
        // ==================================================================

        /// <summary>Show person sheet results.</summary>
        public bool PersonSheetVisible { get; set; }

        /// <summary>Show family sheet results.</summary>
        public bool FamilySheetVisible { get; set; }

        /// <summary>Show search-start button/action.</summary>
        public bool StartSearchVisible { get; set; } = true;

        /// <summary>Show ready/complete indicator.</summary>
        public bool ReadyVisible { get; set; }

        /// <summary>Mark death filter active.</summary>
        public bool DeathMarkActive { get; set; }

        // ==================================================================
        // Data Context
        // ==================================================================

        /// <summary>Current person ID context (for reference lookups).</summary>
        public int PersonId { get; set; }

        /// <summary>Current family ID context.</summary>
        public int FamilyId { get; set; }

        /// <summary>Timestamp of last filter modification.</summary>
        public DateTime LastModified { get; set; } = DateTime.Now;

        // ==================================================================
        // Utility Methods
        // ==================================================================

        /// <summary>Determines if any gender filter is active.</summary>
        public bool HasActiveGenderFilter()
            => MaleChecked || FemalesChecked || Male2Checked || Female2Checked;

        /// <summary>Resets all checkboxes to unchecked state.</summary>
        public void ResetFilters()
        {
            MaleChecked = FemalesChecked = FamOnlyChecked = SelectionChecked = false;
            Male2Checked = Female2Checked = OmitSpouseChecked = false;
            Text1 = Text2 = "";
            DeathMarkActive = false;
            LastModified = DateTime.Now;
        }

        /// <summary>Clones the current filter state.</summary>
        public FilterOptions Clone() => new()
        {
            MaleChecked = MaleChecked,
            FemalesChecked = FemalesChecked,
            FamOnlyChecked = FamOnlyChecked,
            SelectionChecked = SelectionChecked,
            Male2Checked = Male2Checked,
            Female2Checked = Female2Checked,
            OmitSpouseChecked = OmitSpouseChecked,
            MaleVisible = MaleVisible,
            FemalesVisible = FemalesVisible,
            FamOnlyVisible = FamOnlyVisible,
            Male2Visible = Male2Visible,
            Female2Visible = Female2Visible,
            OmitSpouseVisible = OmitSpouseVisible,
            MaleEnabled = MaleEnabled,
            FemaleEnabled = FemaleEnabled,
            FamOnlyEnabled = FamOnlyEnabled,
            OmitSpouseEnabled = OmitSpouseEnabled,
            Label3Text = Label3Text,
            ResultsLabel = ResultsLabel,
            Text1 = Text1,
            Text2 = Text2,
            Text2Visible = Text2Visible,
            PersonSheetVisible = PersonSheetVisible,
            FamilySheetVisible = FamilySheetVisible,
            StartSearchVisible = StartSearchVisible,
            ReadyVisible = ReadyVisible,
            DeathMarkActive = DeathMarkActive,
            PersonId = PersonId,
            FamilyId = FamilyId,
            LastModified = LastModified
        };
    }
}
