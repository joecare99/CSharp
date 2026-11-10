using AA98_AvlnCodeStudio.Base.Building.Models;
using AA98_AvlnCodeStudio.Base.Building.Services;
using AA98_AvlnCodeStudio.Base.Testing.Models;
using AA98_AvlnCodeStudio.Base.Testing.Services;
using AA98_AvlnCodeStudio.Base.Versioning.Models;
using AA98_AvlnCodeStudio.Base.Versioning.Services;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Commands;
using AA98_AvlnCodeStudio.Versioning.Svn.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class WorkspaceChatCommandTests
{
    [TestMethod]
    public async Task InspectWorkspace_ReturnsWorkspaceAndProjectPaths()
    {
        using TemporaryWorkspace workspace = new();
        ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
        BuilderProjectInspectionRequest? capturedRequest = null;
        builder.InspectProjectAsync(Arg.Any<BuilderProjectInspectionRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                capturedRequest = call.Arg<BuilderProjectInspectionRequest>();
                BuilderProjectInspectionResult result = new()
                {
                    ProjectPath = workspace.ProjectPath,
                    ProjectName = "sample",
                    TargetFramework = "net10.0",
                    IsTestProject = true,
                };
                result.Diagnostics.Add(new BuilderDiagnostic
                {
                    Code = "IDE0001",
                    Message = "Inspection diagnostic.",
                    Severity = BuilderDiagnosticSeverity.Warning,
                    FilePath = Path.Combine(workspace.RootPath, "Program.cs"),
                    LineNumber = 3,
                    ColumnNumber = 7,
                });
                return Task.FromResult(result);
            });
        InspectWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), builder);
        using JsonDocument arguments = JsonDocument.Parse(
            JsonSerializer.Serialize(new { workspacePath = workspace.ProjectPath }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(workspace.RootPath, capturedRequest.WorkspaceRootPath);
        Assert.AreEqual(workspace.ProjectPath, capturedRequest.ProjectPath);
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.AreEqual(workspace.ProjectPath, resultDocument.RootElement.GetProperty("EntryPath").GetString());
        Assert.AreEqual(1, resultDocument.RootElement.GetProperty("Projects").GetArrayLength());
        JsonElement project = resultDocument.RootElement.GetProperty("Projects")[0];
        Assert.AreEqual(workspace.ProjectPath, project.GetProperty("ProjectPath").GetString());
        Assert.AreEqual("net10.0", project.GetProperty("TargetFramework").GetString());
        Assert.IsTrue(project.GetProperty("IsTestProject").GetBoolean());
        Assert.AreEqual("IDE0001", project.GetProperty("Diagnostics")[0].GetProperty("Code").GetString());
        Assert.AreEqual("Warning", project.GetProperty("Diagnostics")[0].GetProperty("Severity").GetString());
        await builder.DidNotReceive().BuildProjectAsync(
            Arg.Any<BuilderBuildRequest>(),
            Arg.Any<CancellationToken>());
        await builder.DidNotReceive().RunTargetedTestsAsync(
            Arg.Any<BuilderTargetedTestRequest>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task BuildWorkspace_RejectsProjectOutsideWorkspaceWithoutBuilding()
    {
        using TemporaryWorkspace workspace = new();
        ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
        BuildWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), builder);
        string outsideProjectPath = Path.Combine(workspace.RootPath, "outside.csproj");
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
            projectPath = outsideProjectPath,
        }));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            command.ExecuteAsync(arguments.RootElement, CancellationToken.None));

        await builder.DidNotReceive().BuildProjectAsync(
            Arg.Any<BuilderBuildRequest>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task BuildWorkspace_BuildsIncludedProjectWithRequestedOptions()
    {
        using TemporaryWorkspace workspace = new();
        ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
        BuilderBuildRequest? capturedRequest = null;
        builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                capturedRequest = call.Arg<BuilderBuildRequest>();
                return Task.FromResult(new BuilderBuildResult
                {
                    ProjectPath = workspace.ProjectPath,
                    Succeeded = true,
                });
            });
        BuildWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), builder);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
            projectPath = workspace.ProjectPath,
            configuration = "Release",
            targetFramework = "net10.0",
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(workspace.ProjectPath, capturedRequest.ProjectPath);
        Assert.AreEqual(workspace.RootPath, capturedRequest.WorkspaceRootPath);
        Assert.AreEqual("Release", capturedRequest.Configuration);
        Assert.AreEqual("net10.0", capturedRequest.TargetFramework);
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.IsTrue(resultDocument.RootElement.GetProperty("Succeeded").GetBoolean());
    }

    [TestMethod]
    public async Task BuildWorkspace_WithoutProjectPathBuildsEveryProjectAndReturnsDiagnostics()
    {
        using TemporaryWorkspace workspace = new();
        string secondProjectPath = Path.Combine(workspace.RootPath, "Second.csproj");
        string solutionPath = Path.Combine(workspace.RootPath, "Sample.slnx");
        File.WriteAllText(secondProjectPath, "<Project />");
        File.WriteAllText(solutionPath, """
            <Solution>
              <Project Path="sample.csproj" />
              <Project Path="Second.csproj" />
            </Solution>
            """);
        ICodeStudioBuilderService builder = Substitute.For<ICodeStudioBuilderService>();
        List<BuilderBuildRequest> capturedRequests = [];
        builder.BuildProjectAsync(Arg.Any<BuilderBuildRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                BuilderBuildRequest request = call.Arg<BuilderBuildRequest>();
                capturedRequests.Add(request);
                BuilderBuildResult result = new()
                {
                    ProjectPath = request.ProjectPath,
                    Succeeded = !string.Equals(
                        request.ProjectPath,
                        secondProjectPath,
                        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal),
                };
                if (!result.Succeeded)
                {
                    result.Diagnostics.Add(new BuilderDiagnostic
                    {
                        Code = "CS1002",
                        Message = "Expected ;",
                        Severity = BuilderDiagnosticSeverity.Error,
                        FilePath = Path.Combine(workspace.RootPath, "Second.cs"),
                        LineNumber = 4,
                        ColumnNumber = 12,
                    });
                }

                return Task.FromResult(result);
            });
        BuildWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), builder);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = solutionPath,
            configuration = "Release",
            targetFramework = "net10.0",
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        CollectionAssert.AreEquivalent(
            new[] { workspace.ProjectPath, secondProjectPath },
            capturedRequests.Select(static request => request.ProjectPath).ToArray());
        Assert.IsTrue(capturedRequests.All(request =>
            request.WorkspaceRootPath == workspace.RootPath
            && request.Configuration == "Release"
            && request.TargetFramework == "net10.0"));
        Assert.IsFalse(command.Descriptor.IsReadOnly);
        Assert.IsTrue(command.Descriptor.Description.Contains("all projects", StringComparison.Ordinal));
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.IsFalse(resultDocument.RootElement.GetProperty("Succeeded").GetBoolean());
        JsonElement failedProject = resultDocument.RootElement.GetProperty("Projects").EnumerateArray()
            .Single(project => !project.GetProperty("Succeeded").GetBoolean());
        JsonElement diagnostic = failedProject.GetProperty("Diagnostics")[0];
        Assert.AreEqual("CS1002", diagnostic.GetProperty("Code").GetString());
        Assert.AreEqual(4, diagnostic.GetProperty("LineNumber").GetInt32());
    }

    [TestMethod]
    public void BuildWorkspace_IsMarkedMutating()
    {
        BuildWorkspaceChatCommand command = new(
            new CSharpWorkspaceLoader(),
            Substitute.For<ICodeStudioBuilderService>());

        Assert.IsFalse(command.Descriptor.IsReadOnly);
    }

    [TestMethod]
    public async Task TestWorkspace_RejectsProjectOutsideWorkspaceWithoutRunningTests()
    {
        using TemporaryWorkspace workspace = new();
        ITestExecutionService testExecution = Substitute.For<ITestExecutionService>();
        TestWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), testExecution);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
            projectPath = Path.Combine(workspace.RootPath, "outside.csproj"),
        }));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            command.ExecuteAsync(arguments.RootElement, CancellationToken.None));

        await testExecution.DidNotReceive().RunTestsAsync(
            Arg.Any<TestRunRequest>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task TestWorkspace_RunsIncludedProjectWithRequestedFrameworkAndReturnsSummary()
    {
        using TemporaryWorkspace workspace = new();
        ITestExecutionService testExecution = Substitute.For<ITestExecutionService>();
        TestRunRequest? capturedRequest = null;
        testExecution.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                capturedRequest = call.Arg<TestRunRequest>();
                return Task.FromResult(new TestRunSummary
                {
                    Outcome = TestRunOutcome.Partial,
                    TotalCount = 8,
                    PassedCount = 7,
                    FailedCount = 1,
                    Failures =
                    {
                        new TestRunFailure
                        {
                            TestName = "Sample.Tests.SaveTest",
                            Message = "Expected failure.",
                            SourcePath = Path.Combine(workspace.RootPath, "SaveTests.cs"),
                            LineNumber = 23,
                        },
                    },
                });
            });
        TestWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), testExecution);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
            projectPath = workspace.ProjectPath,
            targetFramework = "net10.0",
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        Assert.IsNotNull(capturedRequest);
        Assert.AreEqual(workspace.ProjectPath, capturedRequest.ProjectPath);
        Assert.AreEqual(workspace.RootPath, capturedRequest.WorkspaceRootPath);
        Assert.AreEqual("net10.0", capturedRequest.TargetFramework);
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.AreEqual("Partial", resultDocument.RootElement.GetProperty("Outcome").GetString());
        Assert.AreEqual(7, resultDocument.RootElement.GetProperty("PassedCount").GetInt32());
        Assert.AreEqual(1, resultDocument.RootElement.GetProperty("FailedCount").GetInt32());
        JsonElement failure = resultDocument.RootElement.GetProperty("Failures")[0];
        Assert.AreEqual("Sample.Tests.SaveTest", failure.GetProperty("TestName").GetString());
        Assert.AreEqual("Expected failure.", failure.GetProperty("Message").GetString());
        Assert.AreEqual(
            Path.Combine(workspace.RootPath, "SaveTests.cs"),
            failure.GetProperty("SourcePath").GetString());
        Assert.AreEqual(23, failure.GetProperty("LineNumber").GetInt32());
        Assert.IsFalse(command.Descriptor.IsReadOnly);
    }

    [TestMethod]
    public async Task TestWorkspace_WithoutProjectPathRunsEveryProjectAndAggregatesResults()
    {
        using TemporaryWorkspace workspace = new();
        string secondProjectPath = Path.Combine(workspace.RootPath, "Second.csproj");
        string solutionPath = Path.Combine(workspace.RootPath, "Sample.slnx");
        File.WriteAllText(secondProjectPath, "<Project />");
        File.WriteAllText(solutionPath, """
            <Solution>
              <Project Path="sample.csproj" />
              <Project Path="Second.csproj" />
            </Solution>
            """);
        ITestExecutionService testExecution = Substitute.For<ITestExecutionService>();
        List<TestRunRequest> capturedRequests = [];
        testExecution.RunTestsAsync(Arg.Any<TestRunRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                TestRunRequest request = call.Arg<TestRunRequest>();
                capturedRequests.Add(request);
                bool failed = string.Equals(
                    request.ProjectPath,
                    secondProjectPath,
                    OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                TestRunSummary summary = new()
                {
                    Outcome = failed ? TestRunOutcome.Failed : TestRunOutcome.Passed,
                    TotalCount = 3,
                    PassedCount = failed ? 2 : 3,
                    FailedCount = failed ? 1 : 0,
                };
                if (failed)
                {
                    summary.Failures.Add(new TestRunFailure
                    {
                        TestName = "Sample.Tests.SecondProjectFailure",
                        Message = "Second project failed.",
                        SourcePath = Path.Combine(workspace.RootPath, "SecondTests.cs"),
                        LineNumber = 31,
                    });
                }

                return Task.FromResult(summary);
            });
        TestWorkspaceChatCommand command = new(new CSharpWorkspaceLoader(), testExecution);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = solutionPath,
            targetFramework = "net10.0",
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        CollectionAssert.AreEquivalent(
            new[] { workspace.ProjectPath, secondProjectPath },
            capturedRequests.Select(static request => request.ProjectPath).ToArray());
        Assert.IsTrue(capturedRequests.All(request =>
            request.WorkspaceRootPath == workspace.RootPath
            && request.TargetFramework == "net10.0"));
        Assert.IsFalse(command.Descriptor.IsReadOnly);
        Assert.IsTrue(command.Descriptor.Description.Contains("all projects", StringComparison.Ordinal));
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.AreEqual("Partial", resultDocument.RootElement.GetProperty("Outcome").GetString());
        Assert.AreEqual(6, resultDocument.RootElement.GetProperty("TotalCount").GetInt32());
        Assert.AreEqual(5, resultDocument.RootElement.GetProperty("PassedCount").GetInt32());
        Assert.AreEqual(1, resultDocument.RootElement.GetProperty("FailedCount").GetInt32());
        Assert.AreEqual(2, resultDocument.RootElement.GetProperty("Projects").GetArrayLength());
        JsonElement failedProject = resultDocument.RootElement.GetProperty("Projects")
            .EnumerateArray()
            .Single(project => project.GetProperty("ProjectPath").GetString() == secondProjectPath);
        JsonElement failure = failedProject.GetProperty("Failures")[0];
        Assert.AreEqual("Sample.Tests.SecondProjectFailure", failure.GetProperty("TestName").GetString());
        Assert.AreEqual(Path.Combine(workspace.RootPath, "SecondTests.cs"), failure.GetProperty("SourcePath").GetString());
        Assert.AreEqual(31, failure.GetProperty("LineNumber").GetInt32());
        await testExecution.Received(2).RunTestsAsync(
            Arg.Any<TestRunRequest>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task UpdateSvnWorkspace_UpdatesOnlyARecognizedWorkspace()
    {
        using TemporaryWorkspace workspace = new();
        ISvnCommandRunner svnRunner = Substitute.For<ISvnCommandRunner>();
        svnRunner.RunAsync(
                workspace.RootPath,
                Arg.Is<IReadOnlyList<string>>(arguments => arguments[0] == "info"),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SvnCommandResult
            {
                StandardOutput = $$"""
                    <info><entry path="{{workspace.RootPath}}" kind="dir">
                    <relative-url>^/trunk</relative-url>
                    <wc-info><wcroot-abspath>{{workspace.RootPath}}</wcroot-abspath></wc-info>
                    </entry></info>
                    """,
            }));
        ISvnWorkingCopyOperations operations = Substitute.For<ISvnWorkingCopyOperations>();
        operations.UpdateAsync(workspace.RootPath, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult("Updated to revision 42."));
        UpdateSvnWorkspaceChatCommand command = new(
            new CSharpWorkspaceLoader(),
            new SvnVersionControlService(svnRunner),
            operations);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        await operations.Received(1).UpdateAsync(workspace.RootPath, Arg.Any<CancellationToken>());
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.AreEqual(workspace.RootPath, resultDocument.RootElement.GetProperty("RootPath").GetString());
        Assert.AreEqual("Updated to revision 42.", resultDocument.RootElement.GetProperty("Output").GetString());
        Assert.IsFalse(command.Descriptor.IsReadOnly);
    }

    [TestMethod]
    public async Task InspectVersionControlWorkspace_ReturnsReadOnlyStatusAndChanges()
    {
        using TemporaryWorkspace workspace = new();
        IVersionControlService versionControl = Substitute.For<IVersionControlService>();
        versionControl.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
            .Returns(call =>
            {
                VersionControlStatusRequest request = call.Arg<VersionControlStatusRequest>();
                Assert.AreEqual(workspace.RootPath, request.RepositoryContextPath);
                Assert.IsTrue(request.IncludeChanges);
                Assert.IsTrue(request.IncludeCapabilities);
                VersionControlStatus status = new()
                {
                    RepositoryRootPath = workspace.RootPath,
                    RepositoryName = "sample",
                    ActiveReferenceName = "trunk",
                    ActiveReferenceKind = VersionControlReferenceKind.Other,
                    IsRepositoryRootDiscovered = true,
                    HasLocalChanges = true,
                };
                status.Capabilities.Add(VersionControlCapability.InspectStatus);
                status.Changes.Add(new VersionControlChangeSummary
                {
                    Path = "Program.cs",
                    ChangeKind = VersionControlChangeKind.Modified,
                    IsIgnored = false,
                });
                return Task.FromResult(status);
            });
        InspectVersionControlWorkspaceChatCommand command = new(
            new CSharpWorkspaceLoader(),
            versionControl);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
        }));

        string result = await command.ExecuteAsync(arguments.RootElement, CancellationToken.None);

        Assert.IsTrue(command.Descriptor.IsReadOnly);
        using JsonDocument resultDocument = JsonDocument.Parse(result);
        Assert.AreEqual(workspace.RootPath, resultDocument.RootElement.GetProperty("RepositoryRootPath").GetString());
        Assert.AreEqual("sample", resultDocument.RootElement.GetProperty("RepositoryName").GetString());
        Assert.AreEqual("trunk", resultDocument.RootElement.GetProperty("ActiveReferenceName").GetString());
        Assert.AreEqual("Other", resultDocument.RootElement.GetProperty("ActiveReferenceKind").GetString());
        Assert.IsTrue(resultDocument.RootElement.GetProperty("HasLocalChanges").GetBoolean());
        Assert.AreEqual(
            "Program.cs",
            resultDocument.RootElement.GetProperty("Changes")[0].GetProperty("Path").GetString());
        Assert.AreEqual(
            "Modified",
            resultDocument.RootElement.GetProperty("Changes")[0].GetProperty("ChangeKind").GetString());
        Assert.AreEqual("InspectStatus", resultDocument.RootElement.GetProperty("Capabilities")[0].GetString());
        await versionControl.Received(1).GetStatusAsync(
            Arg.Is<VersionControlStatusRequest>(request => request.RepositoryContextPath == workspace.RootPath),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task InspectVersionControlWorkspace_RejectsUnrecognizedWorkspace()
    {
        using TemporaryWorkspace workspace = new();
        IVersionControlService versionControl = Substitute.For<IVersionControlService>();
        versionControl.GetStatusAsync(Arg.Any<VersionControlStatusRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new VersionControlStatus
            {
                RepositoryRootPath = workspace.RootPath,
                IsRepositoryRootDiscovered = false,
            }));
        InspectVersionControlWorkspaceChatCommand command = new(
            new CSharpWorkspaceLoader(),
            versionControl);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
        }));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            command.ExecuteAsync(arguments.RootElement, CancellationToken.None));

        await versionControl.Received(1).GetStatusAsync(
            Arg.Any<VersionControlStatusRequest>(),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task UpdateSvnWorkspace_RejectsNonSvnWorkspaceWithoutUpdating()
    {
        using TemporaryWorkspace workspace = new();
        ISvnCommandRunner svnRunner = Substitute.For<ISvnCommandRunner>();
        svnRunner.RunAsync(
                Arg.Any<string>(),
                Arg.Any<IReadOnlyList<string>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new SvnCommandResult
            {
                ExitCode = 1,
                StandardError = "not a working copy",
            }));
        ISvnWorkingCopyOperations operations = Substitute.For<ISvnWorkingCopyOperations>();
        UpdateSvnWorkspaceChatCommand command = new(
            new CSharpWorkspaceLoader(),
            new SvnVersionControlService(svnRunner),
            operations);
        using JsonDocument arguments = JsonDocument.Parse(JsonSerializer.Serialize(new
        {
            workspacePath = workspace.ProjectPath,
        }));

        await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
            command.ExecuteAsync(arguments.RootElement, CancellationToken.None));

        await operations.DidNotReceive().UpdateAsync(
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    private sealed class TemporaryWorkspace : IDisposable
    {
        public TemporaryWorkspace()
        {
            RootPath = Path.Combine(Path.GetTempPath(), $"AvlnChatTest-{Guid.NewGuid():N}");
            Directory.CreateDirectory(RootPath);
            ProjectPath = Path.Combine(RootPath, "sample.csproj");
            File.WriteAllText(ProjectPath, "<Project Sdk=\"Microsoft.NET.Sdk\" />");
        }

        public string RootPath { get; }

        public string ProjectPath { get; }

        public void Dispose()
        {
            foreach (string filePath in Directory.GetFiles(RootPath))
            {
                File.Delete(filePath);
            }

            Directory.Delete(RootPath);
        }
    }
}
