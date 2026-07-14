using BaseLib.Helper;
using GenFreeWin.Models;
using GenFree.Data;
using System;
using System.Collections.Generic;

namespace GenFreeWin.Data;

/// <summary>
/// Defines data access operations required by the picture management workflow.
/// </summary>
public interface IBilderRepository
{
    /// <summary>
    /// Loads all picture links for the given target marker and target number.
    /// </summary>
    /// <param name="marker">The target marker identifying the record type.</param>
    /// <param name="linkedNumber">The related person, family, or source number.</param>
    /// <returns>A list of persisted picture links.</returns>
    IReadOnlyList<BilderListItem> LoadPictures(string marker, int linkedNumber);

    /// <summary>
    /// Loads the persisted details of a picture link by record number.
    /// </summary>
    /// <param name="recordNumber">The technical picture record number.</param>
    /// <returns>The picture details.</returns>
    BilderDetails LoadPictureDetails(int recordNumber);

    /// <summary>
    /// Creates or updates a picture link.
    /// </summary>
    /// <param name="request">The picture data to persist.</param>
    /// <returns>The technical record number of the persisted picture.</returns>
    int SavePicture(BilderSaveRequest request);

    /// <summary>
    /// Deletes a persisted picture link.
    /// </summary>
    /// <param name="recordNumber">The technical picture record number.</param>
    void DeletePicture(int recordNumber);
}

/// <summary>
/// Provides table-based access to persisted picture links.
/// </summary>
public sealed class BilderRepository : IBilderRepository
{
    /// <inheritdoc />
    public IReadOnlyList<BilderListItem> LoadPictures(string marker, int linkedNumber)
    {
        var result = new List<BilderListItem>();
        DataModul.DB_PictureTable.Index = "Perkenn  ";
        DataModul.DB_PictureTable.Seek("=", marker, linkedNumber);

        while (!DataModul.DB_PictureTable.EOF
            && !DataModul.DB_PictureTable.NoMatch
            && DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt() == linkedNumber
            && string.Equals(DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString(), marker, StringComparison.Ordinal))
        {
            result.Add(CreateListItem());
            DataModul.DB_PictureTable.MoveNext();
        }

        return result;
    }

    /// <inheritdoc />
    public BilderDetails LoadPictureDetails(int recordNumber)
    {
        DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
        DataModul.DB_PictureTable.Seek("=", recordNumber);
        return CreateDetails();
    }

    /// <inheritdoc />
    public int SavePicture(BilderSaveRequest request)
    {
        if (request.RecordNumber <= 0)
        {
            DataModul.DB_BildTab = DataModul.MandDB.OpenRecordset("select * from Bilder order By Bilder.Lfnr");
            int nextRecordNumber = 1;
            if (DataModul.DB_BildTab.RecordCount > 0)
            {
                DataModul.DB_BildTab.MoveLast();
                nextRecordNumber = DataModul.DB_BildTab.Fields["LfNr"].AsInt() + 1;
            }

            DataModul.DB_BildTab.AddNew();
            DataModul.DB_BildTab.Fields["LfNr"].Value = nextRecordNumber;
            ApplyFields(request);
            DataModul.DB_BildTab.Update();
            return nextRecordNumber;
        }

        DataModul.DB_BildTab = DataModul.MandDB.OpenRecordset(dbTables.Bilder, RecordsetTypeEnum.dbOpenTable);
        DataModul.DB_BildTab.FindFirst("Bilder.LfNr = " + request.RecordNumber.AsString());
        DataModul.DB_BildTab.Edit();
        ApplyFields(request);
        DataModul.DB_BildTab.Update();
        return request.RecordNumber;
    }

    /// <inheritdoc />
    public void DeletePicture(int recordNumber)
    {
        DataModul.DB_PictureTable.Index = nameof(PictureIndex.Nr);
        DataModul.DB_PictureTable.Seek("=", recordNumber);
        if (!DataModul.DB_PictureTable.NoMatch)
        {
            DataModul.DB_PictureTable.Delete();
        }
    }

    private static BilderListItem CreateListItem() => new(
        DataModul.DB_PictureTable.Fields[PictureFields.LfNr].AsInt(),
        DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString());

    private static BilderDetails CreateDetails() => new(
        DataModul.DB_PictureTable.Fields[PictureFields.LfNr].AsInt(),
        DataModul.DB_PictureTable.Fields[PictureFields.ZuNr].AsInt(),
        DataModul.DB_PictureTable.Fields[PictureFields.Kennz].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Beschreibung].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Bem].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Pfad].AsString(),
        DataModul.DB_PictureTable.Fields[PictureFields.Datei].AsString());

    private static void ApplyFields(BilderSaveRequest request)
    {
        string remark = string.IsNullOrWhiteSpace(request.Remark) ? " " : request.Remark;
        DataModul.DB_BildTab.Fields["ZuNr"].Value = request.LinkedNumber;
        DataModul.DB_BildTab.Fields["Kennz"].Value = request.Marker;
        DataModul.DB_BildTab.Fields["Beschreibung"].Value = request.Description;
        DataModul.DB_BildTab.Fields["Bem"].Value = remark;
        DataModul.DB_BildTab.Fields["Pfad"].Value = request.StoredPath;
        DataModul.DB_BildTab.Fields["Datei"].Value = request.FileName;
    }
}
