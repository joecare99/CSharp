namespace Galaxia.Models.Interfaces
{
    public interface ICorAction
    {
        /// <summary>
        /// Gets the executing corporation.
        /// </summary>
        /// <value>The corporation.</value>
        ICorporation Corporation { get; }
        IFleet Fleet { get; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Execute();
    }
}