using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Service interface for parsing LFM files.
/// </summary>
public interface ILfmParserService
{
    /// <summary>
    /// Parses the LFM content and returns the root object.
    /// </summary>
    /// <param name="content">The LFM file content as string.</param>
    /// <returns>The parsed LfmObject or null if parsing fails.</returns>
    LfmObject? Parse(string content);

    /// <summary>
    /// Loads and parses an LFM file from the given path.
    /// </summary>
    /// <param name="filePath">The path to the LFM file.</param>
    /// <returns>The parsed LfmObject or null if loading/parsing fails.</returns>
    LfmObject? LoadFromFile(string filePath);
}
