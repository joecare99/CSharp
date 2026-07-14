// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="PersonSearchData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Domain model for person search data storage (extracted from NamenSuchViewModel)</summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace GenFreeWin.ViewModels.Models
{
    /// <summary>
    /// Domain model representing person/family/event data extracted from legacy VB-style ViewModel storage.
    /// Consolidates ~25 legacy fields from NamenSuchViewModel into a single, testable, reusable model.
    /// 
    /// This model holds the data state that was previously scattered as private fields in NamenSuchViewModel,
    /// enabling better separation of concerns and improved testability.
    /// </summary>
    public class PersonSearchData
    {
        /// <summary>
        /// Account/Record number (legacy: An)
        /// </summary>
        public int AccountNumber { get; set; }

        /// <summary>
        /// Primary person identifier (legacy: ID)
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// Person specification/type (legacy: PersSp)
        /// </summary>
        public int PersonSpecification { get; set; }

        /// <summary>
        /// Full name of the person (legacy: Namen)
        /// </summary>
        public string PersonName { get; set; } = "";

        /// <summary>
        /// Identifier/marker (legacy: Kennzt)
        /// </summary>
        public string Identifier { get; set; } = "";

        /// <summary>
        /// Remark script indicator (legacy: BemSch)
        /// </summary>
        public short RemarkScript { get; set; }

        /// <summary>
        /// Sequential/sequence number (legacy: LfNR)
        /// </summary>
        public short SequenceNumber { get; set; }

        /// <summary>
        /// Family processing/handling indicator (legacy: Fambehk)
        /// </summary>
        public int FamilyProcessing { get; set; }

        /// <summary>
        /// Occupation/profession (legacy: Beruf)
        /// </summary>
        public short Occupation { get; set; }

        /// <summary>
        /// Array of contact/collection points (legacy: KontSP, size=50)
        /// </summary>
        public string[] ContactPoints { get; set; } = new string[50];

        /// <summary>
        /// Secondary contact/collection points array (legacy: KontSP1, size=50)
        /// </summary>
        public string[] ContactPoints2 { get; set; } = new string[50];

        /// <summary>
        /// Array of first names (legacy: Vorn)
        /// </summary>
        public int[]? FirstNameIndices { get; set; }

        /// <summary>
        /// Array of call/nick names (legacy: Ruf)
        /// </summary>
        public string[]? CallNames { get; set; }

        /// <summary>
        /// Date of birth or relevant date (legacy: Datu)
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Privacy/output mode (legacy: privaus)
        /// </summary>
        public int PrivacyOutputMode { get; set; }

        /// <summary>
        /// Output/display options list (legacy: asOption, corresponds to EOutCfg enum)
        /// </summary>
        public List<string> OutputOptions { get; set; } = new();

        /// <summary>
        /// Related event flag (legacy: EreiRf)
        /// </summary>
        public bool IsRelatedEvent { get; set; }

        /// <summary>
        /// Divorce/separation indicator (legacy: Scheid)
        /// </summary>
        public bool IsDivorced { get; set; }

        /// <summary>
        /// Death mark/indicator (legacy: xDeathMark)
        /// </summary>
        public bool IsDeathMarked { get; set; }

        /// <summary>
        /// Counter iterator I1 (legacy: I1) - used for loop iteration
        /// </summary>
        public int IteratorI1 { get; set; }

        /// <summary>
        /// Counter A (legacy: A) - used for iteration
        /// </summary>
        public int CounterA { get; set; }

        /// <summary>
        /// Counter Z (legacy: Z) - used for iteration
        /// </summary>
        public int CounterZ { get; set; }

        /// <summary>
        /// Counter U (legacy: U) - used for iteration
        /// </summary>
        public int CounterU { get; set; }

        /// <summary>
        /// Module1 private field reference (legacy: Modul1_priv)
        /// </summary>
        public int Module1Flag { get; set; }

        /// <summary>
        /// Module1 designation/description (legacy: Modul1_Bezeichnu)
        /// </summary>
        public string Module1Designation { get; set; } = "";

        /// <summary>
        /// Creates a new instance of PersonSearchData with default/empty values.
        /// </summary>
        public PersonSearchData()
        {
            // Initialize collections to prevent null references
            OutputOptions = new List<string>(50);
            ContactPoints = new string[50];
            ContactPoints2 = new string[50];
        }

        /// <summary>
        /// Resets all data to default/empty state.
        /// </summary>
        public void Clear()
        {
            AccountNumber = 0;
            PersonId = 0;
            PersonSpecification = 0;
            PersonName = "";
            Identifier = "";
            RemarkScript = 0;
            SequenceNumber = 0;
            FamilyProcessing = 0;
            Occupation = 0;
            Array.Clear(ContactPoints, 0, ContactPoints.Length);
            Array.Clear(ContactPoints2, 0, ContactPoints2.Length);
            FirstNameIndices = null;
            CallNames = null;
            DateOfBirth = DateTime.MinValue;
            PrivacyOutputMode = 0;
            OutputOptions.Clear();
            IsRelatedEvent = false;
            IsDivorced = false;
            IsDeathMarked = false;
            IteratorI1 = 0;
            CounterA = 0;
            CounterZ = 0;
            CounterU = 0;
            Module1Flag = 0;
            Module1Designation = "";
        }

        /// <summary>
        /// Returns a string representation of the person data (for debugging).
        /// </summary>
        public override string ToString()
        {
            return $"PersonSearchData: ID={PersonId}, Name={PersonName}, DOB={DateOfBirth:yyyy-MM-dd}";
        }
    }
}
