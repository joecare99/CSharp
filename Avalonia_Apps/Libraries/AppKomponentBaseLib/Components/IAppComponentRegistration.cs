using Microsoft.Extensions.DependencyInjection;

namespace AppKomponentBaseLib.Components;

/// <summary>
/// Defines how a reusable application component contributes itself to a host.
/// </summary>
public interface IAppComponentRegistration
{
    /// <summary>
    /// Gets the component metadata exposed to the host.
    /// </summary>
    AppComponentDescriptor Descriptor { get; }

    /// <summary>
    /// Registers the component services into the host service collection.
    /// </summary>
    /// <param name="services">The host service collection.</param>
    void Register(IServiceCollection services);
}
