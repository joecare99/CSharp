using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class VSTestErrorAdapterIntegrationTests
{
    [TestMethod]
    public async Task AdapterErrorEvents_RejectDiscoveryAndSelectedRun()
    {
        DirectoryInfo? bin = new(AppContext.BaseDirectory);
        while (bin is not null && !string.Equals(bin.Name, "bin", StringComparison.OrdinalIgnoreCase))
        {
            bin = bin.Parent;
        }
        string root = Path.Combine(bin?.Parent?.FullName ?? AppContext.BaseDirectory, $"vstest-error-fixture-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string project = Path.Combine(root, "Controlled.TestAdapter.csproj");
            await File.WriteAllTextAsync(project, FixtureProject);
            await File.WriteAllTextAsync(Path.Combine(root, "Adapter.cs"), FixtureSource);
            ProcessStartInfo start = new("dotnet") { WorkingDirectory = root, RedirectStandardOutput = true, RedirectStandardError = true };
            string inheritedProps = Path.Combine(root, "inherited.props");
            string inheritedRoot = Path.Combine(root, "inherited-artifacts");
            await File.WriteAllTextAsync(inheritedProps, """
                <Project><PropertyGroup>
                  <BaseOutputPath>$(HostArtifactRoot)\bin\</BaseOutputPath>
                  <BaseIntermediateOutputPath>$(HostArtifactRoot)\obj\</BaseIntermediateOutputPath>
                </PropertyGroup></Project>
                """);
            start.Environment["HostArtifactRoot"] = inheritedRoot;
            start.Environment["CustomAfterDirectoryBuildProps"] = inheritedProps;
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
            Assert.IsFalse(Directory.Exists(inheritedRoot), "Fixture build used the inherited artifact root.");

            string output = Path.Combine(root, "bin", "Debug", $"net{Environment.Version.Major}.0");
            string assembly = Path.Combine(output, "Controlled.TestAdapter.dll");
            string mode = Path.Combine(output, "mode.txt");
            string trace = Path.Combine(output, "trace.txt");
            DotNetTestDiscoveryService service = new();

            await File.WriteAllTextAsync(mode, "normal");
            TestDiscoveryResult discovery = await service.DiscoverAsync(assembly, output);
            DiscoveredTestCase selected = discovery.Cases.Single(test => test.FullyQualifiedName == "Controlled.Example");
            TestRunSummary passed = await service.RunCaseAsync(assembly, output, selected);
            Assert.AreEqual(TestRunOutcome.Passed, passed.Outcome);
            StringAssert.Contains(await File.ReadAllTextAsync(trace), "run-result");

            string depsPath = Path.ChangeExtension(assembly, ".deps.json");
            JsonNode deps = JsonNode.Parse(await File.ReadAllTextAsync(depsPath))!;
            string targetName = deps["runtimeTarget"]!["name"]!.GetValue<string>();
            deps["targets"]![targetName]!["Microsoft.Testing.Platform/1.0"] = JsonNode.Parse(
                """{"runtimeTargets":{"runtimes/other/lib/net10.0/Microsoft.Testing.Platform.dll":{"assetType":"runtime","rid":"other"}}}""");
            await File.WriteAllTextAsync(depsPath, deps.ToJsonString());
            await File.WriteAllTextAsync(mode, "empty");
            TestDiscoveryResult empty = await service.DiscoverAsync(assembly, output);
            Assert.AreEqual(0, empty.Cases.Count);
            Assert.IsTrue(empty.Warnings.Any(warning => warning.Contains(
                "possible RID-specific Microsoft.Testing.Platform runtime asset", StringComparison.Ordinal)));
            await File.WriteAllTextAsync(mode, "normal");
            discovery = await service.DiscoverAsync(assembly, output);
            Assert.IsTrue(discovery.Cases.Any(test => test.FullyQualifiedName == "Controlled.Example"));
            Assert.IsFalse(discovery.Warnings.Any(warning => warning.Contains("possible RID-specific", StringComparison.Ordinal)));

            await File.WriteAllTextAsync(trace, "");
            await File.WriteAllTextAsync(mode, "discovery-error");
            InvalidOperationException discoveryError = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.DiscoverAsync(assembly, output));
            StringAssert.Contains(discoveryError.Message, "Controlled discovery failure.");
            string discoveryTrace = await File.ReadAllTextAsync(trace);
            StringAssert.Contains(discoveryTrace, "discovered");
            StringAssert.Contains(discoveryTrace, "discovery-rejected-null");
            StringAssert.Contains(discoveryTrace, "discovery-rejected-empty");
            StringAssert.Contains(discoveryTrace, "discovery-rejected-whitespace");

            await File.WriteAllTextAsync(trace, "");
            await File.WriteAllTextAsync(mode, "run-error");
            InvalidOperationException runError = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => service.RunCaseAsync(assembly, output, selected));
            StringAssert.Contains(runError.Message, "Controlled execution failure.");
            string runTrace = await File.ReadAllTextAsync(trace);
            StringAssert.Contains(runTrace, "run-result");
            StringAssert.Contains(runTrace, "run-rejected-null");
            StringAssert.Contains(runTrace, "run-rejected-empty");
            StringAssert.Contains(runTrace, "run-rejected-whitespace");
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
        using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

        namespace Controlled;

        [FileExtension(".dll")]
        [DefaultExecutorUri("executor://controlled/v1")]
        public sealed class Discoverer : ITestDiscoverer
        {
            public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext context, IMessageLogger logger, ITestCaseDiscoverySink sink)
            {
                foreach (string source in sources)
                {
                    if (File.ReadAllText(Path.Combine(Path.GetDirectoryName(source), "mode.txt")) == "empty")
                        continue;
                    sink.SendTestCase(new TestCase("Controlled.Example", new Uri("executor://controlled/v1"), source));
                    File.AppendAllText(Path.Combine(Path.GetDirectoryName(source), "trace.txt"), "discovered\n");
                    if (File.ReadAllText(Path.Combine(Path.GetDirectoryName(source), "mode.txt")) == "discovery-error")
                    {
                        string[] messages = { null, "", " \t" };
                        string[] names = { "null", "empty", "whitespace" };
                        for (int i = 0; i < messages.Length; i++)
                        {
                            try { logger.SendMessage(TestMessageLevel.Error, messages[i]); }
                            catch (ArgumentException) { File.AppendAllText(Path.Combine(Path.GetDirectoryName(source), "trace.txt"), "discovery-rejected-" + names[i] + "\n"); }
                        }
                        logger.SendMessage(TestMessageLevel.Error, "Controlled discovery failure.");
                    }
                }
            }
        }

        [ExtensionUri("executor://controlled/v1")]
        public sealed class Executor : ITestExecutor
        {
            public void Cancel() { }
            public void RunTests(IEnumerable<string> sources, IRunContext context, IFrameworkHandle handle) => throw new NotSupportedException();
            public void RunTests(IEnumerable<TestCase> tests, IRunContext context, IFrameworkHandle handle)
            {
                foreach (TestCase test in tests)
                {
                    handle.RecordResult(new TestResult(test) { Outcome = TestOutcome.Passed });
                    File.AppendAllText(Path.Combine(Path.GetDirectoryName(test.Source), "trace.txt"), "run-result\n");
                    if (File.ReadAllText(Path.Combine(Path.GetDirectoryName(test.Source), "mode.txt")) == "run-error")
                    {
                        string[] messages = { null, "", " \t" };
                        string[] names = { "null", "empty", "whitespace" };
                        for (int i = 0; i < messages.Length; i++)
                        {
                            try { handle.SendMessage(TestMessageLevel.Error, messages[i]); }
                            catch (ArgumentException) { File.AppendAllText(Path.Combine(Path.GetDirectoryName(test.Source), "trace.txt"), "run-rejected-" + names[i] + "\n"); }
                        }
                        handle.SendMessage(TestMessageLevel.Error, "Controlled execution failure.");
                    }
                }
            }
        }
        """;
}
