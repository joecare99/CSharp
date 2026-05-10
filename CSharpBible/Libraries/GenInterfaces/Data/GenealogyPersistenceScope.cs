namespace GenInterfaces.Data;

/// <summary>
/// Defines the intended persistence scope for genealogy load and save operations.
/// </summary>
public enum GenealogyPersistenceScope
{
    /// <summary>
    /// Lets the persistence implementation decide the most suitable scope.
    /// </summary>
    Auto,

    /// <summary>
    /// Requests loading or persisting the directly addressed entity only when possible.
    /// </summary>
    Entity,

    /// <summary>
    /// Requests loading or persisting the addressed entity together with related data when needed.
    /// </summary>
    RelatedEntities,

    /// <summary>
    /// Requests loading or persisting the complete genealogy aggregate.
    /// </summary>
    FullGenealogy,
}
