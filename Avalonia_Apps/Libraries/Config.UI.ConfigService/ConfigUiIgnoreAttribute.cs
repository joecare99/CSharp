using System;

namespace Config.UI.ConfigService;

/// <summary>
/// Hides a configuration model property from the generated settings UI without
/// excluding it from persistence.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ConfigUiIgnoreAttribute : Attribute
{
}
