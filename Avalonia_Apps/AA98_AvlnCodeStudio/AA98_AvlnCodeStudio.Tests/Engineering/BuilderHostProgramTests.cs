#if NET10_0
using System;
using System.IO;
using System.Threading.Tasks;
using AA98.Builder.Host;
using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies deterministic command handling for the AA98 builder micro-host.
/// </summary>
[TestClass]
public class BuilderHostProgramTests
{
    /// <summary>
    /// Verifies that inspect writes the structured inspection payload and returns success.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_WithInspectCommand_WritesInspectionPayload()
    {
        ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
        builderService
            .InspectProjectAsync(Arg.Any<BuilderProjectInspectionRequest>(), default)
            .Returns(Task.FromResult(new BuilderProjectInspectionResult
            {
                ProjectPath = @"C:\src\SampleProject.csproj",
                ProjectName = "SampleProject",
                TargetFramework = "net10.0",
                IsTestProject = true,
                Diagnostics =
                {
                    new BuilderDiagnostic
                    {
                        Severity = BuilderDiagnosticSeverity.Info,
                        Code = "INF001",
                        Message = "Inspection completed.",
                    },
                },
            }));

        using StringWriter standardOutput = new();
        using StringWriter standardError = new();

        int exitCode = await Program.RunAsync(
            ["inspect", @".\src\SampleProject.csproj", "--configuration", "Release", "--framework", "net10.0"],
            builderService,
            standardOutput,
            standardError).ConfigureAwait(false);

        Assert.AreEqual(0, exitCode);
        string output = standardOutput.ToString();
        StringAssert.Contains(output, "AA98_BUILDER_INSPECTION_RESULT");
        StringAssert.Contains(output, "project=C:\\src\\SampleProject.csproj");
        StringAssert.Contains(output, "name=SampleProject");
        StringAssert.Contains(output, "targetFramework=net10.0");
        StringAssert.Contains(output, "diagnostics=1");
        Assert.AreEqual(string.Empty, standardError.ToString());
        await builderService.Received(1).InspectProjectAsync(
            Arg.Is<BuilderProjectInspectionRequest>(request =>
                request.ProjectPath == @".\src\SampleProject.csproj"
                && request.Configuration == "Release"
                && request.TargetFramework == "net10.0"),
            default);
    }

    /// <summary>
    /// Verifies that build writes artifacts and returns a failure exit code when compilation fails.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_WithBuildCommand_WritesBuildPayloadAndFailureExitCode()
    {
        ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
        builderService
            .BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), default)
            .Returns(Task.FromResult(new BuilderBuildResult
            {
                ProjectPath = @"C:\src\SampleProject.csproj",
                Succeeded = false,
                Artifacts =
                {
                    new BuilderCompilationArtifact
                    {
                        Kind = "Assembly",
                        TargetFramework = "net10.0",
                        Path = @"C:\out\SampleProject.dll",
                    },
                },
                Diagnostics =
                {
                    new BuilderDiagnostic
                    {
                        Severity = BuilderDiagnosticSeverity.Error,
                        Code = "ERR001",
                        Message = "Compilation failed.",
                    },
                },
            }));

        using StringWriter standardOutput = new();
        using StringWriter standardError = new();

        int exitCode = await Program.RunAsync(
            ["build", @".\src\SampleProject.csproj"],
            builderService,
            standardOutput,
            standardError).ConfigureAwait(false);

        Assert.AreEqual(1, exitCode);
        string output = standardOutput.ToString();
        StringAssert.Contains(output, "AA98_BUILDER_BUILD_RESULT");
        StringAssert.Contains(output, "succeeded=False");
        StringAssert.Contains(output, "artifact=Assembly|net10.0|C:\\out\\SampleProject.dll");
        StringAssert.Contains(output, "diagnostic=Error|ERR001||||Compilation failed.");
        Assert.AreEqual(string.Empty, standardError.ToString());
    }

    /// <summary>
    /// Verifies that unknown commands write usage and return a command-line error code.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_WithUnknownCommand_WritesUsageAndError()
    {
        ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
        using StringWriter standardOutput = new();
        using StringWriter standardError = new();

        int exitCode = await Program.RunAsync(
            ["unknown", @".\src\SampleProject.csproj"],
            builderService,
            standardOutput,
            standardError).ConfigureAwait(false);

        Assert.AreEqual(2, exitCode);
        StringAssert.Contains(standardError.ToString(), "Unknown command: unknown");
        StringAssert.Contains(standardOutput.ToString(), "AA98 Builder Host");
    }

    /// <summary>
    /// Verifies that execution exceptions are converted into the structured host error payload.
    /// </summary>
    [TestMethod]
    public async Task RunAsync_WhenBuilderThrows_WritesStructuredError()
    {
        ICodeStudioBuilderService builderService = Substitute.For<ICodeStudioBuilderService>();
        builderService
            .InspectProjectAsync(Arg.Any<BuilderProjectInspectionRequest>(), default)
            .Returns<Task<BuilderProjectInspectionResult>>(_ => throw new InvalidOperationException("boom"));
        using StringWriter standardOutput = new();
        using StringWriter standardError = new();

        int exitCode = await Program.RunAsync(
            ["inspect", @".\src\SampleProject.csproj"],
            builderService,
            standardOutput,
            standardError).ConfigureAwait(false);

        Assert.AreEqual(1, exitCode);
        Assert.AreEqual(string.Empty, standardOutput.ToString());
        string errorOutput = standardError.ToString();
        StringAssert.Contains(errorOutput, "AA98_BUILDER_HOST_ERROR");
        StringAssert.Contains(errorOutput, "type=System.InvalidOperationException");
        StringAssert.Contains(errorOutput, "message=boom");
    }
}
#endif
