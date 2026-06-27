using System;

namespace AppKomponentBaseLib.Configuration;

/// <summary>
/// Defines a reusable setting contribution exposed by an application component.
/// </summary>
public interface IAppSettingContribution
{
    /// <summary>
    /// Gets the stable component identifier that owns the setting.
    /// </summary>
    string ComponentId { get; }

    /// <summary>
    /// Gets the setting metadata exposed to the host.
    /// </summary>
    AppSettingDescriptor Descriptor { get; }
}
