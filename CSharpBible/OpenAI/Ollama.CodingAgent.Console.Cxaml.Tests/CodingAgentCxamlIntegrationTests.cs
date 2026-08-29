using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.Services;
using Ollama.CodingAgent.Application.ViewModels;
using CxamlApplication = ConsoleLib.CommonControls.Application;

namespace Ollama.CodingAgent.Console.Cxaml.Tests;

[TestClass]
public sealed class CodingAgentCxamlIntegrationTests
{
    [TestMethod]
    public void View_BindsTheSharedSessionAndExistingCommands()
    {
        AgentApprovalService approvals = new();
        AgentSessionViewModel session = new(
            Substitute.For<IAgentSessionService>(),
            Substitute.For<IAgentSessionStore>(),
            approvals,
            "cxaml-session",
            ".");
        using CxamlApplication application = new(Substitute.For<IWidgetSet>());

        CxamlLoadResult result = Program.CreateView(session, approvals, application);
        TextBox prompt = result.NamedControls["Prompt"] as TextBox
            ?? throw new AssertFailedException("The prompt control was not materialized.");

        prompt.ApplyNativeText("Inspect the CXAML view.");

        Assert.AreEqual("Inspect the CXAML view.", session.Prompt);
        Assert.IsNotNull(((Button)result.NamedControls["Send"]).Command);
        Assert.IsNotNull(((Button)result.NamedControls["Clear"]).Command);
        Assert.IsNotNull(((Button)result.NamedControls["Reload"]).Command);
        Assert.IsNotNull(((Button)result.NamedControls["Cancel"]).Command);
        StringAssert.Contains(result.NamedControls["Status"].Text, "cxaml-session");
    }
}
