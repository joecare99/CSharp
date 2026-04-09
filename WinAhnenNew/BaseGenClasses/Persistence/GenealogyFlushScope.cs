namespace BaseGenClasses.Persistence;

/// <summary>
/// Defines the intended persistence scope for a genealogy flush request.
/// </summary>
public enum GenealogyFlushScope
{
    /// <summary>
    /// Lets the persistence provider decide the most suitable scope.
    /// </summary>
    Auto,

    /// <summary>
    /// Requests persisting the directly changed entity only when possible.
    /// </summary>
    Entity,

    /// <summary>
    /// Requests persisting the changed entity together with related data when needed.
    /// </summary>
    RelatedEntities,

    /// <summary>
    /// Requests persisting the complete genealogy aggregate.
    /// </summary>
    FullGenealogy
}
