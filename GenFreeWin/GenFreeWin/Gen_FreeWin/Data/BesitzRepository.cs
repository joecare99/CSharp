using BaseLib.Helper;
using GenFreeWin.Models;
using GenFree.Data;
using System;
using System.Collections.Generic;

namespace GenFreeWin.Data;

/// <summary>
/// Defines data access operations required by the ownership selection workflow.
/// </summary>
public interface IBesitzRepository
{
    /// <summary>
    /// Determines whether at least one land-register file exists.
    /// </summary>
    /// <returns><see langword="true" /> when file records exist; otherwise <see langword="false" />.</returns>
    bool HasAkteRecords();

    /// <summary>
    /// Loads all selectable file entries.
    /// </summary>
    /// <returns>A list of file list items ordered by the underlying table index.</returns>
    IReadOnlyList<BesitzAkteListItem> LoadAkteList();

    /// <summary>
    /// Loads all ownership history entries for a given file number.
    /// </summary>
    /// <param name="akte">The file number used by the history table.</param>
    /// <returns>A list of matching ownership history entries.</returns>
    IReadOnlyList<BesitzEntryListItem> LoadEntriesForAkte(string akte);

    /// <summary>
    /// Loads the detailed data of a file by its record number.
    /// </summary>
    /// <param name="recordNumber">The primary record number of the file table.</param>
    /// <returns>The detailed file data.</returns>
    BesitzAkteDetails LoadAkteDetailsByRecordNumber(int recordNumber);

    /// <summary>
    /// Loads the detailed data of a file by its business file number.
    /// </summary>
    /// <param name="akte">The business file number.</param>
    /// <returns>The detailed file data.</returns>
    BesitzAkteDetails LoadAkteDetailsByAkte(string akte);

    /// <summary>
    /// Loads the detailed data of an ownership history entry.
    /// </summary>
    /// <param name="recordNumber">The primary record number of the ownership history table.</param>
    /// <returns>The detailed ownership history entry.</returns>
    BesitzEntryDetails LoadEntryDetails(int recordNumber);

    /// <summary>
    /// Creates a property link between a person and an ownership history entry.
    /// </summary>
    /// <param name="entryRecordNumber">The selected ownership history record number.</param>
    /// <param name="akte">The associated file number.</param>
    /// <param name="personNumber">The selected person number.</param>
    void AddPropertyLink(int entryRecordNumber, string akte, int personNumber);

    /// <summary>
    /// Removes an existing property link between a person and an ownership history entry.
    /// </summary>
    /// <param name="entryRecordNumber">The selected ownership history record number.</param>
    /// <param name="akte">The associated file number.</param>
    /// <param name="personNumber">The selected person number.</param>
    void RemovePropertyLink(int entryRecordNumber, string akte, int personNumber);
}

/// <summary>
/// Provides table-based data access for the ownership selection workflow.
/// </summary>
public sealed class BesitzRepository : IBesitzRepository
{
    /// <inheritdoc />
    public bool HasAkteRecords() => DataModul.DB_HGATable.RecordCount > 0;

    /// <inheritdoc />
    public IReadOnlyList<BesitzAkteListItem> LoadAkteList()
    {
        var result = new List<BesitzAkteListItem>();
        DataModul.DB_HGATable.Index = "Akte";
        DataModul.DB_HGATable.MoveFirst();

        while (!DataModul.DB_HGATable.EOF)
        {
            result.Add(new BesitzAkteListItem(
                DataModul.DB_HGATable.Fields[HGAFields.Nr].AsInt(),
                DataModul.DB_HGATable.Fields[HGAFields.Akte].AsString(),
                DataModul.DB_HGATable.Fields[HGAFields.Kirchspiel].AsString()));
            DataModul.DB_HGATable.MoveNext();
        }

        return result;
    }

    /// <inheritdoc />
    public IReadOnlyList<BesitzEntryListItem> LoadEntriesForAkte(string akte)
    {
        var result = new List<BesitzEntryListItem>();
        DataModul.DB_GbeTable.Index = "AkteJa";
        DataModul.DB_GbeTable.Seek(">=", akte, "0");

        while (!DataModul.DB_GbeTable.EOF
            && !DataModul.DB_GbeTable.NoMatch
            && string.Equals(DataModul.DB_GbeTable.Fields[GBEFields.Akte].AsString(), akte, StringComparison.Ordinal))
        {
            result.Add(new BesitzEntryListItem(
                DataModul.DB_GbeTable.Fields[GBEFields.Nr].AsInt(),
                DataModul.DB_GbeTable.Fields[GBEFields.Jahr].AsString(),
                DataModul.DB_GbeTable.Fields[GBEFields.Name].AsString()));
            DataModul.DB_GbeTable.MoveNext();
        }

        return result;
    }

    /// <inheritdoc />
    public BesitzAkteDetails LoadAkteDetailsByRecordNumber(int recordNumber)
    {
        DataModul.DB_HGATable.Index = "Nr";
        DataModul.DB_HGATable.Seek("=", recordNumber);
        return CreateAkteDetails();
    }

    /// <inheritdoc />
    public BesitzAkteDetails LoadAkteDetailsByAkte(string akte)
    {
        DataModul.DB_HGATable.Index = "Akte";
        DataModul.DB_HGATable.Seek("=", akte.AsInt());
        return CreateAkteDetails();
    }

    /// <inheritdoc />
    public BesitzEntryDetails LoadEntryDetails(int recordNumber)
    {
        DataModul.DB_GbeTable.Index = "Nr";
        DataModul.DB_GbeTable.Seek("=", recordNumber);
        return new BesitzEntryDetails(
            DataModul.DB_GbeTable.Fields[GBEFields.Nr].AsInt(),
            DataModul.DB_GbeTable.Fields[GBEFields.Akte].AsString(),
            DataModul.DB_GbeTable.Fields[GBEFields.Jahr].AsString(),
            DataModul.DB_GbeTable.Fields[GBEFields.Erb].AsString(),
            DataModul.DB_GbeTable.Fields[GBEFields.Abg].AsString(),
            DataModul.DB_GbeTable.Fields[GBEFields.Name].AsString(),
            DataModul.DB_GbeTable.Fields[GBEFields.Geb].AsString());
    }

    /// <inheritdoc />
    public void AddPropertyLink(int entryRecordNumber, string akte, int personNumber)
    {
        DataModul.DB_PropertyTable.AddNew();
        DataModul.DB_PropertyTable.Fields[PropertyFields.Nr].Value = entryRecordNumber;
        DataModul.DB_PropertyTable.Fields[PropertyFields.Akte].Value = akte;
        DataModul.DB_PropertyTable.Fields[PropertyFields.Pers].Value = personNumber;
        DataModul.DB_PropertyTable.Update();
    }

    /// <inheritdoc />
    public void RemovePropertyLink(int entryRecordNumber, string akte, int personNumber)
    {
        DataModul.DB_PropertyTable.Index = nameof(PropertyIndex.NuAkPer);
        DataModul.DB_PropertyTable.Seek("=", entryRecordNumber, akte, personNumber);
        if (!DataModul.DB_PropertyTable.NoMatch)
        {
            DataModul.DB_PropertyTable.Delete();
        }
    }

    private static BesitzAkteDetails CreateAkteDetails() => new(
        DataModul.DB_HGATable.Fields[HGAFields.Nr].AsInt(),
        DataModul.DB_HGATable.Fields[HGAFields.Akte].AsString(),
        DataModul.DB_HGATable.Fields[HGAFields.Kirchspiel].AsString(),
        DataModul.DB_HGATable.Fields[HGAFields.Beschr].AsString(),
        DataModul.DB_HGATable.Fields[HGAFields.Hof].AsString(),
        DataModul.DB_HGATable.Fields[HGAFields.Flur].AsString(),
        DataModul.DB_HGATable.Fields[HGAFields.Parzelle].AsString());
}
