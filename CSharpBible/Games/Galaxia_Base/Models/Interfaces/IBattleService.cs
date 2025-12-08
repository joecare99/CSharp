namespace Galaxia.Models.Interfaces;

public interface IBattleService
{
    /// <summary>
    /// Starts the battle. Contains the battle logic.
    /// </summary>
    /// <param name="fleet1">The fleet1.</param>
    /// <param name="fleet2">The fleet2.</param>
    /// <returns>IFleet. The winning fleet.</returns>
    IFleet StartBattle(IFleet fleet1, IFleet fleet2);
}