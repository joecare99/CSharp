namespace Galaxia.Models.Interfaces
{
    public interface IFleetContainer
    {
        /// <summary>
        /// Gets a value indicating whether this instance is open .
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// This property is typically used to determine if the fleet in the container is currently accessible.
        /// </remarks>
        bool IsOpen { get; }
        /// <summary>
        /// Gets the fleet contained within this container.
        /// </summary>
        IFleet? Fleet { get; }

        bool SetFleet(IFleet? fleet);
    }
}