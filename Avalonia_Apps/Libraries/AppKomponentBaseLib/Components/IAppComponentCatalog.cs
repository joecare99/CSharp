using System.Collections.Generic;

namespace AppKomponentBaseLib.Components;

/// <summary>
/// Provides a host-neutral view over the registered application components.
/// </summary>
public interface IAppComponentCatalog
{
    /// <summary>
    /// Gets the registered component registrations.
    /// </summary>
    IReadOnlyList<IAppComponentRegistration> Registrations { get; }
}
