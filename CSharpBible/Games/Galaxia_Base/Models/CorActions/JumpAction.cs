using Galaxia.Models.Interfaces;

namespace Galaxia.Models.CorActions
{
    public abstract class JumpAction(ICorporation corporation) : ICorAction
    {
        public ICorporation Corporation => corporation;

        public abstract bool Execute();
    }
}