using Galaxia.Models.Interfaces;
using System.Linq;

namespace Galaxia.Models.CorActions
{
    /// <summary>
    /// F�hrt einen Hyperjump einer Flotte aus dem Hyperspace-System in ein Ziel-Sternensystem aus.
    /// </summary>
    public class SingleJump : JumpAction, ICorAction
    {
        /// <summary>
        /// Die Corporation, die die Aktion ausf�hrt.
        /// </summary>
        public ICorporation Corporation { get; }

        /// <summary>
        /// Die Flotte, die den Hyperjump ausf�hren soll.
        /// </summary>
        public IFleet Fleet { get; }

        /// <summary>
        /// Das Ziel-Sternensystem f�r den Hyperjump.
        /// </summary>
        public IStarsystem TargetStarsystem { get; }

        /// <summary>
        /// Erstellt eine neue SingleJump-Aktion.
        /// </summary>
        /// <param name="corporation">Die ausf�hrende Corporation.</param>
        /// <param name="fleet">Die springende Flotte.</param>
        /// <param name="targetStarsystem">Das Ziel-Sternensystem.</param>
        public SingleJump(ICorporation corporation, IFleet fleet, IStarsystem targetStarsystem):base(corporation,fleet)
        {
            TargetStarsystem = targetStarsystem;
        }

        /// <summary>
        /// F�hrt den Hyperjump der Flotte in das Ziel-Sternensystem aus.
        /// </summary>
        /// <returns><c>true</c>, wenn der Sprung erfolgreich war, sonst <c>false</c>.</returns>
        public override bool Execute()
        {
            // �berpr�fe, ob die Flotte zur Corporation geh�rt und sich im Hyperspace befindet
            if (Fleet.Owner != Corporation || Fleet.Container is not IHyperSlot hs )
                return false;

            // �berpr�fe, ob das Zielsystem erreichbar ist (optional, je nach Spielmechanik)
            // Beispiel: Hier wird nur gepr�ft, ob das Zielsystem existiert
            if (TargetStarsystem == null || !hs.GetReachableSectors().Contains(TargetStarsystem.Sector))
                return false;

            // F�hre den Sprung aus, indem die Flotte dem Zielsystem zugewiesen wird
            // Annahme: Das Zielsystem ist offen und kann eine Flotte aufnehmen
            if (TargetStarsystem.IsOpen)
            {
                // Die Flotte verl�sst den HyperSlot und wird dem Zielsystem zugewiesen
                // Dies kann je nach Implementierung variieren
                TargetStarsystem.SetFleet(Fleet);
                return true;
            }

            return false;
        }
    }
}