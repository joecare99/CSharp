using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Workbench.Builder.Cli;
using Workbench.Builder.Core.Abstractions;
using Workbench.Builder.Core.Models.Compilation;
using Workbench.Builder.Core.Models.Diagnostics;
using Workbench.Builder.Core.Models.Inspection;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Models.Projects;
using Workbench.Builder.Core.Models.References;
using Workbench.Builder.Host;

namespace Workbench.Builder.Core.Tests.Host;

/// <summary>
/// Verifies orchestration behavior for the compile host.
/// </summary>
[TestClass]
public class HostApplicationTests
{
    /// <summary>
    /// Verifies that a valid command invokes inspection and compilation and writes emit output.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_ValidArguments_CompilesProjectAsync()
    {
        HostCommandLineParser parser = new();
        IProjectCompilationService compilationService = Substitute.For<IProjectCompilationService>();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult inspectionResult = CreateResult();
        ProjectCompilationResult compilationResult = CreateCompilationResult(inspectionResult, succeeded: true);

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(inspectionResult);
        compilationService.Compile(Arg.Any<ProjectCompilationRequest>()).Returns(compilationResult);
        HostApplication application = new(parser, compilationService, inspectionService, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj" });

        Assert.AreEqual(HostExitCodes.Success, exitCode);
        inspectionService.Received(1).Inspect(Arg.Is<ProjectLoadRequest>(request => request.ProjectFilePath == "sample.csproj"));
        compilationService.Received(1).Compile(Arg.Any<ProjectCompilationRequest>());
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Emit succeeded")));
    }

    /// <summary>
    /// Verifies that an explicit output directory is passed to compilation.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_OutputDirectory_PassesOutputDirectoryToCompilationAsync()
    {
        HostCommandLineParser parser = new();
        IProjectCompilationService compilationService = Substitute.For<IProjectCompilationService>();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult inspectionResult = CreateResult();
        ProjectCompilationResult compilationResult = CreateCompilationResult(inspectionResult, succeeded: true);

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(inspectionResult);
        compilationService.Compile(Arg.Any<ProjectCompilationRequest>()).Returns(compilationResult);
        HostApplication application = new(parser, compilationService, inspectionService, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj", HostArgumentNames.Output, "artifacts" });

        Assert.AreEqual(HostExitCodes.Success, exitCode);
        compilationService.Received(1).Compile(Arg.Is<ProjectCompilationRequest>(request => request.OutputDirectory == "artifacts"));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Emit succeeded")));
    }

    /// <summary>
    /// Verifies that detailed verbosity writes progress output before emit success.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_DetailedVerbosity_WritesProgressMessagesAsync()
    {
        HostCommandLineParser parser = new();
        IProjectCompilationService compilationService = Substitute.For<IProjectCompilationService>();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult inspectionResult = CreateResult();
        ProjectCompilationResult compilationResult = CreateCompilationResult(inspectionResult, succeeded: true);

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(inspectionResult);
        compilationService.Compile(Arg.Any<ProjectCompilationRequest>()).Returns(compilationResult);
        HostApplication application = new(parser, compilationService, inspectionService, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj", HostArgumentNames.Verbosity, "detailed" });

        Assert.AreEqual(HostExitCodes.Success, exitCode);
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("[host] Inspecting project 'sample.csproj'.")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("[host] Resolved target framework 'net10.0'")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("[host] Compiling project to 'C:\\Temp\\Sample'.")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Emit succeeded")));
    }

    /// <summary>
    /// Verifies that structured non-error diagnostics are written to standard output on successful runs.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_SuccessWithProjectWarning_WritesWarningToStandardOutputAsync()
    {
        HostCommandLineParser parser = new();
        IProjectCompilationService compilationService = Substitute.For<IProjectCompilationService>();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult inspectionResult = CreateResult();
        ProjectCompilationResult compilationResult = new(
            inspectionResult,
            new ProjectEmitSupport(true, ProjectEmitKind.Executable, "Emit supported."),
            new[]
            {
                new CompilationArtifactInfo(CompilationArtifactKind.PrimaryOutput, @"C:\Temp\Sample\Sample.Assembly.dll", true),
            },
            new[]
            {
                new BuildDiagnostic(BuildDiagnosticSeverity.Warning, "SYSLIB0057", "Target project build: Loading certificate data through the constructor or Import is obsolete.", @"C:\Temp\Sample\Program.cs", 214, 15),
            },
            succeeded: true);

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(inspectionResult);
        compilationService.Compile(Arg.Any<ProjectCompilationRequest>()).Returns(compilationResult);
        HostApplication application = new(parser, compilationService, inspectionService, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj" });

        Assert.AreEqual(HostExitCodes.Success, exitCode);
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains(@"C:\Temp\Sample\Program.cs(214,15): warning SYSLIB0057: Target project build:")));
        console.DidNotReceive().WriteErrorLine(Arg.Is<string>(text => text.Contains("SYSLIB0057")));
    }

    /// <summary>
    /// Verifies that failed emit output preserves IDE-friendly diagnostic locations.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_EmitFailure_WritesIdeFriendlyDiagnosticLocationAsync()
    {
        HostCommandLineParser parser = new();
        IProjectCompilationService compilationService = Substitute.For<IProjectCompilationService>();
        IProjectInspectionService inspectionService = Substitute.For<IProjectInspectionService>();
        IHostConsole console = Substitute.For<IHostConsole>();
        ProjectInspectionResult inspectionResult = CreateResult();
        ProjectCompilationResult compilationResult = CreateFailedCompilationResult(inspectionResult);

        inspectionService.Inspect(Arg.Any<ProjectLoadRequest>()).Returns(inspectionResult);
        compilationService.Compile(Arg.Any<ProjectCompilationRequest>()).Returns(compilationResult);
        HostApplication application = new(parser, compilationService, inspectionService, console);

        int exitCode = await application.RunAsync(new[] { "sample.csproj" });

        Assert.AreEqual(HostExitCodes.ExecutionFailed, exitCode);
        console.Received(1).WriteErrorLine(Arg.Is<string>(text => text.Contains(@"C:\Temp\Sample\Program.cs(4,19): error CS1002: ; expected")));
    }

    /// <summary>
    /// Verifies that missing project arguments produce usage output and an invalid-arguments exit code.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_MissingProjectPath_ReturnsInvalidArgumentsAsync()
    {
        IHostConsole console = Substitute.For<IHostConsole>();
        HostApplication application = new(
            new HostCommandLineParser(),
            Substitute.For<IProjectCompilationService>(),
            Substitute.For<IProjectInspectionService>(),
            console);

        int exitCode = await application.RunAsync([]);

        Assert.AreEqual(HostExitCodes.InvalidArguments, exitCode);
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Usage: Workbench.Builder.Host")));
    }

    /// <summary>
    /// Verifies that missing output values are reported as command-line errors.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_MissingOutputValue_ReturnsInvalidArgumentsAsync()
    {
        IHostConsole console = Substitute.For<IHostConsole>();
        HostApplication application = new(
            new HostCommandLineParser(),
            Substitute.For<IProjectCompilationService>(),
            Substitute.For<IProjectInspectionService>(),
            console);

        int exitCode = await application.RunAsync(new[] { HostArgumentNames.Output });

        Assert.AreEqual(HostExitCodes.InvalidArguments, exitCode);
        console.Received(1).WriteErrorLine(Arg.Is<string>(text => text.Contains("output directory value is required")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("Usage: Workbench.Builder.Host")));
        console.Received(1).WriteLine(Arg.Is<string>(text => text.Contains("--verbosity normal|detailed")));
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
            new[] { new PackageReferenceInfo("Sample.Package", "1.0.0", null) },
            new[] { new ResolvedReferenceInfo(ReferenceKind.Framework, "System.Runtime", "FrameworkReference", @"C:\Ref\System.Runtime.dll", true) },
            new[] { new BuildDiagnostic(BuildDiagnosticSeverity.Information, "WB0000", "Informational message") },
            isTestProject: false);
    }

    private static ProjectCompilationResult CreateCompilationResult(ProjectInspectionResult inspectionResult, bool succeeded)
    {
        return new ProjectCompilationResult(
            inspectionResult,
            new ProjectEmitSupport(true, ProjectEmitKind.Executable, "Emit supported."),
            new[]
            {
                new CompilationArtifactInfo(CompilationArtifactKind.PrimaryOutput, @"C:\Temp\Sample\Sample.Assembly.dll", true),
                new CompilationArtifactInfo(CompilationArtifactKind.DebugSymbols, @"C:\Temp\Sample\Sample.Assembly.pdb", true),
                new CompilationArtifactInfo(CompilationArtifactKind.RuntimeMetadata, @"C:\Temp\Sample\Sample.Assembly.runtimeconfig.json", true),
                new CompilationArtifactInfo(CompilationArtifactKind.RuntimeHost, @"C:\Temp\Sample\Sample.Assembly.exe", true),
            },
            new[]
            {
                new BuildDiagnostic(BuildDiagnosticSeverity.Information, "WB2000", "Emit completed."),
            },
            succeeded);
    }

    private static ProjectCompilationResult CreateFailedCompilationResult(ProjectInspectionResult inspectionResult)
    {
        return new ProjectCompilationResult(
            inspectionResult,
            new ProjectEmitSupport(true, ProjectEmitKind.Executable, "Emit supported."),
            System.Array.Empty<CompilationArtifactInfo>(),
            new[]
            {
                new BuildDiagnostic(BuildDiagnosticSeverity.Error, "CS1002", "; expected", @"C:\Temp\Sample\Program.cs", 4, 19),
            },
            succeeded: false);
    }
}
