using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Reads a bounded line range from a workspace file.
/// </summary>
public sealed class ReadWorkspaceFileTool : IOllamaTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly WorkspacePathPolicy _workspacePathPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadWorkspaceFileTool"/> class.
    /// </summary>
    /// <param name="workspacePathPolicy">The workspace path policy.</param>
    public ReadWorkspaceFileTool(WorkspacePathPolicy workspacePathPolicy)
    {
        _workspacePathPolicy = workspacePathPolicy ?? throw new ArgumentNullException(nameof(workspacePathPolicy));
    }

    /// <inheritdoc />
    public string Name => "read_workspace_file";

    /// <inheritdoc />
    public string Description => "Reads selected line ranges from files under workspace root.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Read a bounded line range from one existing workspace file. startLine defaults to 1 and lineCount to 120 (max 400).",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "relativePath",
                Type = "string",
                Description = "Required relative file path, including extension. Must remain under workspace root.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "startLine",
                Type = "number",
                Description = "Optional 1-based line number; default 1.",
                Required = false,
            },
            new OllamaToolParameter
            {
                Name = "lineCount",
                Type = "number",
                Description = "Optional number of lines from 1 to 400; default 120. Output includes line numbers.",
                Required = false,
            },
        ],
    };

    /// <inheritdoc />
    public OllamaToolValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return OllamaToolValidationResult.Failure("Input JSON is required.");
        }

        try
        {
            ReadWorkspaceFileToolInput payload = JsonSerializer.Deserialize<ReadWorkspaceFileToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.RelativePath);
            if (payload.StartLine < 1)
            {
                return OllamaToolValidationResult.Failure("startLine must be >= 1.");
            }

            if (payload.LineCount < 1)
            {
                return OllamaToolValidationResult.Failure("lineCount must be between 1 and 400.");
            }

            if (payload.LineCount > 400)
            {
                return OllamaToolValidationResult.Failure("lineCount must be between 1 and 400.");
            }

            _ = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);
            return OllamaToolValidationResult.Success();
        }
        catch (Exception ex)
        {
            return OllamaToolValidationResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        ReadWorkspaceFileToolInput payload = JsonSerializer.Deserialize<ReadWorkspaceFileToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");

        string filePath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);
        if (!File.Exists(filePath))
        {
            return Task.FromResult(new OllamaToolResult
            {
                Success = false,
                Output = $"File '{payload.RelativePath}' was not found under workspace root.",
            });
        }

        string[] allLines = File.ReadAllLines(filePath);
        int lineCount = Math.Clamp(payload.LineCount, 1, 400);
        int startIndex = payload.StartLine - 1;
        if (startIndex >= allLines.Length)
        {
            return Task.FromResult(new OllamaToolResult
            {
                Success = true,
                Output = string.Empty,
            });
        }

        StringBuilder builder = new();
        foreach ((string line, int index) in allLines.Skip(startIndex).Take(lineCount).Select((line, offset) => (line, startIndex + offset + 1)))
        {
            builder.Append(index);
            builder.Append(": ");
            builder.AppendLine(line);
        }

        return Task.FromResult(new OllamaToolResult
        {
            Success = true,
            Output = builder.ToString().TrimEnd(),
        });
    }
}
