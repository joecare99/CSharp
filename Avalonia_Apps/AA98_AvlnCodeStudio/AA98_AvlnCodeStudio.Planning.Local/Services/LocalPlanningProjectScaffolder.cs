using System;
using System.IO;
using System.Text;

namespace AA98_AvlnCodeStudio.Planning.Local.Services;

/// <summary>
/// Creates a portable local DevOps planning project and its default folder structure.
/// </summary>
public sealed class LocalPlanningProjectScaffolder : ILocalPlanningProjectScaffolder
{
    public LocalPlanningProjectScaffoldResult Create(string planningProjectRootPath)
    {
        if (string.IsNullOrWhiteSpace(planningProjectRootPath))
        {
            return new LocalPlanningProjectScaffoldResult(false, "Planning project path is required.");
        }

        string normalizedRootPath = Path.GetFullPath(planningProjectRootPath);
        Directory.CreateDirectory(normalizedRootPath);

        string projectName = ResolveProjectName(normalizedRootPath);
        string projectFilePath = Path.Combine(normalizedRootPath, $"{projectName}.csproj");
        string projectItemsPath = Path.Combine(normalizedRootPath, $"{projectName}.projitems");

        CreatePlanningFolders(normalizedRootPath);
        EnsureProjectItemsFile(projectItemsPath);
        EnsureProjectFile(projectFilePath, projectName);

        return new LocalPlanningProjectScaffoldResult(true, $"Planning project '{projectName}' is ready at '{normalizedRootPath}'.", normalizedRootPath, projectFilePath, projectItemsPath);
    }

    private static void CreatePlanningFolders(string rootPath)
    {
        Directory.CreateDirectory(Path.Combine(rootPath, "Epics"));
        Directory.CreateDirectory(Path.Combine(rootPath, "Features"));
        Directory.CreateDirectory(Path.Combine(rootPath, "BacklogItems"));
        Directory.CreateDirectory(Path.Combine(rootPath, "Tasks"));
    }

    private static void EnsureProjectFile(string projectFilePath, string projectName)
    {
        if (File.Exists(projectFilePath))
        {
            return;
        }

        string content = $"<Project Sdk=\"Microsoft.NET.Sdk\">{Environment.NewLine}" +
            $"  <PropertyGroup>{Environment.NewLine}" +
            $"    <TargetFramework>net8.0</TargetFramework>{Environment.NewLine}" +
            $"    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>{Environment.NewLine}" +
            $"    <EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>{Environment.NewLine}" +
            $"    <EnableDefaultNoneItems>false</EnableDefaultNoneItems>{Environment.NewLine}" +
            $"  </PropertyGroup>{Environment.NewLine}" +
            $"  <Import Project=\"{projectName}.projitems\" Label=\"Shared\" />{Environment.NewLine}" +
            $"</Project>{Environment.NewLine}";

        File.WriteAllText(projectFilePath, content, Encoding.UTF8);
    }

    private static void EnsureProjectItemsFile(string projectItemsPath)
    {
        if (File.Exists(projectItemsPath))
        {
            return;
        }

        string content = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}" +
            $"<Project>{Environment.NewLine}" +
            $"  <ItemGroup>{Environment.NewLine}" +
            $"    <None Include=\"$(MSBuildThisFileDirectory)**/*.md\" Exclude=\"$(MSBuildThisFileDirectory)bin/**;$(MSBuildThisFileDirectory)obj/**;$(MSBuildThisFileDirectory).vs/**\" />{Environment.NewLine}" +
            $"  </ItemGroup>{Environment.NewLine}" +
            $"</Project>{Environment.NewLine}";

        File.WriteAllText(projectItemsPath, content, Encoding.UTF8);
    }

    private static string ResolveProjectName(string rootPath)
    {
        string folderName = Path.GetFileName(Path.TrimEndingDirectorySeparator(rootPath));
        return string.IsNullOrWhiteSpace(folderName) ? "DevOps" : folderName;
    }
}
