namespace Osb.Core.Media;

/// <summary>
/// Identifies the entity whose pictures are exported by the legacy <c>COND.Bildaus</c> routine.
/// Replaces the single-character legacy selector ("P" for person, "F" for family).
/// </summary>
public enum BildausKind
{
    /// <summary>Pictures attached to the currently edited person.</summary>
    Person,

    /// <summary>Pictures attached to the currently edited family.</summary>
    Family,
}
