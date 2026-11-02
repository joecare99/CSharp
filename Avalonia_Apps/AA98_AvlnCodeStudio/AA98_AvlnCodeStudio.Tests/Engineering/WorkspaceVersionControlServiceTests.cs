#if NET10_0
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.UI.Services;
using AA98_AvlnCodeStudio.Versioning.Git.Services;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class WorkspaceVersionControlServiceTests
{
    [TestMethod]
    public async Task GetStatusAsync_SelectsSvnForSubversionWorkingCopy()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnVersionControl", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, ".svn"));
        try
        {
            ISvnCommandRunner svnRunner = Substitute.For<ISvnCommandRunner>();
            svnRunner.RunAsync(root, Arg.Any<System.Collections.Generic.IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult(new SvnCommandResult
                    {
                        StandardOutput = $"""
                            <info><entry path="{root}" kind="dir"><relative-url>^/trunk/sample</relative-url>
                            <wc-info><wcroot-abspath>{root}</wcroot-abspath></wc-info></entry></info>
                            """,
                    }),
                    Task.FromResult(new SvnCommandResult
                    {
                        StandardOutput = $"<status><target path=\"{root}\" /></status>",
                    }));
            IGitCommandRunner gitRunner = Substitute.For<IGitCommandRunner>();
            SvnVersionControlService svn = new(svnRunner);
            GitVersionControlService git = new(gitRunner);
            NullVersionControlService fallback = new();

            WorkspaceVersionControlService service = new(git, svn, fallback);
            VersionControlStatus actual = await service.GetStatusAsync(new VersionControlStatusRequest
            {
                RepositoryContextPath = root,
            });

            Assert.IsTrue(actual.IsRepositoryRootDiscovered);
            Assert.AreEqual("trunk/sample", actual.ActiveReferenceName);
            await svnRunner.Received(2).RunAsync(root, Arg.Any<System.Collections.Generic.IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
            await gitRunner.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetStatusAsync_PreservesGitAdapterSelection()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnVersionControl", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, ".git"));
        try
        {
            ISvnCommandRunner svnRunner = Substitute.For<ISvnCommandRunner>();
            IGitCommandRunner gitRunner = Substitute.For<IGitCommandRunner>();
            gitRunner.RunAsync(root, "rev-parse --show-toplevel", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new GitCommandResult
                {
                    ExitCode = 0,
                    StandardOutput = root,
                }));
            gitRunner.RunAsync(root, "branch --show-current", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new GitCommandResult
                {
                    ExitCode = 0,
                    StandardOutput = "main",
                }));
            gitRunner.RunAsync(root, "status --porcelain --ignored", Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new GitCommandResult { ExitCode = 0 }));
            SvnVersionControlService svn = new(svnRunner);
            GitVersionControlService git = new(gitRunner);
            WorkspaceVersionControlService service = new(git, svn, new NullVersionControlService());

            VersionControlStatus actual = await service.GetStatusAsync(new VersionControlStatusRequest
            {
                RepositoryContextPath = root,
            });

            Assert.AreEqual("main", actual.ActiveReferenceName);
            Assert.IsTrue(actual.IsRepositoryRootDiscovered);
            await svnRunner.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<System.Collections.Generic.IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task GetStatusAsync_SelectsFallbackWhenNoRepositoryMarkerExists()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnVersionControl", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            ISvnCommandRunner svnRunner = Substitute.For<ISvnCommandRunner>();
            IGitCommandRunner gitRunner = Substitute.For<IGitCommandRunner>();
            SvnVersionControlService svn = new(svnRunner);
            GitVersionControlService git = new(gitRunner);
            NullVersionControlService fallback = new();

            WorkspaceVersionControlService service = new(git, svn, fallback);
            VersionControlStatus actual = await service.GetStatusAsync(new VersionControlStatusRequest
            {
                RepositoryContextPath = root,
            });

            Assert.IsFalse(actual.IsRepositoryRootDiscovered);
            Assert.AreEqual(root, actual.RepositoryRootPath);
            await svnRunner.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<System.Collections.Generic.IReadOnlyList<string>>(), Arg.Any<CancellationToken>());
            await gitRunner.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
#endif
