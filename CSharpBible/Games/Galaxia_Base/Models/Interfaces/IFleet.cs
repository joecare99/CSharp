using System;

namespace Galaxia.Models.Interfaces;

public interface IFleet :IDisposable
{
    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>The owner.</value>
    ICorporation Owner { get; }

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    float Size { get; }

    IFleetContainer Container { get; }
    ISector? sector { get; }
    
    bool MoveTo (IFleetContainer container);

    bool Join(IFleet fleet);

    IFleet? Split (float size);

}