using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class DotNetTestExecutionServiceTests
{
    [TestMethod]
    public async Task RunTestsAsync_ReportsPassedFailedAndSkippedTestsAndHonorsTargets()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_DotNetTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.Tests.csproj");
            await File.WriteAllTextAsync(projectPath,
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net{Environment.Version.Major}.0</TargetFramework>
                    <IsTestProject>true</IsTestProject>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.11.0" />
                    <PackageReference Include="MSTest" Version="4.5.1" />
                  </ItemGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(Path.Combine(root, "RunnerTests.cs"),
                """
                using Microsoft.VisualStudio.TestTools.UnitTesting;

                namespace TemporaryTests;

                [TestClass]
                public sealed class RunnerTests
                {
                    [TestMethod]
                    public void PassingTest() => Assert.IsTrue(true);

                    [TestMethod]
                    public void PassingTestExtra() => Assert.IsTrue(true);

                    [TestMethod]
                    public void FailingTest() => Assert.Fail("Expected test failure.");

                    [TestMethod]
                    [Ignore("Used to verify skipped test reporting.")]
                    public void SkippedTest() => Assert.Fail("Ignored tests must not execute.");
                }
                """);

            DotNetTestExecutionService service = new();
            TestRunSummary allResults = await service.RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = root,
                ProjectPath = projectPath,
            });

            Assert.AreEqual(4, allResults.TotalCount);
            Assert.AreEqual(2, allResults.PassedCount);
            Assert.AreEqual(1, allResults.FailedCount);
            Assert.AreEqual(1, allResults.SkippedCount);
            Assert.AreEqual(TestRunOutcome.Partial, allResults.Outcome);
            Assert.AreEqual(4, allResults.Cases.Count);
            Assert.AreEqual("Passed", allResults.Cases.Single(result => result.TestName == "PassingTest").Outcome);
            Assert.AreEqual("Failed", allResults.Cases.Single(result => result.TestName == "FailingTest").Outcome);
            Assert.AreEqual("NotExecuted", allResults.Cases.Single(result => result.TestName == "SkippedTest").Outcome);
            Assert.AreEqual("TemporaryTests.RunnerTests.PassingTest",
                allResults.Cases.Single(result => result.TestName == "PassingTest").QualifiedTestName);
            Assert.IsTrue(allResults.Cases.All(result => result.TestId is not null && result.ExecutionId is not null));
            TestRunFailure failure = allResults.Failures.Single();
            Assert.AreEqual("FailingTest", failure.TestName);
            StringAssert.Contains(failure.Message, "Expected test failure.");
            Assert.AreEqual(Path.GetFullPath(Path.Combine(root, "RunnerTests.cs")), failure.SourcePath);
            int expectedLine = Array.FindIndex(
                File.ReadAllLines(Path.Combine(root, "RunnerTests.cs")),
                static line => line.Contains("Assert.Fail(\"Expected test failure.\")", StringComparison.Ordinal)) + 1;
            Assert.AreEqual(expectedLine, failure.LineNumber);

            TestRunSummary filteredResults = await service.RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = root,
                ProjectPath = projectPath,
                Configuration = "Release",
                Targets = { "TemporaryTests.RunnerTests.PassingTest" },
            });

            Assert.AreEqual(2, filteredResults.TotalCount);
            Assert.AreEqual(2, filteredResults.PassedCount);
            Assert.AreEqual(0, filteredResults.FailedCount);
            Assert.AreEqual(TestRunOutcome.Passed, filteredResults.Outcome);
            Assert.IsTrue(File.Exists(Path.Combine(root, "bin", "Release", $"net{Environment.Version.Major}.0", "Sample.Tests.dll")));

            TestRunSummary exactResults = await service.RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = root,
                ProjectPath = projectPath,
                Configuration = "Release",
                MatchTargetsExactly = true,
                Targets = { "TemporaryTests.RunnerTests.PassingTest" },
            });
            Assert.AreEqual(1, exactResults.TotalCount);
            Assert.AreEqual(1, exactResults.PassedCount);
            await Assert.ThrowsExactlyAsync<ArgumentException>(() => service.RunTestsAsync(new TestRunRequest
            {
                ProjectPath = projectPath,
                MatchTargetsExactly = true,
                Targets = { "  " },
            }));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task RunTestsAsync_RejectsMissingProject()
    {
        DotNetTestExecutionService service = new();
        await Assert.ThrowsExactlyAsync<FileNotFoundException>(() =>
            service.RunTestsAsync(new TestRunRequest { ProjectPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csproj") }));
    }

    [TestMethod]
    public async Task Discovery_RejectsMissingAdaptersAndHonorsPreCancellation()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_DiscoveryValidation_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string assembly = Path.Combine(root, "Sample.dll");
            await File.WriteAllTextAsync(assembly, string.Empty);
            DotNetTestDiscoveryService service = new();
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.DiscoverAsync(assembly, root));
            using CancellationTokenSource canceled = new();
            canceled.Cancel();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => service.DiscoverAsync(assembly, root, canceled.Token));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
