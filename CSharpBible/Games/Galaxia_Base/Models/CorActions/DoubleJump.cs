using Galaxia.Models.Interfaces;
using System.Linq;

namespace Galaxia.Models.CorActions
{
    /// <summary>
    /// Führt einen Hyperjump einer Flotte aus dem Hyperspace-System in ein Ziel-Sternensystem aus.
    /// </summary>
    public class DoubleJump : JumpAction, ICorAction
    {
        /// <summary>
        /// Die Flotte, die den Hyperjump ausführen soll.
        /// </summary>
        public IFleet Fleet { get; }

        /// <summary>
        /// Die zweite Flotte, die den Hyperjump ausführen soll.
        /// </summary>
        public IFleet Fleet2 { get; }

        /// <summary>
        /// Das Ziel-Sternensystem für den Hyperjump.
        /// </summary>
        public IStarsystem TargetStarsystem { get; }

        /// <summary>
        /// Erstellt eine neue DoubleJump-Aktion.
        /// </summary>
        /// <param name="corporation">Die ausführende Corporation.</param>
        /// <param name="fleet">Die springende Flotte.</param>
        /// <param name="targetStarsystem">Das Ziel-Sternensystem.</param>
        public DoubleJump(ICorporation corporation, IFleet fleet1, IFleet fleet2, IStarsystem targetStarsystem):base(corporation)
        {
            Fleet = fleet1;
            Fleet2 = fleet2;
            TargetStarsystem = targetStarsystem;
        }

        /// <summary>
        /// Führt den Hyperjump der Flotte in das Ziel-Sternensystem aus.
        /// </summary>
        /// <returns><c>true</c>, wenn der Sprung erfolgreich war, sonst <c>false</c>.</returns>
        public override bool Execute()
        {
            // Überprüfe, ob die Flotte zur Corporation gehört und sich im Hyperspace befindet
            if (Fleet.Owner != Corporation || Fleet.Container is not IHyperSlot hs )
                return false;

            // Überprüfe, ob die Flotte zur Corporation gehört und sich im Hyperspace befindet
            if (Fleet2.Owner != Corporation || Fleet2.Container is not IHyperSlot hs2)
                return false;

            // Überprüfe, ob das Zielsystem erreichbar ist (optional, je nach Spielmechanik)
            // Beispiel: Hier wird nur geprüft, ob das Zielsystem existiert
            if (TargetStarsystem == null 
                || !hs.GetReachableSectors().Contains(TargetStarsystem.Sector)
                || !hs2.GetReachableSectors().Contains(TargetStarsystem.Sector))
                return false;

            // Führe den Sprung aus, indem die Flotte dem Zielsystem zugewiesen wird
            // Annahme: Das Zielsystem ist offen und kann eine Flotte aufnehmen
            if (TargetStarsystem.IsOpen)
            {
                // Die Flotte verlässt den HyperSlot und wird dem Zielsystem zugewiesen
                // Dies kann je nach Implementierung variieren
                TargetStarsystem.Sector.SetFleet(Fleet);
                TargetStarsystem.Sector.Fleet.Join(Fleet2);
                TargetStarsystem.SetFleet(Fleet);
                return true;
            }

            return false;
        }
    }
}