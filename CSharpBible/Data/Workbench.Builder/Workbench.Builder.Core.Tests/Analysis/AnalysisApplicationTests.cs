using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Workbench.Builder.Analysis;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Core.Models.Diagnostics;

namespace Workbench.Builder.Core.Tests.Analysis;

/// <summary>
/// Verifies orchestration behavior for the analysis host.
/// </summary>
[TestClass]
public class AnalysisApplicationTests
{
    /// <summary>
    /// Verifies that a valid command invokes inspection and writes formatted output.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_ValidArguments_FormatsInspectionResultAsync()
    {
        AnalysisCommandLineParser parser = new();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IProjectInspectionFormatter formatter = Substitute.For<IProjectInspectionFormatter>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult result = CreateResult();

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(result);
        formatter.Format(result, ProjectInspectionOutputFormat.Json).Returns("{\"ok\":true}");
        AnalysisApplication application = new(parser, inspectionService, formatter, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj", AnalysisArgumentNames.Format, "json" });

        Assert.AreEqual(HostExitCodes.Success, exitCode);
        inspectionService.Received(1).Inspect(Arg.Is<ProjectLoadRequest>(request => request.ProjectFilePath == "sample.csproj"));
        formatter.Received(1).Format(result, ProjectInspectionOutputFormat.Json);
        console.Received(1).WriteLine("{\"ok\":true}");
    }

    /// <summary>
    /// Verifies that missing project arguments produce usage output and an invalid-arguments exit code.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_MissingProjectPath_ReturnsInvalidArgumentsAsync()
    {
        IHostConsole console = Substitute.For<IHostConsole>();
        AnalysisApplication application = new(
            new AnalysisCommandLineParser(),
            Substitute.For<IProjectInspectionService>(),
            Substitute.For<IProjectInspectionFormatter>(),
            console);

        int exitCode = await application.RunAsync([]);

        Assert.AreEqual(HostExitCodes.InvalidArguments, exitCode);
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Usage: Workbench.Builder.Analysis")));
    }

    /// <summary>
    /// Verifies that invalid format arguments are reported as command-line errors.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_InvalidFormat_ReturnsInvalidArgumentsAsync()
    {
        IHostConsole console = Substitute.For<IHostConsole>();
        AnalysisApplication application = new(
            new AnalysisCommandLineParser(),
            Substitute.For<IProjectInspectionService>(),
            Substitute.For<IProjectInspectionFormatter>(),
            console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj", AnalysisArgumentNames.Format, "yaml" });

        Assert.AreEqual(HostExitCodes.InvalidArguments, exitCode);
        console.Received(1).WriteErrorLine(Arg.Is<string>(text => text.Contains("not supported")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Usage: Workbench.Builder.Analysis")));
    }

    private static ProjectInspectionResult CreateResult()
    {
        BuildProjectInfo project = new(
            projectFilePath: @"C:\Temp\Sample\Sample.csproj",
            projectDirectory: @"C:\Temp\Sample",
            assemblyName: "Sample.Assembly",
            rootNamespace: "Sample.Namespace",
            targetFramework: "net10.0",
            outputType: "Exe",
            langVersion: "preview",
            nullable: "enable",
            defineConstants: "TRACE",
            implicitUsings: "enable",
            configuration: "Debug",
            runtimeIdentifier: null,
            outputPath: @"bin\Debug\net10.0\",
            intermediateOutputPath: @"obj\Debug\net10.0\",
            isSdkStyle: true,
            isPackable: false);

        return new ProjectInspectionResult(
            project,
            new[] { new CompileItemInfo("Program.cs", @"C:\Temp\Sample\Program.cs", true) },
            new[] { new ProjectReferenceInfo("..\\Shared\\Shared.csproj", @"C:\Temp\Shared\Shared.csproj", true) },
            System.Array.Empty<PackageReferenceInfo>(),
            new[] { new ResolvedReferenceInfo(ReferenceKind.Framework, "System.Runtime", "FrameworkReference", @"C:\Ref\System.Runtime.dll", true) },
            new[] { new BuildDiagnostic(BuildDiagnosticSeverity.Information, "WB0000", "Informational message") },
            isTestProject: false);
    }
}
