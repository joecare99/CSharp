using System;

namespace Config.Service;

/// <summary>
/// Excludes a model property from configuration sections, editors, and persistence.
/// Use this for computed, derived, or service-internal properties that must not be
/// part of the shared configuration surface.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ConfigIgnoreAttribute : Attribute
{
}
