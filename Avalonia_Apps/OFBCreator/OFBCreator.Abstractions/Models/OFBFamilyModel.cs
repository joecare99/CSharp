using System;
using System.Collections.Generic;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Abstractions.Models;

/// <summary>
/// Domain model for OFB (Ortsfamilienbuch) family units.
/// Mirrors the exported data structure used throughout generation and export.
/// </summary>
public sealed class OFBFamilyModel
{
    /// <summary>
    /// Global sequential family number (4-5 digits with leading zeros).
    /// Format: "00001" through "99999".
    /// </summary>
    public string GlobalNumber { get; set; } = default!;

    /// <summary>
    /// Family name group identifier (surname of father or combined parent names).
    /// Used for grouping and sorting families by surname.
    /// </summary>
    public string FamilyName { get; set; } = default!;

    /// <summary>
    /// Husband/partner1 in the family unit.
    /// May be null if only partial data is available.
    /// </summary>
    public IGenPerson? Husband { get; set; }

    /// <summary>
    /// Wife/partner2 in the family unit.
    /// May be null if only partial data is available.
    /// </summary>
    public IGenPerson? Wife { get; set; }

    /// <summary>
    /// Children in this family unit.
    /// Set during population from source data via object initializer or factory method.
    /// </summary>
    public IReadOnlyList<IGenPerson> Children { get; set; } = new List<IGenPerson>().AsReadOnly();

    /// <summary>
    /// Marriage date (MARR event), directly sourced from IGenFamily.MarriageDate.
    /// Set during population from the source family unit.
    /// </summary>
    public IGenDate? MarriageDate { get; set; }

    /// <summary>
    /// Marriage place, used for place indexing. Sourced from IGenFamily.MarriagePlace.
    /// </summary>
    public IGenPlace? MarriagePlace { get; set; }

    /// <summary>
    /// Calculated family formation date following priority rules:
    /// 1) MarriageDate (MARR) → primary
    /// 2) FirstChildBirthDate → if no MARR
    /// 3) Wife.BirthDate - 5 years → estimated
    /// 4) Husband.BirthDate + 30 → fallback
    /// </summary>
    public DateTime? FormationDate { get; set; }

    /// <summary>
    /// Original source reference ID (GEDCOM family pointer).
    /// </summary>
    public string SourceRefId { get; set; } = default!;
}
