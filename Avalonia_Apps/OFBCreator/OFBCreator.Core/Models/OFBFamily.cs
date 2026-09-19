using System;
using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models;

/// <summary>
/// OFB (Ortsfamilienbuch) domain model representing a family unit with global numbering.
/// </summary>
public class OFBFamily
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
    /// Husband/party1 in the family unit.
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
    /// </summary>
    public IReadOnlyList<IGenPerson> Children { get; set; } = new List<IGenPerson>().AsReadOnly();

    /// <summary>
    /// Marriage date (MARR event), primary source for family formation date.
    /// Set during population from the source family unit.
    /// </summary>
    public IGenDate? MarriageDate { get; set; }

    /// <summary>
    /// Marriage place, used for place indexing.
    /// </summary>
    public IGenPlace? MarriagePlace { get; set; }

    /// <summary>
    /// Calculated family formation date according to priority rules:
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
