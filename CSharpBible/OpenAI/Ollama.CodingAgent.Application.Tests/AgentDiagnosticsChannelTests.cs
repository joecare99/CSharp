using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.Application.Tests;

[TestClass]
public sealed class AgentDiagnosticsChannelTests
{
    [TestMethod]
    public void Record_PublishesTheSameProviderNeutralEvent()
    {
        AgentDiagnosticsChannel channel = new();
        AgentDiagnosticEvent? received = null;
        channel.EventRecorded += (_, diagnosticEvent) => received = diagnosticEvent;
        AgentDiagnosticEvent diagnosticEvent = new()
        {
            CorrelationId = "run-1",
            EventName = "completion.thinking",
            Detail = "Reasoning fragment",
        };

        channel.Record(diagnosticEvent);

        Assert.AreSame(diagnosticEvent, received);
    }
}
