// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="NameFieldViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>MVVM ViewModel for a single name field (primary name + synonym)</summary>
// ***********************************************************************

using CommunityToolkit.Mvvm.ComponentModel;

namespace Gen_FreeWin.ViewModels
{
    /// <summary>
    /// MVVM ViewModel representing a single name entry field (line).
    /// Encapsulates PrimaryName and Synonym in one bindable unit.
    /// </summary>
    public partial class NameFieldViewModel : ObservableObject
    {
        /// <summary>
        /// Line number (1-15) for identification.
        /// </summary>
        public int LineNumber { get; init; }

        /// <summary>
        /// Primary name (given name) text for this line.
        /// </summary>
        [ObservableProperty]
        public partial string PrimaryName { get; set; } = "";

        /// <summary>
        /// Synonym or called name text for this line.
        /// </summary>
        [ObservableProperty]
        public partial string Synonym { get; set; } = "";

        /// <summary>
        /// Indicates if this field has been modified.
        /// </summary>
        [ObservableProperty]
        public partial bool IsModified { get; set; }

        /// <summary>
        /// Indicates if this field is valid (not empty or has minimal content).
        /// </summary>
        [ObservableProperty]
        public partial bool IsValid { get; set; } = true;

        /// <summary>
        /// Creates a new NameFieldViewModel for the given line number.
        /// </summary>
        /// <param name="lineNumber">Line number 1-15</param>
        public NameFieldViewModel(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Clears both primary name and synonym for this field.
        /// </summary>
        public void Clear()
        {
            PrimaryName = "";
            Synonym = "";
            IsModified = false;
        }

        /// <summary>
        /// Check if field is empty (both primary and synonym are blank).
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(PrimaryName) && string.IsNullOrWhiteSpace(Synonym);

        /// <summary>
        /// Gets both values as a display string.
        /// </summary>
        public string DisplayText => $"{PrimaryName.TrimEnd()} - {Synonym.TrimEnd()}".TrimEnd(' ', '-');

        /// <summary>
        /// Resets the modified flag.
        /// </summary>
        public void ResetModified()
        {
            IsModified = false;
        }

        /// <summary>
        /// Marks this field as modified and triggers validation.
        /// </summary>
        public void MarkModified()
        {
            IsModified = true;
            ValidateField();
        }

        /// <summary>
        /// Validates the field content (e.g., checks for minimum length, special chars).
        /// </summary>
        private void ValidateField()
        {
            // Simple validation: field is valid if not empty OR both are empty
            IsValid = !IsEmpty;
        }
    }
}
