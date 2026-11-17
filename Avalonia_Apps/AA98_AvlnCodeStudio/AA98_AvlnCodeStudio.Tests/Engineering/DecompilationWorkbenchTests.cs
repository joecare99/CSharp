using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Decompilation;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.UI.Resources;
using Code.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DecompilationWorkbenchTests
{
    private static AssemblyTypeInfo Type(string name) => new(name, name, 1, true, [], [], []);

    [TestMethod]
    public async Task GermanResourcesLocalizeStatusAndPreserveErrorDetails()
    {
        CultureInfo? previous = UiStrings.Culture;
        try
        {
            UiStrings.Culture = CultureInfo.GetCultureInfo("de-DE");
            IAssemblyDecompilationService service = Substitute.For<IAssemblyDecompilationService>();
            service.InspectAsync("bad", Arg.Any<CancellationToken>()).Returns<Task<AssemblyInspection>>(_ => throw new IOException("details"));
            using DecompilationWorkbench workbench = new(service);
            Assert.AreEqual("Eine verwaltete Assembly zur Untersuchung auswählen.", workbench.Status);
            await workbench.InspectAsync("bad");
            Assert.AreEqual("Assembly kann nicht untersucht werden: details", workbench.Status);
            Assert.AreEqual("Assembly-Typen", UiStrings.DecompileTypesLabel);
            Assert.AreEqual("Assembly dekompilieren...", UiStrings.DecompileAssemblyMenuText);
        }
        finally
        {
            UiStrings.Culture = previous;
        }
    }

    [TestMethod]
    public async Task RealAssembly_ExplicitTypeProducesNavigableArtifact()
    {
        string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "decompile-test-" + Guid.NewGuid().ToString("N")));
        try
        {
            using DecompilationWorkbench workbench = new(new AssemblyDecompilationService(root));
            await workbench.InspectAsync(typeof(DecompilationWorkbenchTests).Assembly.Location);
            Assert.IsNull(workbench.SelectedType);
            Assert.IsNull(await workbench.DecompileSelectedAsync());
            AssemblyTypeInfo type = workbench.Types.Single(t => t.FullName == typeof(DecompilationWorkbenchTests).FullName);
            workbench.SelectType(type);
            CodeLocation? result = await workbench.DecompileSelectedAsync();
            Assert.IsNotNull(result, workbench.Status);
            Assert.IsTrue(result.Path.StartsWith(root, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(File.Exists(result.Path));
            Assert.IsTrue(result.Line > 0);
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task EmptyAndFailureClearPreviousSelection()
    {
        IAssemblyDecompilationService service = Substitute.For<IAssemblyDecompilationService>();
        AssemblyTypeInfo type = Type("A");
        service.InspectAsync("first", Arg.Any<CancellationToken>()).Returns(new AssemblyInspection("first", "First", "1", [type]));
        service.InspectAsync("empty", Arg.Any<CancellationToken>()).Returns(new AssemblyInspection("empty", "Empty", "1", []));
        service.InspectAsync("bad", Arg.Any<CancellationToken>()).Returns<Task<AssemblyInspection>>(_ => throw new InvalidDataException("bad input"));
        using DecompilationWorkbench workbench = new(service);
        await workbench.InspectAsync("first");
        workbench.SelectType(type);
        await workbench.InspectAsync("empty");
        Assert.AreEqual(0, workbench.Types.Count);
        Assert.IsNull(workbench.SelectedType);
        Assert.AreEqual(UiStrings.DecompileNoTypesText, workbench.Status);
        await workbench.InspectAsync("bad");
        Assert.IsNull(workbench.Assembly);
        StringAssert.Contains(workbench.Status, "bad input");
        Assert.IsFalse(workbench.IsBusy);
    }

    [TestMethod]
    public async Task OlderInspectionCannotReplaceNewContextEvenWhenCancellationIgnored()
    {
        IAssemblyDecompilationService service = Substitute.For<IAssemblyDecompilationService>();
        var old = new TaskCompletionSource<AssemblyInspection>(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken token = default;
        service.InspectAsync("old", Arg.Any<CancellationToken>()).Returns(call =>
        {
            token = call.Arg<CancellationToken>();
            return old.Task;
        });
        service.InspectAsync("new", Arg.Any<CancellationToken>()).Returns(new AssemblyInspection("new", "New", "1", [Type("New")]));
        using DecompilationWorkbench workbench = new(service);
        Task pending = workbench.InspectAsync("old");
        await workbench.InspectAsync("new");
        Assert.IsTrue(token.IsCancellationRequested);
        old.SetResult(new AssemblyInspection("old", "Old", "1", [Type("Old")]));
        await pending;
        Assert.AreEqual("new", workbench.Assembly!.AssemblyPath);
        Assert.AreEqual("New", workbench.Types[0].FullName);
    }

    [TestMethod]
    public async Task SelectionChangeAndDisposeRejectLateDecompilation()
    {
        IAssemblyDecompilationService service = Substitute.For<IAssemblyDecompilationService>();
        AssemblyTypeInfo first = Type("First");
        AssemblyTypeInfo second = Type("Second");
        service.InspectAsync("a", Arg.Any<CancellationToken>()).Returns(new AssemblyInspection("a", "A", "1", [first, second]));
        var pendingArtifact = new TaskCompletionSource<DecompiledTypeArtifact>(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken token = default;
        service.DecompileTypeAsync("a", "First", Arg.Any<CancellationToken>()).Returns(call =>
        {
            token = call.Arg<CancellationToken>();
            return pendingArtifact.Task;
        });
        using DecompilationWorkbench workbench = new(service);
        await workbench.InspectAsync("a");
        workbench.SelectType(first);
        Task<CodeLocation?> pending = workbench.DecompileSelectedAsync();
        workbench.SelectType(second);
        Assert.IsTrue(token.IsCancellationRequested);
        workbench.Dispose();
        pendingArtifact.SetResult(new DecompiledTypeArtifact("a", "First", "source.cs", "", new CodeLocation("source.cs")));
        Assert.IsNull(await pending);
    }

    [TestMethod]
    public async Task DecompilationFailureDoesNotReturnLocation()
    {
        IAssemblyDecompilationService service = Substitute.For<IAssemblyDecompilationService>();
        AssemblyTypeInfo type = Type("A");
        service.InspectAsync("a", Arg.Any<CancellationToken>()).Returns(new AssemblyInspection("a", "A", "1", [type]));
        service.DecompileTypeAsync("a", "A", Arg.Any<CancellationToken>()).Returns<Task<DecompiledTypeArtifact>>(_ => throw new IOException("protected"));
        using DecompilationWorkbench workbench = new(service);
        await workbench.InspectAsync("a");
        workbench.SelectType(type);
        Assert.IsNull(await workbench.DecompileSelectedAsync());
        StringAssert.Contains(workbench.Status, "protected");
        Assert.IsFalse(workbench.IsBusy);
    }
}
