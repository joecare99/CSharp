namespace AA28_DataGridExt.Model;

/// <summary>
/// Represents a department displayed in the sample data grid.
/// </summary>
public class Department
{
    /// <summary>
    /// Gets or sets the department identifier.
    /// </summary>
    public int Id { get; set; } = -1;

    /// <summary>
    /// Gets or sets the localized or display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the longer display description.
    /// </summary>
    public string? Description { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => Name ?? string.Empty;
}
