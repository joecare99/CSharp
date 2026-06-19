using System;
using System.IO;

namespace Workbench.Builder.Core.Tests.TestData;

/// <summary>
/// Provides stable paths to copied SDK-style test data projects.
/// </summary>
internal static class TestDataProjectPaths
{
    public static string SimpleConsoleAppProjectPath => GetPath("SimpleConsoleApp", "SimpleConsoleApp.csproj");

    public static string SimpleLibraryProjectPath => GetPath("SimpleLibrary", "SimpleLibrary.csproj");

    public static string SimpleTestProjectProjectPath => GetPath("SimpleTestProject", "SimpleTestProject.csproj");

    public static string ProjectWithProjectReferenceProjectPath => GetPath("ProjectWithProjectReference", "ProjectWithProjectReference.csproj");

    private static string GetPath(params string[] relativeSegments)
    {
        string testDataPath = Path.Combine(AppContext.BaseDirectory, "TestData");
        if (!Directory.Exists(testDataPath))
        {
            throw new DirectoryNotFoundException($"The copied test data directory was not found at '{testDataPath}'.");
        }

        string combinedPath = testDataPath;
        foreach (string relativeSegment in relativeSegments)
        {
            combinedPath = Path.Combine(combinedPath, relativeSegment);
        }

        return combinedPath;
    }
}
