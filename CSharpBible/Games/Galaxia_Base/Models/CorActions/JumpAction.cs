using Galaxia.Models.Interfaces;

namespace Galaxia.Models.CorActions
{
    public abstract class JumpAction(ICorporation corporation, IFleet? fleet) : ICorAction
    {
        public ICorporation Corporation => corporation;
        public IFleet? Fleet => fleet;
        public abstract bool Execute();
    }
}