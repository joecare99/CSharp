#if NET10_0
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class SvnWorkingCopyOperationsTests
{
    [TestMethod]
    public async Task UpdateAsync_RunsScopedUpdateAndReturnsOutput()
    {
        string path = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "AvlnSvn", Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(path);
        try
        {
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            runner.RunAsync(path, Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SvnCommandResult { StandardOutput = "Updating 'sample':\nUpdated to revision 42." }));
            SvnWorkingCopyOperations operations = new(runner);

            string output = await operations.UpdateAsync(path);

            Assert.AreEqual("Updating 'sample':\nUpdated to revision 42.", output);
            await runner.Received(1).RunAsync(
                path,
                Arg.Is<IReadOnlyList<string>>(args => args.Count == 3
                    && args[0] == "update"
                    && args[1] == "--"
                    && args[2] == path),
                Arg.Any<CancellationToken>());
        }
        finally
        {
            Directory.Delete(path, recursive: true);
        }
    }

    [TestMethod]
    public async Task UpdateAsync_ReportsCommandFailure()
    {
        string path = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "AvlnSvn", Guid.NewGuid().ToString("N")));
        Directory.CreateDirectory(path);
        try
        {
            ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
            runner.RunAsync(path, Arg.Any<IReadOnlyList<string>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new SvnCommandResult
                {
                    ExitCode = 1,
                    StandardError = "update failed",
                }));
            SvnWorkingCopyOperations operations = new(runner);

            InvalidOperationException exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => operations.UpdateAsync(path));

            StringAssert.Contains(exception.Message, "update failed");
        }
        finally
        {
            Directory.Delete(path, recursive: true);
        }
    }

    [TestMethod]
    public async Task UpdateAsync_RejectsMissingDirectoryBeforeRunningSvn()
    {
        ISvnCommandRunner runner = Substitute.For<ISvnCommandRunner>();
        SvnWorkingCopyOperations operations = new(runner);

        await Assert.ThrowsExactlyAsync<DirectoryNotFoundException>(() =>
            operations.UpdateAsync(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))));

        await runner.DidNotReceive().RunAsync(
            Arg.Any<string>(),
            Arg.Any<IReadOnlyList<string>>(),
            Arg.Any<CancellationToken>());
    }
}
#endif
