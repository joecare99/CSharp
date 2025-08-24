using Galaxia.Models.CorActions;
using Galaxia.Models.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Galaxia.Models
{
    /// <summary>
    /// Repräsentiert eine intergalaktische Corporation im Spiel.
    /// </summary>
    public class Corporation : ICorporation
    {
        private bool xExecute;

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public Color CorpColor { get; }

        /// <inheritdoc/>
        public IList<IFleet> Fleets { get; }

        /// <inheritdoc/>
        public IStarsystem Home { get; }

        /// <inheritdoc/>
        public IList<IStarsystem> Stars { get; }

        /// <inheritdoc/>
        public IHyperspaceSys hyperspace { get; }

        public IList<ICorAction> Actions => throw new System.NotImplementedException();

        /// <summary>
        /// Erstellt eine neue Instanz der <see cref="Corporation"/> Klasse.
        /// </summary>
        /// <param name="name">Der Name der Corporation.</param>
        /// <param name="description">Die Beschreibung der Corporation.</param>
        /// <param name="corpColor">Die Farbe der Corporation.</param>
        /// <param name="home">Das Heimatsystem der Corporation.</param>
        /// <param name="stars">Die zugewiesenen Sternsysteme.</param>
        /// <param name="hyperspace">Das Hyperspace-System der Corporation.</param>
        public Corporation(
            string name,
            string description,
            Color corpColor,
            IStarsystem home,
            IList<IStarsystem> stars,
            IHyperspaceSys hyperspace)
        {
            Name = name;
            Description = description;
            CorpColor = corpColor;
            Home = home;
            Stars = stars;
            this.hyperspace = hyperspace;
            Fleets = new List<IFleet>();
        }

        public IList<ICorAction> GetPossibleActions()
        {
            IList<ICorAction> possibleActions = [];
            // Wenn Hyperslot verfügbar ist, füge die Aktion hinzu
            if (hyperspace.IsAvailable && !Actions.Any(a => a is EmbarkFleet))
            {
                possibleActions.Add(new EmbarkFleet(this, null, Home));
            }

            if (hyperspace.CombReachableSectors().Count() > 0 && !Actions.Any(a => a is JumpAction))
            {
                possibleActions.Add(new DoubleJump(this, null,null, Home));
            }

            if (hyperspace.HyperSlots.Count(a=> a.Fleet != null) > Actions.Count(a => a is JumpAction) && !Actions.Any(a => a is DoubleJump))
            {
                possibleActions.Add(new SingleJump(this, null, Home));
            }

            return possibleActions;
        }

        public void ExecuteActions()
        {
            xExecute = true;
            // Logik zur Ausführung der Aktionen
            
        }
    }
}