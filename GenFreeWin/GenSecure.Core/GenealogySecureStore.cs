using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using global::GenSecure.Contracts;

namespace GenSecure.Core;

/// <summary>
/// Persists genealogies in a sharded, Git-friendly file structure with per-person encryption.
/// </summary>
public sealed class GenealogySecureStore : IGenealogySecureStore
{
    private const string ManifestFileName = "manifest.json";
    private const string CurrentVersion = "1.0";
    private readonly MasterKeyBackupService _masterKeyBackupService;
    private readonly GenSecureStoreOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogySecureStore"/> class.
    /// </summary>
    /// <param name="masterKeyBackupService">The master key provider.</param>
    /// <param name="options">The store options.</param>
    public GenealogySecureStore(MasterKeyBackupService masterKeyBackupService, GenSecureStoreOptions options)
    {
        _masterKeyBackupService = masterKeyBackupService ?? throw new ArgumentNullException(nameof(masterKeyBackupService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Save(string sGenealogyId, IGenealogy genealogy, Func<IGenPerson, StoreMode>? getPersonStoreMode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sGenealogyId);
        ArgumentNullException.ThrowIfNull(genealogy);

        EnsureSupportedCollections(genealogy);

        string sGenealogyRootDirectory = GetGenealogyRootDirectory(sGenealogyId);
        string sManifestFilePath = GetManifestFilePath(sGenealogyId);
        GenealogyManifestRecord? previousManifest = File.Exists(sManifestFilePath)
            ? CryptoUtilities.ReadJson<GenealogyManifestRecord>(sManifestFilePath)
            : null;

        IReadOnlyList<IGenPerson> lstPersons = genealogy.Entitys.OfType<IGenPerson>()
            .OrderBy(genPerson => GetEntityIdentitySeed(genPerson))
            .ToArray();
        IReadOnlyList<IGenFamily> lstFamilies = genealogy.Entitys.OfType<IGenFamily>()
            .OrderBy(genFamily => GetEntityIdentitySeed(genFamily))
            .ToArray();
        IReadOnlyList<IGenPlace> lstPlaces = genealogy.Places
            .OrderBy(genPlace => GetPlaceIdentitySeed(genPlace))
            .ToArray();

        EnsureNoUnsupportedEntities(genealogy.Entitys, lstPersons.Count + lstFamilies.Count);

        Dictionary<IGenEntity, string> dictEntityRecordIds = CreateEntityRecordIdMap(lstPersons, lstFamilies);
        Dictionary<IGenPlace, string> dictPlaceRecordIds = CreatePlaceRecordIdMap(lstPlaces);
        Dictionary<IGenPerson, StoreMode> dictPersonStoreModes = lstPersons.ToDictionary(
            genPerson => genPerson,
            genPerson => getPersonStoreMode?.Invoke(genPerson) ?? GetDefaultStoreMode(genPerson));
        PersonSecureStore personStore = CreatePersonStore(sGenealogyRootDirectory);

        foreach (IGenPerson genPerson in lstPersons)
        {
            string sRecordId = dictEntityRecordIds[genPerson];
            StoreMode eStoreMode = dictPersonStoreModes[genPerson];
            GenealogyEntityRecord personRecord = CreateEntityRecord(genPerson, sRecordId, dictEntityRecordIds, dictPlaceRecordIds, "person");
            personStore.Save(sRecordId, personRecord, eStoreMode);
        }

        foreach (IGenFamily genFamily in lstFamilies)
        {
            string sRecordId = dictEntityRecordIds[genFamily];
            GenealogyEntityRecord familyRecord = CreateEntityRecord(genFamily, sRecordId, dictEntityRecordIds, dictPlaceRecordIds, "family");
            CryptoUtilities.WriteJson(GetShardedCategoryFilePath(sGenealogyRootDirectory, "families", sRecordId, ".family.json"), familyRecord);
        }

        foreach (IGenPlace genPlace in lstPlaces)
        {
            string sRecordId = dictPlaceRecordIds[genPlace];
            GenealogyPlaceRecord placeRecord = CreatePlaceRecord(genPlace, sRecordId, dictPlaceRecordIds);
            CryptoUtilities.WriteJson(GetShardedCategoryFilePath(sGenealogyRootDirectory, "places", sRecordId, ".place.json"), placeRecord);
        }

        var manifest = new GenealogyManifestRecord
        {
            Version = CurrentVersion,
            GenealogyId = sGenealogyId,
            UId = genealogy.UId,
            UpdatedUtc = DateTimeOffset.UtcNow,
            Persons = lstPersons.Select(genPerson => new GenealogyManifestEntry
            {
                RecordId = dictEntityRecordIds[genPerson],
                UId = genPerson.UId,
                StoreMode = dictPersonStoreModes[genPerson],
            }).ToList(),
            Families = lstFamilies.Select(genFamily => new GenealogyManifestEntry
            {
                RecordId = dictEntityRecordIds[genFamily],
                UId = genFamily.UId,
            }).ToList(),
            Places = lstPlaces.Select(genPlace => new GenealogyManifestEntry
            {
                RecordId = dictPlaceRecordIds[genPlace],
                UId = genPlace.UId,
            }).ToList(),
        };

        Directory.CreateDirectory(sGenealogyRootDirectory);
        CryptoUtilities.WriteJson(sManifestFilePath, manifest);
        CleanupObsoleteRecords(sGenealogyRootDirectory, previousManifest, manifest, personStore);
    }

    /// <inheritdoc />
    public IGenealogy Load(string sGenealogyId, IGenealogyModelFactory factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sGenealogyId);
        ArgumentNullException.ThrowIfNull(factory);

        string sManifestFilePath = GetManifestFilePath(sGenealogyId);
        if (!File.Exists(sManifestFilePath))
        {
            throw new FileNotFoundException($"The genealogy '{sGenealogyId}' does not exist.", sManifestFilePath);
        }

        GenealogyManifestRecord manifest = CryptoUtilities.ReadJson<GenealogyManifestRecord>(sManifestFilePath);
        EnsureManifestVersion(manifest);
        string sGenealogyRootDirectory = GetGenealogyRootDirectory(sGenealogyId);
        PersonSecureStore personStore = CreatePersonStore(sGenealogyRootDirectory);
        IGenealogy genealogy = factory.CreateGenealogy(manifest.UId);
        Dictionary<string, IGenPlace> dictPlaces = new(StringComparer.Ordinal);
        Dictionary<string, IGenEntity> dictEntities = new(StringComparer.Ordinal);
        Dictionary<string, GenealogyEntityRecord> dictEntityRecords = new(StringComparer.Ordinal);

        foreach (GenealogyManifestEntry manifestEntry in manifest.Places.OrderBy(entry => entry.RecordId, StringComparer.Ordinal))
        {
            GenealogyPlaceRecord placeRecord = CryptoUtilities.ReadJson<GenealogyPlaceRecord>(GetShardedCategoryFilePath(sGenealogyRootDirectory, "places", manifestEntry.RecordId, ".place.json"));
            IGenPlace genPlace = factory.CreatePlace(placeRecord.UId, placeRecord.ID, placeRecord.LastChange);
            genPlace.Name = placeRecord.Name;
            genPlace.Type = placeRecord.Type;
            genPlace.GOV_ID = placeRecord.GovId;
            genPlace.Latitude = placeRecord.Latitude;
            genPlace.Longitude = placeRecord.Longitude;
            genPlace.Notes = placeRecord.Notes;
            genealogy.Places.Add(genPlace);
            SetGenealogyOwner(genPlace, genealogy);
            dictPlaces.Add(manifestEntry.RecordId, genPlace);
        }

        foreach (GenealogyManifestEntry manifestEntry in manifest.Persons.OrderBy(entry => entry.RecordId, StringComparer.Ordinal))
        {
            GenealogyEntityRecord personRecord = personStore.Load<GenealogyEntityRecord>(manifestEntry.RecordId);
            IGenPerson genPerson = factory.CreatePerson(personRecord.UId, personRecord.ID, personRecord.LastChange);
            genealogy.Entitys.Add(genPerson);
            SetGenealogyOwner(genPerson, genealogy);
            dictEntities.Add(manifestEntry.RecordId, genPerson);
            dictEntityRecords.Add(manifestEntry.RecordId, personRecord);
        }

        foreach (GenealogyManifestEntry manifestEntry in manifest.Families.OrderBy(entry => entry.RecordId, StringComparer.Ordinal))
        {
            GenealogyEntityRecord familyRecord = CryptoUtilities.ReadJson<GenealogyEntityRecord>(GetShardedCategoryFilePath(sGenealogyRootDirectory, "families", manifestEntry.RecordId, ".family.json"));
            IGenFamily genFamily = factory.CreateFamily(familyRecord.UId, familyRecord.ID, familyRecord.LastChange);
            genealogy.Entitys.Add(genFamily);
            SetGenealogyOwner(genFamily, genealogy);
            dictEntities.Add(manifestEntry.RecordId, genFamily);
            dictEntityRecords.Add(manifestEntry.RecordId, familyRecord);
        }

        foreach (GenealogyManifestEntry manifestEntry in manifest.Places.OrderBy(entry => entry.RecordId, StringComparer.Ordinal))
        {
            GenealogyPlaceRecord placeRecord = CryptoUtilities.ReadJson<GenealogyPlaceRecord>(GetShardedCategoryFilePath(sGenealogyRootDirectory, "places", manifestEntry.RecordId, ".place.json"));
            if (!string.IsNullOrWhiteSpace(placeRecord.ParentRecordId))
            {
                dictPlaces[manifestEntry.RecordId].Parent = ResolvePlace(dictPlaces, placeRecord.ParentRecordId);
            }
        }

        foreach (KeyValuePair<string, GenealogyEntityRecord> kvp in dictEntityRecords.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            IGenEntity genEntity = ResolveEntity(dictEntities, kvp.Key);
            HydrateEntity(factory, genEntity, kvp.Value, dictEntities, dictPlaces);
        }

        return genealogy;
    }

    /// <inheritdoc />
    public bool Exists(string sGenealogyId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sGenealogyId);
        return File.Exists(GetManifestFilePath(sGenealogyId));
    }

    /// <inheritdoc />
    public void Delete(string sGenealogyId, DeleteMode eMode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sGenealogyId);

        string sGenealogyRootDirectory = GetGenealogyRootDirectory(sGenealogyId);
        string sManifestFilePath = GetManifestFilePath(sGenealogyId);
        if (!Directory.Exists(sGenealogyRootDirectory))
        {
            return;
        }

        if (eMode == DeleteMode.SoftDelete)
        {
            Directory.Delete(sGenealogyRootDirectory, recursive: true);
            return;
        }

        if (eMode != DeleteMode.SecureDelete)
        {
            throw new ArgumentOutOfRangeException(nameof(eMode), eMode, "Unsupported delete mode.");
        }

        if (File.Exists(sManifestFilePath))
        {
            GenealogyManifestRecord manifest = CryptoUtilities.ReadJson<GenealogyManifestRecord>(sManifestFilePath);
            EnsureManifestVersion(manifest);
            PersonSecureStore personStore = CreatePersonStore(sGenealogyRootDirectory);
            foreach (GenealogyManifestEntry manifestEntry in manifest.Persons)
            {
                personStore.Delete(manifestEntry.RecordId, DeleteMode.SecureDelete);
            }
        }

        DeleteDirectoryIfExists(Path.Combine(sGenealogyRootDirectory, "families"));
        DeleteDirectoryIfExists(Path.Combine(sGenealogyRootDirectory, "places"));
        DeleteDirectoryIfExists(Path.Combine(sGenealogyRootDirectory, _options.SecureDataDirectoryName, _options.MasterKeyDirectoryName));
        DeleteDirectoryIfExists(Path.Combine(sGenealogyRootDirectory, _options.SecureDataDirectoryName, _options.KeyDirectoryName));
        if (File.Exists(sManifestFilePath))
        {
            File.Delete(sManifestFilePath);
        }
    }

    private static void EnsureSupportedCollections(IGenealogy genealogy)
    {
        if (genealogy.Sources.Count > 0 || genealogy.Repositories.Count > 0 || genealogy.Medias.Count > 0 || genealogy.Transactions.Count > 0)
        {
            throw new NotSupportedException("The current GenSecure genealogy store version supports entities and places only.");
        }
    }

    private static void EnsureNoUnsupportedEntities(IList<IGenEntity> lstEntities, int iSupportedCount)
    {
        if (lstEntities.Count != iSupportedCount)
        {
            throw new NotSupportedException("The current GenSecure genealogy store version supports person and family entities only.");
        }
    }

    private static void SetGenealogyOwner<T>(T value, IGenealogy genealogy)
    {
        if (value is IHasOwner<IGenealogy> hasOwner)
        {
            hasOwner.SetOwner(genealogy);
        }
    }

    private static GenealogyEntityRecord CreateEntityRecord(
        IGenEntity genEntity,
        string sRecordId,
        IReadOnlyDictionary<IGenEntity, string> dictEntityRecordIds,
        IReadOnlyDictionary<IGenPlace, string> dictPlaceRecordIds,
        string sEntityKind)
    {
        return new GenealogyEntityRecord
        {
            RecordId = sRecordId,
            EntityKind = sEntityKind,
            UId = genEntity.UId,
            ID = genEntity.ID,
            LastChange = genEntity.LastChange,
            Facts = genEntity.Facts
                .Where(genFact => genFact is not null)
                .Select((genFact, iIndex) => CreateFactRecord(genFact!, iIndex, dictEntityRecordIds, dictPlaceRecordIds))
                .OrderBy(record => record.RecordId, StringComparer.Ordinal)
                .ToList(),
            Connections = genEntity.Connects
                .Where(genConnection => genConnection is not null && genConnection.Entity is not null)
                .Select(genConnection => CreateConnectionRecord(genConnection!, dictEntityRecordIds))
                .OrderBy(record => record.TargetRecordId, StringComparer.Ordinal)
                .ThenBy(record => record.ConnectionType)
                .ToList(),
        };
    }

    private static GenealogyFactRecord CreateFactRecord(
        IGenFact genFact,
        int iIndex,
        IReadOnlyDictionary<IGenEntity, string> dictEntityRecordIds,
        IReadOnlyDictionary<IGenPlace, string> dictPlaceRecordIds)
    {
        string sRecordId = genFact.UId != Guid.Empty
            ? $"fact-{genFact.UId:N}"
            : $"fact-{iIndex:D6}-{genFact.eFactType}";

        return new GenealogyFactRecord
        {
            RecordId = sRecordId,
            UId = genFact.UId,
            ID = genFact.ID,
            LastChange = genFact.LastChange,
            FactType = genFact.eFactType,
            Data = genFact.Data,
            Date = CreateDateRecord(genFact.Date),
            PlaceRecordId = genFact.Place is not null ? ResolvePlaceRecordId(dictPlaceRecordIds, genFact.Place) : null,
            Connections = genFact.Entities
                .Where(genConnection => genConnection is not null && genConnection.Entity is not null)
                .Select(genConnection => CreateConnectionRecord(genConnection!, dictEntityRecordIds))
                .OrderBy(record => record.TargetRecordId, StringComparer.Ordinal)
                .ThenBy(record => record.ConnectionType)
                .ToList(),
        };
    }

    private static GenealogyDateRecord? CreateDateRecord(IGenDate? genDate)
    {
        if (genDate is null)
        {
            return null;
        }

        return new GenealogyDateRecord
        {
            UId = genDate.UId,
            ID = genDate.ID,
            LastChange = genDate.LastChange,
            DateModifier = genDate.eDateModifier,
            DateType1 = genDate.eDateType1,
            Date1 = genDate.Date1,
            DateType2 = genDate.eDateType2,
            Date2 = genDate.Date2,
            DateText = genDate.DateText,
        };
    }

    private static GenealogyConnectionRecord CreateConnectionRecord(IGenConnects genConnection, IReadOnlyDictionary<IGenEntity, string> dictEntityRecordIds)
    {
        if (genConnection.Entity is null)
        {
            throw new InvalidOperationException("Connections without a target entity cannot be persisted.");
        }

        return new GenealogyConnectionRecord
        {
            UId = genConnection.UId,
            TargetRecordId = ResolveEntityRecordId(dictEntityRecordIds, genConnection.Entity),
            ConnectionType = genConnection.eGenConnectionType,
        };
    }

    private static GenealogyPlaceRecord CreatePlaceRecord(IGenPlace genPlace, string sRecordId, IReadOnlyDictionary<IGenPlace, string> dictPlaceRecordIds)
    {
        return new GenealogyPlaceRecord
        {
            RecordId = sRecordId,
            UId = genPlace.UId,
            ID = genPlace.ID,
            LastChange = genPlace.LastChange,
            Name = genPlace.Name,
            Type = genPlace.Type,
            GovId = genPlace.GOV_ID,
            Latitude = genPlace.Latitude,
            Longitude = genPlace.Longitude,
            Notes = genPlace.Notes,
            ParentRecordId = genPlace.Parent is not null ? ResolvePlaceRecordId(dictPlaceRecordIds, genPlace.Parent) : null,
        };
    }

    private static Dictionary<IGenEntity, string> CreateEntityRecordIdMap(IEnumerable<IGenPerson> lstPersons, IEnumerable<IGenFamily> lstFamilies)
    {
        Dictionary<IGenEntity, string> dictResult = new();
        HashSet<string> setUsed = new(StringComparer.Ordinal);
        int iPersonIndex = 0;
        foreach (IGenPerson genPerson in lstPersons)
        {
            string sBaseRecordId = BuildEntityRecordId("person", iPersonIndex++, genPerson.UId, genPerson.IndRefID);
            dictResult.Add(genPerson, EnsureUniqueRecordId(sBaseRecordId, setUsed));
        }

        int iFamilyIndex = 0;
        foreach (IGenFamily genFamily in lstFamilies)
        {
            string sBaseRecordId = BuildEntityRecordId("family", iFamilyIndex++, genFamily.UId, genFamily.FamilyRefID);
            dictResult.Add(genFamily, EnsureUniqueRecordId(sBaseRecordId, setUsed));
        }

        return dictResult;
    }

    private static Dictionary<IGenPlace, string> CreatePlaceRecordIdMap(IEnumerable<IGenPlace> lstPlaces)
    {
        Dictionary<IGenPlace, string> dictResult = new();
        HashSet<string> setUsed = new(StringComparer.Ordinal);
        int iIndex = 0;
        foreach (IGenPlace genPlace in lstPlaces)
        {
            string? sSeed = !string.IsNullOrWhiteSpace(genPlace.Name) ? genPlace.Name : genPlace.GOV_ID;
            string sBaseRecordId = BuildEntityRecordId("place", iIndex++, genPlace.UId, sSeed);
            dictResult.Add(genPlace, EnsureUniqueRecordId(sBaseRecordId, setUsed));
        }

        return dictResult;
    }

    private static string BuildEntityRecordId(string sPrefix, int iIndex, Guid gUid, string? sSeed)
    {
        if (!string.IsNullOrWhiteSpace(sSeed))
        {
            return $"{sPrefix}-{NormalizeToken(sSeed)}";
        }

        if (gUid != Guid.Empty)
        {
            return $"{sPrefix}-{gUid:N}";
        }

        return $"{sPrefix}-{iIndex:D6}";
    }

    private static string EnsureUniqueRecordId(string sBaseRecordId, ISet<string> setUsed)
    {
        string sRecordId = sBaseRecordId;
        int iSuffix = 1;
        while (!setUsed.Add(sRecordId))
        {
            sRecordId = $"{sBaseRecordId}-{iSuffix:D2}";
            iSuffix++;
        }

        return sRecordId;
    }

    private static string NormalizeToken(string sValue)
    {
        List<char> lstChars = new(sValue.Length);
        bool xLastWasDash = false;
        foreach (char ch in sValue.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                lstChars.Add(ch);
                xLastWasDash = false;
                continue;
            }

            if (!xLastWasDash)
            {
                lstChars.Add('-');
                xLastWasDash = true;
            }
        }

        string sResult = new string(lstChars.ToArray()).Trim('-');
        return string.IsNullOrWhiteSpace(sResult) ? "item" : sResult;
    }

    private static string GetEntityIdentitySeed(IGenEntity genEntity)
    {
        return genEntity switch
        {
            IGenPerson genPerson => BuildEntityRecordId("person", 0, genPerson.UId, genPerson.IndRefID),
            IGenFamily genFamily => BuildEntityRecordId("family", 0, genFamily.UId, genFamily.FamilyRefID),
            _ => BuildEntityRecordId("entity", 0, genEntity.UId, null),
        };
    }

    private static string GetPlaceIdentitySeed(IGenPlace genPlace)
        => BuildEntityRecordId("place", 0, genPlace.UId, !string.IsNullOrWhiteSpace(genPlace.Name) ? genPlace.Name : genPlace.GOV_ID);

    private static StoreMode GetDefaultStoreMode(IGenPerson genPerson)
        => genPerson.Facts.Any(genFact => genFact?.eFactType is EFactType.Death or EFactType.Burial)
            ? StoreMode.Plaintext
            : StoreMode.Encrypted;

    private static string ResolveEntityRecordId(IReadOnlyDictionary<IGenEntity, string> dictEntityRecordIds, IGenEntity genEntity)
    {
        if (dictEntityRecordIds.TryGetValue(genEntity, out string? sRecordId))
        {
            return sRecordId;
        }

        throw new InvalidOperationException($"The entity '{genEntity.UId}' is not part of the persisted genealogy graph.");
    }

    private static string ResolvePlaceRecordId(IReadOnlyDictionary<IGenPlace, string> dictPlaceRecordIds, IGenPlace genPlace)
    {
        if (dictPlaceRecordIds.TryGetValue(genPlace, out string? sRecordId))
        {
            return sRecordId;
        }

        throw new InvalidOperationException($"The place '{genPlace.UId}' is not part of the persisted genealogy graph.");
    }

    private static IGenEntity ResolveEntity(IReadOnlyDictionary<string, IGenEntity> dictEntities, string sRecordId)
    {
        if (dictEntities.TryGetValue(sRecordId, out IGenEntity? genEntity))
        {
            return genEntity;
        }

        throw new InvalidDataException($"The entity record '{sRecordId}' could not be resolved.");
    }

    private static IGenPlace ResolvePlace(IReadOnlyDictionary<string, IGenPlace> dictPlaces, string sRecordId)
    {
        if (dictPlaces.TryGetValue(sRecordId, out IGenPlace? genPlace))
        {
            return genPlace;
        }

        throw new InvalidDataException($"The place record '{sRecordId}' could not be resolved.");
    }

    private static void HydrateEntity(
        IGenealogyModelFactory factory,
        IGenEntity genEntity,
        GenealogyEntityRecord entityRecord,
        IReadOnlyDictionary<string, IGenEntity> dictEntities,
        IReadOnlyDictionary<string, IGenPlace> dictPlaces)
    {
        foreach (GenealogyFactRecord factRecord in entityRecord.Facts.OrderBy(record => record.RecordId, StringComparer.Ordinal))
        {
            IGenFact genFact = factory.CreateFact(genEntity, factRecord.UId, factRecord.ID, factRecord.LastChange, factRecord.FactType);
            if (genFact is IHasOwner<IGenEntity> hasOwner)
            {
                hasOwner.SetOwner(genEntity);
            }

            genFact.Data = factRecord.Data;
            if (factRecord.Date is not null)
            {
                IGenDate genDate = factory.CreateDate(factRecord.Date.UId, factRecord.Date.ID, factRecord.Date.LastChange);
                genDate.eDateModifier = factRecord.Date.DateModifier;
                genDate.eDateType1 = factRecord.Date.DateType1;
                genDate.Date1 = factRecord.Date.Date1;
                genDate.eDateType2 = factRecord.Date.DateType2;
                genDate.Date2 = factRecord.Date.Date2;
                genDate.DateText = factRecord.Date.DateText;
                genFact.Date = genDate;
            }

            if (!string.IsNullOrWhiteSpace(factRecord.PlaceRecordId))
            {
                genFact.Place = ResolvePlace(dictPlaces, factRecord.PlaceRecordId);
            }

            foreach (GenealogyConnectionRecord connectionRecord in factRecord.Connections.OrderBy(record => record.TargetRecordId, StringComparer.Ordinal).ThenBy(record => record.ConnectionType))
            {
                genFact.Entities.Add(factory.CreateConnection(ResolveEntity(dictEntities, connectionRecord.TargetRecordId), connectionRecord.UId, connectionRecord.ConnectionType));
            }

            genEntity.Facts.Add(genFact);
        }

        foreach (GenealogyConnectionRecord connectionRecord in entityRecord.Connections.OrderBy(record => record.TargetRecordId, StringComparer.Ordinal).ThenBy(record => record.ConnectionType))
        {
            genEntity.Connects.Add(factory.CreateConnection(ResolveEntity(dictEntities, connectionRecord.TargetRecordId), connectionRecord.UId, connectionRecord.ConnectionType));
        }
    }

    private void CleanupObsoleteRecords(
        string sGenealogyRootDirectory,
        GenealogyManifestRecord? previousManifest,
        GenealogyManifestRecord currentManifest,
        PersonSecureStore personStore)
    {
        if (previousManifest is null)
        {
            return;
        }

        HashSet<string> setCurrentPersons = currentManifest.Persons.Select(entry => entry.RecordId).ToHashSet(StringComparer.Ordinal);
        foreach (GenealogyManifestEntry manifestEntry in previousManifest.Persons)
        {
            if (!setCurrentPersons.Contains(manifestEntry.RecordId))
            {
                personStore.Delete(manifestEntry.RecordId, DeleteMode.SoftDelete);
            }
        }

        CleanupPlainRecords(previousManifest.Families, currentManifest.Families, sGenealogyRootDirectory, "families", ".family.json");
        CleanupPlainRecords(previousManifest.Places, currentManifest.Places, sGenealogyRootDirectory, "places", ".place.json");
    }

    private static void CleanupPlainRecords(
        IEnumerable<GenealogyManifestEntry> previousEntries,
        IEnumerable<GenealogyManifestEntry> currentEntries,
        string sGenealogyRootDirectory,
        string sCategory,
        string sExtension)
    {
        HashSet<string> setCurrent = currentEntries.Select(entry => entry.RecordId).ToHashSet(StringComparer.Ordinal);
        foreach (GenealogyManifestEntry manifestEntry in previousEntries)
        {
            if (setCurrent.Contains(manifestEntry.RecordId))
            {
                continue;
            }

            string sFilePath = GetShardedCategoryFilePath(sGenealogyRootDirectory, sCategory, manifestEntry.RecordId, sExtension);
            if (File.Exists(sFilePath))
            {
                File.Delete(sFilePath);
            }
        }
    }

    private PersonSecureStore CreatePersonStore(string sGenealogyRootDirectory)
    {
        return new PersonSecureStore(_masterKeyBackupService, new GenSecureStoreOptions
        {
            RootDirectory = Path.Combine(sGenealogyRootDirectory, _options.SecureDataDirectoryName),
            DataDirectoryName = _options.DataDirectoryName,
            KeyDirectoryName = _options.KeyDirectoryName,
            MasterKeyDirectoryName = _options.MasterKeyDirectoryName,
            LocalMasterKeyFileName = _options.LocalMasterKeyFileName,
            RecoveryKeyFileName = _options.RecoveryKeyFileName,
            GenealogyDirectoryName = _options.GenealogyDirectoryName,
            SecureDataDirectoryName = _options.SecureDataDirectoryName,
        });
    }

    private string GetGenealogyRootDirectory(string sGenealogyId)
    {
        string sFileId = CryptoUtilities.ToDeterministicFileId(sGenealogyId);
        return Path.Combine(_options.GenealogyDirectoryPath, sFileId);
    }

    private string GetManifestFilePath(string sGenealogyId)
        => Path.Combine(GetGenealogyRootDirectory(sGenealogyId), ManifestFileName);

    private static string GetShardedCategoryFilePath(string sRootDirectory, string sCategory, string sRecordId, string sExtension)
        => CryptoUtilities.GetShardedFilePath(Path.Combine(sRootDirectory, sCategory), sRecordId, sExtension);

    private static void DeleteDirectoryIfExists(string sDirectoryPath)
    {
        if (Directory.Exists(sDirectoryPath))
        {
            Directory.Delete(sDirectoryPath, recursive: true);
        }
    }

    private static void EnsureManifestVersion(GenealogyManifestRecord manifest)
    {
        if (!string.Equals(manifest.Version, CurrentVersion, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"The genealogy manifest version '{manifest.Version}' is not supported.");
        }
    }
}
