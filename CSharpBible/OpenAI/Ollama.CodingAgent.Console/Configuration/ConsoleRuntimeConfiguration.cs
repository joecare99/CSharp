using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;

namespace Ollama.CodingAgent.Console.Configuration;

/// <summary>
/// Holds the mutable console configuration used by future prompts.
/// </summary>
public sealed class ConsoleRuntimeConfiguration
{
    public ConsoleRuntimeConfiguration(string workspacePath, string endpoint, string model)
    {
        WorkspacePath = Path.GetFullPath(workspacePath);
        Endpoint = endpoint;
        Model = model;
    }

    public string Endpoint { get; private set; }
    public string Model { get; private set; }
    public string WorkspacePath { get; private set; }

    public void Set(string endpoint, string model, string workspacePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(workspacePath);
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("The endpoint must be an absolute HTTP or HTTPS URL.", nameof(endpoint));
        }

        Endpoint = uri.AbsoluteUri;
        Model = model.Trim();
        WorkspacePath = Path.GetFullPath(workspacePath);
    }
}