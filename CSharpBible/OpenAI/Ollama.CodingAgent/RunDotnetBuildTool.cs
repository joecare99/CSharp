using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Runs dotnet build for a bounded project or solution path.
/// </summary>
public sealed class RunDotnetBuildTool : IOllamaTool
{
    private readonly WorkspacePathPolicy _workspacePathPolicy;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="RunDotnetBuildTool"/> class.
    /// </summary>
    /// <param name="workspacePathPolicy">The workspace path policy.</param>
    public RunDotnetBuildTool(WorkspacePathPolicy workspacePathPolicy)
    {
        _workspacePathPolicy = workspacePathPolicy ?? throw new ArgumentNullException(nameof(workspacePathPolicy));
    }

    /// <inheritdoc />
    public string Name => "run_dotnet_build";

    /// <inheritdoc />
    public string Description => "Runs dotnet build for a project or solution inside workspace root.";

    /// <inheritdoc />
    public OllamaToolSchema Schema => new()
    {
        Summary = "Run dotnet build on one existing .csproj, .sln, or .slnx file. configuration defaults to Debug.",
        Parameters =
        [
            new OllamaToolParameter
            {
                Name = "relativePath",
                Type = "string",
                Description = "Required relative project/solution path under workspace root.",
                Required = true,
            },
            new OllamaToolParameter
            {
                Name = "configuration",
                Type = "string",
                Description = "Optional configuration name; default Debug.",
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
            RunDotnetBuildToolInput payload = JsonSerializer.Deserialize<RunDotnetBuildToolInput>(input, JsonOptions)
                ?? throw new InvalidOperationException("Invalid JSON input.");
            ArgumentException.ThrowIfNullOrWhiteSpace(payload.RelativePath);

            string fullPath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);
            string extension = Path.GetExtension(fullPath);
            if (!string.Equals(extension, ".csproj", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".sln", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(extension, ".slnx", StringComparison.OrdinalIgnoreCase))
            {
                return OllamaToolValidationResult.Failure("relativePath must target a .csproj, .sln, or .slnx file.");
            }

            if (!File.Exists(fullPath))
            {
                return OllamaToolValidationResult.Failure("Target build file does not exist.");
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
        RunDotnetBuildToolInput payload = JsonSerializer.Deserialize<RunDotnetBuildToolInput>(input, JsonOptions)
            ?? throw new InvalidOperationException("Invalid JSON input.");
        string fullPath = _workspacePathPolicy.ResolveWorkspacePath(payload.RelativePath);

        if (!File.Exists(fullPath))
        {
            return new OllamaToolResult
            {
                Success = false,
                Output = $"Target '{payload.RelativePath}' does not exist.",
            };
        }

        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _workspacePathPolicy.WorkspaceRoot,
        };
        startInfo.ArgumentList.Add("build");
        startInfo.ArgumentList.Add(fullPath);
        startInfo.ArgumentList.Add("--nologo");
        startInfo.ArgumentList.Add("-v");
        startInfo.ArgumentList.Add("minimal");
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add(string.IsNullOrWhiteSpace(payload.Configuration) ? "Debug" : payload.Configuration);

        using Process process = new()
        {
            StartInfo = startInfo,
        };
        process.Start();
        Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
        Task<string> errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync(cancellationToken);

        string output = await outputTask;
        string error = await errorTask;
        string combinedOutput = CombineOutput(output, error);
        if (combinedOutput.Length > 7000)
        {
            combinedOutput = combinedOutput[..7000];
        }

        return new OllamaToolResult
        {
            Success = process.ExitCode == 0,
            Output = combinedOutput,
        };
    }

    private static string CombineOutput(string output, string error)
    {
        StringBuilder builder = new();
        if (!string.IsNullOrWhiteSpace(output))
        {
            builder.AppendLine(output.Trim());
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            builder.AppendLine(error.Trim());
        }

        return builder.ToString().Trim();
    }
}
