using System;

namespace Config.Service;

/// <summary>
/// Marks a configuration property as sensitive, for example a password or token.
/// Sensitive values are persisted by the store, but configuration UIs must render
/// them masked and must never echo them into logs or diagnostic output.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SensitiveConfigPropertyAttribute : Attribute
{
}
