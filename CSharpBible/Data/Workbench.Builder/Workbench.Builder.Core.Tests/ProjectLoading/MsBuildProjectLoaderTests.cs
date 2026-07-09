using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workbench.Builder.Core.Models.Loading;
using Workbench.Builder.Core.Services.Loading;
using Workbench.Builder.Core.Tests.TestData;

namespace Workbench.Builder.Core.Tests.ProjectLoading;

/// <summary>
/// Verifies MSBuild-based loading of SDK-style sample projects.
/// </summary>
[TestClass]
public class MsBuildProjectLoaderTests
{
    [TestCleanup]
    public void Cleanup()
    {
        MsBuildProjectLoader.ResetForTests();
    }

    /// <summary>
    /// Verifies that loading a simple console project evaluates the planned V1.1 core properties and compile items.
    /// </summary>
    [TestMethod]
    public void Load_SimpleConsoleProject_ReturnsExpectedProperties()
    {
        var loader = new MsBuildProjectLoader();

        var project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath));

        Assert.AreEqual("SimpleConsoleApp", project.Properties.AssemblyName);
        Assert.AreEqual("Workbench.Builder.TestData.SimpleConsoleApp", project.Properties.RootNamespace);
        Assert.AreEqual("net8.0", project.Properties.TargetFramework);
        Assert.AreEqual("Exe", project.Properties.OutputType);
        Assert.AreEqual("enable", project.Properties.ImplicitUsings);
        Assert.AreEqual(2, project.CompileItems.Count);
        Assert.AreEqual(0, project.PackageReferences.Count);
        Assert.IsTrue(project.IsSdkStyle);
    }

    /// <summary>
    /// Verifies that loading a project with a project reference returns the referenced project path.
    /// </summary>
    [TestMethod]
    public void Load_ProjectWithProjectReference_ReturnsReferencedProject()
    {
        var loader = new MsBuildProjectLoader();

        var project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.ProjectWithProjectReferenceProjectPath));

        Assert.AreEqual(1, project.ProjectReferences.Count);
        StringAssert.EndsWith(project.ProjectReferences[0].ProjectFilePath, "ReferencedLibrary.csproj");
        Assert.IsTrue(project.ProjectReferences[0].Exists);
    }

    /// <summary>
    /// Verifies that loading a null request throws <see cref="ArgumentNullException"/>.
    /// </summary>
    [TestMethod]
    public void Load_NullRequest_ThrowsArgumentNullException()
    {
        MsBuildProjectLoader loader = new();

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() => loader.Load(null!));

        Assert.AreEqual("request", exception.ParamName);
    }

    /// <summary>
    /// Verifies that loading a request without a project file path throws <see cref="ArgumentException"/>.
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("   ")]
    public void Load_MissingProjectFilePath_ThrowsArgumentException(string? projectFilePath)
    {
        MsBuildProjectLoader loader = new();
        ProjectLoadRequest request = new(projectFilePath!);

        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() => loader.Load(request));

        Assert.AreEqual("request", exception.ParamName);
        StringAssert.StartsWith(exception.Message, "A project file path is required.");
    }

    /// <summary>
    /// Verifies that loading a non-existing project file throws <see cref="FileNotFoundException"/>.
    /// </summary>
    [TestMethod]
    public void Load_MissingProjectFile_ThrowsFileNotFoundException()
    {
        MsBuildProjectLoader loader = new();
        string missingProjectPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "MissingProject.csproj");

        FileNotFoundException exception = Assert.ThrowsExactly<FileNotFoundException>(() => loader.Load(new ProjectLoadRequest(missingProjectPath)));

        Assert.AreEqual(Path.GetFullPath(missingProjectPath), exception.FileName);
        Assert.AreEqual("The specified project file could not be found.", exception.Message.Split(Environment.NewLine)[0]);
    }

    /// <summary>
    /// Verifies that loading fails with a clear exception when no SDK path and no Visual Studio instance are available.
    /// </summary>
    [TestMethod]
    public void Load_WhenNoSdkPathAndNoVisualStudioInstanceExist_ThrowsInvalidOperationException()
    {
        MsBuildProjectLoader.ResetForTests();
        MsBuildProjectLoader.IsMsBuildLocatorRegisteredOverride = static () => false;
        MsBuildProjectLoader.FindDotNetSdkPathOverride = static () => null;
        MsBuildProjectLoader.QueryVisualStudioInstancesOverride = static () => Array.Empty<Microsoft.Build.Locator.VisualStudioInstance>();
        MsBuildProjectLoader loader = new();

        InvalidOperationException exception = Assert.ThrowsExactly<InvalidOperationException>(() => loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath)));

        Assert.AreEqual("No MSBuild instance or .NET SDK path could be detected for registration.", exception.Message);
    }

    /// <summary>
    /// Verifies that the second registration guard returns immediately when registration completes while the caller is waiting for the lock.
    /// </summary>
    [TestMethod]
    public void EnsureMsBuildRegistered_WhenRegistrationCompletesWhileWaitingForLock_ReturnsFromSecondGuard()
    {
        MsBuildProjectLoader.ResetForTests();

        object syncRoot = GetPrivateStaticField<object>("SyncRoot");
        lock (syncRoot)
        {
            SetPrivateStaticField("s_isLocatorRegistered", false);

            Task invocation = Task.Run(InvokeEnsureMsBuildRegistered);
            Task.Delay(100).Wait();

            SetPrivateStaticField("s_isLocatorRegistered", true);
            Assert.IsFalse(invocation.IsCompleted);
        }

        InvokeEnsureMsBuildRegistered();
        Assert.IsTrue(GetPrivateStaticField<bool>("s_isLocatorRegistered"));
    }

    /// <summary>
    /// Verifies that a DOTNET_ROOT entry without an sdk subfolder is ignored when probing SDK locations.
    /// </summary>
    [TestMethod]
    public void FindDotNetSdkPath_WhenRootHasNoSdkDirectory_IgnoresThatRoot()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string? originalDotNetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        Directory.CreateDirectory(tempRoot);

        try
        {
            Environment.SetEnvironmentVariable("DOTNET_ROOT", tempRoot);

            string? result = InvokeFindDotNetSdkPath();

            Assert.IsTrue(result is null || Path.IsPathRooted(result));
        }
        finally
        {
            Environment.SetEnvironmentVariable("DOTNET_ROOT", originalDotNetRoot);
            Directory.Delete(tempRoot, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that an explicit runtime identifier is forwarded into the evaluated project properties.
    /// </summary>
    [TestMethod]
    public void Load_WhenRuntimeIdentifierIsSpecified_PreservesRuntimeIdentifier()
    {
        MsBuildProjectLoader loader = new();

        LoadedProjectModel project = loader.Load(new ProjectLoadRequest(TestDataProjectPaths.SimpleConsoleAppProjectPath, runtimeIdentifier: "win-x64"));

        Assert.AreEqual("win-x64", project.Properties.RuntimeIdentifier);
    }

    /// <summary>
    /// Verifies that a multi-target project with no concrete first target framework remains on the initially loaded project.
    /// </summary>
    [TestMethod]
    public void Load_WhenTargetFrameworksContainOnlySeparators_DoesNotReloadForFirstTargetFramework()
    {
        string projectDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string projectFilePath = Path.Combine(projectDirectory, "SeparatorOnlyTargetFrameworks.csproj");
        Directory.CreateDirectory(projectDirectory);

        try
        {
            File.WriteAllText(
                projectFilePath,
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFrameworks> ; ; </TargetFrameworks>
                  </PropertyGroup>
                </Project>
                """);

            MsBuildProjectLoader loader = new();
            LoadedProjectModel project = loader.Load(new ProjectLoadRequest(projectFilePath));

            Assert.AreEqual(string.Empty, project.Properties.TargetFramework);
            Assert.AreEqual(0, project.CompileItems.Count);
        }
        finally
        {
            Directory.Delete(projectDirectory, recursive: true);
        }
    }

    private static void InvokeEnsureMsBuildRegistered()
    {
        MethodInfo method = typeof(MsBuildProjectLoader).GetMethod("EnsureMsBuildRegistered", BindingFlags.NonPublic | BindingFlags.Static)!;
        method.Invoke(null, null);
    }

    private static string? InvokeFindDotNetSdkPath()
    {
        MethodInfo method = typeof(MsBuildProjectLoader).GetMethod("FindDotNetSdkPath", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string?)method.Invoke(null, null);
    }

    private static T GetPrivateStaticField<T>(string fieldName)
    {
        FieldInfo field = typeof(MsBuildProjectLoader).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!;
        return (T)field.GetValue(null)!;
    }

    private static void SetPrivateStaticField(string fieldName, object value)
    {
        FieldInfo field = typeof(MsBuildProjectLoader).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static)!;
        field.SetValue(null, value);
    }
}
