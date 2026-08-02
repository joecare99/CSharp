namespace RnzTrauer.Core.Domain;

/// <summary>Qualification retained for a parsed date instead of silently treating it as exact.</summary>
public enum DateQualification
{
    Exact,
    Before,
    After,
    Calculated,
    Estimated,
}
