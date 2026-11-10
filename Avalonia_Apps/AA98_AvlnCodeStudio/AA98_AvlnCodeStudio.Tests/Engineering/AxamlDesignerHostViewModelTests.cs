using AA98_AxamlDesigner.Host.ViewModels;
using AA98_AxamlDesigner.Host.Commands;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Host.Mcp;
using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlDesignerHostViewModelTests
{
    [TestMethod]
    public void Host_CodeAndDesignerShareBufferAndUnappliedDraftsProtectDocument()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AxamlHostBuffer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string first = Path.Combine(directory, "First.axaml");
            string second = Path.Combine(directory, "Second.axaml");
            Encoding encoding = new UnicodeEncoding(false, true, true);
            File.WriteAllBytes(first, AA98_AvlnCodeStudio.Model.Documents.TextDocumentEncoding.Encode(
                "<TextBox Width='100' Text='Before' />", encoding));
            File.WriteAllText(second, "<TextBox Text='Other' />");
            AxamlDesignerHostViewModel host = new();
            Assert.IsTrue(host.OpenDocument(first));
            host.Markup = "<TextBox Width='100' Text='Code edit' />";
            Assert.AreEqual("Code edit", host.Designer.PreviewRoot!.Properties["Text"]);
            host.Designer.SelectedNode = host.Designer.PreviewRoot;
            host.Designer.SelectedProperty = "Text";
            host.Designer.SelectedValue = "Designer edit";
            host.Designer.ApplySelectedValueCommand.Execute(null);
            StringAssert.Contains(host.Markup, "Designer edit");
            Assert.IsTrue(host.SaveDocument());
            byte[] saved = File.ReadAllBytes(first);
            Assert.IsTrue(saved.AsSpan().StartsWith(encoding.Preamble));
            Assert.IsFalse(host.IsDirty);

            AxamlPropertyRowViewModel width = host.Designer.PropertyRows.Single(row => row.Name == "Width");
            Assert.IsFalse(width.TrySetValue("-2"));
            Assert.IsTrue(host.HasUnsavedChanges);
            Assert.IsFalse(host.OpenDocument(second));
            Assert.AreEqual("-2", width.Value);
            Assert.IsTrue(host.OpenDocument(second, discardUnsavedChanges: true));
            Assert.IsFalse(host.HasUnsavedChanges);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

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
    public void HostViewModel_RequiresExplicitWorkspaceContextAndClearsItWhenDocumentLeavesScope()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AxamlWorkspaceContext-{Guid.NewGuid():N}");
        string other = Path.Combine(Path.GetTempPath(), $"AxamlWorkspaceOther-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(other);
        try
        {
            string inWorkspacePath = Path.Combine(root, "View.axaml");
            string outsidePath = Path.Combine(other, "Other.axaml");
            File.WriteAllText(inWorkspacePath, "<TextBlock />");
            File.WriteAllText(outsidePath, "<TextBlock />");
            AxamlDesignerHostViewModel viewModel = new();

            Assert.AreEqual(string.Empty, viewModel.WorkspaceRootPath);
            StringAssert.Contains(viewModel.WorkspaceContextStatus, "unavailable");
            Assert.IsTrue(viewModel.OpenDocument(inWorkspacePath));
            Assert.IsTrue(viewModel.SetWorkspaceContext(root));
            Assert.AreEqual(Path.GetFullPath(root), viewModel.WorkspaceRootPath);
            WorkspaceFileChangeProposal proposal = viewModel.PrepareWorkspaceChanges(
                [WorkspaceFileChangeRequest.CreateNew("Generated.txt", "safe workspace change")]);
            viewModel.ApplyWorkspaceChanges(proposal, confirmed: true);
            Assert.AreEqual("safe workspace change", File.ReadAllText(Path.Combine(root, "Generated.txt")));
            using (JsonDocument state = JsonDocument.Parse(viewModel.GetDesignerStateJson()))
            {
                Assert.AreEqual(
                    Path.GetFullPath(root),
                    state.RootElement.GetProperty("WorkspaceRootPath").GetString());
            }

            Assert.IsFalse(viewModel.SetWorkspaceContext(other));
            Assert.AreEqual(Path.GetFullPath(root), viewModel.WorkspaceRootPath);
            Assert.IsTrue(viewModel.OpenDocument(outsidePath));
            Assert.AreEqual(string.Empty, viewModel.WorkspaceRootPath);
            StringAssert.Contains(viewModel.WorkspaceContextStatus, "No project/workspace selected");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
            Directory.Delete(other, recursive: true);
        }
    }

    [TestMethod]
    public void HostViewModel_WorkspaceUndoRedoSynchronizesOpenDocumentAndClearsHistoryOnContextChange()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AxamlWorkspaceUndo-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string original = "<TextBlock Text='Before' />";
            const string updated = "<TextBlock Text='After' />";
            File.WriteAllText(path, original);
            AxamlDesignerHostViewModel viewModel = new();
            Assert.IsTrue(viewModel.OpenDocument(path));
            Assert.IsTrue(viewModel.SetWorkspaceContext(root));

            WorkspaceFileChangeProposal proposal = viewModel.PrepareWorkspaceChanges(
                [WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", original, updated)]);
            viewModel.ApplyWorkspaceChanges(proposal, confirmed: true);

            Assert.IsTrue(viewModel.UndoWorkspaceChangeCommand.CanExecute(null));
            Assert.AreEqual(updated, File.ReadAllText(path));
            Assert.IsFalse(viewModel.IsDirty);
            viewModel.UndoWorkspaceChangeCommand.Execute(null);
            Assert.AreEqual(original, File.ReadAllText(path));
            StringAssert.Contains(viewModel.GetDesignerStateJson(), "Before");

            Assert.IsTrue(viewModel.RedoWorkspaceChangeCommand.CanExecute(null));
            viewModel.RedoWorkspaceChangeCommand.Execute(null);
            Assert.AreEqual(updated, File.ReadAllText(path));
            StringAssert.Contains(viewModel.GetDesignerStateJson(), "After");

            viewModel.ClearWorkspaceContext();
            Assert.IsFalse(viewModel.UndoWorkspaceChangeCommand.CanExecute(null));
            Assert.IsFalse(viewModel.RedoWorkspaceChangeCommand.CanExecute(null));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task HostViewModel_CreatesMissingBindingMemberOnlyAfterWorkspaceReview()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AxamlHostMvvm-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            string axamlPath = Path.Combine(root, "View.axaml");
            string viewModelPath = Path.Combine(root, "ViewModel.cs");
            const string markup = """
                <UserControl xmlns="https://github.com/avaloniaui"
                        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                        xmlns:vm="using:Sample"
                        x:Class="Sample.View"
                        x:DataType="vm:ViewModel">
                  <StackPanel><TextBox /></StackPanel>
                </UserControl>
                """;
            File.WriteAllText(
                projectPath,
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup>
                  <ItemGroup><PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.2" /></ItemGroup>
                </Project>
                """);
            File.WriteAllText(
                Path.Combine(root, "View.axaml.cs"),
                """
                namespace Sample;
                public partial class View
                {
                    public object? DataContext { get; set; }
                    public View() => DataContext = new ViewModel();
                }
                """);
            File.WriteAllText(
                viewModelPath,
                """
                using CommunityToolkit.Mvvm.ComponentModel;
                namespace Sample;
                public partial class ViewModel : ObservableObject
                {
                }
                """);
            File.WriteAllText(axamlPath, markup);
            ProcessStartInfo buildStartInfo = new("dotnet")
            {
                WorkingDirectory = AppContext.BaseDirectory,
                ArgumentList = { "build", projectPath, "--verbosity", "quiet" },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            buildStartInfo.Environment.Remove("MSBuildSDKsPath");
            buildStartInfo.Environment.Remove("MSBuildExtensionsPath");
            buildStartInfo.Environment.Remove("MSBuildExtensionsPath32");
            buildStartInfo.Environment.Remove("MSBuildToolsPath");
            buildStartInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR");
            buildStartInfo.Environment.Remove("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
            using Process build = Process.Start(buildStartInfo)
                ?? throw new InvalidOperationException("Could not start the standalone ViewModel fixture build.");
            Task<string> buildOutput = build.StandardOutput.ReadToEndAsync();
            Task<string> buildError = build.StandardError.ReadToEndAsync();
            await build.WaitForExitAsync();
            Assert.AreEqual(0, build.ExitCode, $"{await buildOutput}{await buildError}");

            AxamlDesignerHostViewModel viewModel = new();
            Assert.IsTrue(viewModel.OpenDocument(axamlPath));
            Assert.IsTrue(viewModel.SetWorkspaceEntry(projectPath));
            viewModel.Designer.SetBindingContext([]);
            viewModel.Designer.SelectedNode = viewModel.Designer.PreviewRoot?.Children.Single();
            AxamlBindingRowViewModel binding = viewModel.Designer.BindingRows.Single();
            binding.NewMemberName = "Caption";

            WorkspaceFileChangeProposal? proposal = null;
            viewModel.ReviewWorkspaceFileChangesAsync = (candidate, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                proposal = candidate;
                viewModel.WorkspaceFileChangeService.Apply(candidate, confirmed: true);
                return Task.FromResult(true);
            };

            await viewModel.Designer.ApplyBindingRowAsync(binding);

            Assert.IsNotNull(proposal, $"{binding.Diagnostic} {viewModel.StatusText}");
            Assert.AreEqual(2, proposal!.Changes.Count);
            StringAssert.Contains(File.ReadAllText(axamlPath), "{Binding Caption, Mode=TwoWay}");
            StringAssert.Contains(File.ReadAllText(viewModelPath), "private string _caption = string.Empty;");

            Assert.IsTrue(viewModel.WorkspaceFileChangeService.Undo());
            Assert.AreEqual(markup, File.ReadAllText(axamlPath));
            Assert.IsFalse(File.ReadAllText(viewModelPath).Contains("_caption", StringComparison.Ordinal));
            Assert.IsTrue(viewModel.WorkspaceFileChangeService.Redo());
            StringAssert.Contains(File.ReadAllText(axamlPath), "{Binding Caption, Mode=TwoWay}");
            StringAssert.Contains(File.ReadAllText(viewModelPath), "private string _caption = string.Empty;");
            Assert.IsFalse(viewModel.IsDirty);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
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
            services.AddAxamlDesignerMcpExports((action, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(action());
            });
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
