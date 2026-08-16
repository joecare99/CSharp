using System;
using System.IO;
using Config.Service;

namespace RnzTrauer.Core;

/// <summary>
/// Configuration section for announcement core functionality (import/export/media). Stored in JSON under a stable key.
/// </summary>
public sealed class AnnouncementsConfig
{
    /// <summary>Default import directory for HTML obituary sources.</summary>
    public string? ImportDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "RnzTrauer_Imports");

    /// <summary>Output directory for exported notices.</summary>
    public string? ExportDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "RnzTrauer_Exports");

    /// <summary>Default PDF template path for exports.</summary>
    public string? PdfTemplatePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "RnzTrauer_Exports", "templates", "default_template.pdf");

    /// <summary>Maximum notice age in days before auto-archive (0 = never).</summary>
    public int MaxNoticeAge { get; set; } = 90;

    /// <summary>Whether to enable automatic export scheduling.</summary>
    public bool EnableAutoExport { get; set; } = false;

    /// <summary>Cron expression for auto-export (if enabled).</summary>
    public string? ExportCronExpression { get; set; } = "0 8 * * MON-FRI"; // Daily 8:00 AM on weekdays
}
