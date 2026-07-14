// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="SearchUIState.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>UI state model for search interface (extracted from NamenSuchViewModel)</summary>
// ***********************************************************************

namespace Gen_FreeWin.ViewModels.Models
{
    /// <summary>
    /// Domain model representing the complete UI state of the search interface.
    /// Consolidates ~25 UI-related properties from NamenSuchViewModel (checkboxes, visibility, enable-states).
    /// 
    /// Enables:
    /// - Separation of UI state from data models
    /// - Snapshot/restore of UI state
    /// - Testable UI state management
    /// - Clear contracts for View↔ViewModel binding
    /// </summary>
    public class SearchUIState
    {
        // ====================================================================
        // FILTER CHECKBOXES (Search Criteria Selection)
        // ====================================================================

        /// <summary>
        /// Include males in search results (legacy: Male_Checked)
        /// </summary>
        public bool MaleChecked { get; set; }

        /// <summary>
        /// Include females in search results (legacy: Females_Checked)
        /// </summary>
        public bool FemaleChecked { get; set; }

        /// <summary>
        /// Search family-only mode (legacy: FamOnly_Checked)
        /// </summary>
        public bool FamilyOnlyChecked { get; set; }

        /// <summary>
        /// Use selection mode (legacy: Selection_Checked)
        /// </summary>
        public bool SelectionChecked { get; set; }

        /// <summary>
        /// Filter family members - males (legacy: Male2_Checked)
        /// </summary>
        public bool FamilyMaleChecked { get; set; }

        /// <summary>
        /// Filter family members - females (legacy: Female2_Checked)
        /// </summary>
        public bool FamilyFemaleChecked { get; set; }

        /// <summary>
        /// Omit spouse from results (legacy: OmitSpouse_Checked)
        /// </summary>
        public bool OmitSpouseChecked { get; set; }

        // ====================================================================
        // SEARCH INPUT FIELDS
        // ====================================================================

        /// <summary>
        /// Primary search text input (legacy: Text1_Text)
        /// </summary>
        public string SearchText { get; set; } = "";

        /// <summary>
        /// Secondary/additional search text (legacy: Text2_Text)
        /// </summary>
        public string AdditionalSearchText { get; set; } = "";

        /// <summary>
        /// Person number for specific search (legacy: PersNr)
        /// </summary>
        public int PersonNumber { get; set; }

        /// <summary>
        /// Family number for family search (legacy: FamNr)
        /// </summary>
        public int FamilyNumber { get; set; }

        // ====================================================================
        // VISIBILITY STATE (Conditional UI Display)
        // ====================================================================

        /// <summary>
        /// Show male filter checkbox (legacy: Male_Visible)
        /// </summary>
        public bool MaleVisible { get; set; }

        /// <summary>
        /// Show female filter checkbox (legacy: Females_Visible)
        /// </summary>
        public bool FemaleVisible { get; set; }

        /// <summary>
        /// Show family-only checkbox (legacy: FamOnly_Visible)
        /// </summary>
        public bool FamilyOnlyVisible { get; set; }

        /// <summary>
        /// Show family male filter checkbox (legacy: Male2_Visible)
        /// </summary>
        public bool FamilyMaleVisible { get; set; }

        /// <summary>
        /// Show family female filter checkbox (legacy: Female2_Visible)
        /// </summary>
        public bool FamilyFemaleVisible { get; set; }

        /// <summary>
        /// Show omit spouse checkbox (legacy: OmitSpouse_Visible)
        /// </summary>
        public bool OmitSpouseVisible { get; set; }

        /// <summary>
        /// Show secondary text input (legacy: Text2_Visible)
        /// </summary>
        public bool AdditionalTextVisible { get; set; }

        /// <summary>
        /// Show search start button (legacy: StartSearch_Visible)
        /// </summary>
        public bool StartSearchVisible { get; set; }

        /// <summary>
        /// Show ready/completed indicator (legacy: Ready_Visible)
        /// </summary>
        public bool ReadyVisible { get; set; }

        /// <summary>
        /// Show label 9 (legacy: Label9_Visible)
        /// </summary>
        public bool Label9Visible { get; set; }

        /// <summary>
        /// Show label 4 (legacy: Label4_Visible)
        /// </summary>
        public bool Label4Visible { get; set; }

        /// <summary>
        /// Show label 10 (legacy: Label10_Visible)
        /// </summary>
        public bool Label10Visible { get; set; }

        /// <summary>
        /// Show combo box 1 (legacy: ComboBox1_Visible)
        /// </summary>
        public bool ComboBox1Visible { get; set; }

        /// <summary>
        /// Show person sheet (legacy: PersonSheet_Visible)
        /// </summary>
        public bool PersonSheetVisible { get; set; }

        /// <summary>
        /// Show family sheet (legacy: FamilySheet_Visible)
        /// </summary>
        public bool FamilySheetVisible { get; set; }

        // ====================================================================
        // ENABLE/DISABLE STATE (Interactive Control)
        // ====================================================================

        /// <summary>
        /// Enable male filter interaction (legacy: Male_Enabled)
        /// </summary>
        public bool MaleEnabled { get; set; }

        /// <summary>
        /// Enable female filter interaction (legacy: Female_Enabled)
        /// </summary>
        public bool FemaleEnabled { get; set; }

        /// <summary>
        /// Enable family-only mode toggle (legacy: FamOnly_Enabled)
        /// </summary>
        public bool FamilyOnlyEnabled { get; set; }

        /// <summary>
        /// Enable omit spouse toggle (legacy: OmitSpouse_Enabled)
        /// </summary>
        public bool OmitSpouseEnabled { get; set; }

        // ====================================================================
        // TEXT CONTENT + DISPLAY STATE
        // ====================================================================

        /// <summary>
        /// Label 3 display text (legacy: Label3_Text)
        /// </summary>
        public string Label3Text { get; set; } = "";

        /// <summary>
        /// Death mark flag for display (legacy: xDeathMark)
        /// </summary>
        public bool DeathMarkDisplay { get; set; }

        /// <summary>
        /// Family perspective control (legacy: FamPerschalt)
        /// </summary>
        public int FamilyPerspective { get; set; }

        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// Resets all UI state to default/neutral values (all false, empty strings).
        /// </summary>
        public void ResetToDefaults()
        {
            // Checkboxes → all false
            MaleChecked = false;
            FemaleChecked = false;
            FamilyOnlyChecked = false;
            SelectionChecked = false;
            FamilyMaleChecked = false;
            FamilyFemaleChecked = false;
            OmitSpouseChecked = false;

            // Text fields → empty
            SearchText = "";
            AdditionalSearchText = "";
            Label3Text = "";

            // Numbers → 0
            PersonNumber = 0;
            FamilyNumber = 0;
            FamilyPerspective = 0;

            // Flags → false
            DeathMarkDisplay = false;

            // Visibility → default (typically true for visible)
            MaleVisible = true;
            FemaleVisible = true;
            FamilyOnlyVisible = true;
            FamilyMaleVisible = true;
            FamilyFemaleVisible = true;
            OmitSpouseVisible = true;
            AdditionalTextVisible = false;
            StartSearchVisible = true;
            ReadyVisible = false;
            Label9Visible = true;
            Label4Visible = false;
            Label10Visible = false;
            ComboBox1Visible = false;
            PersonSheetVisible = false;
            FamilySheetVisible = false;

            // Enable → default (typically true for enabled)
            MaleEnabled = true;
            FemaleEnabled = true;
            FamilyOnlyEnabled = true;
            OmitSpouseEnabled = true;
        }

        /// <summary>
        /// Creates a snapshot/copy of current UI state for undo/restore purposes.
        /// </summary>
        public SearchUIState Clone()
        {
            return new SearchUIState
            {
                MaleChecked = MaleChecked,
                FemaleChecked = FemaleChecked,
                FamilyOnlyChecked = FamilyOnlyChecked,
                SelectionChecked = SelectionChecked,
                FamilyMaleChecked = FamilyMaleChecked,
                FamilyFemaleChecked = FamilyFemaleChecked,
                OmitSpouseChecked = OmitSpouseChecked,
                SearchText = SearchText,
                AdditionalSearchText = AdditionalSearchText,
                PersonNumber = PersonNumber,
                FamilyNumber = FamilyNumber,
                MaleVisible = MaleVisible,
                FemaleVisible = FemaleVisible,
                FamilyOnlyVisible = FamilyOnlyVisible,
                FamilyMaleVisible = FamilyMaleVisible,
                FamilyFemaleVisible = FamilyFemaleVisible,
                OmitSpouseVisible = OmitSpouseVisible,
                AdditionalTextVisible = AdditionalTextVisible,
                StartSearchVisible = StartSearchVisible,
                ReadyVisible = ReadyVisible,
                Label9Visible = Label9Visible,
                Label4Visible = Label4Visible,
                Label10Visible = Label10Visible,
                ComboBox1Visible = ComboBox1Visible,
                PersonSheetVisible = PersonSheetVisible,
                FamilySheetVisible = FamilySheetVisible,
                MaleEnabled = MaleEnabled,
                FemaleEnabled = FemaleEnabled,
                FamilyOnlyEnabled = FamilyOnlyEnabled,
                OmitSpouseEnabled = OmitSpouseEnabled,
                Label3Text = Label3Text,
                DeathMarkDisplay = DeathMarkDisplay,
                FamilyPerspective = FamilyPerspective
            };
        }

        /// <summary>
        /// Returns a summary string of current filter selection for debugging.
        /// </summary>
        public override string ToString()
        {
            var filters = new System.Collections.Generic.List<string>();
            if (MaleChecked)
                filters.Add("Male");
            if (FemaleChecked)
                filters.Add("Female");
            if (FamilyOnlyChecked)
                filters.Add("FamilyOnly");
            if (OmitSpouseChecked)
                filters.Add("NoSpouse");

            var filterStr = string.Join("|", filters);
            return $"SearchUIState: Text='{SearchText}', Filters=[{filterStr}]";
        }
    }
}
