using System;
using System.Collections.Generic;

namespace OFBCreator.Projects.Models;

/// <summary>
/// Portable family-book project settings shared by all OFBCreator hosts.
/// File paths are relative to the project file when stored as relative paths.
/// </summary>
public sealed class OFBProject
{
    public const int CurrentSchemaVersion = 7;

    public const int DefaultDateEstimationCutoffYear = 900;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    public string Id { get; set; } = Guid.NewGuid().ToString("D");

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string EntryTemplate { get; set; } = "gc";

    public string ExportCulture { get; set; } = OFBExportCulture.Default;

    public int? DateEstimationCutoffYear { get; set; } = DefaultDateEstimationCutoffYear;

    public string? InputPath { get; set; }

    public string? OutputPath { get; set; }

    public string? DataSource { get; set; }

    public string? PlaceId { get; set; }

    public bool IncludeDescendants { get; set; }

    public string? Preface { get; set; }

    public string? Legend { get; set; }

    public OFBProjectLayout Layout { get; set; } = new();

    /// <summary>Ordered non-destructive filters and transformations applied only during export.</summary>
    public List<OFBExportRule> ExportRules { get; set; } = [];

    /// <summary>Evidence threshold and saved review choices for surname grouping.</summary>
    public OFBGroupingPolicy GroupingPolicy { get; set; } = new();

    /// <summary>Project-specific merge, separation, and manual-label decisions.</summary>
    public List<OFBGroupingDecision> GroupingDecisions { get; set; } = [];
}
