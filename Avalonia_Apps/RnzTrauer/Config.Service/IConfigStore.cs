using System;
using System.Threading.Tasks;

namespace Config.Service;

/// <summary>
/// Generic contract for loading and saving configuration sections by their stable key.
/// Each section's JSON document has the section key as its file or logical path.
/// </summary>
public interface IConfigStore
{
    /// <summary>
    /// Loads a configuration model of type <typeparamref name="T"/> from storage.
    /// Falls back to <paramref name="fallbackValue"/> when no value exists for this section.
    /// </summary>
    /// <param name="sectionName">The stable section key.</param>
    /// <param name="fallbackValue">Default model returned when the section has no stored value.</param>
    /// <returns>The loaded or fallback configuration model.</returns>
    Task<T> LoadAsync<T>(string sectionName, T fallbackValue);

    /// <summary>Saves a configuration model of type <typeparamref name="T"/> to storage by its stable key.</summary>
    /// <param name="sectionName">The stable section key.</param>
    /// <param name="value">The model to persist.</param>
    Task SaveAsync<T>(string sectionName, T value);

    /// <summary>Removes the stored configuration for <paramref name="sectionName"/> if it exists.</summary>
    Task ResetAsync(string sectionName);
}
