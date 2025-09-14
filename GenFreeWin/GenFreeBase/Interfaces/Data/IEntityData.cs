using System;
using System.Collections.Generic;

namespace GenFree.Interfaces.Data
{
    public interface IEntityData: IHasID<int>
    {
        /// <summary>
        /// Gets date when the record was created.
        /// </summary>
        /// <value>The creation date.</value>
        DateTime dAnlDatum { get; }
        /// <summary>
        /// Gets date of the last change.
        /// </summary>
        /// <value>The last change date.</value>
        DateTime dEditDat { get; }
        /// <summary>
        /// Gets GUID of this entity.
        /// </summary>
        /// <value>The GUID.</value>
        Guid? gUid { get; }
        /// <summary>
        /// Gets the list of related events.
        /// </summary>
        /// <value>The list of events.</value>
        IList<IEventData> Events { get; }
        /// <summary>
        /// Gets the list of related entities.
        /// </summary>
        /// <value>The list of related entities.</value>
        IList<ILinkData> Connects { get; }
    }
}