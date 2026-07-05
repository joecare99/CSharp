// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
//
// Last Modified By : Mir
// Last Modified On : 2025
// ***********************************************************************
// <copyright file="HGAkteDataService.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Implementation of HGakte (Grundbuchakte) data access, wrapping legacy DataModul</summary>
// ***********************************************************************

using BaseLib.Helper;
using Gen_FreeWin.Models;
using Gen_FreeWin.Services.Interfaces;
using GenFree.Data;
using GenFree.Helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gen_FreeWin.Services
{
    /// <summary>
    /// Implements HGakte (Grundbuchakte) data access and persistence operations.
    /// Encapsulates all direct DataModul interactions to isolate legacy database layer.
    /// </summary>
    public class HGAkteDataService : IHGAkteDataService
    {
        /// <summary>
        /// Loads all Grundbuchakten in a single operation.
        /// </summary>
        public async Task<List<HGAkteModel>> LoadAllAktenAsync()
        {
            return await Task.Run(() =>
            {
                var akten = new List<HGAkteModel>();

                if (DataModul.DB_HGATable == null || DataModul.DB_HGATable.RecordCount == 0)
                {
                    return akten;
                }

                DataModul.DB_HGATable.MoveFirst();
                while (!DataModul.DB_HGATable.EOF)
                {
                    var akte = MapFromDatabase_HGA(DataModul.DB_HGATable.Fields);
                    akten.Add(akte);
                    DataModul.DB_HGATable.MoveNext();
                }

                return akten;
            });
        }

        /// <summary>
        /// Loads a single Akte by its numeric ID.
        /// </summary>
        public async Task<HGAkteModel> LoadAkteByIdAsync(int akteId)
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_HGATable == null)
                {
                    return null;
                }

                DataModul.DB_HGATable.Index = "Nr";
                DataModul.DB_HGATable.Seek("=", akteId);

                if (DataModul.DB_HGATable.NoMatch)
                {
                    return null;
                }

                return MapFromDatabase_HGA(DataModul.DB_HGATable.Fields);
            });
        }

        /// <summary>
        /// Loads a single Akte by its Akte number (string identifier).
        /// </summary>
        public async Task<HGAkteModel> LoadAkteByNumberAsync(string akteNumber)
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_HGATable == null || string.IsNullOrWhiteSpace(akteNumber))
                {
                    return null;
                }

                DataModul.DB_HGATable.Index = "Akte";
                DataModul.DB_HGATable.Seek("=", akteNumber.Trim());

                if (DataModul.DB_HGATable.NoMatch)
                {
                    return null;
                }

                return MapFromDatabase_HGA(DataModul.DB_HGATable.Fields);
            });
        }

        /// <summary>
        /// Saves a new Akte to the database.
        /// </summary>
        public async Task<int> SaveAkteAsync(HGAkteModel akte)
        {
            return await Task.Run(() =>
            {
                if (akte == null || !akte.IsValid())
                {
                    throw new ArgumentException("Akte must be valid before saving.");
                }

                DataModul.DB_HGATable.AddNew();
                SetFieldsFromModel_HGA(akte);
                DataModul.DB_HGATable.Update();

                return akte.Id;
            });
        }

        /// <summary>
        /// Updates an existing Akte record.
        /// </summary>
        public async Task UpdateAkteAsync(HGAkteModel akte)
        {
            await Task.Run(() =>
            {
                if (akte == null || !akte.IsValid())
                {
                    throw new ArgumentException("Akte must be valid before updating.");
                }

                DataModul.DB_HGATable.Index = "Nr";
                DataModul.DB_HGATable.Seek("=", akte.Id);

                if (DataModul.DB_HGATable.NoMatch)
                {
                    throw new InvalidOperationException($"Akte with ID {akte.Id} not found.");
                }

                DataModul.DB_HGATable.Edit();
                SetFieldsFromModel_HGA(akte);
                DataModul.DB_HGATable.Update();
            });
        }

        /// <summary>
        /// Deletes an Akte by its ID.
        /// </summary>
        public async Task DeleteAkteAsync(int akteId)
        {
            await Task.Run(() =>
            {
                DataModul.DB_HGATable.Index = "Nr";
                DataModul.DB_HGATable.Seek("=", akteId);

                if (!DataModul.DB_HGATable.NoMatch)
                {
                    DataModul.DB_HGATable.Delete();
                }
            });
        }

        /// <summary>
        /// Gets the next available Akte ID (for creating new Akten).
        /// </summary>
        public async Task<int> GetNextAkteIdAsync()
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_HGATable == null || DataModul.DB_HGATable.RecordCount == 0)
                {
                    return 1;
                }

                DataModul.DB_HGATable.Index = "Nr";
                DataModul.DB_HGATable.MoveLast();

                return DataModul.DB_HGATable.Fields[HGAFields.Nr].AsInt() + 1;
            });
        }

        /// <summary>
        /// Loads all Grundbucheinträge (GBE) for a given Akte.
        /// </summary>
        public async Task<List<GBEModel>> LoadGBEsForAkteAsync(string akteNumber)
        {
            return await Task.Run(() =>
            {
                var gbes = new List<GBEModel>();

                if (DataModul.DB_GbeTable == null || string.IsNullOrWhiteSpace(akteNumber))
                {
                    return gbes;
                }

                DataModul.DB_GbeTable.Index = "Akte";
                DataModul.DB_GbeTable.Seek("=", akteNumber.Trim());

                while (!DataModul.DB_GbeTable.EOF && !DataModul.DB_GbeTable.NoMatch
                    && DataModul.DB_GbeTable.Fields[GBEFields.Akte].AsString() == akteNumber.Trim())
                {
                    var gbe = MapFromDatabase_GBE(DataModul.DB_GbeTable.Fields);
                    gbes.Add(gbe);
                    DataModul.DB_GbeTable.MoveNext();
                }

                return gbes;
            });
        }

        /// <summary>
        /// Loads a single GBE by its numeric ID.
        /// </summary>
        public async Task<GBEModel> LoadGBEByIdAsync(int gbeId)
        {
            return await Task.Run(() =>
            {
                if (DataModul.DB_GbeTable == null)
                {
                    return null;
                }

                DataModul.DB_GbeTable.Index = "Nr";
                DataModul.DB_GbeTable.Seek("=", gbeId);

                if (DataModul.DB_GbeTable.NoMatch)
                {
                    return null;
                }

                return MapFromDatabase_GBE(DataModul.DB_GbeTable.Fields);
            });
        }

        /// <summary>
        /// Saves a new GBE record.
        /// </summary>
        public async Task<int> SaveGBEAsync(GBEModel gbe)
        {
            return await Task.Run(() =>
            {
                if (gbe == null || !gbe.IsValid())
                {
                    throw new ArgumentException("GBE must be valid before saving.");
                }

                DataModul.DB_GbeTable.AddNew();
                SetFieldsFromModel_GBE(gbe);
                DataModul.DB_GbeTable.Update();

                return gbe.Id;
            });
        }

        /// <summary>
        /// Updates an existing GBE record.
        /// </summary>
        public async Task UpdateGBEAsync(GBEModel gbe)
        {
            await Task.Run(() =>
            {
                if (gbe == null || !gbe.IsValid())
                {
                    throw new ArgumentException("GBE must be valid before updating.");
                }

                DataModul.DB_GbeTable.Index = "Nr";
                DataModul.DB_GbeTable.Seek("=", gbe.Id);

                if (DataModul.DB_GbeTable.NoMatch)
                {
                    throw new InvalidOperationException($"GBE with ID {gbe.Id} not found.");
                }

                DataModul.DB_GbeTable.Edit();
                SetFieldsFromModel_GBE(gbe);
                DataModul.DB_GbeTable.Update();
            });
        }

        /// <summary>
        /// Deletes a GBE by its ID.
        /// </summary>
        public async Task DeleteGBEAsync(int gbeId)
        {
            await Task.Run(() =>
            {
                DataModul.DB_GbeTable.Index = "Nr";
                DataModul.DB_GbeTable.Seek("=", gbeId);

                if (!DataModul.DB_GbeTable.NoMatch)
                {
                    DataModul.DB_GbeTable.Delete();
                }
            });
        }

        /// <summary>
        /// Loads all properties/persons using a specific Akte.
        /// </summary>
        public async Task<List<PropertyUsageModel>> LoadPropertyUsagesAsync(string akteNumber)
        {
            return await Task.Run(() =>
            {
                var usages = new List<PropertyUsageModel>();

                if (DataModul.DB_PropertyTable == null || string.IsNullOrWhiteSpace(akteNumber))
                {
                    return usages;
                }

                DataModul.DB_PropertyTable.Index = "Akte";
                DataModul.DB_PropertyTable.Seek("=", akteNumber.Trim());

                while (!DataModul.DB_PropertyTable.EOF && !DataModul.DB_PropertyTable.NoMatch
                    && DataModul.DB_PropertyTable.Fields[PropertyFields.Akte].AsString() == akteNumber.Trim())
                {
                    var usage = new PropertyUsageModel
                    {
                        AkteNumber = akteNumber.Trim(),
                        PersonId = DataModul.DB_PropertyTable.Fields[PropertyFields.Pers].AsInt(),
                        PersonName = "" // Will be populated by UseCase via Modul1
                    };
                    usages.Add(usage);
                    DataModul.DB_PropertyTable.MoveNext();
                }

                return usages;
            });
        }

        /// <summary>
        /// Searches Akten by Akte number pattern (partial match).
        /// </summary>
        public async Task<List<HGAkteModel>> SearchAktenAsync(string searchPattern)
        {
            return await Task.Run(() =>
            {
                var results = new List<HGAkteModel>();

                if (DataModul.DB_HGATable == null || string.IsNullOrWhiteSpace(searchPattern))
                {
                    return results;
                }

                string pattern = searchPattern.Trim().ToLower();
                DataModul.DB_HGATable.MoveFirst();

                while (!DataModul.DB_HGATable.EOF)
                {
                    var akteNumber = DataModul.DB_HGATable.Fields[HGAFields.Akte].AsString();
                    if (akteNumber.ToLower().Contains(pattern))
                    {
                        results.Add(MapFromDatabase_HGA(DataModul.DB_HGATable.Fields));
                    }
                    DataModul.DB_HGATable.MoveNext();
                }

                return results;
            });
        }

        /// <summary>
        /// Maps database fields to HGAkteModel.
        /// </summary>
        private HGAkteModel MapFromDatabase_HGA(object? fields)
        {
            if (fields == null)
                return null;

            try
            {
                dynamic dynFields = fields;
                return new HGAkteModel
                {
                    Id = Convert.ToInt32(dynFields[HGAFields.Nr] ?? 0),
                    AkteNumber = (string)(dynFields[HGAFields.Akte] ?? ""),
                    Kirchspiel = (string)(dynFields[HGAFields.Kirchspiel] ?? ""),
                    Beschreibung = (string)(dynFields[HGAFields.Beschr] ?? ""),
                    Hof = (string)(dynFields[HGAFields.Hof] ?? ""),
                    Brandkasse = (string)(dynFields[HGAFields.Brandkasse] ?? ""),
                    Bemerkungen = (string)(dynFields[HGAFields.Bem] ?? ""),
                    Flur = (string)(dynFields[HGAFields.Flur] ?? ""),
                    Parzelle = (string)(dynFields[HGAFields.Parzelle] ?? "")
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapFromDatabase_HGA error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Maps HGAkteModel to database fields (for Add/Edit).
        /// </summary>
        private void SetFieldsFromModel_HGA(HGAkteModel model)
        {
            DataModul.DB_HGATable.Fields[HGAFields.Nr].Value = model.Id;
            DataModul.DB_HGATable.Fields[HGAFields.Akte].Value = model.AkteNumber;
            DataModul.DB_HGATable.Fields[HGAFields.Kirchspiel].Value = model.Kirchspiel;
            DataModul.DB_HGATable.Fields[HGAFields.Beschr].Value = model.Beschreibung;
            DataModul.DB_HGATable.Fields[HGAFields.Hof].Value = model.Hof;
            DataModul.DB_HGATable.Fields[HGAFields.Brandkasse].Value = model.Brandkasse;
            DataModul.DB_HGATable.Fields[HGAFields.Bem].Value = model.Bemerkungen;
            DataModul.DB_HGATable.Fields[HGAFields.Flur].Value = model.Flur;
            DataModul.DB_HGATable.Fields[HGAFields.Parzelle].Value = model.Parzelle;
        }

        /// <summary>
        /// Maps database fields to GBEModel.
        /// </summary>
        private GBEModel MapFromDatabase_GBE(dynamic fields)
        {
            return new GBEModel
            {
                Id = Convert.ToInt32(fields[GBEFields.Nr] ?? 0),
                AkteNumber = (string)fields[GBEFields.Akte] ?? "",
                Jahr = (string)fields[GBEFields.Jahr] ?? "",
                Name = (string)fields[GBEFields.Name] ?? "",
                Geb = (string)fields[GBEFields.Geb] ?? "",
                Erb = (string)fields[GBEFields.Erb] ?? "",
                Abg = (string)fields[GBEFields.Abg] ?? ""
            };
        }

        /// <summary>
        /// Maps GBEModel to database fields (for Add/Edit).
        /// </summary>
        private void SetFieldsFromModel_GBE(GBEModel model)
        {
            DataModul.DB_GbeTable.Fields[GBEFields.Nr].Value = model.Id;
            DataModul.DB_GbeTable.Fields[GBEFields.Akte].Value = model.AkteNumber;
            DataModul.DB_GbeTable.Fields[GBEFields.Jahr].Value = model.Jahr;
            DataModul.DB_GbeTable.Fields[GBEFields.Name].Value = model.Name;
            DataModul.DB_GbeTable.Fields[GBEFields.Geb].Value = model.Geb;
            DataModul.DB_GbeTable.Fields[GBEFields.Erb].Value = model.Erb;
            DataModul.DB_GbeTable.Fields[GBEFields.Abg].Value = model.Abg;
        }
    }
}
