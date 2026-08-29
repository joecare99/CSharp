using System;
using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Ollama.CodingAgent.Application.Interfaces;
using Ollama.CodingAgent.Application.ViewModels;
using Ollama.CodingAgent.Console.Cxaml;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Cxaml.ExamplesTests;

[TestClass]
public sealed class CxamlExamplesTests
{
    [TestMethod]
    public void Calc32ViewLoadsFromEmbeddedCxaml()
    {
        var view = Calc32Cons.Cxaml.Program.CreateView();
        Assert.IsInstanceOfType(view, typeof(Panel));
        Assert.AreEqual(50, view.size.Width);
    }

    [TestMethod]
    public void LeonardoViewLoadsFromEmbeddedCxaml()
    {
        var view = Leonardo.ST.Cxaml.Program.CreateView();
        Assert.IsInstanceOfType(view, typeof(Panel));
        Assert.AreEqual(60, view.size.Width);
    }

    [TestMethod]
    public void DetectiveViewLoadsFromEmbeddedCxaml()
    {
        var view = DetectiveGame.Console.Cxaml.Program.CreateView();
        Assert.IsInstanceOfType(view, typeof(Panel));
        Assert.AreEqual(80, view.size.Width);
    }

    [TestMethod]
    public void OllamaViewLoadsFromEmbeddedCxaml()
    {
        var sessionService = Substitute.For<IAgentSessionService>();
        var sessionStore = Substitute.For<IAgentSessionStore>();
        var approvalService = Substitute.For<IAgentApprovalService>();
        var session = new AgentSessionViewModel(
            sessionService,
            sessionStore,
            approvalService,
            "test-session",
            Environment.CurrentDirectory);
        var application = Substitute.For<IApplication>();
        application.size.Returns(new System.Drawing.Size(100, 30));

        var view = Program.CreateView(session, approvalService, application);

        Assert.IsInstanceOfType(view.Root, typeof(Panel));
        Assert.AreEqual(10, view.NamedControls.Count);
        Assert.IsInstanceOfType(view.NamedControls["Transcript"], typeof(Terminal));
        Assert.IsInstanceOfType(view.NamedControls["Planning"], typeof(ListBox));
        Assert.IsInstanceOfType(view.NamedControls["Prompt"], typeof(TextBox));
    }
}
