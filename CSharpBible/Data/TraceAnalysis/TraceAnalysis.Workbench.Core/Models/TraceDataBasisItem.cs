namespace TraceAnalysis.Workbench.Core.Models;

/// <summary>
/// Describes one derived structural item of a loaded trace data basis.
/// </summary>
public sealed class TraceDataBasisItem
{
    /// <summary>
    /// Initializes a new instance of <see cref="TraceDataBasisItem"/>.
    /// </summary>
    /// <param name="columnName">The field or column name.</param>
    /// <param name="groupName">The derived group name.</param>
    /// <param name="formatText">The format text.</param>
    /// <param name="typeName">The derived type name.</param>
    public TraceDataBasisItem(string columnName, string? groupName, string? formatText, string typeName)
    {
        ColumnName = columnName;
        GroupName = groupName;
        FormatText = formatText;
        TypeName = typeName;
    }

    /// <summary>
    /// Gets the field or column name.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the derived group name.
    /// </summary>
    public string? GroupName { get; }

    /// <summary>
    /// Gets the format text.
    /// </summary>
    public string? FormatText { get; }

    /// <summary>
    /// Gets the derived type name.
    /// </summary>
    public string TypeName { get; }
}
