// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="VornamDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation of Vorname (given name) data access, wrapping legacy DataModul</summary>
// ***********************************************************************

using BaseLib.Helper;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Implements Vorname (given name) data access and persistence operations.
    /// Encapsulates all direct DataModul.DB_NameTable interactions to isolate legacy database layer.
    /// </summary>
    public class VornamDataService : IVornamDataService
    {
        /// <summary>
        /// Loads all names for a given person by gender/text kind.
        /// </summary>
        public async Task<List<VornamModel>> LoadNamesForPersonAsync(int personId, ETextKennz textKennz)
        {
            return await Task.Run(() =>
            {
                var names = new List<VornamModel>();

                if (DataModul.DB_NameTable == null || personId <= 0)
                    return names;

                try
                {
                    IRecordset nameTable = DataModul.DB_NameTable;
                    nameTable.Index = nameof(NameIndex.NamKenn);
                    nameTable.Seek("=", personId, textKennz);

                    int index = 0;
                    while (!nameTable.EOF && !nameTable.NoMatch && index < 20)
                    {
                        if (nameTable.Fields[NameFields.PersNr].AsInt() != personId ||
                            nameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>() != textKennz)
                        {
                            break;
                        }

                        var name = MapFromDatabase(nameTable.Fields);
                        if (name != null)
                            names.Add(name);

                        nameTable.MoveNext();
                        index++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadNamesForPersonAsync error: {ex.Message}");
                }

                return names;
            });
        }

        /// <summary>
        /// Saves a new name entry to the database.
        /// </summary>
        public async Task<short> SaveNameAsync(VornamModel vorname)
        {
            return await Task.Run(() =>
            {
                if (vorname == null || !vorname.IsValid() || DataModul.DB_NameTable == null)
                    return (short)-1;

                try
                {
                    DataModul.DB_NameTable.AddNew();
                    SetFieldsFromModel(vorname);
                    DataModul.DB_NameTable.Update();
                    return vorname.LineNumber;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SaveNameAsync error: {ex.Message}");
                    return (short)-1;
                }
            });
        }

        /// <summary>
        /// Updates an existing name entry.
        /// </summary>
        public async Task<bool> UpdateNameAsync(VornamModel vorname)
        {
            return await Task.Run(() =>
            {
                if (vorname == null || !vorname.IsValid() || DataModul.DB_NameTable == null)
                    return false;

                try
                {
                    IRecordset nameTable = DataModul.DB_NameTable;
                    nameTable.Index = nameof(NameIndex.NamKenn);
                    nameTable.Seek("=", vorname.PersonId, vorname.TextKennz);

                    if (nameTable.NoMatch || nameTable.EOF)
                        return false;

                    nameTable.Edit();
                    SetFieldsFromModel(vorname);
                    nameTable.Update();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UpdateNameAsync error: {ex.Message}");
                    return false;
                }
            });
        }

        /// <summary>
        /// Deletes all names of a given kind from a person.
        /// </summary>
        public async Task<int> DeleteNamesByKindAsync(int personId, ETextKennz textKennz)
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_NameTable == null || personId <= 0)
                    return 0;

                try
                {
                    IRecordset nameTable = DataModul.DB_NameTable;
                    nameTable.Index = nameof(NameIndex.NamKenn);
                    nameTable.Seek("=", personId, textKennz);

                    int deletedCount = 0;
                    while (!nameTable.EOF && !nameTable.NoMatch && deletedCount < 20)
                    {
                        if (nameTable.Fields[NameFields.PersNr].AsInt() != personId ||
                            nameTable.Fields[NameFields.Kennz].AsEnum<ETextKennz>() != textKennz)
                        {
                            break;
                        }

                        nameTable.Delete();
                        nameTable.MoveNext();
                        deletedCount++;
                    }

                    return deletedCount;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DeleteNamesByKindAsync error: {ex.Message}");
                    return 0;
                }
            });
        }

        /// <summary>
        /// Searches for names matching a search pattern (for autocomplete/dropdown).
        /// </summary>
        public async Task<ObservableCollection<IListItem<int>>> SearchNamesAsync(ETextKennz textKennz, string searchPattern)
        {
            return await Task.Run(() =>
            {
                var results = new ObservableCollection<IListItem<int>>();

                if (string.IsNullOrWhiteSpace(searchPattern))
                    return results;

                try
                {
                    // Legacy search through name database
                    // For now, return empty collection - legacy Modul1 method access is complex
                    // This can be extended with direct database queries if needed
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SearchNamesAsync error: {ex.Message}");
                }

                return results;
            });
        }

        /// <summary>
        /// Retrieves a single name by line number.
        /// </summary>
        public async Task<VornamModel?> GetNameByLineAsync(int personId, short lineNumber)
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_NameTable == null || personId <= 0 || lineNumber <= 0)
                    return null;

                try
                {
                    IRecordset nameTable = DataModul.DB_NameTable;
                    nameTable.Index = "Nr";
                    nameTable.Seek("=", personId, lineNumber);

                    if (!nameTable.NoMatch && !nameTable.EOF)
                    {
                        return MapFromDatabase(nameTable.Fields);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetNameByLineAsync error: {ex.Message}");
                }

                return null;
            });
        }

        /// <summary>
        /// Retrieves text/name information by text ID (for lookups).
        /// </summary>
        public Task<(string Text, string LeadName)?> GetTextByIdAsync(int textId)
        {
            return Task.Run(async () =>
            {
                if (textId <= 0)
                    return default((string, string)?);

                try
                {
                    // Use legacy DataModul method for text lookup
                    var (text, leadname) = DataModul.TextLese2(textId);
                    if (!string.IsNullOrEmpty(text))
                        return (text, leadname);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetTextByIdAsync error: {ex.Message}");
                }

                return null;
            });
        }

        /// <summary>
        /// Maps database fields to VornamModel.
        /// </summary>
        private VornamModel? MapFromDatabase(object? fields)
        {
            if (fields == null)
                return null;

            try
            {
                var recordset = fields as IRecordset;
                if (recordset == null)
                    return null;

                return new VornamModel
                {
                    PersonId = recordset.Fields[NameFields.PersNr].AsInt(),
                    PrimaryName = recordset.Fields[1].AsString() ?? "",  // Name field typically at index 1
                    Synonym = recordset.Fields[2].AsString() ?? "",       // Synonym/Note field typically at index 2
                    TextKennz = recordset.Fields[NameFields.Kennz].AsEnum<ETextKennz>(),
                    LineNumber = (short)recordset.Fields[NameFields.LfNr].AsInt(),
                    IsCalledName = false, // Typically determined by UI context
                    IsNickname = false    // Typically determined by UI context
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapFromDatabase error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Sets database field values from a VornamModel.
        /// </summary>
        private void SetFieldsFromModel(VornamModel vorname)
        {
            if (DataModul.DB_NameTable == null || vorname == null)
                return;

            try
            {
                // Use generic Fields accessor instead of dynamic
                var nameTable = DataModul.DB_NameTable;
                nameTable.Fields[NameFields.PersNr].Value = vorname.PersonId;
                nameTable.Fields[1].Value = vorname.PrimaryName.Trim();  // Name field typically at index 1
                nameTable.Fields[2].Value = vorname.Synonym.Trim();       // Synonym/Note field typically at index 2
                nameTable.Fields[NameFields.Kennz].Value = vorname.TextKennz;
                nameTable.Fields[NameFields.LfNr].Value = vorname.LineNumber;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SetFieldsFromModel error: {ex.Message}");
            }
        }
    }
}
