using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;

namespace Ollama.CodingAgent.Console.Infrastructure;

/// <summary>
/// Reports capabilities of the current operating system.
/// </summary>
public sealed class SystemPlatformInfo : IPlatformInfo
{
    public bool IsWindows => OperatingSystem.IsWindows();
}
