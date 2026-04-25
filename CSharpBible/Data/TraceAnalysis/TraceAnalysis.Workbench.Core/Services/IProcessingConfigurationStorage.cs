using TraceAnalysis.Workbench.Core.Models;

namespace TraceAnalysis.Workbench.Core.Services;

/// <summary>
/// Provides persistence for processing configuration artifacts.
/// </summary>
public interface IProcessingConfigurationStorage
{
    /// <summary>
    /// Loads a processing configuration from the specified path.
    /// </summary>
    /// <param name="filePath">The configuration file path.</param>
    /// <returns>The loaded processing configuration.</returns>
    ProcessingConfigurationModel Load(string filePath);

    /// <summary>
    /// Saves a processing configuration to the specified path.
    /// </summary>
    /// <param name="filePath">The target configuration file path.</param>
    /// <param name="configuration">The configuration to save.</param>
    void Save(string filePath, ProcessingConfigurationModel configuration);
}
