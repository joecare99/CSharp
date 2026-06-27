using AppKomponentBaseLib.Components;
using System.Collections.Generic;

namespace AppKomponentBaseLib.Configuration;

/// <summary>
/// Defines a reusable component registration that also contributes settings metadata.
/// </summary>
public interface IAppSettingsContributor : IAppComponentRegistration
{
    /// <summary>
    /// Gets the settings contributed by the component.
    /// </summary>
    IReadOnlyList<IAppSettingContribution> Settings { get; }
}
