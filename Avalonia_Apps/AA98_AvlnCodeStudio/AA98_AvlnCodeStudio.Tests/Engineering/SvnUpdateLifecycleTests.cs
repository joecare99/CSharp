#if NET10_0
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class SvnUpdateLifecycleTests
{
    [TestMethod]
    [DataRow("approval")]
    [DataRow("update")]
    [DataRow("status")]
    [DataRow("update-error")]
    [DataRow("status-error")]
    [DataRow("cancel")]
    [DataRow("update-cancel")]
    [DataRow("status-cancel")]
    public async Task SupersededOperation_DoesNotPublishToNewWorkspace(string stage)
    {
        await WithHostAsync(async (host, service, operations, oldPath, newPath, oldStatus, newStatus) =>
        {
            TaskCompletionSource<bool> approval = NewSource<bool>();
            TaskCompletionSource<string> update = NewSource<string>();
            TaskCompletionSource<VersionControlStatus> status = NewSource<VersionControlStatus>();
            TaskCompletionSource<bool> statusRequested = NewSource<bool>();
            host.ApproveSvnUpdateAsync = (_, _) => stage == "approval" ? approval.Task : Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(_ => stage == "approval" ? Task.FromResult("old output") : update.Task);
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(call =>
                {
                    if (call.Arg<VersionControlStatusRequest>().RepositoryContextPath == Path.GetDirectoryName(newPath))
                        return Task.FromResult(newStatus);
                    statusRequested.TrySetResult(true);
                    return status.Task;
                });

            Task pending = host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            if (stage is "status" or "status-error" or "status-cancel")
            {
                update.SetResult("old output");
                await statusRequested.Task.WaitAsync(TimeSpan.FromSeconds(10));
            }

            await host.OpenWorkspaceFileAsync(newPath);
            host.Editor.StatusText = "new status";
            host.Editor.NotificationText = "new notification";
            switch (stage)
            {
                case "approval": status.SetResult(oldStatus); approval.SetResult(true); break;
                case "update":
                case "status": update.TrySetResult("old output"); status.TrySetResult(oldStatus); break;
                case "update-error": update.SetException(new IOException("old failure")); break;
                case "status-error": status.SetException(new IOException("old failure")); break;
                case "cancel": host.UpdateSvnWorkspaceCommand.Cancel(); update.SetException(new OperationCanceledException()); break;
                case "update-cancel": update.SetException(new OperationCanceledException()); break;
                case "status-cancel": status.SetException(new OperationCanceledException()); break;
            }
            await pending;
            Assert.AreEqual(newPath, host.Workspace?.EntryPath);
            Assert.AreSame(newStatus, host.VersionControlStatus);
            Assert.AreEqual("new status", host.Editor.StatusText);
            Assert.AreEqual("new notification", host.Editor.NotificationText);
            Assert.IsFalse(host.SvnUpdateBusy);
            if (stage == "approval")
            {
                await operations.DidNotReceive().UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
            }
        });
    }

    [TestMethod]
    public async Task ReentryDuringUpdatePublication_PreservesNewMissingClientWarning()
    {
        await WithHostAsync(async (host, service, operations, _, newPath, _, _) =>
        {
            host.VersionControlError = "old warning";
            host.Editor.NotificationText = "old warning";
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("old output"));
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(call => call.Arg<VersionControlStatusRequest>().RepositoryContextPath == Path.GetDirectoryName(newPath)
                    ? Task.FromException<VersionControlStatus>(new VersionControlClientNotFoundException("new-client", new Win32Exception(2)))
                    : Task.FromResult(Status("refreshed")));
            Task? opened = null;
            host.Editor.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(host.Editor.NotificationText) && host.Editor.NotificationText == string.Empty && opened is null)
                {
                    opened = host.OpenWorkspaceFileAsync(newPath);
                    Assert.IsTrue(opened.IsCompletedSuccessfully);
                }
            };
            await host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            Assert.IsNotNull(opened);
            await opened;
            StringAssert.Contains(host.VersionControlError!, "new-client");
            StringAssert.Contains(host.Editor.NotificationText, "new-client");
            Assert.IsNull(host.VersionControlStatus);
        });
    }

    [TestMethod]
    [DataRow("busy")]
    [DataRow("clear-notification")]
    [DataRow("clear-error")]
    [DataRow("publish-status")]
    [DataRow("success")]
    [DataRow("failure")]
    public async Task SynchronousPropertyReentry_PreservesNewWorkspace(string boundary)
    {
        await WithHostAsync(async (host, service, operations, _, newPath, _, newStatus) =>
        {
            host.VersionControlError = "old warning";
            host.Editor.NotificationText = "old warning";
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(boundary == "failure" ? Task.FromException<string>(new IOException("old failure")) : Task.FromResult("old output"));
            VersionControlStatus refreshed = Status("refreshed");
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(call => Task.FromResult(call.Arg<VersionControlStatusRequest>().RepositoryContextPath == Path.GetDirectoryName(newPath)
                    ? newStatus : refreshed));
            Task? opened = null;
            void OpenNewer()
            {
                if (opened is not null) return;
                opened = host.OpenWorkspaceFileAsync(newPath);
                Assert.IsTrue(opened.IsCompletedSuccessfully);
                host.Editor.StatusText = "new status";
                host.Editor.NotificationText = "new notification";
            }
            host.PropertyChanged += (_, e) =>
            {
                if ((boundary == "busy" && e.PropertyName == nameof(host.SvnUpdateBusy) && host.SvnUpdateBusy)
                    || (boundary == "clear-error" && e.PropertyName == nameof(host.VersionControlError) && host.VersionControlError is null)
                    || (boundary == "publish-status" && e.PropertyName == nameof(host.VersionControlStatus) && ReferenceEquals(host.VersionControlStatus, refreshed)))
                    OpenNewer();
            };
            host.Editor.PropertyChanged += (_, e) =>
            {
                if ((boundary == "clear-notification" && e.PropertyName == nameof(host.Editor.NotificationText) && host.Editor.NotificationText == string.Empty)
                    || (boundary == "success" && e.PropertyName == nameof(host.Editor.StatusText) && host.Editor.StatusText == "SVN update completed.")
                    || (boundary == "failure" && e.PropertyName == nameof(host.Editor.StatusText) && host.Editor.StatusText == "SVN update failed."))
                    OpenNewer();
            };
            await host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            Assert.IsNotNull(opened, boundary);
            await opened;
            Assert.AreSame(newStatus, host.VersionControlStatus);
            Assert.AreEqual("new status", host.Editor.StatusText);
            Assert.AreEqual("new notification", host.Editor.NotificationText);
            Assert.IsFalse(host.SvnUpdateBusy);
        });
    }

    [TestMethod]
    public async Task ApprovalIsExclusive_AndNormalUpdateRefreshesStatus()
    {
        await WithHostAsync(async (host, service, operations, oldPath, _, _, _) =>
        {
            TaskCompletionSource<bool> approval = NewSource<bool>();
            int approvals = 0;
            host.ApproveSvnUpdateAsync = (path, _) =>
            {
                Assert.AreEqual(Path.GetDirectoryName(oldPath), path);
                approvals++;
                return approval.Task;
            };
            VersionControlStatus refreshed = Status("refreshed");
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(refreshed));
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("updated"));
            Task first = host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            Assert.IsFalse(host.UpdateSvnWorkspaceCommand.CanExecute(null));
            approval.SetResult(true);
            await first;
            Assert.AreEqual(1, approvals);
            await operations.Received(1).UpdateAsync(Path.GetDirectoryName(oldPath)!, Arg.Any<CancellationToken>());
            Assert.AreSame(refreshed, host.VersionControlStatus);
            Assert.AreEqual("SVN update completed.", host.Editor.StatusText);
            Assert.AreEqual("updated", host.Editor.NotificationText);
            Assert.IsFalse(host.SvnUpdateBusy);
            Assert.IsTrue(host.UpdateSvnWorkspaceCommand.CanExecute(null));
        });
    }

    [TestMethod]
    [DataRow("update")]
    [DataRow("status")]
    [DataRow("publish-status")]
    public async Task Cancellation_StopsFurtherPublication(string stage)
    {
        await WithHostAsync(async (host, service, operations, _, _, _, _) =>
        {
            TaskCompletionSource<string> update = NewSource<string>();
            TaskCompletionSource<VersionControlStatus> status = NewSource<VersionControlStatus>();
            TaskCompletionSource<bool> statusRequested = NewSource<bool>();
            VersionControlStatus refreshed = Status("refreshed");
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(update.Task);
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    statusRequested.TrySetResult(true);
                    return status.Task;
                });
            host.Editor.NotificationText = "original notification";
            host.PropertyChanged += (_, e) =>
            {
                if (stage == "publish-status" && e.PropertyName == nameof(host.VersionControlStatus)
                    && ReferenceEquals(host.VersionControlStatus, refreshed))
                    host.UpdateSvnWorkspaceCommand.Cancel();
            };
            Task pending = host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            if (stage != "update")
            {
                update.SetResult("updated");
                await statusRequested.Task.WaitAsync(TimeSpan.FromSeconds(10));
            }
            if (stage != "publish-status")
                host.UpdateSvnWorkspaceCommand.Cancel();
            update.TrySetResult("updated");
            status.TrySetResult(refreshed);
            await pending;
            Assert.AreEqual("SVN update was cancelled.", host.Editor.StatusText);
            Assert.AreEqual("original notification", host.Editor.NotificationText);
            Assert.IsFalse(host.SvnUpdateBusy);
            if (stage == "update")
                await service.DidNotReceive().GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>());
        });
    }

    [TestMethod]
    public async Task DeclinedApproval_DoesNotUpdate_AndEmptyOutputUsesRefreshedSummary()
    {
        await WithHostAsync(async (host, service, operations, _, _, _, _) =>
        {
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(false);
            await host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            Assert.AreEqual("SVN update was not approved; no changes were made.", host.Editor.StatusText);
            await operations.DidNotReceive().UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

            VersionControlStatus refreshed = Status("refreshed");
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(" "));
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(refreshed));
            await host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            Assert.AreSame(refreshed, host.VersionControlStatus);
            Assert.AreEqual(host.VersionControlSummary, host.Editor.NotificationText);
        });
    }

    [TestMethod]
    public async Task ReopeningSameRoot_InvalidatesOlderUpdate()
    {
        await WithHostAsync(async (host, service, operations, oldPath, _, _, _) =>
        {
            string second = Path.Combine(Path.GetDirectoryName(oldPath)!, "Second.slnx");
            await File.WriteAllTextAsync(second, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            TaskCompletionSource<VersionControlStatus> status = NewSource<VersionControlStatus>();
            TaskCompletionSource<bool> requested = NewSource<bool>();
            VersionControlStatus newer = Status("newer");
            int calls = 0;
            host.ApproveSvnUpdateAsync = (_, _) => Task.FromResult(true);
            operations.UpdateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("old output"));
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    if (++calls != 1) return Task.FromResult(newer);
                    requested.SetResult(true);
                    return status.Task;
                });
            Task pending = host.UpdateSvnWorkspaceCommand.ExecuteAsync(null);
            await requested.Task.WaitAsync(TimeSpan.FromSeconds(10));
            await host.OpenWorkspaceFileAsync(second);
            host.Editor.StatusText = "new status";
            host.Editor.NotificationText = "new notification";
            status.SetResult(Status("stale"));
            await pending;
            Assert.AreSame(newer, host.VersionControlStatus);
            Assert.AreEqual("new status", host.Editor.StatusText);
            Assert.AreEqual("new notification", host.Editor.NotificationText);
        });
    }

    private static TaskCompletionSource<T> NewSource<T>() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static VersionControlStatus Status(string name)
    {
        VersionControlStatus result = new() { IsRepositoryRootDiscovered = true, ActiveReferenceName = name };
        result.Capabilities.Add(VersionControlCapability.UpdateWorkingCopy);
        return result;
    }

    private static async Task WithHostAsync(Func<MainWindowViewModel, IVersionControlService, ISvnWorkingCopyOperations, string, string, VersionControlStatus, VersionControlStatus, Task> action)
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"svn-update-{Guid.NewGuid():N}");
        string oldRoot = Path.Combine(root, "old");
        string newRoot = Path.Combine(root, "new");
        Directory.CreateDirectory(oldRoot);
        Directory.CreateDirectory(newRoot);
        string oldPath = Path.Combine(oldRoot, "Old.slnx");
        string newPath = Path.Combine(newRoot, "New.slnx");
        try
        {
            await File.WriteAllTextAsync(Path.Combine(oldRoot, "App.csproj"), "<Project />");
            await File.WriteAllTextAsync(Path.Combine(newRoot, "App.csproj"), "<Project />");
            await File.WriteAllTextAsync(oldPath, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            await File.WriteAllTextAsync(newPath, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            VersionControlStatus oldStatus = Status("old");
            VersionControlStatus newStatus = Status("new");
            IVersionControlService service = Substitute.For<IVersionControlService>();
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(oldStatus));
            ISvnWorkingCopyOperations operations = Substitute.For<ISvnWorkingCopyOperations>();
            IAvaloniaEditorComponent editor = Substitute.For<IAvaloniaEditorComponent>();
            editor.EditorViewModel.Returns(new EditorViewModel());
            MainWindowViewModel host = new(editor, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(), null, null, versionControlService: service, svnWorkingCopyOperations: operations);
            await host.OpenWorkspaceFileAsync(oldPath);
            service.ClearReceivedCalls();
            await action(host, service, operations, oldPath, newPath, oldStatus, newStatus);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }
}
#endif
