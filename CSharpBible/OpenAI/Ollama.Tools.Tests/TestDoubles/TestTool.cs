using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.Tools.Tests.TestDoubles;

internal sealed class TestTool : IOllamaTool
{
    public required string Name { get; init; }

    public required string Description { get; init; }

    public OllamaToolSchema Schema { get; init; } = new()
    {
        Summary = "Accepts a plain string input.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "input",
                Description = "The tool input.",
                Required = true,
            },
        ],
    };

    public required string ResultText { get; init; }

    public OllamaToolValidationResult ValidationResult { get; init; } = OllamaToolValidationResult.Success();

    public OllamaToolValidationResult Validate(string input) => ValidationResult;

    public Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default) => Task.FromResult(new OllamaToolResult
    {
        Output = $"{ResultText}:{input}",
        Success = true,
    });
}
