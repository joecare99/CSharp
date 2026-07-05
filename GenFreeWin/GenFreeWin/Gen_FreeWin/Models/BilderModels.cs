using System;

namespace Gen_FreeWin.Models;

/// <summary>
/// Represents a persisted picture entry shown in the picture selection list.
/// </summary>
public sealed class BilderListItem : IEquatable<BilderListItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BilderListItem"/> class.
    /// </summary>
    /// <param name="recordNumber">The technical picture record number.</param>
    /// <param name="description">The user-facing picture description.</param>
    /// <param name="storedPath">The persisted path value from the database.</param>
    /// <param name="fileName">The persisted file name.</param>
    public BilderListItem(int recordNumber, string description, string storedPath, string fileName)
    {
        RecordNumber = recordNumber;
        Description = description ?? string.Empty;
        StoredPath = storedPath ?? string.Empty;
        FileName = fileName ?? string.Empty;
    }

    /// <summary>
    /// Gets the technical picture record number.
    /// </summary>
    public int RecordNumber { get; }

    /// <summary>
    /// Gets the user-facing picture description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the stored path fragment as persisted in the picture table.
    /// </summary>
    public string StoredPath { get; }

    /// <summary>
    /// Gets the stored file name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the fixed-width display text used by the legacy picture list.
    /// </summary>
    public string DisplayText => string.Format("{0}{1}", (Description + "                                           ").Substring(0, 40), RecordNumber);

    /// <inheritdoc />
    public override string ToString() => DisplayText;

    /// <inheritdoc />
    public bool Equals(BilderListItem? other)
    {
        return other is not null
            && RecordNumber == other.RecordNumber
            && string.Equals(Description, other.Description, StringComparison.Ordinal)
            && string.Equals(StoredPath, other.StoredPath, StringComparison.Ordinal)
            && string.Equals(FileName, other.FileName, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as BilderListItem);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 23) + RecordNumber.GetHashCode();
            hash = (hash * 23) + Description.GetHashCode();
            hash = (hash * 23) + StoredPath.GetHashCode();
            hash = (hash * 23) + FileName.GetHashCode();
            return hash;
        }
    }
}

/// <summary>
/// Contains the editable data of a persisted picture entry.
/// </summary>
public sealed class BilderDetails
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BilderDetails"/> class.
    /// </summary>
    /// <param name="recordNumber">The technical picture record number.</param>
    /// <param name="linkedNumber">The related person, family, or source number.</param>
    /// <param name="marker">The record marker identifying the target domain.</param>
    /// <param name="description">The stored picture description.</param>
    /// <param name="remark">The stored picture remark.</param>
    /// <param name="storedPath">The persisted path fragment.</param>
    /// <param name="fileName">The persisted file name.</param>
    public BilderDetails(int recordNumber, int linkedNumber, string marker, string description, string remark, string storedPath, string fileName)
    {
        RecordNumber = recordNumber;
        LinkedNumber = linkedNumber;
        Marker = marker ?? string.Empty;
        Description = description ?? string.Empty;
        Remark = remark ?? string.Empty;
        StoredPath = storedPath ?? string.Empty;
        FileName = fileName ?? string.Empty;
    }

    /// <summary>
    /// Gets the technical picture record number.
    /// </summary>
    public int RecordNumber { get; }

    /// <summary>
    /// Gets the related person, family, or source number.
    /// </summary>
    public int LinkedNumber { get; }

    /// <summary>
    /// Gets the target marker identifying the record type.
    /// </summary>
    public string Marker { get; }

    /// <summary>
    /// Gets the stored picture description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the stored picture remark.
    /// </summary>
    public string Remark { get; }

    /// <summary>
    /// Gets the persisted path fragment.
    /// </summary>
    public string StoredPath { get; }

    /// <summary>
    /// Gets the stored file name.
    /// </summary>
    public string FileName { get; }
}

/// <summary>
/// Describes the data required to create or update a picture link.
/// </summary>
public sealed class BilderSaveRequest
{
    /// <summary>
    /// Gets or sets the technical picture record number.
    /// Use <c>0</c> to create a new entry.
    /// </summary>
    public int RecordNumber { get; set; }

    /// <summary>
    /// Gets or sets the related person, family, or source number.
    /// </summary>
    public int LinkedNumber { get; set; }

    /// <summary>
    /// Gets or sets the target marker identifying the record type.
    /// </summary>
    public string Marker { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the picture description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the picture remark.
    /// </summary>
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the persisted path fragment.
    /// </summary>
    public string StoredPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file name to store.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
