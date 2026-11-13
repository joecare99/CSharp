using AA98_AvlnCodeStudio.Base.Profiling.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Base.Profiling.Services;

/// <summary>
/// Runs the optional dotnet-counters, dotnet-trace, and dotnet-dump command-line tools.
/// </summary>
public sealed class DotNetProfilingService : IDotNetProfilingService
{
    private readonly DotNetProfilingToolPaths _toolPaths;
    private readonly IProfilingProcessLauncher _processLauncher;

    public DotNetProfilingService(DotNetProfilingToolPaths? toolPaths = null)
        : this(toolPaths ?? new DotNetProfilingToolPaths(), new ProfilingProcessLauncher())
    {
    }

    internal DotNetProfilingService(
        DotNetProfilingToolPaths toolPaths,
        IProfilingProcessLauncher processLauncher)
    {
        _toolPaths = toolPaths ?? throw new ArgumentNullException(nameof(toolPaths));
        _processLauncher = processLauncher ?? throw new ArgumentNullException(nameof(processLauncher));
    }

    /// <inheritdoc/>
    public Task<IDotNetProfilingSession> StartAsync(
        DotNetProfilingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        if (request.ProcessId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "A positive target process ID is required.");
        }

        (string tool, IReadOnlyList<string> arguments, string? artifactPath) = BuildCommand(request);
        IProfilingProcess process;
        try
        {
            process = _processLauncher.Start(tool, arguments);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new DotNetProfilingException(
                $"Could not start profiling tool '{tool}': {exception.Message}",
                tool,
                innerException: exception);
        }

        DotNetProfilingSession session = new(
            request.ProcessId,
            request.Operation,
            tool,
            artifactPath,
            process);
        session.AttachCancellation(cancellationToken);
        return Task.FromResult<IDotNetProfilingSession>(session);
    }

    private (string Tool, IReadOnlyList<string> Arguments, string? ArtifactPath) BuildCommand(
        DotNetProfilingRequest request)
    {
        string tool;
        List<string> arguments = [];
        string? artifactPath = null;
        switch (request.Operation)
        {
            case DotNetProfilingOperation.Counters:
                tool = _toolPaths.Counters;
                arguments.AddRange(["monitor", "--process-id", request.ProcessId.ToString()]);
                break;
            case DotNetProfilingOperation.Trace:
                tool = _toolPaths.Trace;
                artifactPath = CreateArtifactPath(request.ArtifactRootDirectory, "trace", ".nettrace");
                arguments.AddRange(["collect", "--process-id", request.ProcessId.ToString(), "--output", artifactPath]);
                break;
            case DotNetProfilingOperation.HeapDump:
                tool = _toolPaths.Dump;
                artifactPath = CreateArtifactPath(request.ArtifactRootDirectory, "heap", ".dmp");
                arguments.AddRange(["collect", "--process-id", request.ProcessId.ToString(), "--type", "Heap", "--output", artifactPath]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request), "The profiling operation is not supported.");
        }

        return (tool, arguments, artifactPath);
    }

    private static string CreateArtifactPath(string? artifactRootDirectory, string artifactName, string extension)
    {
        if (string.IsNullOrWhiteSpace(artifactRootDirectory))
        {
            throw new ArgumentException(
                "A destination directory is required for trace and heap-dump operations.",
                nameof(artifactRootDirectory));
        }

        string root = Path.GetFullPath(artifactRootDirectory);
        Directory.CreateDirectory(root);
        EnsureNotReparsePoint(root);

        string artifactDirectory = Path.Combine(root, "dotnet-profiling");
        Directory.CreateDirectory(artifactDirectory);
        EnsureNotReparsePoint(artifactDirectory);

        string relativeDirectory = Path.GetRelativePath(root, artifactDirectory);
        if (Path.IsPathRooted(relativeDirectory)
            || relativeDirectory == "."
            || relativeDirectory == ".."
            || relativeDirectory.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relativeDirectory.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new IOException("The profiling artifact directory resolves outside the selected destination.");
        }

        string fileName = $"{artifactName}-{DateTimeOffset.UtcNow:yyyyMMddTHHmmssfffZ}-{Guid.NewGuid():N}{extension}";
        string artifactPath = Path.GetFullPath(Path.Combine(artifactDirectory, fileName));
        string relativePath = Path.GetRelativePath(root, artifactPath);
        if (Path.IsPathRooted(relativePath)
            || relativePath == ".."
            || relativePath.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relativePath.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal))
        {
            throw new IOException("The profiling artifact path resolves outside the selected destination.");
        }

        return artifactPath;
    }

    private static void EnsureNotReparsePoint(string directory)
    {
        if ((File.GetAttributes(directory) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException($"Profiling output directory '{directory}' must not be a symbolic link or reparse point.");
        }
    }
}
