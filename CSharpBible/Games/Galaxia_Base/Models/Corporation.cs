using BaseLib.Helper;
using Galaxia.Models.CorActions;
using Galaxia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Galaxia.Models
{
    /// <summary>
    /// Repräsentiert eine intergalaktische Corporation.
    /// </summary>
    public class Corporation : ICorporation
    {
        /// <inheritdoc/>
        public string Name { get; }
        /// <inheritdoc/>
        public string Description { get; }
        /// <inheritdoc/>
        public Color CorpColor { get; }
        /// <inheritdoc/>
        public IList<IFleet> Fleets { get; } = [];
        /// <inheritdoc/>
        public IStarsystem Home { get; }
        /// <inheritdoc/>
        public IList<IStarsystem> Stars { get; }= [];
        /// <inheritdoc/>
        public IHyperspaceSys hyperspace { get; }

        public IList<ICorAction> Actions { get; } = [];

        /// <summary>
        /// Voller Konstruktor bei dem ein Hyperspace-System übergeben wird.
        /// </summary>
        public Corporation(string name,
                           string description,
                           Color corpColor,
                           IStarsystem home,
                           IHyperspaceSys hyperspace)
        {
            Name = name;
            Description = description;
            CorpColor = corpColor;
            Home = home;
            Stars.Add(Home);
            this.hyperspace = hyperspace;
        }

        /// <summary>
        /// Komfort-Konstruktor: Erstellt automatisch ein Hyperspace-System aus Space.
        /// </summary>
        public Corporation(string name,
                           string description,
                           Color corpColor,
                           IStarsystem home,
                           ISpace space)
        {
            Name = name;
            Description = description;
            CorpColor = corpColor;
            Home = home;
            Stars.Add(Home);
            hyperspace = new HyperspaceSys(this, space);
        }

        public IList<ICorAction> GetPossibleActions()
        {
            var possibleActions = new List<ICorAction>();
            // if ships are in the hyperspace-slots , they can jump to reachable sectors
            if (!Actions.Any(a => a is DoubleJump) && hyperspace.HyperSlots.Any(s => s.Fleet != null && !Actions.Any(a => a.Fleet == s.Fleet)))
            {
                possibleActions.Add(new SingleJump(this, null, null));
            }

            // Test if a double jump is possible (2 ships in hyperspace and combined reachable sectors)
            if (!Actions.Any(a => a is JumpAction) && hyperspace.CombReachableSectors().Count() > 0)
            {
                possibleActions.Add(new DoubleJump(this, hyperspace.HyperSlots[0].Fleet, hyperspace.HyperSlots[1].Fleet, null));
            }

            // test if a Hyperslot is free or gets free this turn -> Embark is possible 
            if (hyperspace.IsAvailable || Actions.Any(a => a is JumpAction) && !Actions.Any(a => a is EmbarkFleet))
            {
                possibleActions.Add(new EmbarkFleet(this, null, null ));
            }

            return possibleActions;
        }

        public void ExecuteProduction()
        {
            // Get the Production System of the Game
            var prodSys = IoC.GetRequiredService<IProductionSystem>();
            // Execute production in all owned starsystems

        }

        public void ExecuteActions()
        {
            // Signal to the (Game-) Engine that the turn is over for this Corporation

        }
    }
}