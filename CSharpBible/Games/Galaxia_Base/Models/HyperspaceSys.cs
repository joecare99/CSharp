using Galaxia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Galaxia.Models
{
    /// <summary>
    /// Implementierung des Hyperspace-Systems einer Corporation.
    /// </summary>
    public class HyperspaceSys : IHyperspaceSys
    {
        private readonly List<IHyperSlot> _hyperSlots;
        private int _startSpaceTime;

        public ICorporation corporation { get; }
        public ISpace space { get; }
        // Korrigierte Eigenschaft gemäß Interface (IsAvailible).
        public bool IsAvailible => _startSpaceTime != space.SpaceTime && _hyperSlots.Any(slot => slot.IsOpen);
        public IReadOnlyList<IHyperSlot> HyperSlots => _hyperSlots;

        public bool IsAvailable => HyperSlots.Any(slot => slot.IsOpen);

        public HyperspaceSys(ICorporation corporation, ISpace space)
        {
            this.corporation = corporation;
            this.space = space;
            _hyperSlots = new List<IHyperSlot>
            {
                new HyperSlot(this),
                new HyperSlot(this)
            };
        }

        public bool Embark(IFleet fleet)
        {
            if (_startSpaceTime != space.SpaceTime)
            {
                foreach (var slot in _hyperSlots)
                {
                    if (slot is HyperSlot hs && slot.Fleet == null)
                    {
                        hs.SetFleet(fleet);
                        _startSpaceTime = space.SpaceTime;
                        return true;
                    }
                }
            }
            return false;
        }

        public IEnumerable<ISector> CombReachableSectors()
        {
            IEnumerable<ISector> reachableSectors = space.Sectors.Values;
            foreach (var slot in _hyperSlots)
            {
                if (slot is HyperSlot hs)
                {
                    reachableSectors = reachableSectors.Intersect(hs.GetReachableSectors());
                }
            }
            return reachableSectors;
        }

        private class HyperSlot(IHyperspaceSys hyperspaceSys) : IHyperSlot
        {
            private int _startSpaceTime;
            IHyperspaceSys HyperspaceSys => hyperspaceSys;
            public IFleet? Fleet { get; private set; }
            public bool IsOpen => Fleet == null;
            public ISector? StartSector { get; private set; }

            public float JumpEnergy
            {
                get => Fleet == null ? 0 : HyperspaceSys.space.SpaceTime - _startSpaceTime;
                private set => _startSpaceTime = HyperspaceSys.space.SpaceTime;
            }

            public bool SetFleet(IFleet? fleet)
            {
                if (IsOpen || fleet == null)
                    Fleet = fleet;
                StartSector = fleet?.sector;
                JumpEnergy = 0;
                return true;
            }

            public IEnumerable<ISector> GetReachableSectors()
            {
                foreach (ISector sector in HyperspaceSys.space.Sectors.Values)
                {
                    if (StartSector != null
                        && sector.Position.DistanceTo(StartSector.Position) == (int)Math.Round(JumpEnergy))
                    {
                        yield return sector;
                    }
                }
            }
        }
    }
}