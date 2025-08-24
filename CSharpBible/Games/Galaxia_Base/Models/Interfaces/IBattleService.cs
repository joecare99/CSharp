using Galaxia.Models.Interfaces;

namespace Galaxia.Models
{
    internal interface IBattleService
    {
        IFleet StartBattle(IFleet fleet1, IFleet fleet2);
    }
}