using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ProjectExplorerPreviewTests
{
    [TestMethod]
    public async Task Selection_IsImmediateButDebouncedPreviewAndExplicitActivationAreDistinct()
    {
        string root = Path.GetTempPath();
        ProjectExplorerItem first = new("first", "First.cs", Path.Combine(root, "First.cs"), ProjectExplorerItemKind.File);
        ProjectExplorerItem second = new("second", "Second.cs", Path.Combine(root, "Second.cs"), ProjectExplorerItemKind.File);
        RecordingOpener opener = new();
        using ProjectExplorerViewModel explorer = new(new Source(first, second), new ProjectExplorerState(), opener);
        await explorer.LoadAsync(root);
        ProjectExplorerItem? selected = null;
        explorer.UserSelectionChanged += item => selected = item;

        explorer.SelectedItem = explorer.RootItems[0];
        explorer.SelectedItem = explorer.RootItems[1];
        Assert.AreSame(second, selected);
        await opener.Previewed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        CollectionAssert.AreEqual(new[] { "second" }, opener.PreviewIds.ToArray());

        explorer.SelectedItem = explorer.RootItems[0];
        await explorer.OpenAsync(explorer.RootItems[0]);
        await Task.Delay(250);
        CollectionAssert.AreEqual(new[] { "second" }, opener.PreviewIds.ToArray());
        CollectionAssert.AreEqual(new[] { "first" }, opener.ActivatedIds.ToArray());

        await explorer.LoadAsync(root);
        await Task.Delay(250);
        Assert.AreEqual("first", explorer.SelectedItem?.Id);
        CollectionAssert.AreEqual(new[] { "second" }, opener.PreviewIds.ToArray());
    }

    [TestMethod]
    public async Task NewSelection_CancelsPendingExplicitActivation()
    {
        string root = Path.GetTempPath();
        ProjectExplorerItem file = new("file", "File.cs", Path.Combine(root, "File.cs"), ProjectExplorerItemKind.File);
        ProjectExplorerItem folder = new("folder", "Folder", Path.Combine(root, "Folder"), ProjectExplorerItemKind.Folder);
        PendingActivationOpener opener = new();
        using ProjectExplorerViewModel explorer = new(new Source(file, folder), new ProjectExplorerState(), opener);
        await explorer.LoadAsync(root);

        explorer.SelectedItem = explorer.RootItems[0];
        Task activation = explorer.OpenAsync(explorer.RootItems[0]);
        CancellationToken activationToken = await opener.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        explorer.SelectedItem = explorer.RootItems[1];
        bool wasCancelled = activationToken.IsCancellationRequested;
        opener.Release.TrySetResult();
        await activation.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.IsTrue(wasCancelled, "An older explicit activation must be cancelled when selection changes.");
        Assert.IsFalse(opener.Activated);
        Assert.AreEqual("folder", explorer.SelectedItem?.Id);
        Assert.AreEqual(string.Empty, explorer.ErrorMessage);
    }

    [TestMethod]
    public async Task Refresh_OlderFailureDoesNotReplaceNewerResult()
    {
        string root = Path.GetTempPath();
        string oldRoot = Path.Combine(root, "old");
        string newRoot = Path.Combine(root, "new");
        ProjectExplorerItem current = new("current", "Current", newRoot, ProjectExplorerItemKind.Folder);
        DelayedFailureSource source = new(oldRoot, current);
        using ProjectExplorerViewModel explorer = new(source, new ProjectExplorerState(), new RecordingOpener());

        Task oldRefresh = explorer.LoadAsync(oldRoot);
        await explorer.LoadAsync(newRoot);
        source.OldRefresh.SetException(new IOException("Stale refresh failed."));
        await oldRefresh;

        Assert.AreEqual("current", explorer.RootItems.Single().Id);
        Assert.AreEqual(string.Empty, explorer.ErrorMessage);
    }

    [TestMethod]
    public async Task Refresh_CancelsAnOlderHierarchyLoad()
    {
        string root = Path.GetTempPath();
        string oldRoot = Path.Combine(root, "old");
        string newRoot = Path.Combine(root, "new");
        ProjectExplorerItem current = new("current", "Current", newRoot, ProjectExplorerItemKind.Folder);
        CancellableSource source = new(oldRoot, current);
        using ProjectExplorerViewModel explorer = new(source, new ProjectExplorerState(), new RecordingOpener());

        Task oldRefresh = explorer.LoadAsync(oldRoot);
        CancellationToken oldToken = await source.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await explorer.LoadAsync(newRoot);
        bool wasCancelled = oldToken.IsCancellationRequested;
        source.OldRefresh.TrySetResult(new ProjectExplorerSnapshot(oldRoot, []));
        await oldRefresh.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.IsTrue(wasCancelled, "A superseded hierarchy scan must not keep running.");
        Assert.AreEqual("current", explorer.RootItems.Single().Id);
        Assert.AreEqual(string.Empty, explorer.ErrorMessage);
    }

    [TestMethod]
    public async Task Refresh_PropagatesCallerCancellation()
    {
        string root = Path.GetTempPath();
        ProjectExplorerItem current = new("current", "Current", root, ProjectExplorerItemKind.Folder);
        CancellableSource source = new(root, current);
        using ProjectExplorerViewModel explorer = new(source, new ProjectExplorerState(), new RecordingOpener());
        using CancellationTokenSource cancellation = new();

        Task refresh = explorer.LoadAsync(root, cancellation.Token);
        await source.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        cancellation.Cancel();
        try
        {
            await refresh.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.Fail("Caller cancellation must be propagated.");
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }

        Assert.IsFalse(explorer.IsBusy);
    }

    [TestMethod]
    public async Task Dispose_CancelsAnInFlightHierarchyLoad()
    {
        string root = Path.GetTempPath();
        ProjectExplorerItem current = new("current", "Current", root, ProjectExplorerItemKind.Folder);
        CancellableSource source = new(root, current);
        ProjectExplorerViewModel explorer = new(source, new ProjectExplorerState(), new RecordingOpener());

        Task refresh = explorer.LoadAsync(root);
        CancellationToken token = await source.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        explorer.Dispose();
        bool wasCancelled = token.IsCancellationRequested;
        source.OldRefresh.TrySetResult(new ProjectExplorerSnapshot(root, []));
        await refresh.WaitAsync(TimeSpan.FromSeconds(5));

        Assert.IsTrue(wasCancelled);
        Assert.AreEqual(0, explorer.RootItems.Count);
    }

    private sealed class Source(params ProjectExplorerItem[] items) : IProjectExplorerSource
    {
        public Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default) =>
            Task.FromResult(new ProjectExplorerSnapshot(rootPath, items));
    }

    private sealed class DelayedFailureSource(string oldRoot, ProjectExplorerItem current) : IProjectExplorerSource
    {
        public TaskCompletionSource<ProjectExplorerSnapshot> OldRefresh { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default) =>
            rootPath == oldRoot
                ? OldRefresh.Task
                : Task.FromResult(new ProjectExplorerSnapshot(rootPath, [current]));
    }

    private sealed class CancellableSource(string oldRoot, ProjectExplorerItem current) : IProjectExplorerSource
    {
        public TaskCompletionSource<CancellationToken> Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<ProjectExplorerSnapshot> OldRefresh { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task<ProjectExplorerSnapshot> RefreshAsync(string rootPath, CancellationToken cancellationToken = default)
        {
            if (rootPath != oldRoot)
            {
                return new ProjectExplorerSnapshot(rootPath, [current]);
            }

            Started.TrySetResult(cancellationToken);
            return await OldRefresh.Task.WaitAsync(cancellationToken);
        }
    }

    private sealed class PendingActivationOpener : IProjectExplorerPreviewOpener
    {
        public TaskCompletionSource<CancellationToken> Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool Activated { get; private set; }

        public Task OpenAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default) =>
            ActivateAsync(item, cancellationToken);

        public Task PreviewAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public async Task ActivateAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
        {
            Started.TrySetResult(cancellationToken);
            await Release.Task;
            cancellationToken.ThrowIfCancellationRequested();
            Activated = true;
        }
    }

    private sealed class RecordingOpener : IProjectExplorerPreviewOpener
    {
        public ConcurrentQueue<string> PreviewIds { get; } = new();

        public ConcurrentQueue<string> ActivatedIds { get; } = new();

        public TaskCompletionSource Previewed { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task OpenAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default) =>
            ActivateAsync(item, cancellationToken);

        public Task PreviewAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
        {
            PreviewIds.Enqueue(item.Id);
            Previewed.TrySetResult();
            return Task.CompletedTask;
        }

        public Task ActivateAsync(ProjectExplorerItem item, CancellationToken cancellationToken = default)
        {
            ActivatedIds.Enqueue(item.Id);
            return Task.CompletedTask;
        }
    }
}
