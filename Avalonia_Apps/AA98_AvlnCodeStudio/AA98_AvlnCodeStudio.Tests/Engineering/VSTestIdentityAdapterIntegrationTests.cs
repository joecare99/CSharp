using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class VSTestIdentityAdapterIntegrationTests
{
    [TestMethod]
    public async Task SameIdWithDifferentResultName_IsRejectedByRealVSTestRun()
    {
        DirectoryInfo? bin = new(AppContext.BaseDirectory);
        while (bin is not null && !string.Equals(bin.Name, "bin", StringComparison.OrdinalIgnoreCase))
        {
            bin = bin.Parent;
        }
        string root = Path.Combine(bin?.Parent?.FullName ?? AppContext.BaseDirectory, $"vstest-identity-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "Identity.TestAdapter.csproj");
            await File.WriteAllTextAsync(project, FixtureProject);
            await File.WriteAllTextAsync(Path.Combine(root, "Adapter.cs"), FixtureSource);
            ProcessStartInfo start = new("dotnet") { WorkingDirectory = root, RedirectStandardOutput = true, RedirectStandardError = true };
            foreach (string name in new[] { "HostArtifactRoot", "CustomAfterDirectoryBuildProps", "DirectoryBuildPropsPath" })
            {
                start.Environment.Remove(name);
            }
            string intermediate = Path.Combine(root, "obj") + Path.DirectorySeparatorChar;
            string outputRoot = Path.Combine(root, "bin") + Path.DirectorySeparatorChar;
            foreach (string argument in new[]
            {
                "build", project, "--nologo", "--verbosity", "quiet",
                $"-p:BaseOutputPath={outputRoot}",
                $"-p:BaseIntermediateOutputPath={intermediate}",
                $"-p:MSBuildProjectExtensionsPath={intermediate}",
                "-p:ImportDirectoryBuildProps=false",
                "-p:ImportDirectoryBuildTargets=false",
            })
            {
                start.ArgumentList.Add(argument);
            }
            using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture build.");
            Task<string> stdout = process.StandardOutput.ReadToEndAsync();
            Task<string> stderr = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            Assert.AreEqual(0, process.ExitCode, $"{await stdout}\n{await stderr}");

            string output = Path.Combine(root, "bin", "Debug", $"net{Environment.Version.Major}.0");
            string assembly = Path.Combine(output, "Identity.TestAdapter.dll");
            string mode = Path.Combine(output, "mode.txt");
            string trace = Path.Combine(output, "trace.txt");
            DotNetTestDiscoveryService service = new();
            await File.WriteAllTextAsync(mode, "normal");
            TestDiscoveryResult discovery = await service.DiscoverAsync(assembly, output);
            DiscoveredTestCase selected = discovery.Cases.Single(test => test.FullyQualifiedName == "Identity.Selected");
            TestRunSummary passed = await service.RunCaseAsync(assembly, output, selected);
            Assert.AreEqual(TestRunOutcome.Passed, passed.Outcome);
            Assert.AreEqual(selected.Id, passed.Cases.Single().TestId);

            await File.WriteAllTextAsync(trace, "");
            await File.WriteAllTextAsync(mode, "different-name");
            InvalidOperationException error = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.RunCaseAsync(assembly, output, selected));
            StringAssert.Contains(error.Message, "different adapter case");
            Assert.AreEqual($"Identity.Selected|Identity.Other|{selected.Id}", (await File.ReadAllTextAsync(trace)).Trim());
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string FixtureProject => $"""
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net{Environment.Version.Major}.0</TargetFramework>
            <IsTestProject>true</IsTestProject>
            <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
          </PropertyGroup>
          <ItemGroup>
            <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.12.0" />
          </ItemGroup>
        </Project>
        """;

    private const string FixtureSource = """
        using System;
        using System.Collections.Generic;
        using System.IO;
        using Microsoft.VisualStudio.TestPlatform.ObjectModel;
        using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

        namespace Identity;

        [FileExtension(".dll")]
        [DefaultExecutorUri("executor://identity/v1")]
        public sealed class Discoverer : ITestDiscoverer
        {
            public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext context, Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.IMessageLogger logger, ITestCaseDiscoverySink sink)
            {
                foreach (string source in sources)
                    sink.SendTestCase(new TestCase("Identity.Selected", new Uri("executor://identity/v1"), source));
            }
        }

        [ExtensionUri("executor://identity/v1")]
        public sealed class Executor : ITestExecutor
        {
            public void Cancel() { }
            public void RunTests(IEnumerable<string> sources, IRunContext context, IFrameworkHandle handle) => throw new NotSupportedException();
            public void RunTests(IEnumerable<TestCase> tests, IRunContext context, IFrameworkHandle handle)
            {
                foreach (TestCase test in tests)
                {
                    string directory = Path.GetDirectoryName(test.Source);
                    TestCase resultCase = File.ReadAllText(Path.Combine(directory, "mode.txt")) == "different-name"
                        ? new TestCase("Identity.Other", test.ExecutorUri, test.Source) { Id = test.Id }
                        : test;
                    File.AppendAllText(Path.Combine(directory, "trace.txt"), test.FullyQualifiedName + "|" + resultCase.FullyQualifiedName + "|" + resultCase.Id);
                    handle.RecordResult(new TestResult(resultCase) { Outcome = TestOutcome.Passed });
                }
            }
        }
        """;
}
