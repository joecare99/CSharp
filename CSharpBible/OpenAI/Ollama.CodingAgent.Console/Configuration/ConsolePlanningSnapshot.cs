using Ollama.CodingAgent.Console.Configuration;
using Ollama.CodingAgent.Console.Commands;
using Ollama.CodingAgent.Console.Interfaces;
using Ollama.CodingAgent.Console.Infrastructure;
using Ollama.CodingAgent.Console.Presentation;
using Ollama.CodingAgent.Console.Services;
using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
namespace Ollama.CodingAgent.Console.Configuration;

/// <summary>
/// Describes the planning context shown beside the agent conversation.
/// </summary>
public sealed record ConsolePlanningSnapshot(
    string Feature,
    string Backlog,
    string CurrentTask,
    string PreviousTask,
    string NextTask);
