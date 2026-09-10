using AA98_AvlnCodeStudio.Diagnostics.Debug.Consumers;
using AA98_AvlnCodeStudio.Diagnostics.Debug.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using Code.Navigation;
using Diagnostics.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies reusable diagnostics consumer behavior.
/// </summary>
[TestClass]
public class DiagnosticsConsumerTests
{
    /// <summary>
    /// Verifies that the diagnostics UI consumer exposes the consumed diagnostics collection.
    /// </summary>
    [TestMethod]
    public async Task DiagnosticCollectionViewModel_ConsumesDiagnosticsIntoItems()
    {
        DiagnosticCollectionViewModel viewModel = new();
        Diagnostic diagnostic = new()
        {
            Code = "PLN031",
            Severity = DiagnosticSeverity.Warning,
            Message = "Missing parent.",
        };

        await viewModel.ConsumeAsync(new[] { diagnostic });

        Assert.AreEqual(1, viewModel.Items.Count);
        Assert.AreSame(diagnostic, viewModel.Items[0]);
        Assert.AreEqual("Diagnostics: 1", viewModel.SummaryText);
    }

    [TestMethod]
    public async Task ActivateAsync_NavigableDiagnosticDelegatesOnce()
    {
        ICodeNavigator navigator = Substitute.For<ICodeNavigator>();
        Diagnostic diagnostic = new()
        {
            Code = "PLN001",
            Severity = DiagnosticSeverity.Error,
            Message = "Planning root missing.",
            SourcePath = System.IO.Path.GetFullPath("planning.md"),
            LineNumber = 12,
            ColumnNumber = 4,
        };
        DiagnosticCollectionViewModel viewModel = new(new DiagnosticLocationMapper(), navigator);

        await viewModel.ActivateAsync(diagnostic);

        await navigator.Received(1).NavigateAsync(
            Arg.Is<CodeLocation>(location => location.Line == 12 && location.Column == 4),
            Arg.Any<CancellationToken>());
        Assert.AreEqual(string.Empty, viewModel.ActivationErrorText);
    }

    [TestMethod]
    public async Task ActivateAsync_UnavailableDiagnosticDoesNotNavigate()
    {
        ICodeNavigator navigator = Substitute.For<ICodeNavigator>();
        DiagnosticCollectionViewModel viewModel = new(new DiagnosticLocationMapper(), navigator);

        await viewModel.ActivateAsync(new Diagnostic
        {
            Code = "PLN002",
            Severity = DiagnosticSeverity.Warning,
            Message = "No source location.",
        });

        await navigator.DidNotReceiveWithAnyArgs().NavigateAsync(default!, default);
        StringAssert.Contains(viewModel.ActivationErrorText, "no source path");
    }

    [TestMethod]
    public async Task ActivateAsync_NavigatorFailurePreservesItemsAndExposesError()
    {
        ICodeNavigator navigator = Substitute.For<ICodeNavigator>();
        navigator.NavigateAsync(Arg.Any<CodeLocation>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Editor unavailable.")));
        Diagnostic diagnostic = new()
        {
            Code = "PLN003",
            Severity = DiagnosticSeverity.Error,
            Message = "Broken source.",
            SourcePath = System.IO.Path.GetFullPath("planning.md"),
            LineNumber = 3,
        };
        DiagnosticCollectionViewModel viewModel = new(new DiagnosticLocationMapper(), navigator);
        await viewModel.ConsumeAsync(new[] { diagnostic });

        await viewModel.ActivateAsync(diagnostic);

        Assert.AreEqual(1, viewModel.Items.Count);
        Assert.AreSame(diagnostic, viewModel.Items[0]);
        Assert.AreEqual("Editor unavailable.", viewModel.ActivationErrorText);
    }

    /// <summary>
    /// Verifies that the debug diagnostics consumer produces a stable text format.
    /// </summary>
    [TestMethod]
    public void DebugDiagnosticConsumer_FormatsDiagnosticDeterministically()
    {
        Diagnostic diagnostic = new()
        {
            Code = "PLN001",
            Severity = DiagnosticSeverity.Error,
            Message = "Planning root missing.",
            SourcePath = "DevOps",
            LineNumber = 12,
        };

        string formatted = DebugDiagnosticConsumer.FormatDiagnostic(diagnostic);

        Assert.AreEqual("Error|PLN001|DevOps|12|Planning root missing.", formatted);
    }

    /// <summary>
    /// Verifies that the diagnostics UI registration exposes the collection view model as consumer.
    /// </summary>
    [TestMethod]
    public void AddDiagnosticsUi_RegistersCollectionViewModelAsConsumer()
    {
        ServiceCollection services = new();

        services.AddDiagnosticsUi();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        DiagnosticCollectionViewModel viewModel = serviceProvider.GetRequiredService<DiagnosticCollectionViewModel>();
        IDiagnosticConsumer consumer = serviceProvider.GetRequiredService<IDiagnosticConsumer>();

        Assert.AreSame(viewModel, consumer);
    }

    /// <summary>
    /// Verifies that multiple diagnostics consumers can be registered for the same contract.
    /// </summary>
    [TestMethod]
    public void AddDiagnosticsUiAndDebugDiagnostics_RegistersMultipleConsumers()
    {
        ServiceCollection services = new();

        services.AddDiagnosticsUi();
        services.AddDebugDiagnostics();

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        IDiagnosticConsumer[] consumers = serviceProvider.GetServices<IDiagnosticConsumer>().ToArray();

        Assert.IsTrue(consumers.OfType<DiagnosticCollectionViewModel>().Any());
        Assert.IsTrue(consumers.OfType<DebugDiagnosticConsumer>().Any());
    }
}
