using System.Threading;
using System.Threading.Tasks;

namespace Ollama.Tools.Abstractions;

/// <summary>
/// Defines a callable tool in the Ollama tool layer.
/// </summary>
public interface IOllamaTool
{
    /// <summary>
    /// Gets the unique tool name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the human-readable tool description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the declared input schema for the tool.
    /// </summary>
    OllamaToolSchema Schema { get; }

    /// <summary>
    /// Validates the provided input before execution.
    /// </summary>
    /// <param name="input">The tool input payload.</param>
    /// <returns>The validation outcome.</returns>
    OllamaToolValidationResult Validate(string input);

    /// <summary>
    /// Executes the tool with the provided input.
    /// </summary>
    /// <param name="input">The tool input payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The tool execution result.</returns>
    Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default);
}
