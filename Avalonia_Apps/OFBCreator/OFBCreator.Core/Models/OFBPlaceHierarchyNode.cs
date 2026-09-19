/// <summary>
/// Hierarchical place node for tree-structured place index.
/// </summary>
public sealed class OFBPlaceHierarchyNode
{
    /// <summary>
    /// Unique place identifier (e.g., GEDCOM REF_ID or generated ID).
    /// </summary>
    public string PlaceId { get; }

    /// <summary>
    /// Display name of the place.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Parent place identifier, if this is a sub-location (e.g., city within county).
    /// </summary>
    public string? ParentPlaceId { get; set; }

    /// <summary>
    /// Child places in the hierarchy.
    /// Populated when building the complete tree structure.
    /// </summary>
    public OFBPlaceHierarchyNode[] Children { get; set; } = [];

    public OFBPlaceHierarchyNode( string name, string placeId )
    {
        Name = name ?? throw new ArgumentNullException( nameof( name ) );
        PlaceId = placeId ?? throw new ArgumentNullException( nameof( placeId ) );
    }
}
