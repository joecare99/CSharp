using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Base.UI.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Navigation;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using Avalonia.Controls;
using Avln_CommonDialogs.Base.Interfaces;
using Code.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Property.Editor;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class EditorDocumentWorkspaceTests
{
    [TestMethod]
    public void EditorRegistry_UsesPriorityThenStableIdRegardlessOfRegistrationOrder()
    {
        RegistryFactory lower = new("lower", 1);
        RegistryFactory laterId = new("zeta", 2);
        RegistryFactory earlierId = new("alpha", 2);

        DocumentEditorRegistry<string> first = new([lower, laterId, earlierId]);
        DocumentEditorRegistry<string> second = new([earlierId, lower, laterId]);
        Assert.AreSame(earlierId, first.FindFactory("file.cs"));
        Assert.AreSame(earlierId, second.FindFactory("file.cs"));
        Assert.IsNull(first.FindFactory("file.unknown"));
        Assert.ThrowsExactly<ArgumentException>(() =>
            new DocumentEditorRegistry<string>([earlierId, new RegistryFactory("alpha", 3)]));
    }

    [TestMethod]
    public async Task EditorActivation_CancelsExplorerPreviewStillWaitingForDebounce()
    {
        string root = Path.GetTempPath();
        ProjectExplorerItem item = new("file", "File.cs", Path.Combine(root, "File.cs"), ProjectExplorerItemKind.File);
        RecordingPreviewOpener opener = new();
        using ProjectExplorerViewModel explorer = new(new TestExplorerSource(item), new ProjectExplorerState(), opener);
        TestEditorComponentFactory components = new();
        IAvaloniaEditorComponent initial = components.Create();
        EditorDocumentWorkspace workspace = new(initial, components);
        _ = new MainWindowViewModel(
            initial,
            new PlanningExplorerViewModel(new MarkdownPlanningReader()),
            new DiagnosticCollectionViewModel(),
            null,
            explorer,
            editorWorkspace: workspace);

        explorer.SelectedItem = explorer.RootItems.Single();
        workspace.ActiveTab.ActivateByUser();
        await Task.Delay(400);

        Assert.IsFalse(opener.Previewed.Task.IsCompleted, "A pending explorer preview must not override a newer editor activation.");
    }

    [TestMethod]
    public async Task ExternalDocumentOpen_CancelsExplorerPreviewStillWaitingForDebounce()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ExternalNavigation_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Opened.cs");
            await File.WriteAllTextAsync(path, "class Opened {}");
            ProjectExplorerItem item = new("file", "Other.cs", Path.Combine(root, "Other.cs"), ProjectExplorerItemKind.File);
            RecordingPreviewOpener opener = new();
            using ProjectExplorerViewModel explorer = new(new TestExplorerSource(item), new ProjectExplorerState(), opener);
            TestEditorComponentFactory components = new();
            IAvaloniaEditorComponent initial = components.Create();
            EditorDocumentWorkspace workspace = new(initial, components);
            _ = new MainWindowViewModel(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                explorer,
                editorWorkspace: workspace);

            explorer.SelectedItem = explorer.RootItems.Single();
            await workspace.OpenPreviewAsync(path);
            await Task.Delay(400);

            Assert.AreEqual(path, workspace.ActiveTab.FilePath);
            Assert.IsFalse(opener.Previewed.Task.IsCompleted, "A later document open must supersede the pending explorer preview.");
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task ExplorerFolderSelection_RejectsAnOlderInFlightDocumentPreview()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_FolderNavigation_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Slow.cs");
            await File.WriteAllTextAsync(path, "class Slow {}");
            DelayedReadStorage storage = new(path);
            TestEditorComponentFactory components = new(storage);
            IAvaloniaEditorComponent initial = components.Create();
            EditorDocumentWorkspace workspace = new(initial, components);
            ProjectExplorerItem folder = new("folder", "Folder", root, ProjectExplorerItemKind.Folder);
            using ProjectExplorerViewModel explorer = new(new TestExplorerSource(folder), new ProjectExplorerState(), new RecordingPreviewOpener());
            _ = new MainWindowViewModel(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                explorer,
                editorWorkspace: workspace);

            Task<IEditorDocument?> slowOpen = workspace.OpenPreviewAsync(path);
            await storage.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
            explorer.SelectedItem = explorer.RootItems.Single();
            storage.Continue.TrySetResult();

            Assert.IsNull(await slowOpen);
            Assert.IsNull(workspace.FindByPath(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task EditorContributions_AreHostedWithoutAnAxamlSpecificShellAndDisposedWithTheirTab()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_Contributions_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.custom");
            await File.WriteAllTextAsync(path, "custom editor data");
            TestEditorComponentFactory components = new();
            IAvaloniaEditorComponent initial = components.Create();
            IPropertyProvider provider = Substitute.For<IPropertyProvider>();
            provider.Context.Returns(new PropertyProviderContext(PropertyScope.Session, "Custom", "First target"));
            provider.Properties.Returns(Array.Empty<IPropertyItem>());
            TestContributions contributions = new(provider, extendedProperties: true);
            EditorDocumentWorkspace workspace = new(initial, components, [new TestDocumentFactory(components, contributions)]);
            MainWindowViewModel workbench = new(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                editorWorkspace: workspace);

            await workspace.OpenPreviewAsync(path);
            EditorTabViewModel tab = workspace.ActiveTab;
            Assert.AreEqual("custom", tab.EditorKind);
            Assert.AreSame(contributions.PaletteView, tab.PaletteView);
            tab.ActivateByUser(designerSelection: true);
            Assert.AreSame(contributions.PropertiesView, workbench.SelectedEditorPropertyView);
            StringAssert.Contains(workbench.PropertySelectionCaption, "First target");
            provider.Context.Returns(new PropertyProviderContext(PropertyScope.Session, "Custom", "Second target"));
            contributions.NotifySelectionChanged();
            StringAssert.Contains(workbench.PropertySelectionCaption, "Second target");

            contributions.SetDraft(true);
            Assert.IsTrue(tab.IsProtected);
            Assert.IsFalse(workspace.TryClose(tab));
            workspace.CreateNewDocument();
            Assert.IsNull(workbench.SelectedEditorPropertyView);
            Assert.IsTrue(workspace.TryClose(tab, discardProtected: true));
            Assert.IsTrue(contributions.IsDisposed);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task EditorContributions_StandardPropertyProviderCanApplyAndDiscard()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_StandardProperties_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.custom");
            await File.WriteAllTextAsync(path, "custom");
            TestEditorComponentFactory components = new();
            IAvaloniaEditorComponent initial = components.Create();
            IPropertyProvider provider = Substitute.For<IPropertyProvider>();
            provider.Context.Returns(new PropertyProviderContext(PropertyScope.Session, "Custom", "Selected object"));
            provider.Properties.Returns(Array.Empty<IPropertyItem>());
            TestContributions contributions = new(provider, extendedProperties: false);
            EditorDocumentWorkspace workspace = new(initial, components, [new TestDocumentFactory(components, contributions)]);
            MainWindowViewModel workbench = new(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                editorWorkspace: workspace);

            await workspace.OpenPreviewAsync(path);
            workspace.ActiveTab.ActivateByUser(designerSelection: true);
            Assert.IsTrue(workbench.HasStandardEditorPropertySelection);
            await workbench.ApplyEditorPropertiesCommand.ExecuteAsync(null);
            await workbench.DiscardEditorPropertiesCommand.ExecuteAsync(null);
            await provider.Received(1).ApplyAsync(Arg.Any<CancellationToken>());
            await provider.Received(1).DiscardAsync(Arg.Any<CancellationToken>());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task BinaryEditor_UsesItsOwnDocumentAndContributionsWithoutTextConversion()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinaryEditor_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Image.bin");
            string textPath = Path.Combine(root, "Source.cs");
            byte[] original = [0x00, 0xff, 0x42];
            byte[] edited = [0xfe, 0x00, 0x24];
            await File.WriteAllBytesAsync(path, original);
            await File.WriteAllTextAsync(textPath, "class Source {}");
            TestEditorComponentFactory components = new();
            IAvaloniaEditorComponent initial = components.Create();
            IPropertyProvider provider = Substitute.For<IPropertyProvider>();
            provider.Context.Returns(new PropertyProviderContext(PropertyScope.Session, "Image", "Pixel"));
            provider.Properties.Returns(Array.Empty<IPropertyItem>());
            TestContributions contributions = new(provider, extendedProperties: true);
            BinaryDocumentFactory binaryFactory = new() { Contributions = contributions };
            EditorDocumentWorkspace workspace = new(initial, components, [binaryFactory]);
            MainWindowViewModel workbench = new(
                initial,
                new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(),
                null,
                null,
                editorWorkspace: workspace);

            Assert.IsNotNull(await workspace.OpenPreviewAsync(path));
            EditorTabViewModel tab = workspace.ActiveTab;
            BinaryDocumentSession session = binaryFactory.Sessions.Single();
            Assert.IsTrue(tab.IsPreview);
            Assert.IsFalse(tab.HasTextEditor);
            Assert.AreSame(binaryFactory.Views.Single(), tab.View);
            Assert.AreSame(contributions.PaletteView, tab.PaletteView);
            CollectionAssert.AreEqual(original, session.Content);
            Assert.AreEqual(string.Empty, tab.Editor.Text);
            Assert.IsFalse(workspace.CanApplyChange(path, string.Empty, string.Empty));
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                workspace.ApplyChange(path, string.Empty, string.Empty, "text", "text"));

            bool pinnedBeforeMutation = false;
            session.ContentChanging += (_, _) => pinnedBeforeMutation = !tab.IsPreview;
            string originalWindowTitle = workbench.WindowTitle;
            session.ReplaceContent(edited);
            Assert.IsTrue(pinnedBeforeMutation);
            Assert.IsTrue(tab.IsProtected);
            Assert.AreNotEqual(originalWindowTitle, workbench.WindowTitle);
            contributions.SetSelection(null);
            tab.ActivateByUser(designerSelection: true);
            Assert.IsFalse(workbench.HasEditorPropertyView);
            StringAssert.Contains(workbench.PropertySelectionCaption, "no selection");
            contributions.SetSelection(provider);
            Assert.AreSame(contributions.PropertiesView, workbench.SelectedEditorPropertyView);
            StringAssert.Contains(workbench.PropertySelectionCaption, "Pixel");
            CollectionAssert.AreEqual(original, await File.ReadAllBytesAsync(path));

            await tab.SaveCommand.ExecuteAsync(null);
            CollectionAssert.AreEqual(edited, await File.ReadAllBytesAsync(path));
            Assert.IsFalse(tab.IsDirty);
            Assert.IsNotNull(await workspace.OpenPreviewAsync(textPath));
            Assert.AreEqual("class Source {}", workspace.ActiveTab.Editor.Text);
            Assert.AreSame(tab, workspace.FindByPath(path));
            Assert.IsTrue(workspace.TryClose(tab));
            Assert.IsTrue(session.IsDisposed);
            Assert.IsTrue(contributions.IsDisposed);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task BinaryEditor_SaveAsCoordinatesOpenTargetsAndConcurrentWrites()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinarySave_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.bin");
            string second = Path.Combine(root, "Second.bin");
            string destination = Path.Combine(root, "Saved.bin");
            await File.WriteAllBytesAsync(first, [0x01, 0x00]);
            await File.WriteAllBytesAsync(second, [0x02, 0x00]);
            TestEditorComponentFactory components = new();
            BinaryDocumentFactory binaryFactory = new();
            EditorDocumentWorkspace workspace = new(components.Create(), components, [binaryFactory]);
            await workspace.OpenOrActivateAsync(first);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            BinaryDocumentSession firstSession = binaryFactory.Sessions[0];
            await workspace.OpenOrActivateAsync(second);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            BinaryDocumentSession secondSession = binaryFactory.Sessions[1];
            firstSession.ReplaceContent([0x11, 0x00]);

            binaryFactory.SelectSavePathAsync = _ => Task.FromResult<string?>(second);
            await firstTab.SaveAsCommand.ExecuteAsync(null);
            StringAssert.Contains(firstTab.Editor.NotificationText, "already open");
            CollectionAssert.AreEqual(new byte[] { 0x02, 0x00 }, await File.ReadAllBytesAsync(second));
            Assert.AreSame(firstTab, workspace.FindByPath(first));
            Assert.IsTrue(firstTab.IsDirty);

            TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
            firstSession.BeforeWriteAsync = async cancellationToken =>
            {
                started.TrySetResult();
                await release.Task.WaitAsync(cancellationToken);
            };
            binaryFactory.SelectSavePathAsync = _ => Task.FromResult<string?>(destination);
            Task save = firstTab.SaveAsCommand.ExecuteAsync(null);
            try
            {
                await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
                Assert.IsTrue(workspace.HasInProgressSaves);
                Assert.IsFalse(workspace.TryClose(firstTab, discardProtected: true));
                await Assert.ThrowsExactlyAsync<IOException>(() => workspace.OpenPreviewAsync(destination));
                secondSession.ReplaceContent([0x22, 0x00]);
                await secondTab.SaveAsCommand.ExecuteAsync(null);
                StringAssert.Contains(secondTab.Editor.NotificationText, "already being saved");
                firstSession.ReplaceContent([0x33, 0x00]);
            }
            finally
            {
                release.TrySetResult();
                await save;
            }

            Assert.IsFalse(workspace.HasInProgressSaves);
            Assert.AreSame(firstTab, workspace.FindByPath(destination));
            Assert.AreSame(secondTab, workspace.FindByPath(second));
            CollectionAssert.AreEqual(new byte[] { 0x11, 0x00 }, await File.ReadAllBytesAsync(destination));
            Assert.IsTrue(firstTab.IsDirty, "An edit made during the write must remain dirty.");
            firstSession.BeforeWriteAsync = null;
            await firstTab.SaveCommand.ExecuteAsync(null);
            CollectionAssert.AreEqual(new byte[] { 0x33, 0x00 }, await File.ReadAllBytesAsync(destination));
            Assert.IsFalse(firstTab.IsDirty);
            CollectionAssert.AreEqual(new byte[] { 0x01, 0x00 }, await File.ReadAllBytesAsync(first));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task BinaryEditor_SaveAsSelectionProtectsTheTabUntilCanceled()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinaryClose_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Image.bin");
            string destination = Path.Combine(root, "Saved.bin");
            string nextPath = Path.Combine(root, "Next.cs");
            await File.WriteAllBytesAsync(path, [0x00, 0xff]);
            await File.WriteAllTextAsync(nextPath, "class Next {}");
            TaskCompletionSource<string?> selected = new(TaskCreationOptions.RunContinuationsAsynchronously);
            TestEditorComponentFactory components = new();
            BinaryDocumentFactory binaryFactory = new() { SelectSavePathAsync = _ => selected.Task };
            EditorDocumentWorkspace workspace = new(components.Create(), components, [binaryFactory]);
            await workspace.OpenPreviewAsync(path);
            EditorTabViewModel tab = workspace.ActiveTab;
            BinaryDocumentSession session = binaryFactory.Sessions.Single();

            Task save = tab.SaveAsCommand.ExecuteAsync(null);
            Assert.IsTrue(workspace.HasInProgressSaves);
            Assert.IsFalse(workspace.TryClose(tab, discardProtected: true));
            await workspace.OpenPreviewAsync(nextPath);
            Assert.IsFalse(tab.IsPreview);
            Assert.AreSame(tab, workspace.FindByPath(path));
            selected.TrySetResult(null);
            await save;

            Assert.IsFalse(workspace.HasInProgressSaves);
            Assert.IsTrue(workspace.TryClose(tab, discardProtected: true));
            Assert.IsTrue(session.IsDisposed);
            Assert.IsFalse(File.Exists(destination));
            CollectionAssert.AreEqual(new byte[] { 0x00, 0xff }, await File.ReadAllBytesAsync(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task OpenDialog_DefaultsToAllFilesForRegisteredEditors()
    {
        IOpenFileDialog openDialog = Substitute.For<IOpenFileDialog>();
        openDialog.MutableFilters.Returns(new List<FileTypeFilter>());
        openDialog.ShowAsync().Returns(ValueTask.FromResult<IReadOnlyList<string>>(["Image.bin"]));
        ISaveFileDialog saveDialog = Substitute.For<ISaveFileDialog>();
        AvaloniaEditorFileDialogService service = new(openDialog, saveDialog);

        Assert.AreEqual("Image.bin", await service.ShowOpenFileDialogAsync(null));
        Assert.AreEqual("Open file", openDialog.Title);
        Assert.AreEqual("All files", openDialog.MutableFilters[0].Name);
        CollectionAssert.Contains(openDialog.MutableFilters[0].Patterns.ToArray(), "*");
        Assert.AreEqual("Text documents", openDialog.MutableFilters[1].Name);
    }

    [TestMethod]
    public async Task BinaryEditor_RegistersThroughHostAndSupportsExplorerActivation()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinaryHost_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Image.bin");
            await File.WriteAllBytesAsync(path, [0xff, 0x00, 0xfe]);
            TestEditorComponentFactory components = new();
            BinaryDocumentFactory binaryFactory = new();
            ServiceCollection services = new();
            AA98_AvlnCodeStudio.UI.App.ConfigureServices(
                services, () => null, root, Path.Combine(root, "system"), Path.Combine(root, "group"), "test");
            services.AddSingleton<IAvaloniaEditorComponentFactory>(components);
            services.AddSingleton<IAvaloniaEditorComponent>(components.Create());
            services.AddSingleton<IDocumentEditorFactory<EditorTabViewModel>>(binaryFactory);
            using ServiceProvider provider = services.BuildServiceProvider();
            EditorDocumentWorkspace workspace = provider.GetRequiredService<EditorDocumentWorkspace>();
            ICodeNavigator navigator = new EditorCodeNavigator(
                new WorkbenchEditorDocumentHost(() => workspace),
                new WorkbenchEditorCaretService(() => workspace));
            CodeNavigatorProjectExplorerItemOpener opener = new(navigator, () => workspace);
            ProjectExplorerItem item = new("image", "Image.bin", path, ProjectExplorerItemKind.File);

            await opener.PreviewAsync(item);
            EditorTabViewModel tab = workspace.ActiveTab;
            Assert.IsTrue(tab.IsPreview);
            await opener.ActivateAsync(item);
            Assert.IsFalse(tab.IsPreview);
            Assert.AreSame(tab, workspace.ActiveTab);
            Assert.AreEqual(1, binaryFactory.Sessions.Count);
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
                navigator.NavigateAsync(new CodeLocation(path, line: 1)));
            provider.Dispose();
            Assert.IsTrue(binaryFactory.Sessions.Single().IsDisposed);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task BinaryEditor_RejectsAnIncorrectDocumentIdentity()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinaryIdentity_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Image.bin");
            byte[] original = [0xff, 0x00, 0x12];
            await File.WriteAllBytesAsync(path, original);
            TestEditorComponentFactory components = new();
            BinaryDocumentFactory binaryFactory = new()
            {
                ConfigureSession = session => session.OpenedPathOverride = Path.Combine(root, "Other.bin"),
            };
            using EditorDocumentWorkspace workspace = new(components.Create(), components, [binaryFactory]);

            Assert.IsNull(await workspace.OpenPreviewAsync(path));
            Assert.IsTrue(workspace.ActiveTab.IsDiagnostic);
            Assert.AreEqual(path, workspace.ActiveTab.FilePath);
            CollectionAssert.AreEqual(original, await File.ReadAllBytesAsync(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task BinaryEditor_DisposingWorkspaceRejectsALateLoad()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_BinaryLateLoad_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "Image.bin");
            await File.WriteAllBytesAsync(path, [0x00, 0xff]);
            TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
            TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
            TestEditorComponentFactory components = new();
            BinaryDocumentFactory binaryFactory = new()
            {
                ConfigureSession = session => session.BeforeReadAsync = async cancellationToken =>
                {
                    started.TrySetResult();
                    await release.Task.WaitAsync(cancellationToken);
                },
            };
            EditorDocumentWorkspace workspace = new(components.Create(), components, [binaryFactory]);
            Task<IEditorDocument?> open = workspace.OpenPreviewAsync(path);
            await started.Task.WaitAsync(TimeSpan.FromSeconds(10));
            workspace.Dispose();
            release.TrySetResult();

            await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => open);
            Assert.IsTrue(binaryFactory.Sessions.Single().IsDisposed);
            Assert.AreEqual(0, workspace.Tabs.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task SaveAs_RejectsAnOpenDestinationAndPreservesBothDocuments()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_SaveConflict_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.cs");
            string second = Path.Combine(root, "Second.cs");
            await File.WriteAllTextAsync(first, "first");
            await File.WriteAllTextAsync(second, "second");
            IEditorFileDialogService dialog = Substitute.For<IEditorFileDialogService>();
            dialog.ShowSaveFileDialogAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(second));
            TestEditorComponentFactory factory = new(dialog: dialog);
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);
            await workspace.OpenOrActivateAsync(first);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            firstTab.Editor.Text = "unsaved first";
            await workspace.OpenOrActivateAsync(second);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            workspace.Activate(firstTab);

            await firstTab.Editor.SaveAsCommand.ExecuteAsync(null);
            StringAssert.Contains(firstTab.Editor.NotificationText, "already open");
            Assert.AreSame(firstTab, workspace.FindByPath(first));
            Assert.AreSame(secondTab, workspace.FindByPath(second));
            Assert.AreEqual("unsaved first", firstTab.Editor.Text);
            Assert.AreEqual("second", await File.ReadAllTextAsync(second));
            Assert.IsTrue(firstTab.Editor.IsDirty);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task SaveAs_ReservesDestinationUntilTheWriteFinishes()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_SaveRace_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.cs");
            string second = Path.Combine(root, "Second.cs");
            string destination = Path.Combine(root, "New.cs");
            await File.WriteAllTextAsync(first, "first");
            await File.WriteAllTextAsync(second, "second");
            IEditorFileDialogService dialog = Substitute.For<IEditorFileDialogService>();
            dialog.ShowSaveFileDialogAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(destination));
            DelayedWriteStorage storage = new();
            TestEditorComponentFactory factory = new(storage, dialog);
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);
            await workspace.OpenOrActivateAsync(first);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            await workspace.OpenOrActivateAsync(second);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            Task firstSave = firstTab.Editor.SaveAsCommand.ExecuteAsync(null);
            try
            {
                await storage.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
                Assert.IsTrue(workspace.HasInProgressSaves);
                Assert.IsFalse(workspace.TryClose(firstTab, discardProtected: true));
                await Assert.ThrowsExactlyAsync<IOException>(() => workspace.OpenPreviewAsync(destination));
                await secondTab.Editor.SaveAsCommand.ExecuteAsync(null);
                StringAssert.Contains(secondTab.Editor.NotificationText, "already being saved");
            }
            finally
            {
                storage.Continue.TrySetResult();
                await firstSave;
            }

            Assert.IsFalse(workspace.HasInProgressSaves);
            Assert.AreSame(firstTab, workspace.FindByPath(destination));
            Assert.AreSame(secondTab, workspace.FindByPath(second));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task SaveAs_RequiresResolvingAxamlDraftsBeforeChangingFileType()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_DraftSaveAs_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string axaml = Path.Combine(root, "View.axaml");
            string destination = Path.Combine(root, "View.txt");
            await File.WriteAllTextAsync(axaml, "<TextBox Width='100' />");
            IEditorFileDialogService dialog = Substitute.For<IEditorFileDialogService>();
            dialog.ShowSaveFileDialogAsync(Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<string?>(destination));
            TestEditorComponentFactory factory = new(dialog: dialog);
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);
            await workspace.OpenOrActivateAsync(axaml);
            EditorTabViewModel tab = workspace.ActiveTab;
            tab.Designer!.SelectedNode = tab.Designer.PreviewRoot;
            AxamlPropertyRowViewModel width = tab.Designer.PropertyRows.Single(row => row.Name == "Width");
            Assert.IsFalse(width.TrySetValue("-2"));

            await tab.Editor.SaveAsCommand.ExecuteAsync(null);
            StringAssert.Contains(tab.Editor.NotificationText, "property drafts");
            Assert.IsFalse(File.Exists(destination));
            Assert.AreEqual("-2", width.Value);
            Assert.AreSame(tab, workspace.FindByPath(axaml));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task AxamlTabs_KeepIndependentDesignerAndDraftStateWithOneSharedBufferEach()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_AxamlTabs_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.axaml");
            string second = Path.Combine(root, "Second.axaml");
            await File.WriteAllTextAsync(first, "<TextBox Width='100' Text='First' />");
            await File.WriteAllTextAsync(second, "<TextBox Width='200' Text='Second' />");
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);

            await workspace.OpenPreviewAsync(first);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            AxamlDesignerViewModel firstDesigner = firstTab.Designer!;
            firstDesigner.SelectedNode = firstDesigner.PreviewRoot;
            AxamlPropertyRowViewModel width = firstDesigner.PropertyRows.Single(row => row.Name == "Width");
            Assert.IsFalse(width.TrySetValue("-2"));
            Assert.IsFalse(firstTab.IsPreview);
            Assert.IsTrue(firstTab.IsProtected);

            await workspace.OpenPreviewAsync(second);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            Assert.AreNotSame(firstDesigner, secondTab.Designer);
            secondTab.Editor.Text = "<TextBox Width='250' Text='Changed' />";
            Assert.AreEqual("250", secondTab.Designer!.PreviewRoot!.Properties["Width"]);
            secondTab.Designer.SelectedNode = secondTab.Designer.PreviewRoot;
            secondTab.Designer.SelectedProperty = "Text";
            secondTab.Designer.SelectedValue = "Designer edit";
            secondTab.Designer.ApplySelectedValueCommand.Execute(null);
            StringAssert.Contains(secondTab.Editor.Text, "Designer edit");

            workspace.Activate(firstTab);
            Assert.AreEqual("-2", firstDesigner.PropertyRows.Single(row => row.Name == "Width").Value);
            Assert.AreEqual("<TextBox Width='100' Text='First' />", firstTab.Editor.Text);
            Assert.IsFalse(workspace.TryClose(firstTab));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public void EditorContributions_CreatePaletteOnlyWhenRequested()
    {
        TestEditorComponentFactory factory = new();
        IDocumentEditorContributions<Control> contributions = Substitute.For<IDocumentEditorContributions<Control>>();
        Control palette = new Border();
        contributions.PaletteView.Returns(palette);
        using EditorTabViewModel tab = new(factory.Create(), "custom", contributions);

        _ = contributions.DidNotReceive().PaletteView;
        Assert.AreSame(palette, tab.PaletteView);
        _ = contributions.Received(1).PaletteView;
    }

    [TestMethod]
    public async Task Preview_ProgrammaticActivationDoesNotPinButUserActivationDoes()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_Activation_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.cs");
            await File.WriteAllTextAsync(path, "class View {}");
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);
            await workspace.OpenPreviewAsync(path);
            EditorTabViewModel tab = workspace.ActiveTab;

            workspace.Activate(tab);
            Assert.IsTrue(tab.IsPreview);
            tab.ActivateByUser();
            Assert.IsFalse(tab.IsPreview);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Save_PreservesEditsMadeWhileWritingThePreviousSnapshot()
    {
        DelayedWriteStorage storage = new();
        FileEditorDocument document = new();
        document.Load(Path.Combine(Path.GetTempPath(), "concurrent-save.cs"), "original");
        EditorWorkflow workflow = new(document, new DesignEditorFileDialogService(), storage);
        workflow.UpdateText("snapshot");

        Task save = workflow.SaveAsync();
        await storage.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
        workflow.UpdateText("later edit");
        storage.Continue.TrySetResult();
        await save;

        Assert.AreEqual("snapshot", storage.Saved);
        Assert.AreEqual("snapshot", document.SavedContent);
        Assert.AreEqual("later edit", document.Content);
        Assert.IsTrue(document.IsDirty);
    }

    [TestMethod]
    public async Task Preview_ReplacesOnlyUnprotectedTabsAndPinsBeforeUserMutation()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_Preview_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.cs");
            string second = Path.Combine(root, "Second.cs");
            string third = Path.Combine(root, "Third.cs");
            await File.WriteAllTextAsync(first, "first");
            await File.WriteAllTextAsync(second, "second");
            await File.WriteAllTextAsync(third, "third");
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);

            await workspace.OpenPreviewAsync(first);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            Assert.IsTrue(firstTab.IsPreview);
            await workspace.OpenPreviewAsync(second);
            Assert.IsNull(workspace.FindByPath(first));
            EditorTabViewModel secondTab = workspace.ActiveTab;
            bool pinnedBeforeMutation = false;
            secondTab.Editor.ContentChanging += (_, _) => pinnedBeforeMutation = !secondTab.IsPreview;
            secondTab.Editor.Text = "changed";
            Assert.IsTrue(pinnedBeforeMutation);
            Assert.IsTrue(secondTab.Editor.IsDirty);

            await workspace.OpenPreviewAsync(third);
            Assert.AreSame(secondTab, workspace.FindByPath(second));
            Assert.IsTrue(workspace.ActiveTab.IsPreview);
            Assert.AreEqual(1, workspace.Tabs.Count(tab => tab.IsPreview));
            Assert.IsFalse(workspace.TryClose(secondTab));
            Assert.IsTrue(workspace.TryClose(secondTab, discardProtected: true));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Preview_LateLoadCannotReplaceNewerSelection()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_PreviewRace_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string first = Path.Combine(root, "First.cs");
            string second = Path.Combine(root, "Second.cs");
            await File.WriteAllTextAsync(first, "first");
            await File.WriteAllTextAsync(second, "second");
            DelayedReadStorage storage = new(first);
            TestEditorComponentFactory factory = new(storage);
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);

            Task<IEditorDocument?> slowOpen = workspace.OpenPreviewAsync(first);
            await storage.Started.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await workspace.OpenPreviewAsync(second);
            storage.Continue.TrySetResult();
            Assert.IsNull(await slowOpen);
            Assert.AreEqual(second, workspace.ActiveTab.FilePath);
            Assert.IsNull(workspace.FindByPath(first));
            Assert.AreEqual(1, workspace.Tabs.Count(tab => tab.IsPreview));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Open_UnsupportedBinaryAndMissingFilesUseDiagnosticInsteadOfText()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_UnsafeFile_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string binary = Path.Combine(root, "Binary.cs");
            string unknown = Path.Combine(root, "Image.png");
            string missing = Path.Combine(root, "Missing.cs");
            await File.WriteAllBytesAsync(binary, [0x41, 0x00, 0x42]);
            await File.WriteAllTextAsync(unknown, "not an editor format");
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace workspace = new(factory.Create(), factory);

            foreach (string path in new[] { binary, unknown, missing })
            {
                Assert.IsNull(await workspace.OpenPreviewAsync(path));
                Assert.IsTrue(workspace.ActiveTab.IsDiagnostic);
                Assert.AreEqual(path, workspace.ActiveTab.FilePath);
                Assert.AreEqual(string.Empty, workspace.ActiveTab.Editor.Text);
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task TextStorage_RetainsExistingUnicodeEncodingAndRejectsBinaryBeforeSave()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_TextEncoding_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            FileSystemTextDocumentStorageService storage = new();
            foreach (Encoding encoding in new Encoding[]
            {
                new UTF8Encoding(false, true),
                new UTF8Encoding(true, true),
                new UnicodeEncoding(false, true, true),
                new UnicodeEncoding(true, true, true),
                new UTF32Encoding(false, true, true),
                new UTF32Encoding(true, true, true),
            })
            {
                string path = Path.Combine(root, $"{encoding.WebName}-{encoding.GetPreamble().Length}.txt");
                await File.WriteAllBytesAsync(path, TextDocumentEncoding.Encode("Grüße", encoding));
                Assert.AreEqual("Grüße", await storage.ReadAllTextAsync(path));
                await storage.SaveAllTextAsync(path, "Änderung");
                CollectionAssert.AreEqual(TextDocumentEncoding.Encode("Änderung", encoding), await File.ReadAllBytesAsync(path));
            }

            string binary = Path.Combine(root, "bad.txt");
            byte[] original = [0xff, 0xfe, 0x00];
            await File.WriteAllBytesAsync(binary, original);
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => storage.SaveAllTextAsync(binary, "must not overwrite"));
            CollectionAssert.AreEqual(original, await File.ReadAllBytesAsync(binary));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task OpenOrActivateAsync_CreatesIndependentTabsAndReusesAlreadyOpenFile()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorTabs_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string firstPath = Path.Combine(root, "First.cs");
            string secondPath = Path.Combine(root, "Second.cs");
            await File.WriteAllTextAsync(firstPath, "class First {}");
            await File.WriteAllTextAsync(secondPath, "class Second {}");

            var factory = new TestEditorComponentFactory();
            var workspace = new EditorDocumentWorkspace(factory.Create(), factory);

            await workspace.OpenOrActivateAsync(firstPath);
            EditorTabViewModel firstTab = workspace.ActiveTab;
            Assert.IsTrue(workspace.IsDocumentOpen(firstPath));
            await workspace.OpenOrActivateAsync(secondPath);
            EditorTabViewModel secondTab = workspace.ActiveTab;
            Assert.IsTrue(workspace.IsDocumentOpen(secondPath));

            firstTab.Editor.Text = "class First { int Value; }";
            await workspace.OpenOrActivateAsync(firstPath);

            Assert.AreEqual(3, workspace.Tabs.Count);
            Assert.AreSame(firstTab, workspace.ActiveTab);
            Assert.AreNotSame(firstTab.Editor, secondTab.Editor);
            Assert.AreEqual("class First { int Value; }", firstTab.Editor.Text);
            Assert.AreEqual("class Second {}", secondTab.Editor.Text);
            Assert.IsTrue(workspace.IsDocumentOpen(Path.GetFullPath(firstPath)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_UpdatesCleanOpenDocumentAndRejectsDirtyDocument()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorChange_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string original = "<Window />";
            await File.WriteAllTextAsync(path, original);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    original,
                    "<Window Title=\"Updated\" />"),
            ]);

            changes.Apply(proposal, confirmed: true);

            Assert.AreEqual("<Window Title=\"Updated\" />", documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(documents.ActiveTab.Editor.Text, await File.ReadAllTextAsync(path));

            documents.ActiveTab.Editor.Text = "<Window Title=\"Unsaved\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(() => changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    "<Window Title=\"Updated\" />",
                    "<Window Title=\"Generated\" />"),
            ]));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_ExplicitlyReviewsDirtyDocumentAndRestoresItOnUndo()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorDirtyChange_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string saved = "<Window />";
            const string dirty = "<Window Title=\"Unsaved\" />";
            const string proposed = "<Window Title=\"Reviewed\" />";
            await File.WriteAllTextAsync(path, saved);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            documents.ActiveTab.Editor.Text = dirty;

            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    saved,
                    proposed,
                    expectedOpenDocumentContent: dirty),
            ]);
            changes.Apply(proposal, confirmed: true);

            Assert.AreEqual(proposed, documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(proposed, await File.ReadAllTextAsync(path));

            Assert.IsTrue(changes.Undo());
            Assert.AreEqual(dirty, documents.ActiveTab.Editor.Text);
            Assert.IsTrue(documents.ActiveTab.Editor.IsDirty);
            Assert.AreEqual(saved, await File.ReadAllTextAsync(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_RejectsReviewedOpenBufferIfTheTabWasClosed()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ClosedReview_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            await File.WriteAllTextAsync(path, "<Window />");
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            EditorTabViewModel tab = documents.ActiveTab;
            tab.Editor.Text = "<Window Title='Unsaved' />";
            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting(
                    "View.axaml",
                    "<Window />",
                    "<Window Title='Reviewed' />",
                    expectedOpenDocumentContent: tab.Editor.Text),
            ]);

            Assert.IsTrue(documents.TryClose(tab, discardProtected: true));
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(() => changes.Apply(proposal, confirmed: true));
            Assert.AreEqual("<Window />", await File.ReadAllTextAsync(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_RejectsDocumentThatBecameDirtyAfterPreparation()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorStaleChange_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string original = "<Window />";
            await File.WriteAllTextAsync(path, original);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", original, "<Window Title=\"Generated\" />"),
            ]);

            documents.ActiveTab.Editor.Text = "<Window Title=\"User edit\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(
                () => changes.Apply(proposal, confirmed: true));

            Assert.AreEqual(original, await File.ReadAllTextAsync(path));
            Assert.AreEqual("<Window Title=\"User edit\" />", documents.ActiveTab.Editor.Text);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task WorkspaceChange_UndoSynchronizesCleanDocumentAndRejectsDirtyDocument()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorUndo_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string path = Path.Combine(root, "View.axaml");
            const string original = "<Window />";
            await File.WriteAllTextAsync(path, original);
            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(path);
            WorkspaceFileChangeService changes = new(documents);
            WorkspaceFileChangeProposal proposal = changes.Prepare(root,
            [
                WorkspaceFileChangeRequest.ReplaceExisting("View.axaml", original, "<Window Title=\"Generated\" />"),
            ]);
            changes.Apply(proposal, confirmed: true);

            Assert.IsTrue(changes.Undo());
            Assert.AreEqual(original, documents.ActiveTab.Editor.Text);
            Assert.IsFalse(documents.ActiveTab.Editor.IsDirty);

            Assert.IsTrue(changes.Redo());
            documents.ActiveTab.Editor.Text = "<Window Title=\"Unsaved\" />";
            Assert.ThrowsExactly<WorkspaceOpenDocumentException>(() => changes.Undo());
            Assert.AreEqual("<Window Title=\"Generated\" />", await File.ReadAllTextAsync(path));
            Assert.AreEqual("<Window Title=\"Unsaved\" />", documents.ActiveTab.Editor.Text);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task OpenOrActivateAsync_UsesPlatformPathCaseRules()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_EditorCase_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string upperPath = Path.Combine(root, "View.axaml");
            string lowerPath = Path.Combine(root, "view.axaml");
            await File.WriteAllTextAsync(upperPath, "upper");
            if (!OperatingSystem.IsWindows())
            {
                await File.WriteAllTextAsync(lowerPath, "lower");
            }

            TestEditorComponentFactory factory = new();
            EditorDocumentWorkspace documents = new(factory.Create(), factory);
            await documents.OpenOrActivateAsync(upperPath);
            EditorTabViewModel upperTab = documents.ActiveTab;
            await documents.OpenOrActivateAsync(lowerPath);
            EditorTabViewModel lowerTab = documents.ActiveTab;

            if (OperatingSystem.IsWindows())
            {
                Assert.AreSame(upperTab, lowerTab);
                Assert.AreSame(upperTab, documents.FindByPath(lowerPath));
            }
            else
            {
                Assert.AreNotSame(upperTab, lowerTab);
                Assert.AreEqual("upper", upperTab.Editor.Text);
                Assert.AreEqual("lower", lowerTab.Editor.Text);
                Assert.AreSame(upperTab, documents.FindByPath(upperPath));
                Assert.AreSame(lowerTab, documents.FindByPath(lowerPath));
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class RegistryFactory(string id, int priority) : IDocumentEditorFactory<string>
    {
        public string Id => id;
        public int Priority => priority;
        public bool CanOpen(string filePath) => Path.GetExtension(filePath) == ".cs";
        public string Create(string filePath) => filePath;
    }

    private sealed class TestExplorerSource(ProjectExplorerItem item) : IProjectExplorerSource
    {
        public Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ProjectExplorerSnapshot(rootPath, [item]));
    }

    private sealed class RecordingPreviewOpener : IProjectExplorerPreviewOpener
    {
        public TaskCompletionSource Previewed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task OpenAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ActivateAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task PreviewAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
        {
            Previewed.TrySetResult();
            return Task.CompletedTask;
        }
    }

    private sealed class TestEditorComponentFactory(
        AA98_AvlnCodeStudio.Base.OS.Services.ITextDocumentStorageService? storage = null,
        IEditorFileDialogService? dialog = null) : IAvaloniaEditorComponentFactory
    {
        public IAvaloniaEditorComponent Create()
        {
            var workflow = new EditorWorkflow(
                new FileEditorDocument(),
                dialog ?? new DesignEditorFileDialogService(),
                storage ?? new FileSystemTextDocumentStorageService());
            return new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new TextBlock());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }

    private sealed class TestDocumentFactory(
        IAvaloniaEditorComponentFactory components,
        TestContributions contributions) : IDocumentEditorFactory<EditorTabViewModel>
    {
        public string Id => "custom";
        public int Priority => 1;
        public bool CanOpen(string filePath) => Path.GetExtension(filePath) == ".custom";
        public EditorTabViewModel Create(string filePath) => new(components.Create(), Id, contributions);
    }

    private sealed class BinaryDocumentFactory : IDocumentEditorFactory<EditorTabViewModel>
    {
        public string Id => "binary-test";
        public int Priority => 1;
        public List<BinaryDocumentSession> Sessions { get; } = [];
        public List<Control> Views { get; } = [];
        public IDocumentEditorContributions<Control>? Contributions { get; set; }
        public Action<BinaryDocumentSession>? ConfigureSession { get; set; }
        public Func<CancellationToken, Task<string?>> SelectSavePathAsync { get; set; } =
            _ => Task.FromResult<string?>(null);

        public bool CanOpen(string filePath) => Path.GetExtension(filePath) == ".bin";

        public EditorTabViewModel Create(string filePath)
        {
            BinaryDocumentSession session = new();
            ConfigureSession?.Invoke(session);
            Control view = new Border();
            Sessions.Add(session);
            Views.Add(view);
            return new EditorTabViewModel(session, view, token => SelectSavePathAsync(token), Id, Contributions);
        }
    }

    private sealed class BinaryDocumentSession : IDocumentEditorSession
    {
        private byte[] _savedContent = [];

        public string? FilePath { get; private set; }
        public string DisplayName => Path.GetFileName(FilePath) ?? "Untitled.bin";
        public byte[] Content { get; private set; } = [];
        public bool IsDirty => !Content.SequenceEqual(_savedContent);
        public bool IsDisposed { get; private set; }
        public string? OpenedPathOverride { get; set; }
        public Func<CancellationToken, Task>? BeforeReadAsync { get; set; }
        public Func<CancellationToken, Task>? BeforeWriteAsync { get; set; }
        public event EventHandler? ContentChanging;
        public event EventHandler? StateChanged;

        public async Task OpenAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (BeforeReadAsync is not null)
            {
                await BeforeReadAsync(cancellationToken);
            }

            Content = await File.ReadAllBytesAsync(filePath, cancellationToken);
            _savedContent = Content.ToArray();
            FilePath = OpenedPathOverride ?? filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SaveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            byte[] snapshot = Content.ToArray();
            if (BeforeWriteAsync is not null)
            {
                await BeforeWriteAsync(cancellationToken);
            }

            await File.WriteAllBytesAsync(filePath, snapshot, cancellationToken);
            _savedContent = snapshot;
            FilePath = filePath;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceContent(byte[] content)
        {
            ContentChanging?.Invoke(this, EventArgs.Empty);
            Content = content.ToArray();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class TestContributions(IPropertyProvider provider, bool extendedProperties)
        : IDocumentEditorContributions<Control>
    {
        public Control PaletteView { get; } = new Border();
        public Control? PropertiesView { get; } = extendedProperties ? new Border() : null;
        public IPropertyProvider? SelectedProperties { get; private set; } = provider;
        public bool HasUnappliedDrafts { get; private set; }
        public bool IsDisposed { get; private set; }
        public event EventHandler? SelectionChanged;
        public event EventHandler? DraftStateChanged;

        public void NotifySelectionChanged() => SelectionChanged?.Invoke(this, EventArgs.Empty);
        public void SetSelection(IPropertyProvider? value)
        {
            SelectedProperties = value;
            NotifySelectionChanged();
        }

        public void SetDraft(bool value)
        {
            HasUnappliedDrafts = value;
            DraftStateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() => IsDisposed = true;
    }

    private sealed class DelayedReadStorage(string delayedPath) : AA98_AvlnCodeStudio.Base.OS.Services.ITextDocumentStorageService
    {
        private readonly FileSystemTextDocumentStorageService _inner = new();

        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Continue { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (filePath == delayedPath)
            {
                Started.TrySetResult();
                await Continue.Task.WaitAsync(cancellationToken);
            }

            return await _inner.ReadAllTextAsync(filePath, cancellationToken);
        }

        public Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default) =>
            _inner.SaveAllTextAsync(filePath, content, cancellationToken);
    }

    private sealed class DelayedWriteStorage : AA98_AvlnCodeStudio.Base.OS.Services.ITextDocumentStorageService
    {
        public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Continue { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public string? Saved { get; private set; }

        public Task<string> ReadAllTextAsync(string filePath, CancellationToken cancellationToken = default) =>
            new FileSystemTextDocumentStorageService().ReadAllTextAsync(filePath, cancellationToken);

        public async Task SaveAllTextAsync(string filePath, string content, CancellationToken cancellationToken = default)
        {
            Started.TrySetResult();
            await Continue.Task.WaitAsync(cancellationToken);
            Saved = content;
        }
    }
}
