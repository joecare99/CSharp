namespace Osb.Core.Media;

/// <summary>
/// A single picture record read from the picture table (<c>Bildtable</c>).
/// </summary>
public sealed class BildausEntry
{
    /// <summary>Gets the number of the entity (person or family) the picture belongs to; field <c>ZuNr</c>.</summary>
    public int OwnerNumber { get; set; }

    /// <summary>Gets the raw path of the picture; field <c>Pfad</c>. A leading '#' marks a path relative to the picture base directory.</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets the file name of the picture; field <c>Datei</c>.</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>Gets the optional description of the picture; field <c>Beschreibung</c>. <see langword="null"/> when the field is <see cref="System.DBNull"/>.</summary>
    public string? Description { get; set; }

    /// <summary>Gets the optional remark of the picture; field <c>Bem</c>. <see langword="null"/> when the field is <see cref="System.DBNull"/>.</summary>
    public string? Remark { get; set; }
}
