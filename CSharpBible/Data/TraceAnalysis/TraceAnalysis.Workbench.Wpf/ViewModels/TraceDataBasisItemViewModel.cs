using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Wpf.ViewModels;

/// <summary>
/// Represents one structural item of the loaded trace data basis.
/// </summary>
public sealed class TraceDataBasisItemViewModel
{
    public TraceDataBasisItemViewModel(TraceDataBasisItem item)
    {
        ColumnName = item.ColumnName;
        GroupName = item.GroupName ?? string.Empty;
        FormatText = item.FormatText ?? string.Empty;
        TypeName = item.TypeName;
    }

    /// <summary>
    /// Gets the column name.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the group name.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets the format text.
    /// </summary>
    public string FormatText { get; }

    /// <summary>
    /// Gets the type name.
    /// </summary>
    public string TypeName { get; }
}
