using System.Collections.Generic;

namespace Galaxia.Models.Interfaces;
/// <summary>
/// Interface IHyperspaceSys 
/// </summary>
/// <info>The Hyperspacesystem of a corporation</info>
public interface IHyperspaceSys
{
    /// <summary>
    /// Gets the corporation that owns this hyperspace system.
    /// </summary>
    ICorporation corporation { get; }
    /// <summary>
    /// Gets the space.
    /// </summary>
    /// <value>The space.</value>
    ISpace space { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is available for embarkation.
    /// </summary>
    /// <value><c>true</c> if this instance is available; otherwise, <c>false</c>.</value>
    bool IsAvailable { get; }
    /// <summary>
    /// Gets the hyper slots.
    /// </summary>
    /// <value>The hyper slots.</value>
    IReadOnlyList<IHyperSlot> HyperSlots { get; }
    bool IsAvailible { get; }

    /// <summary>
    /// Gets the combined reachable sectors.
    /// </summary>
    /// <returns>IEnumerable&lt;ISector&gt;.</returns>
    IEnumerable<ISector> CombReachableSectors();
    /// <summary>
    /// Embarks the specified fleet.
    /// </summary>
    /// <param name="fleet">The fleet to embark.</param>
    /// <returns><c>true</c> if the embarkation was successful; otherwise, <c>false</c>.</returns>
    bool Embark(IFleet fleet);
        
}