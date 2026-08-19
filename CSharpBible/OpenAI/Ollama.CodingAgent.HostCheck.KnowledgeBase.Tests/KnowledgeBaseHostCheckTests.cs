using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ollama.CodingAgent;
using Ollama.CodingAgent.Models;

namespace Ollama.CodingAgent.HostCheck.KnowledgeBase.Tests;

[TestClass]
public sealed class KnowledgeBaseHostCheckTests
{
    [TestMethod]
    public async Task MalformedCaseReporter_HandlesSuccessAndFailureActions()
    {
        MethodInfo reporter = Assembly.Load("Ollama.CodingAgent.HostCheck.KnowledgeBase")
            .GetType("Ollama.CodingAgent.HostCheck.KnowledgeBase.Program", throwOnError: true)!
            .GetMethod("TryMalformedCaseAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

        await (Task)reporter.Invoke(null, [new Func<Task>(() => Task.CompletedTask), "success"])!;
        await (Task)reporter.Invoke(null, [new Func<Task>(() => Task.FromException(new InvalidOperationException("expected"))), "failure"])!;
    }

    [TestMethod]
    public async Task RunAsync_UsesCallerProvidedWorkspaceForDefaultAndImportFlows()
    {
        string root = Path.Combine(AppContext.BaseDirectory, "CoverageWorkspaces", Guid.NewGuid().ToString("N"));
        string vault = Path.Combine(root, "vault");
        Directory.CreateDirectory(vault);
        await File.WriteAllTextAsync(Path.Combine(vault, "page.md"), "# Page\n\nDependency injection");
        MethodInfo run = Assembly.Load("Ollama.CodingAgent.HostCheck.KnowledgeBase")
            .GetType("Ollama.CodingAgent.HostCheck.KnowledgeBase.Program", throwOnError: true)!
            .GetMethod("RunAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

        try
        {
            Assert.AreEqual(0, await (Task<int>)run.Invoke(null, [Array.Empty<string>(), root])!);
            Assert.AreEqual(0, await (Task<int>)run.Invoke(null, [new[] { vault }, Path.Combine(root, "import")])!);
        }
        finally
        {
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }

    }

    [TestMethod]
    public async Task Main_UsesInjectableDefaultWorkspaceFactory()
    {
        Type programType = Assembly.Load("Ollama.CodingAgent.HostCheck.KnowledgeBase")
            .GetType("Ollama.CodingAgent.HostCheck.KnowledgeBase.Program", throwOnError: true)!;
        FieldInfo factory = programType.GetField("DefaultRootFactory", BindingFlags.NonPublic | BindingFlags.Static)!;
        FieldInfo invalidEntryFactory = programType.GetField("InvalidEntryFactory", BindingFlags.NonPublic | BindingFlags.Static)!;
        Delegate original = (Delegate)factory.GetValue(null)!;
        Delegate originalInvalidEntryFactory = (Delegate)invalidEntryFactory.GetValue(null)!;
        MethodInfo createDefaultRoot = programType.GetMethod("CreateDefaultRoot", BindingFlags.NonPublic | BindingFlags.Static)!;
        Assert.IsInstanceOfType(createDefaultRoot.Invoke(null, null), typeof(string));
        string root = Path.Combine(AppContext.BaseDirectory, "CoverageWorkspaces", Guid.NewGuid().ToString("N"));
        try
        {
            factory.SetValue(null, (Func<string>)(() => root));
            invalidEntryFactory.SetValue(null, (Func<LocalKnowledgeEntry>)(() => new LocalKnowledgeEntry
            {
                Id = "valid",
                Title = "valid",
                Summary = "valid",
            }));
            MethodInfo main = programType.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static)!;
            Assert.AreEqual(0, await (Task<int>)main.Invoke(null, [Array.Empty<string>()])!);
        }
        finally
        {
            factory.SetValue(null, original);
            invalidEntryFactory.SetValue(null, originalInvalidEntryFactory);
            if (Directory.Exists(root))
            {
                Directory.Delete(root, recursive: true);
            }
        }
    }
}
