#if NET10_0
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Editor.Components;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Configuration;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.UI.ViewModels;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class MissingVersionControlClientTests
{
    [TestMethod]
    public async Task MissingSvnExecutable_LeavesWorkspaceAndExplorerUsableAndReportsError()
    {
        await WithWorkspaceAsync(async (root, solution, file) =>
        {
            string missing = Path.Combine(root, "no-such-svn-client");
            ServiceCollection registrations = new();
            App.ConfigureServices(registrations, static () => null, Path.Combine(root, "config"));
            registrations.AddSingleton<IAvaloniaEditorComponentFactory>(new TestEditorComponentFactory());
            registrations.AddSingleton<ISvnCommandRunner>(new SvnCommandRunner(missing));
            registrations.AddSingleton(new CodeStudioSettings());
            using ServiceProvider services = registrations.BuildServiceProvider();
            MainWindowViewModel host = services.GetRequiredService<MainWindowViewModel>();

            await host.OpenWorkspaceFileAsync(solution);

            Assert.AreEqual(solution, host.Workspace?.EntryPath);
            Assert.IsNull(host.VersionControlStatus);
            Assert.IsFalse(host.CanUpdateSvnWorkspace);
            StringAssert.Contains(host.VersionControlSummary, missing);
            StringAssert.Contains(host.Editor.NotificationText, missing);
            var explorer = host.ProjectExplorer!;
            var solutionItem = explorer.RootItems.Single();
            var fileItem = solutionItem.Children.Single(i => i.Name == "Files").Children.Single(i => i.Name == "Readme.txt");
            await explorer.OpenAsync(fileItem);
            Assert.AreEqual(file, host.EditorWorkspace.ActiveTab.FilePath);
            Assert.AreEqual("still available", host.Editor.Text);
            Assert.AreEqual("still available", await File.ReadAllTextAsync(file));
        });
    }

    [TestMethod]
    public async Task NormalSvnStatus_ReplacesMissingClientWarning()
    {
        await WithWorkspaceAsync(async (root, solution, _) =>
        {
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            bool missing = true;
            runner.RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(call =>
                {
                    if (missing)
                    {
                        throw new VersionControlClientNotFoundException("svn", new Win32Exception(2));
                    }
                    var args = call.Arg<IReadOnlyList<string>>();
                    return Task.FromResult(new SvnCommandResult
                    {
                        StandardOutput = args[0] == "info"
                            ? new XDocument(new XElement("info", new XElement("entry",
                                new XElement("relative-url", "^/trunk"),
                                new XElement("wc-info", new XElement("wcroot-abspath", root))))).ToString()
                            : "<status><target /></status>",
                    });
                });
            MainWindowViewModel host = CreateHost(runner);
            await host.OpenWorkspaceFileAsync(solution);
            StringAssert.Contains(host.VersionControlSummary, "svn");

            missing = false;
            await host.OpenWorkspaceFileAsync(solution);
            Assert.IsTrue(host.VersionControlStatus?.IsRepositoryRootDiscovered == true);
            StringAssert.Contains(host.VersionControlSummary, "trunk");
            Assert.IsNull(host.VersionControlError);
            Assert.IsFalse(host.Editor.NotificationText.Contains("svn", StringComparison.OrdinalIgnoreCase));
        });
    }

    [TestMethod]
    public async Task OtherFailuresAndCancellation_AreNotSwallowed()
    {
        await WithWorkspaceAsync(async (_, solution, _) =>
        {
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            runner.RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(_ => Task.FromException<SvnCommandResult>(new Win32Exception(5)));
            MainWindowViewModel host = CreateHost(runner);
            Win32Exception error = await Assert.ThrowsExactlyAsync<Win32Exception>(() => host.OpenWorkspaceFileAsync(solution));
            Assert.AreEqual(5, error.NativeErrorCode);
            Assert.AreEqual(solution, host.Workspace?.EntryPath);

            using CancellationTokenSource cancellation = new();
            runner.RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    cancellation.Cancel();
                    return Task.FromException<SvnCommandResult>(new VersionControlClientNotFoundException("svn", new Win32Exception(2)));
                });
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => host.OpenWorkspaceFileAsync(solution, cancellation.Token));
            Assert.IsNull(host.VersionControlError);
        });
    }

    [TestMethod]
    public async Task OlderMissingClientResult_DoesNotOverwriteNewWorkspace()
    {
        await WithWorkspaceAsync(async (_, solution, _) =>
        {
            string second = Path.Combine(Path.GetDirectoryName(solution)!, "Second.slnx");
            await File.WriteAllTextAsync(second, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            TaskCompletionSource<SvnCommandResult> pending = new(TaskCreationOptions.RunContinuationsAsynchronously);
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            int calls = 0;
            runner.RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(_ => ++calls == 1
                    ? pending.Task
                    : Task.FromResult(new SvnCommandResult { ExitCode = 1 }));
            MainWindowViewModel host = CreateHost(runner);
            Task older = host.OpenWorkspaceFileAsync(solution);
            await host.OpenWorkspaceFileAsync(second);
            pending.SetException(new VersionControlClientNotFoundException("svn", new Win32Exception(2)));
            await older;
            Assert.AreEqual(second, host.Workspace?.EntryPath);
            Assert.IsNull(host.VersionControlError);
            Assert.IsNotNull(host.VersionControlStatus);
        });
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task ReentrantNewerOpen_KeepsItsVersionControlState(bool missingClient)
    {
        await WithWorkspaceAsync(async (_, solution, _) =>
        {
            string second = Path.Combine(Path.GetDirectoryName(solution)!, "Second.slnx");
            await File.WriteAllTextAsync(second, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            IVersionControlService service = Substitute.For<IVersionControlService>();
            VersionControlStatus expected = new()
            {
                IsRepositoryRootDiscovered = true,
                RepositoryName = "newer",
                ActiveReferenceName = "main",
            };
            int calls = 0;
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(_ =>
                {
                    if (++calls == 1 && missingClient)
                    {
                        return Task.FromException<VersionControlStatus>(
                            new VersionControlClientNotFoundException("svn", new Win32Exception(2)));
                    }

                    return Task.FromResult(expected);
                });
            IAvaloniaEditorComponent editor = Substitute.For<IAvaloniaEditorComponent>();
            editor.EditorViewModel.Returns(new EditorViewModel());
            DiagnosticCollectionViewModel diagnostics = new();
            MainWindowViewModel host = new(editor, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                diagnostics, null, null, versionControlService: service);
            Task? newerOpen = null;
            NotifyCollectionChangedEventHandler? onChanged = null;
            onChanged = (_, _) =>
            {
                diagnostics.Items.CollectionChanged -= onChanged;
                newerOpen = host.OpenWorkspaceFileAsync(second);
                Assert.IsTrue(newerOpen.IsCompletedSuccessfully);
                Assert.AreEqual(1, calls);
                if (missingClient)
                {
                    StringAssert.Contains(host.VersionControlSummary, "svn");
                }
                else
                {
                    Assert.AreSame(expected, host.VersionControlStatus);
                }
            };
            diagnostics.Items.CollectionChanged += onChanged;

            await host.OpenWorkspaceFileAsync(solution);
            Assert.IsNotNull(newerOpen);
            await newerOpen;
            Assert.AreEqual(second, host.Workspace?.EntryPath);
            if (missingClient)
            {
                StringAssert.Contains(host.VersionControlSummary, "svn");
                StringAssert.Contains(host.Editor.NotificationText, "svn");
                Assert.IsNull(host.VersionControlStatus);
            }
            else
            {
                Assert.AreSame(expected, host.VersionControlStatus);
                StringAssert.Contains(host.VersionControlSummary, "main");
            }
            Assert.AreEqual(1, calls, "The superseded open must not fetch or clear VCS state.");
        });
    }

    [TestMethod]
    [DataRow("reset-notification")]
    [DataRow("reset-error")]
    [DataRow("reset-status")]
    [DataRow("publish-error")]
    [DataRow("publish-status")]
    [DataRow("publish-notification")]
    public async Task ReentrantVcsPropertyChange_KeepsNewerState(string boundary)
    {
        await WithWorkspaceAsync(async (_, solution, _) =>
        {
            string olderRoot = Path.GetDirectoryName(solution)!;
            string newerRoot = Path.Combine(olderRoot, "newer");
            Directory.CreateDirectory(newerRoot);
            string newerSolution = Path.Combine(newerRoot, "Newer.slnx");
            await File.WriteAllTextAsync(newerSolution, "<Solution><Project Path=\"App.csproj\" /></Solution>");
            await File.WriteAllTextAsync(Path.Combine(newerRoot, "App.csproj"), "<Project />");
            bool seedWarning = boundary is "reset-notification" or "reset-error";
            bool newerWarning = boundary is "reset-notification" or "publish-error" or "publish-notification";
            bool olderWarning = boundary is "publish-error" or "publish-notification";
            VersionControlStatus initial = new() { IsRepositoryRootDiscovered = true, ActiveReferenceName = "initial" };
            VersionControlStatus older = new() { IsRepositoryRootDiscovered = true, ActiveReferenceName = "older" };
            VersionControlStatus newer = new() { IsRepositoryRootDiscovered = true, ActiveReferenceName = "newer" };
            int olderCalls = 0;
            IVersionControlService service = Substitute.For<IVersionControlService>();
            service.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
                .Returns(call =>
                {
                    if (call.Arg<VersionControlStatusRequest>().RepositoryContextPath == newerRoot)
                    {
                        return newerWarning ? MissingStatus("newer-client") : Task.FromResult(newer);
                    }

                    olderCalls++;
                    return olderCalls == 1
                        ? seedWarning ? MissingStatus("initial-client") : Task.FromResult(initial)
                        : olderWarning ? MissingStatus("older-client") : Task.FromResult(older);
                });
            IAvaloniaEditorComponent editor = Substitute.For<IAvaloniaEditorComponent>();
            editor.EditorViewModel.Returns(new EditorViewModel());
            MainWindowViewModel host = new(editor, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
                new DiagnosticCollectionViewModel(), null, null, versionControlService: service);
            await host.OpenWorkspaceFileAsync(solution);

            Task? newerOpen = null;
            PropertyChangedEventHandler? vmChanged = null;
            PropertyChangedEventHandler? editorChanged = null;
            void OpenNewer()
            {
                host.PropertyChanged -= vmChanged;
                host.Editor.PropertyChanged -= editorChanged;
                newerOpen = host.OpenWorkspaceFileAsync(newerSolution);
                Assert.IsTrue(newerOpen.IsCompletedSuccessfully);
                if (newerWarning)
                {
                    StringAssert.Contains(host.VersionControlSummary, "newer-client");
                    StringAssert.Contains(host.Editor.NotificationText, "newer-client");
                }
                else
                {
                    Assert.AreSame(newer, host.VersionControlStatus);
                }
            }

            vmChanged = (_, args) =>
            {
                if ((boundary == "reset-error" && args.PropertyName == nameof(host.VersionControlError) && host.VersionControlError is null)
                    || (boundary == "reset-status" && args.PropertyName == nameof(host.VersionControlStatus) && host.VersionControlStatus is null)
                    || (boundary == "publish-error" && args.PropertyName == nameof(host.VersionControlError) && host.VersionControlError?.Contains("older-client") == true)
                    || (boundary == "publish-status" && args.PropertyName == nameof(host.VersionControlStatus) && ReferenceEquals(host.VersionControlStatus, older)))
                {
                    OpenNewer();
                }
            };
            editorChanged = (_, args) =>
            {
                if (args.PropertyName == nameof(host.Editor.NotificationText)
                    && ((boundary == "reset-notification" && host.Editor.NotificationText == string.Empty)
                        || (boundary == "publish-notification" && host.Editor.NotificationText.Contains("older-client"))))
                {
                    OpenNewer();
                }
            };
            host.PropertyChanged += vmChanged;
            host.Editor.PropertyChanged += editorChanged;
            await host.OpenWorkspaceFileAsync(solution);

            Assert.IsNotNull(newerOpen, $"Boundary {boundary} was not reached.");
            await newerOpen;
            Assert.AreEqual(newerSolution, host.Workspace?.EntryPath);
            if (newerWarning)
            {
                StringAssert.Contains(host.VersionControlSummary, "newer-client");
                StringAssert.Contains(host.Editor.NotificationText, "newer-client");
                Assert.IsNull(host.VersionControlStatus);
            }
            else
            {
                Assert.AreSame(newer, host.VersionControlStatus);
                StringAssert.Contains(host.VersionControlSummary, "newer");
            }
            Assert.AreEqual(boundary.StartsWith("reset-", StringComparison.Ordinal) ? 1 : 2, olderCalls);
        });
    }

    [TestMethod]
    public async Task CancellationDuringNotificationReset_StopsBeforeVcsFetch()
    {
        await WithWorkspaceAsync(async (_, solution, _) =>
        {
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            runner.RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(_ => Task.FromException<SvnCommandResult>(
                    new VersionControlClientNotFoundException("svn", new Win32Exception(2))));
            MainWindowViewModel host = CreateHost(runner);
            await host.OpenWorkspaceFileAsync(solution);
            using CancellationTokenSource cancellation = new();
            host.Editor.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(host.Editor.NotificationText)
                    && host.Editor.NotificationText == string.Empty)
                {
                    cancellation.Cancel();
                }
            };

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => host.OpenWorkspaceFileAsync(solution, cancellation.Token));
            StringAssert.Contains(host.VersionControlSummary, "svn");
            await runner.Received(1).RunAsync(Arg.Any<string>(), Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
        });
    }

    private static Task<VersionControlStatus> MissingStatus(string client) =>
        Task.FromException<VersionControlStatus>(new VersionControlClientNotFoundException(client, new Win32Exception(2)));

    [TestMethod]
    public async Task MissingGitExecutable_IsDistinguishedFromMissingWorkingDirectory()
    {
        await WithWorkspaceAsync(async (root, _, _) =>
        {
            GitCommandRunner runner = new(Path.Combine(root, "no-such-git-client"));
            await Assert.ThrowsExactlyAsync<VersionControlClientNotFoundException>(() => runner.RunAsync(root, "status"));
            await Assert.ThrowsExactlyAsync<Win32Exception>(() => runner.RunAsync(Path.Combine(root, "no-directory"), "status"));
        });
    }

    private static MainWindowViewModel CreateHost(ISvnCommandRunner runner)
    {
        IAvaloniaEditorComponent editor = Substitute.For<IAvaloniaEditorComponent>();
        editor.EditorViewModel.Returns(new EditorViewModel());
        return new MainWindowViewModel(editor, new PlanningExplorerViewModel(new MarkdownPlanningReader()),
            new DiagnosticCollectionViewModel(), null, null,
            versionControlService: new WorkspaceVersionControlService(
                new GitVersionControlService(Substitute.For<IGitCommandRunner>()),
                new SvnVersionControlService(runner), new NullVersionControlService()));
    }

    private static async Task WithWorkspaceAsync(Func<string, string, string, Task> action)
    {
        string root = Path.Combine(Directory.GetCurrentDirectory(), $"missing-vcs-{Guid.NewGuid():N}");
        string workspace = Path.Combine(root, "workspace");
        Directory.CreateDirectory(Path.Combine(root, ".svn"));
        Directory.CreateDirectory(workspace);
        string file = Path.Combine(workspace, "Readme.txt");
        string solution = Path.Combine(workspace, "Sample.slnx");
        try
        {
            await File.WriteAllTextAsync(file, "still available");
            await File.WriteAllTextAsync(Path.Combine(workspace, "App.csproj"), "<Project />");
            await File.WriteAllTextAsync(solution, "<Solution><Project Path=\"App.csproj\" /><Folder Name=\"/Files/\"><File Path=\"Readme.txt\" /></Folder></Solution>");
            await action(root, solution, file);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private sealed class TestEditorComponentFactory : IAvaloniaEditorComponentFactory
    {
        public IAvaloniaEditorComponent Create()
        {
            EditorWorkflow workflow = new(new FileEditorDocument(), new DesignEditorFileDialogService(), new FileSystemTextDocumentStorageService());
            return new AvaloniaEditorComponent(workflow, new EditorViewModel(workflow), new TextBlock());
        }

        IEditorComponent IEditorComponentFactory.Create() => Create();
    }
}
#endif
