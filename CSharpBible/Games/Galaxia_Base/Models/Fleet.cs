using Galaxia.Models.Interfaces;

namespace Galaxia.Models
{
    /// <summary>
    /// Repr�sentiert eine Flotte, die immer zu einer Corporation geh�rt und sich in einem IFleetContainer befindet.
    /// Flotten mit einer Gr��e &gt; 1 k�nnen in einen Sektor aufgeteilt werden.
    /// </summary>
    public class Fleet : IFleet
    {
        /// <summary>
        /// Die Corporation, der diese Flotte geh�rt.
        /// </summary>
        public ICorporation Owner { get; }

        /// <summary>
        /// Die aktuelle Gr��e der Flotte.
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// Das aktuelle Container-Objekt, in dem sich die Flotte befindet (z.B. Sektor, HyperSlot, Sternensystem).
        /// </summary>
        public IFleetContainer Container { get; private set; }

        /// <summary>
        /// Der aktuelle Sektor, in dem sich die Flotte befindet (falls anwendbar).
        /// </summary>
        public ISector? sector => Container as ISector;

        /// <summary>
        /// Erstellt eine neue Flotte mit Besitzer, Gr��e und Start-Container.
        /// </summary>
        /// <param name="owner">Die zugeh�rige Corporation.</param>
        /// <param name="size">Die Gr��e der Flotte.</param>
        /// <param name="container">Das Start-Container-Objekt.</param>
        public Fleet(ICorporation owner, float size, IFleetContainer container)
        {
            Owner = owner;
            Size = size;
            Container = container;
            container.SetFleet(this);
        }

        /// <summary>
        /// Versucht, eine andere Flotte mit dieser zu vereinen (Join).
        /// </summary>
        /// <param name="fleet">Die zu vereinigende Flotte.</param>
        /// <returns><c>true</c>, wenn erfolgreich; sonst <c>false</c>.</returns>
        public bool Join(IFleet fleet)
        {
            if (fleet.Owner == Owner && fleet.Container.IsOpen)
            {
                Size += fleet.Size;
                fleet.Container.SetFleet(null);
                fleet.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Teilt die Flotte auf, falls die Gr��e &gt; gew�nschte Teilgr��e ist.
        /// Die neue Flotte wird im selben Container platziert.
        /// </summary>
        /// <param name="size">Die Gr��e der abzutrennenden Flotte.</param>
        /// <returns>Eine neue Flotte mit der gew�nschten Gr��e oder <c>null</c>, falls nicht m�glich.</returns>
        public IFleet? Split(float size)
        {
            if (size >= Size || size <= 0 || Container is not IStarsystem stsystem)
                return null;

            Size -= size;
            return new Fleet(Owner, size, stsystem.Sector);
        }

        /// <summary>
        /// Setzt den Container der Flotte (z.B. nach einem Sprung).
        /// </summary>
        /// <param name="container">Das neue Container-Objekt.</param>
        public bool MoveTo(IFleetContainer container)
        {
            if (container == null || !container.IsOpen)
                return false;

            if (Container == container)
                return true;

            var _oldContainer = Container;
            Container = container;
            var _result = container.SetFleet(this);
            if(_result) _oldContainer.SetFleet(null);
            return _result;
        }

        public void Dispose()
        {
           Size = 0;
        }
    }
}