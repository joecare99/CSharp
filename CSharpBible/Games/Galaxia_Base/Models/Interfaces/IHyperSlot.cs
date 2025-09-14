using System.Collections.Generic;

namespace Galaxia.Models.Interfaces
{
    public interface IHyperSlot: IFleetContainer
    {
        /// <summary>
        /// Gets the starting sector for the hyper slot.
        /// </summary>
        ISector? StartSector { get; }
        /// <summary>
        /// Gets the jump energy available at the hyper slot.
        /// </summary>
        float JumpEnergy { get; }

        /// <summary>
        /// Gets the reachable sectors.
        /// </summary>
        /// <returns>IEnumerable&lt;ISector&gt;.</returns>
        IEnumerable<ISector> GetReachableSectors();
    }
}