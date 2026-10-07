using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models;

/// <summary>Contains the detached genealogy view and diagnostics produced by export rules.</summary>
public sealed record OFBExportOverlayResult(
    IGenealogy Genealogy,
    IReadOnlyList<OFBExportRuleDiagnostic> Diagnostics,
    IReadOnlyList<OFBPersonPrivacyPreview> PersonPreviews);
