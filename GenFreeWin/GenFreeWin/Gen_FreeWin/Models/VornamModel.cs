// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="VornamModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for Vorname (given name) and synonym management</summary>
// ***********************************************************************

using GenFree.Data;

namespace GenFreeWin.Models
{
    /// <summary>
    /// Domain model representing a Name entry with synonym/alias information.
    /// Encapsulates all name-related data for a person.
    /// </summary>
    public class VornamModel
    {
        /// <summary>
        /// Gets or sets the person ID this name belongs to.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Gets or sets the primary name/Vorname (given name).
        /// </summary>
        public string PrimaryName { get; set; } = "";

        /// <summary>
        /// Gets or sets the synonym/Leitname (lead name for display).
        /// </summary>
        public string Synonym { get; set; } = "";

        /// <summary>
        /// Gets or sets the text kind/Kennzeichen (E.g. ETextKennz.F_, ETextKennz.V_).
        /// </summary>
        public ETextKennz TextKennz { get; set; }

        /// <summary>
        /// Gets or sets the sequence number for names (LfNR - Laufnummer).
        /// </summary>
        public short LineNumber { get; set; }

        /// <summary>
        /// Gets or sets whether this is the called name (Rufname).
        /// </summary>
        public bool IsCalledName { get; set; }

        /// <summary>
        /// Gets or sets whether this is a nickname (Spitzname).
        /// </summary>
        public bool IsNickname { get; set; }

        /// <summary>
        /// Gets or sets the display text (full formatted name with prefix marker).
        /// </summary>
        public string DisplayText { get; set; } = "";

        /// <summary>
        /// Gets or sets associated metadata/context from the name lookup.
        /// </summary>
        public (string Text, ETextKennz TextKennz) NameDesignation { get; set; }

        /// <summary>
        /// Validates the name model according to business rules.
        /// </summary>
        /// <returns>True if valid; otherwise, false.</returns>
        public bool IsValid()
        {
            // Primary name must be at least 2 characters (legacy rule)
            return !string.IsNullOrWhiteSpace(PrimaryName) && PrimaryName.Trim().Length >= 2 && PersonId > 0;
        }

        /// <summary>
        /// Generates a display string for the name with context markers.
        /// </summary>
        /// <returns>Formatted display string for UI presentation.</returns>
        public string GenerateDisplayText()
        {
            if (string.IsNullOrEmpty(PrimaryName))
                return DisplayText;

            string marker = "";
            if (IsCalledName && IsNickname)
                marker = " [R★]";
            else if (IsCalledName)
                marker = " [R]";
            else if (IsNickname)
                marker = " [★]";

            DisplayText = PrimaryName + marker;
            if (!string.IsNullOrEmpty(Synonym))
            {
                DisplayText += $" ({Synonym})";
            }

            return DisplayText;
        }

        /// <summary>
        /// Creates a deep copy of this model.
        /// </summary>
        /// <returns>A new VornamModel instance with copied values.</returns>
        public VornamModel Clone()
        {
            return new VornamModel
            {
                PersonId = PersonId,
                PrimaryName = PrimaryName,
                Synonym = Synonym,
                TextKennz = TextKennz,
                LineNumber = LineNumber,
                IsCalledName = IsCalledName,
                IsNickname = IsNickname,
                DisplayText = DisplayText,
                NameDesignation = NameDesignation
            };
        }
    }
}
