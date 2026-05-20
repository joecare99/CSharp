namespace RnzTrauer.Core.Services.Interfaces;

/// <summary>
/// Provides JSON-based configuration loading for the ported tools.
/// </summary>
public interface IConfigLoader
{
    /// <summary>
    /// Loads a configuration instance from the specified JSON file.
    /// </summary>
    T Load<T>(string sFilePath) where T : new();
}
