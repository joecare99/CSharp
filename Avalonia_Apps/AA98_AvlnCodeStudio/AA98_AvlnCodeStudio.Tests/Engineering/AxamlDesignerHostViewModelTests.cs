using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AxamlDesigner.Host.Commands;
using AA98_AvlnCodeStudio.Host.Mcp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerHostViewModelTests
{
    [TestMethod]
    public void HostViewModel_OpenEditSave_WritesDesignerMarkup()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AxamlDesigner-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "View.axaml");
            File.WriteAllText(
                path,
                "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Before' /></StackPanel></UserControl>");
            AxamlDesignerHostViewModel viewModel = new();

            viewModel.OpenDocument(path);
            viewModel.Designer.SelectedNode = viewModel.Designer.PreviewRoot?.Children[0];
            viewModel.Designer.SelectedProperty = "Text";
            viewModel.Designer.SelectedValue = "After";
            viewModel.Designer.ApplySelectedValueCommand.Execute(null);
            viewModel.SaveCommand.Execute(null);

            StringAssert.Contains(File.ReadAllText(path), "Text='After'");
            StringAssert.Contains(viewModel.StatusText, "Saved");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void HostViewModel_TracksDirtyStateAndProtectsCurrentDocumentUntilExplicitDiscard()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AxamlDirty-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string firstPath = Path.Combine(directory, "First.axaml");
            string secondPath = Path.Combine(directory, "Second.axaml");
            File.WriteAllText(firstPath, "<TextBlock Text='First' />");
            File.WriteAllText(secondPath, "<TextBlock Text='Second' />");
            AxamlDesignerHostViewModel viewModel = new();

            Assert.IsTrue(viewModel.OpenDocument(firstPath));
            Assert.IsFalse(viewModel.IsDirty);
            viewModel.Designer.SelectedNode = viewModel.Designer.PreviewRoot;
            viewModel.Designer.SelectedProperty = "Text";
            viewModel.Designer.SelectedValue = "Edited";
            viewModel.Designer.ApplySelectedValueCommand.Execute(null);

            Assert.IsTrue(viewModel.IsDirty);
            StringAssert.Contains(viewModel.WindowTitle, "*");
            Assert.IsFalse(viewModel.OpenDocument(secondPath));
            Assert.AreEqual(Path.GetFullPath(firstPath), viewModel.FilePath);
            StringAssert.Contains(viewModel.GetDesignerStateJson(), "Edited");

            Assert.IsTrue(viewModel.OpenDocument(secondPath, discardUnsavedChanges: true));
            Assert.IsFalse(viewModel.IsDirty);
            StringAssert.Contains(viewModel.GetDesignerStateJson(), "Second");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void HostViewModel_SaveDocumentAsCreatesFileAndClearsDirtyState()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AxamlSaveAs-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string sourcePath = Path.Combine(directory, "Source.axaml");
            string saveAsPath = Path.Combine(directory, "Saved.axaml");
            File.WriteAllText(sourcePath, "<TextBlock Text='Before' />");
            AxamlDesignerHostViewModel viewModel = new();
            Assert.IsTrue(viewModel.OpenDocument(sourcePath));
            viewModel.Designer.SelectedNode = viewModel.Designer.PreviewRoot;
            viewModel.Designer.SelectedProperty = "Text";
            viewModel.Designer.SelectedValue = "After";
            viewModel.Designer.ApplySelectedValueCommand.Execute(null);

            Assert.IsTrue(viewModel.IsDirty);
            Assert.IsTrue(viewModel.SaveDocumentAs(saveAsPath));

            Assert.IsFalse(viewModel.IsDirty);
            Assert.AreEqual(Path.GetFullPath(saveAsPath), viewModel.FilePath);
            StringAssert.Contains(File.ReadAllText(saveAsPath), "Text='After'");
            Assert.IsTrue(viewModel.WindowTitle.EndsWith("Designer", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task McpExports_OpenEditAndSave_OperateOnlyOnTheCurrentAxamlDocument()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AxamlMcp-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string path = Path.Combine(directory, "View.axaml");
            File.WriteAllText(
                path,
                "<UserControl xmlns='https://github.com/avaloniaui'><StackPanel><TextBlock Text='Before' /></StackPanel></UserControl>");
            ServiceCollection services = new();
            services.AddSingleton(new AxamlDesignerHostViewModel());
            services.AddSingleton<ComponentMcpCommandCatalog>();
            services.AddAxamlDesignerMcpExports();
            using ServiceProvider provider = services.BuildServiceProvider();
            ComponentMcpCommandCatalog catalog = provider.GetRequiredService<ComponentMcpCommandCatalog>();
            using JsonDocument openArguments = JsonDocument.Parse(JsonSerializer.Serialize(new { path }));
            await catalog.ExecuteAsync("designer.open", openArguments.RootElement, CancellationToken.None);
            using JsonDocument setPropertyArguments = JsonDocument.Parse(
                """{"nodePath":"0","propertyName":"Text","value":"After"}""");
            await catalog.ExecuteAsync("designer.set_property", setPropertyArguments.RootElement, CancellationToken.None);
            using JsonDocument emptyArguments = JsonDocument.Parse("{}");
            await catalog.ExecuteAsync("designer.save", emptyArguments.RootElement, CancellationToken.None);

            StringAssert.Contains(File.ReadAllText(path), "Text='After'");
            Assert.AreEqual(7, catalog.Descriptors.Count);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
