using Galaxia.Models.Interfaces;

namespace Galaxia.Models.CorActions
{
    /// <summary>
    /// Führt das Einschiffen (Embark) einer Flotte von einem Sternensystem in das Hyperspace-System der Corporation aus.
    /// </summary>
    public class EmbarkFleet : ICorAction
    {
        /// <summary>
        /// Die Corporation, die die Aktion ausführt.
        /// </summary>
        public ICorporation Corporation { get; }

        /// <summary>
        /// Die Flotte, die eingeschifft werden soll.
        /// </summary>
        public IFleet? Fleet { get; }

        /// <summary>
        /// Die Größe des Flottenteils, der eingeschifft werden soll. 
        /// </summary>
        public float FleetSize { get; }

        /// <summary>
        /// Das Sternensystem, aus dem die Flotte startet.
        /// </summary>
        public IStarsystem? SourceStarsystem { get; }

        /// <summary>
        /// Erstellt eine neue EmbarkFleet-Aktion.
        /// </summary>
        /// <param name="corporation">Die ausführende Corporation.</param>
        /// <param name="fleet">Die einzuschiffende Flotte.</param>
        /// <param name="sourceStarsystem">Das Start-Sternensystem.</param>
        public EmbarkFleet(ICorporation corporation, IFleet? fleet, IStarsystem? sourceStarsystem,float fleetSize =0f)
        {
            Corporation = corporation;
            Fleet = fleet;
            FleetSize = fleetSize;
            SourceStarsystem = sourceStarsystem;
        }

        /// <summary>
        /// Führt das Einschiffen der Flotte aus.
        /// </summary>
        /// <returns><c>true</c>, wenn das Einschiffen erfolgreich war, sonst <c>false</c>.</returns>
        public bool Execute()
        {
            // Überprüfe, ob die Flotte zur Corporation gehört und sich im angegebenen Sternensystem befindet
            if (Fleet?.Owner != Corporation || Fleet?.Container != SourceStarsystem)
                return false;

            // Versuche, die Flotte ins Hyperspace-System einzuschiffen
            if ((FleetSize > 1f) && (Fleet!.Size > FleetSize)) 
            {
                // Nicht genug Platz in den HyperSlots
                return Corporation.hyperspace.Embark(Fleet.Split(FleetSize)!);
            }
            else
                return Corporation.hyperspace.Embark(Fleet!);
        }
    }
}