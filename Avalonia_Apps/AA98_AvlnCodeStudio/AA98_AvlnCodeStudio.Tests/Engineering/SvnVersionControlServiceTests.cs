#if NET10_0
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class SvnVersionControlServiceTests
{
    [TestMethod]
    public async Task GetStatusAsync_MapsWorkingCopyReferenceAndChanges()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnSvn", Guid.NewGuid().ToString("N"));
        FakeSvnCommandRunner runner = new((_, arguments) =>
        {
            if (arguments[0] == "info")
            {
                return new SvnCommandResult
                {
                    StandardOutput = $$"""
                        <info>
                          <entry path="{{root}}" kind="dir">
                            <relative-url>^/trunk/AvlnSvn</relative-url>
                            <wc-info><wcroot-abspath>{{root}}</wcroot-abspath></wc-info>
                          </entry>
                        </info>
                        """,
                };
            }

            return new SvnCommandResult
            {
                StandardOutput = $$"""
                    <status>
                      <target path="{{root}}">
                        <entry path="{{Path.Combine(root, "src", "Program.cs")}}"><wc-status item="modified" props="none" /></entry>
                        <entry path="{{Path.Combine(root, "new file.cs")}}"><wc-status item="unversioned" props="none" /></entry>
                        <entry path="{{Path.Combine(root, "old.txt")}}"><wc-status item="deleted" props="none" /></entry>
                        <entry path="{{Path.Combine(root, "renamed.cs")}}"><wc-status item="added" props="none" moved-from="{{Path.Combine(root, "old-name.cs")}}" /></entry>
                        <entry path="{{Path.Combine(root, "ignored.bin")}}"><wc-status item="ignored" props="none" /></entry>
                      </target>
                    </status>
                    """,
            };
        });
        SvnVersionControlService service = new(runner);

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = root,
        });

        Assert.AreEqual(root, status.RepositoryRootPath);
        Assert.AreEqual(Path.GetFileName(root), status.RepositoryName);
        Assert.IsTrue(status.IsRepositoryRootDiscovered);
        Assert.AreEqual("trunk/AvlnSvn", status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Other, status.ActiveReferenceKind);
        Assert.IsTrue(status.HasLocalChanges);
        CollectionAssert.AreEquivalent(
            new[]
            {
                VersionControlCapability.InspectStatus,
                VersionControlCapability.InspectReferences,
                VersionControlCapability.EnumerateChanges,
                VersionControlCapability.UpdateWorkingCopy,
            },
            (System.Collections.ICollection)status.Capabilities);
        Assert.AreEqual(5, status.Changes.Count);
        Assert.AreEqual(Path.Combine("src", "Program.cs"), status.Changes[0].Path);
        Assert.AreEqual(VersionControlChangeKind.Modified, status.Changes[0].ChangeKind);
        Assert.AreEqual(VersionControlChangeKind.Added, status.Changes[1].ChangeKind);
        Assert.AreEqual(VersionControlChangeKind.Deleted, status.Changes[2].ChangeKind);
        Assert.AreEqual(VersionControlChangeKind.Moved, status.Changes[3].ChangeKind);
        Assert.AreEqual("old-name.cs", status.Changes[3].PreviousPath);
        Assert.IsTrue(status.Changes[4].IsIgnored);
        Assert.IsFalse(status.Changes[4].IsStaged);
    }

    [TestMethod]
    public async Task GetStatusAsync_SkipsChangeEnumerationWhenDisabled()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnSvn", Guid.NewGuid().ToString("N"));
        FakeSvnCommandRunner runner = new((_, arguments) => arguments[0] == "info"
            ? new SvnCommandResult
            {
                StandardOutput = $$"""
                    <info><entry path="{{root}}" kind="dir"><relative-url>^/trunk</relative-url>
                    <wc-info><wcroot-abspath>{{root}}</wcroot-abspath></wc-info></entry></info>
                    """,
            }
            : throw new AssertFailedException("SVN status should not run when changes are disabled."));
        SvnVersionControlService service = new(runner);

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = root,
            IncludeChanges = false,
            IncludeCapabilities = false,
        });

        Assert.IsTrue(status.IsRepositoryRootDiscovered);
        Assert.AreEqual(0, status.Changes.Count);
        Assert.AreEqual(0, status.Capabilities.Count);
        Assert.AreEqual(1, runner.Calls.Count);
    }

    [TestMethod]
    public async Task GetStatusAsync_LeavesReferenceUnknownAtRepositoryRoot()
    {
        string root = Path.Combine(Path.GetTempPath(), "AvlnSvn", Guid.NewGuid().ToString("N"));
        FakeSvnCommandRunner runner = new((_, _) => new SvnCommandResult
        {
            StandardOutput = $$"""
                <info><entry path="{{root}}" kind="dir"><relative-url>^/</relative-url>
                <wc-info><wcroot-abspath>{{root}}</wcroot-abspath></wc-info></entry></info>
                """,
        });
        SvnVersionControlService service = new(runner);

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = root,
            IncludeChanges = false,
        });

        Assert.IsNull(status.ActiveReferenceName);
        Assert.AreEqual(VersionControlReferenceKind.Unknown, status.ActiveReferenceKind);
    }

    [TestMethod]
    public async Task GetStatusAsync_ReturnsFallbackStatusWhenInfoCannotDiscoverWorkingCopy()
    {
        FakeSvnCommandRunner runner = new((_, _) => new SvnCommandResult
        {
            ExitCode = 1,
            StandardError = "not a working copy",
        });
        SvnVersionControlService service = new(runner);
        string path = Path.Combine(Path.GetTempPath(), "not-a-working-copy");

        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = path,
        });

        Assert.IsFalse(status.IsRepositoryRootDiscovered);
        Assert.AreEqual(path, status.RepositoryRootPath);
        Assert.AreEqual(0, status.Changes.Count);
    }

    [TestMethod]
    public async Task GetStatusAsync_ReadsCurrentSvnWorkingCopyWhenAvailable()
    {
        try
        {
            _ = await new SvnCommandRunner().RunAsync(
                Directory.GetCurrentDirectory(),
                ["--version", "--quiet"]);
        }
        catch (Win32Exception)
        {
            Assert.Inconclusive("The SVN command-line client is not available for the integration test.");
        }

        string? workingCopyPath = Directory.GetCurrentDirectory();
        while (!string.IsNullOrWhiteSpace(workingCopyPath)
            && !Directory.Exists(Path.Combine(workingCopyPath, ".svn")))
        {
            workingCopyPath = Path.GetDirectoryName(workingCopyPath);
        }

        if (string.IsNullOrWhiteSpace(workingCopyPath))
        {
            Assert.Inconclusive("No SVN working copy is available for the integration test.");
        }

        string? configuredWorkspacePath = Environment.GetEnvironmentVariable("AVLN_SVN_WORKSPACE_PATH");
        bool includeChanges = Directory.Exists(configuredWorkspacePath);
        SvnVersionControlService service = new(new SvnCommandRunner());
        VersionControlStatus status = await service.GetStatusAsync(new VersionControlStatusRequest
        {
            RepositoryContextPath = includeChanges ? configuredWorkspacePath : workingCopyPath,
            IncludeChanges = includeChanges,
        });

        Assert.IsTrue(status.IsRepositoryRootDiscovered);
        Assert.IsTrue(status.Capabilities.Contains(VersionControlCapability.EnumerateChanges));
        if (includeChanges)
        {
            Assert.IsTrue(status.Changes.All(static change => !Path.IsPathRooted(change.Path)));
            Assert.AreEqual(
                status.Changes.Any(static change => !change.IsIgnored
                    && change.ChangeKind != VersionControlChangeKind.Unknown),
                status.HasLocalChanges);
        }
    }

    private sealed class FakeSvnCommandRunner(
        Func<string, IReadOnlyList<string>, SvnCommandResult> execute) : ISvnCommandRunner
    {
        public List<IReadOnlyList<string>> Calls { get; } = [];

        public Task<SvnCommandResult> RunAsync(
            string workingDirectory,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Calls.Add(arguments);
            return Task.FromResult(execute(workingDirectory, arguments));
        }
    }
}
#endif
