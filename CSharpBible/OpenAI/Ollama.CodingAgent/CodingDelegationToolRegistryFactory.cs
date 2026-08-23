using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using Ollama.Tools;
using Ollama.Tools.Abstractions;

namespace Ollama.CodingAgent;

/// <summary>
/// Creates the tool registry used for delegated coding tasks.
/// </summary>
public sealed class CodingDelegationToolRegistryFactory
{
    private readonly WorkspacePathPolicy _workspacePathPolicy;
    private readonly IHttpClientFactory? _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodingDelegationToolRegistryFactory"/> class.
    /// </summary>
    /// <param name="workspacePathPolicy">The workspace path policy.</param>
    /// <param name="httpClientFactory">Optional HTTP client factory used for web lookups; when omitted a short-timeout client is created.</param>
    public CodingDelegationToolRegistryFactory(
        WorkspacePathPolicy workspacePathPolicy,
        IHttpClientFactory? httpClientFactory = null)
    {
        _workspacePathPolicy = workspacePathPolicy ?? throw new ArgumentNullException(nameof(workspacePathPolicy));
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Creates the registry with safe coding task tools.
    /// </summary>
    /// <returns>The tool registry.</returns>
    public IOllamaToolRegistry CreateRegistry()
    {
        HttpClient webLookupClient = _httpClientFactory?.CreateClient(OllamaHttpClientNames.WebLookup)
            ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        IOllamaTool[] tools =
        [
            new ListWorkspaceFilesTool(_workspacePathPolicy),
            new ReadWorkspaceFileTool(_workspacePathPolicy),
            new WriteWorkspaceFileTool(_workspacePathPolicy),
            new RunDotnetBuildTool(_workspacePathPolicy),
            new RunDotnetTestTool(_workspacePathPolicy),
            new WebLookupTool(webLookupClient, new WebKnowledgePolicy()),
            new LocalWikiWriteTool(new LocalKnowledgeBaseStore(Path.Combine(_workspacePathPolicy.WorkspaceRoot, ".agent", "local-wiki.json"))),
            new LocalWikiSearchTool(new LocalKnowledgeBaseStore(Path.Combine(_workspacePathPolicy.WorkspaceRoot, ".agent", "local-wiki.json"))),
        ];

        return new OllamaToolRegistry(tools);
    }
}
