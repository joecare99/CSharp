// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="PersonDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation for person name and genealogy data access</summary>
// ***********************************************************************

using GenFreeWin.Services.Interfaces;
using GenFree.Data;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.Sys;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenFreeWin.Services
{
    /// <summary>
    /// Implements person data queries and name consolidation logic.
    /// Abstracts direct DataModul access for person genealogy operations.
    /// </summary>
    public class PersonDataService : IPersonDataService
    {
        private readonly IModul1 _modul1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonDataService"/> class.
        /// </summary>
        /// <param name="modul1">The genealogy data module.</param>
        public PersonDataService(IModul1 modul1)
        {
            _modul1 = modul1 ?? throw new ArgumentNullException(nameof(modul1));
        }

        /// <summary>
        /// Gets the formatted full name for a person.
        /// </summary>
        public Task<string> GetFullNameAsync(int personId)
        {
            return Task.Run(() =>
            {
                try
                {
                    _modul1.PersInArb = personId;
                    _modul1.Person_ReadNames(personId, _modul1.Person);

                    _modul1.Person.SetFullSurname(_modul1.Person.SurName);
                    if (_modul1.Person.Prefix != "")
                        _modul1.Person.SetFullSurname(_modul1.Person.Prefix + " " + _modul1.Person.FullSurName);
                    if (_modul1.Person.Suffix != "")
                        _modul1.Person.SetFullSurname(_modul1.Person.FullSurName + " " + _modul1.Person.Suffix);
                    if (_modul1.Person.Alias != "")
                    {
                        // Append alias if present: " (alias)"
                        var aliasText = " (" + _modul1.Person.Alias + ")";
                        _modul1.Person.SetFullSurname(_modul1.Person.FullSurName + aliasText);
                    }

                    return _modul1.Person.Givennames + " " + _modul1.Person.FullSurName;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.GetFullNameAsync error: {ex.Message}");
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Gets the sex indicator for a person.
        /// </summary>
        public Task<string> GetSexAsync(int personId)
        {
            return Task.Run(() =>
            {
                try
                {
                    return DataModul.Person.GetSex(personId);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.GetSexAsync error: {ex.Message}");
                    return string.Empty;
                }
            });
        }

        /// <summary>
        /// Consolidates names and aliases when merging two persons.
        /// </summary>
        public Task<bool> ConsolidateNamesAsync(int sourcePersonId, int targetPersonId)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Stub: Extracted from DubViewModel.Namen() method
                    // TODO: Full implementation:
                    // - Load source person name data
                    // - Load target person name data
                    // - Merge remarks (Bem1, Bem2, Bem3) with *** separator
                    // - Consolidate alias (C_) and usage name (U_) text entries
                    // - Update target person record with consolidated names
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.ConsolidateNamesAsync error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Gets ancestor/ancestry data indicators for a person.
        /// Returns a tuple of (ancestorCount, ancestorData).
        /// </summary>
        public Task<(int AncestorCount, string AncestorData)> GetAncestorDataAsync(int personId)
        {
            return Task.Run(() =>
            {
                try
                {
                    _modul1.PersInArb = personId;
                    _modul1.Person_ReadNames(personId, _modul1.Person);

                    var ancestorData = _modul1.Ancesters_GetPersonData(personId, out var ancestorCount, out _);
                    return (ancestorCount, ancestorData ?? string.Empty);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.GetAncestorDataAsync error: {ex.Message}");
                    return (0, string.Empty);
                }
            });
        }

        /// <summary>
        /// Retrieves the parent link for a person (parents' marriage record).
        /// Returns 0 if no parent family exists.
        /// </summary>
        public Task<int> GetParentFamilyAsync(int personId)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Use ReadAllPers to get child links (links to parents' marriage)
                    var childEnumerable = DataModul.Link?.ReadAllPers(personId, ELinkKennz.lkChild);
                    if (childEnumerable != null)
                    {
                        var childLinks = new List<ILinkData>(childEnumerable);
                        if (childLinks.Count > 0)
                        {
                            // Return the first parent family ID found
                            return childLinks[0].iFamNr;
                        }
                    }
                    return 0;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.GetParentFamilyAsync error: {ex.Message}");
                    return 0;
                }
            });
        }

        /// <summary>
        /// Retrieves all marriage/partnership family IDs for a person.
        /// </summary>
        public Task<int[]> GetSpouseFamiliesAsync(int personId)
        {
            return Task.Run(() =>
            {
                try
                {
                    var families = new List<int>();

                    // Check all spouse kennz types (lkFather=4, lkMother=5, lkChild=2, lk9=9, lkGodparent=6, etc.)
                    for (int kennz = 4; kennz <= 9; kennz++)
                    {
                        var eLinkKennz = (ELinkKennz)kennz;
                        foreach (var link in DataModul.Link.ReadAllPers(personId, eLinkKennz))
                        {
                            if (!families.Contains(link.iFamNr))
                                families.Add(link.iFamNr);
                            if (families.Count > 99)
                                break;
                        }
                    }

                    return families.ToArray();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"PersonDataService.GetSpouseFamiliesAsync error: {ex.Message}");
                    return Array.Empty<int>();
                }
            });
        }
    }
}
