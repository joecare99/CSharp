using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Galaxia.Models.Interfaces;
/// <summary>
/// Interface ICorporation
/// </summary>
/// <info>Corporations are the intergalactic player in the game</info>
public interface ICorporation
{
    /// <summary>
    /// Gets the name of the corporation.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }
    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    string Description { get; }
    /// <summary>
    /// Gets the color of the corporation.
    /// </summary>
    /// <value>The color of the corp.</value>
    Color CorpColor { get; }
    /// <summary>
    /// Gets the fleets of the corporation.
    /// </summary>
    /// <value>The fleets.</value>
    IList<IFleet> Fleets { get; }
    /// <summary>
    /// Gets the home of the corporation.
    /// </summary>
    /// <value>The home.</value>
    IStarsystem Home { get; }
    /// <summary>
    /// Gets the starsystems assigned to the corp.
    /// </summary>
    /// <value>The stars.</value>
    IList<IStarsystem> Stars { get; }
    /// <summary>
    /// Gets the hyperspace for this Corporation.
    /// </summary>
    /// <value>The hyperspace.</value>
    IHyperspaceSys hyperspace { get; }
    /// <summary>
    /// Gets the planed actions.
    /// </summary>
    /// <value>The actions.</value>
    IList<ICorAction> Actions { get; }
    /// <summary>
    /// Gets the list of possible actions.
    /// </summary>
    /// <returns>IList&lt;ICorAction&gt;.</returns>
    IList<ICorAction> GetPossibleActions();
    void ExecuteActions();
}