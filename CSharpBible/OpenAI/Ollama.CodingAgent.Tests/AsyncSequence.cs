using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace Ollama.CodingAgent.Tests;

internal static class AsyncSequence
{
    public static async IAsyncEnumerable<T> From<T>(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            yield return value;
            await System.Threading.Tasks.Task.Yield();
        }
    }
}
