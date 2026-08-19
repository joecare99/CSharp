using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;

namespace Ollama.CodingAgent.Desktop.Tests;

internal sealed class TestWorkspace : IDisposable
{
    public TestWorkspace()
    {
        RootPath = Path.Combine(AppContext.BaseDirectory, "CoverageWorkspaces", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(RootPath);
    }

    public string RootPath { get; }

    public string PathFor(params string[] paths)
    {
        string path = RootPath;
        foreach (string segment in paths)
        {
            path = Path.Combine(path, segment);
        }

        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(RootPath))
        {
            Directory.Delete(RootPath, recursive: true);
        }
    }
}
