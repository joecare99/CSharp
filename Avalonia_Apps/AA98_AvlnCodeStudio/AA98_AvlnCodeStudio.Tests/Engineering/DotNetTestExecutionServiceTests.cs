using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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

    [TestMethod]
    public async Task Discovery_MissingAdapterReportsTestingPlatformOnlyWhenManifestProvidesEvidence()
    {
        string root = Path.Combine(AppContext.BaseDirectory, $"PlatformDetection_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string assembly = Path.Combine(root, "Sample.dll");
            string deps = Path.ChangeExtension(assembly, ".deps.json");
            await File.WriteAllTextAsync(assembly, string.Empty);
            await File.WriteAllTextAsync(deps, TestingPlatformDependencies);
            DotNetTestDiscoveryService service = new();
            DiscoveredTestCase testCase = new("id", "executor://sample", assembly, "Sample.Test", "Test", null, null);

            InvalidOperationException error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.DiscoverAsync(assembly, root));
            StringAssert.Contains(error.Message, "Microsoft.Testing.Platform-only projects are not supported");
            StringAssert.Contains(error.Message, "runner");
            error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.RunCaseAsync(assembly, root, testCase));
            StringAssert.Contains(error.Message, "Microsoft.Testing.Platform-only projects are not supported");
            error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.DebugCaseAsync(assembly, root, testCase, (_, _) => throw new AssertFailedException("Unexpected attach")));
            StringAssert.Contains(error.Message, "Microsoft.Testing.Platform-only projects are not supported");

            await File.WriteAllTextAsync(deps,
                """{"runtimeTarget":{"name":".NETCoreApp,Version=v10.0/win-x64"},"targets":{".NETCoreApp,Version=v10.0":{},".NETCoreApp,Version=v10.0/win-x64":{"Microsoft.Testing.Platform/2.5.1":{"runtime":{"lib/net9.0/Microsoft.Testing.Platform.dll":{"assemblyVersion":"2.5.1.0"}}}}}}""");
            error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.DiscoverAsync(assembly, root));
            StringAssert.Contains(error.Message, "Microsoft.Testing.Platform-only projects are not supported");

            await File.WriteAllTextAsync(deps,
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"runtime":{"Microsoft.Testing.Platform.dll":{}}}}}}""");
            error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.DiscoverAsync(assembly, root));
            StringAssert.Contains(error.Message, "Microsoft.Testing.Platform-only projects are not supported");

            foreach (string? uncertain in new[]
            {
                null,
                "{invalid json",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.NET.Test.Sdk/1.0":{}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"other":{"Microsoft.Testing.Platform/1.0":{"runtime":{"lib/Microsoft.Testing.Platform.dll":{}}}},"active":{}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"other":{"Microsoft.Testing.Platform/1.0":{"runtime":{"Microsoft.Testing.Platform.dll":{}}}},"active":{}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"dependencies":{}}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"runtimeTargets":{"runtimes/win/lib/Microsoft.Testing.Platform.dll":{"assetType":"runtime","rid":"win"}}}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"runtimeTargets":{"Microsoft.Testing.Platform.dll":{"assetType":"runtime","rid":"other"}}}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"runtime":{"NotMicrosoft.Testing.Platform.dll":{}}}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform.Extensions/1.0":{"runtime":{"lib/Microsoft.Testing.Platform.dll":{}}}}}}""",
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/1.0":{"resources":{"lib/Microsoft.Testing.Platform.dll":{}}}}}}""",
            })
            {
                if (uncertain is null)
                {
                    File.Delete(deps);
                }
                else
                {
                    await File.WriteAllTextAsync(deps, uncertain);
                }

                error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => service.DiscoverAsync(assembly, root));
                Assert.AreEqual("No VSTest adapter was found beside the built test assembly.", error.Message);
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Discovery_EmptyResultsWarnAboutPlatformWithoutClassifyingCoexistence()
    {
        string root = Path.Combine(AppContext.BaseDirectory, $"PlatformWarning_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string assembly = Path.Combine(root, "Sample.dll");
            string deps = Path.ChangeExtension(assembly, ".deps.json");
            await File.WriteAllTextAsync(deps, TestingPlatformDependencies);
            TestDiscoveryResult empty = new([], ["Adapter warning"]);
            TestDiscoveryResult warned = await DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, CancellationToken.None);
            Assert.AreEqual(2, warned.Warnings.Count);
            Assert.AreEqual("Adapter warning", warned.Warnings[0]);
            StringAssert.Contains(warned.Warnings[1], "Microsoft.Testing.Platform-only projects are not supported");
            StringAssert.Contains(warned.Warnings[1], "VSTest adapter");

            await File.WriteAllTextAsync(deps, TestingPlatformDependencies.Replace(
                "\"Microsoft.Testing.Platform/2.0.0\"", "\"Microsoft.TestPlatform.TestHost/1.0\":{},\"Microsoft.Testing.Platform/2.0.0\""));
            TestDiscoveryResult coexistence = new(
                [new DiscoveredTestCase("id", "executor://sample", assembly, "Sample.Test", "Test", null, null)],
                ["Existing warning"]);
            Assert.AreSame(coexistence, await DotNetTestDiscoveryService.AddPlatformWarningAsync(coexistence, assembly, CancellationToken.None));

            await File.WriteAllTextAsync(deps,
                """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.TestPlatform.TestHost/1.0":{},"Microsoft.Testing.Platform/2.0.0":{"runtime":{"Microsoft.Testing.Platform.dll":{}}}}}}""");
            Assert.AreEqual(2, (await DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, CancellationToken.None)).Warnings.Count);
            Assert.AreSame(coexistence, await DotNetTestDiscoveryService.AddPlatformWarningAsync(coexistence, assembly, CancellationToken.None));

            File.Delete(deps);
            Assert.AreSame(empty, await DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, CancellationToken.None));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Discovery_PlatformMetadataIsBoundedAndCancellationIsNotSwallowed()
    {
        string root = Path.Combine(AppContext.BaseDirectory, $"PlatformBounds_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string assembly = Path.Combine(root, "Sample.dll");
            string deps = Path.ChangeExtension(assembly, ".deps.json");
            TestDiscoveryResult empty = new([], []);
            using CancellationTokenSource canceled = new();
            canceled.Cancel();

            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, canceled.Token));
            TestDiscoveryResult nonempty = new(
                [new DiscoveredTestCase("id", "executor://sample", assembly, "Sample.Test", "Test", null, null)], []);
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => DotNetTestDiscoveryService.AddPlatformWarningAsync(nonempty, assembly, canceled.Token));

            using (FileStream stream = File.Create(deps))
            {
                stream.SetLength(16 * 1024 * 1024 + 1);
            }

            Assert.AreSame(empty, await DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, CancellationToken.None));
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, canceled.Token));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task Discovery_VSTestAndTestingPlatformCoexistenceStillDiscoversCases()
    {
        string assembly = typeof(DotNetTestExecutionServiceTests).Assembly.Location;
        using JsonDocument deps = JsonDocument.Parse(await File.ReadAllTextAsync(Path.ChangeExtension(assembly, ".deps.json")));
        JsonElement root = deps.RootElement;
        string target = root.GetProperty("runtimeTarget").GetProperty("name").GetString()!;
        Assert.IsTrue(root.GetProperty("targets").GetProperty(target).EnumerateObject().Any(library =>
            library.Name.StartsWith("Microsoft.Testing.Platform/", StringComparison.OrdinalIgnoreCase)
            && library.Value.TryGetProperty("runtime", out JsonElement runtime)
            && runtime.EnumerateObject().Any(asset => asset.Name.EndsWith("/Microsoft.Testing.Platform.dll", StringComparison.OrdinalIgnoreCase))));
        TestDiscoveryResult empty = new([], []);
        Assert.AreEqual(1, (await DotNetTestDiscoveryService.AddPlatformWarningAsync(empty, assembly, CancellationToken.None)).Warnings.Count);
        TestDiscoveryResult result = await new DotNetTestDiscoveryService().DiscoverAsync(assembly, AppContext.BaseDirectory);
        Assert.IsTrue(result.Cases.Any(test => test.FullyQualifiedName.Contains(
            nameof(DotNetTestExecutionServiceTests), StringComparison.Ordinal)));
        Assert.IsFalse(result.Warnings.Any(warning => warning.Contains(
            "Microsoft.Testing.Platform-only projects are not supported", StringComparison.Ordinal)));
    }

    private const string TestingPlatformDependencies =
        """{"runtimeTarget":{"name":"active"},"targets":{"active":{"Microsoft.Testing.Platform/2.0.0":{"runtime":{"lib/net8.0/Microsoft.Testing.Platform.dll":{}}}}}}""";

    [TestMethod]
    public async Task AdapterResolver_IncludesExplicitAdaptersOutsideAssemblyOutput()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_ExternalAdapters_{Guid.NewGuid():N}");
        string output = Path.Combine(root, "bin", "Debug", "net10.0");
        string externalAdapters = Path.Combine(root, "TestAdapters");
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(externalAdapters);
        try
        {
            string adjacentAdapter = Path.Combine(output, "Local.TestAdapter.dll");
            string externalAdapter = Path.Combine(externalAdapters, "External.TestAdapter.dll");
            await File.WriteAllBytesAsync(adjacentAdapter, []);
            await File.WriteAllBytesAsync(externalAdapter, []);
            await File.WriteAllTextAsync(Path.Combine(root, "vstest-adapters.json"),
                """{"adapterPaths":["TestAdapters/External.TestAdapter.dll"]}""");

            string[] adapters = await VSTestAdapterResolver.ResolveAsync(
                Path.Combine(output, "Sample.Tests.dll"), root, CancellationToken.None);

            CollectionAssert.AreEquivalent(new[] { adjacentAdapter, externalAdapter }, adapters);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [TestMethod]
    public async Task AdapterResolver_RejectsAbsoluteAndOutOfScopeConfiguredPaths()
    {
        string root = Path.Combine(Path.GetTempPath(), $"AA98_InvalidAdapters_{Guid.NewGuid():N}");
        string outside = Path.Combine(Path.GetDirectoryName(root)!, $"AA98_Outside_{Guid.NewGuid():N}.TestAdapter.dll");
        Directory.CreateDirectory(root);
        await File.WriteAllBytesAsync(outside, []);
        try
        {
            foreach (string configuredPath in new[] { outside, Path.Combine("..", Path.GetFileName(outside)) })
            {
                await File.WriteAllTextAsync(Path.Combine(root, "vstest-adapters.json"),
                    JsonSerializer.Serialize(new { adapterPaths = new[] { configuredPath } }));

                await Assert.ThrowsExactlyAsync<InvalidDataException>(() =>
                    VSTestAdapterResolver.ResolveAsync(
                        Path.Combine(root, "bin", "Sample.Tests.dll"), root, CancellationToken.None));
            }
        }
        finally
        {
            Directory.Delete(root, recursive: true);
            File.Delete(outside);
        }
    }
}
