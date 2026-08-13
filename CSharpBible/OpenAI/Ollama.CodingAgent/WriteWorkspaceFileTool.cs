using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Writes one file under the configured workspace root.
/// </summary>
public sealed class WriteWorkspaceFileTool : IOllamaTool
{
    private static readonly string[] AllowedExtensions =
    [
        ".cs", ".csproj", ".sln", ".slnx", ".json", ".props", ".targets", ".md", ".txt", ".yml", ".yaml"
    ];

    private readonly WorkspacePathPolicy _workspacePathPolicy;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteWorkspaceFileTool"/> class.
    /// </summary>
    /// <param name="workspacePathPolicy">The workspace path policy.</param>
    public WriteWorkspaceFileTool(WorkspacePathPolicy workspacePathPolicy)
    {
        _workspacePathPolicy = workspacePathPolicy ?? throw new ArgumentNullException(nameof(workspacePathPolicy));
    }

    /// <inheritdoc />
    public string Name => "write_workspace_file";

    /// <inheritdoc />
    public string Description => "Writes content to a file under workspace root with extension and overwrite guards.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Write text to an allowed source/document file. Existing files require overwrite=true; content is capped at 200000 characters.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "relativePath",
                Type = "string",
                Description = "Required relative path under workspace root. Allowed extensions: .cs, .csproj, .sln, .slnx, .json, .props, .targets, .md, .txt, .yml, .yaml.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "content",
                Type = "string",
                Description = "Required UTF-8 text content, maximum 200000 characters.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "overwrite",
                Type = "boolean",
                Description = "Optional boolean; default false. Set true explicitly to replace an existing file.",
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
            WriteWorkspaceFileToolInput payload = JsonSerializer.Deserialize<WriteWorkspaceFileToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.RelativePath);
            ArgumentNullException.ThrowIfNull(payload.Content);
            if (payload.Content.Length > 200_000)
            {
                return OllamaToolValidationResult.Failure("content is too large (max 200000 chars).");
            }

            string targetPath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);
            string extension = Path.GetExtension(targetPath);
            if (Array.IndexOf(AllowedExtensions, extension) < 0)
            {
                return OllamaToolValidationResult.Failure($"File extension '{extension}' is not allowed.");
            }

            if (File.Exists(targetPath) && !payload.Overwrite)
            {
                return OllamaToolValidationResult.Failure("Target file exists and overwrite=false.");
            }

            return OllamaToolValidationResult.Success();
        }
        catch (Exception ex)
        {
            return OllamaToolValidationResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public async Task<OllamaToolResult> ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        WriteWorkspaceFileToolInput payload = JsonSerializer.Deserialize<WriteWorkspaceFileToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");
        string targetPath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? _workspacePathPolicy.WorkspaceRoot);

        if (File.Exists(targetPath) && !payload.Overwrite)
        {
            return new OllamaToolResult
            {
                Success = false,
                Output = "Target file exists and overwrite=false.",
            };
        }

        await File.WriteAllTextAsync(targetPath, payload.Content, cancellationToken);
        return new OllamaToolResult
        {
            Success = true,
            Output = $"Wrote file '{payload.RelativePath}' ({payload.Content.Length} chars).",
        };
    }
}
