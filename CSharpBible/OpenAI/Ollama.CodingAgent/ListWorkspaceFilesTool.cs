using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Lists workspace files under a bounded root path.
/// </summary>
public sealed class ListWorkspaceFilesTool : IOllamaTool
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly WorkspacePathPolicy _workspacePathPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListWorkspaceFilesTool"/> class.
    /// </summary>
    /// <param name="workspacePathPolicy">The workspace path policy.</param>
    public ListWorkspaceFilesTool(WorkspacePathPolicy workspacePathPolicy)
    {
        _workspacePathPolicy = workspacePathPolicy ?? throw new ArgumentNullException(nameof(workspacePathPolicy));
    }

    /// <inheritdoc />
    public string Name => "list_workspace_files";

    /// <inheritdoc />
    public string Description => "Lists files under the configured workspace root.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "List files recursively. Use {} for workspace root; maxFiles defaults to 100 and is capped at 300.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "relativePath",
                Type = "string",
                Description = "Optional relative folder path; omit or use '.' for workspace root. Paths outside the root are rejected.",
                Required = false,
            },
            new OllamaToolParameter
            {
                Name = "maxFiles",
                Type = "number",
                Description = "Optional integer limit from 1 to 300; default 100. Results are relative paths.",
                Required = false,
            },
        ],
    };

    /// <inheritdoc />
    public OllamaToolValidationResult Validate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return OllamaToolValidationResult.Success();
        }

        try
        {
            ListWorkspaceFilesToolInput payload = JsonSerializer.Deserialize<ListWorkspaceFilesToolInput>(input, JsonOptions) ?? new ListWorkspaceFilesToolInput();
            if (payload.MaxFiles < 1 || payload.MaxFiles > 300)
            {
                return OllamaToolValidationResult.Failure("maxFiles must be between 1 and 300.");
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
        ListWorkspaceFilesToolInput payload = string.IsNullOrWhiteSpace(input)
            ? new ListWorkspaceFilesToolInput()
            : JsonSerializer.Deserialize<ListWorkspaceFilesToolInput>(input, JsonOptions) ?? new ListWorkspaceFilesToolInput();
        int maxFiles = Math.Clamp(payload.MaxFiles, 1, 300);
        string startPath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);

        if (!Directory.Exists(startPath))
        {
            return Task.FromResult(new OllamaToolResult
            {
                Success = false,
                Output = $"Directory '{payload.RelativePath}' does not exist under workspace root.",
            });
        }

        string[] files = Directory.GetFiles(startPath, "*", SearchOption.AllDirectories)
            .Take(maxFiles)
            .Select(path => Path.GetRelativePath(_workspacePathPolicy.WorkspaceRoot, path))
            .ToArray();

        return Task.FromResult(new OllamaToolResult
        {
            Success = true,
            Output = JsonSerializer.Serialize(files, new JsonSerializerOptions { WriteIndented = true }),
        });
    }
}
