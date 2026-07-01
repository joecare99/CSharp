using AA98_AvlnCodeStudio.Diagnostics.Debug.Consumers;
using AA98_AvlnCodeStudio.Diagnostics.Debug.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.DependencyInjection;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AppKomponentBaseLib.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
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
