using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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

            Assert.AreEqual(3, allResults.TotalCount);
            Assert.AreEqual(1, allResults.PassedCount);
            Assert.AreEqual(1, allResults.FailedCount);
            Assert.AreEqual(1, allResults.SkippedCount);
            Assert.AreEqual(TestRunOutcome.Partial, allResults.Outcome);

            TestRunSummary filteredResults = await service.RunTestsAsync(new TestRunRequest
            {
                WorkspaceRootPath = root,
                ProjectPath = projectPath,
                Targets = { "TemporaryTests.RunnerTests.PassingTest" },
            });

            Assert.AreEqual(1, filteredResults.TotalCount);
            Assert.AreEqual(1, filteredResults.PassedCount);
            Assert.AreEqual(0, filteredResults.FailedCount);
            Assert.AreEqual(TestRunOutcome.Passed, filteredResults.Outcome);
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
}
