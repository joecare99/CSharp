using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class CSharpReferencedCompletionTests
{
    [TestMethod]
    public async Task CompletionAndQuickInfo_FollowChangesToReferencedOpenBuffer()
    {
        string fixtureRoot = Path.Combine(AppContext.BaseDirectory, "Fixtures", "LanguageReferenceOverlay");
        string root = Path.Combine(
            Environment.GetEnvironmentVariable("AA98_LANGUAGE_TEST_ROOT") ?? AppContext.BaseDirectory,
            $"LanguageReferenceCompletion_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            foreach (string project in new[] { "Consumer", "Library" })
            {
                string destination = Path.Combine(root, project);
                Directory.CreateDirectory(destination);
                foreach (string file in new[] { $"{project}.csproj", $"{project}.cs" })
                {
                    File.Copy(Path.Combine(fixtureRoot, project, file), Path.Combine(destination, file));
                }
            }

            string consumerProject = Path.Combine(root, "Consumer", "Consumer.csproj");
            string libraryProject = Path.Combine(root, "Library", "Library.csproj");
            string consumerPath = Path.Combine(root, "Consumer", "Consumer.cs");
            string libraryPath = Path.Combine(root, "Library", "Library.cs");
            string savedConsumer = await File.ReadAllTextAsync(consumerPath);
            string savedLibrary = await File.ReadAllTextAsync(libraryPath);
            string savedConsumerProject = await File.ReadAllTextAsync(consumerProject);
            string savedLibraryProject = await File.ReadAllTextAsync(libraryProject);
            await RestoreAsync(consumerProject);
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(consumerProject);
            using CSharpSyntaxDiagnosticService service = new();
            const string consumer = "public class Consumer { void Read(ReferenceLibrary.LibraryType item) { item.Draft; } }";
            const string firstDraft = """
                namespace ReferenceLibrary;
                public sealed class LibraryType
                {
                    /// <summary>First buffer version.</summary>
                    public int DraftFirst => 1;
                }
                """;
            const string secondDraft = """
                namespace ReferenceLibrary;
                public sealed class LibraryType
                {
                    /// <summary>Second buffer version.</summary>
                    public string DraftSecond => "";
                }
                """;
            Dictionary<string, string> openDocuments = new() { [libraryPath] = firstDraft };
            int position = consumer.IndexOf("Draft", StringComparison.Ordinal) + "Draft".Length;

            var diskItems = await service.GetCompletionItemsAsync(consumer, consumerPath, position, workspace);
            Assert.IsFalse(diskItems.Any(item => item.DisplayText is "DraftFirst" or "DraftSecond"));
            var firstItems = await service.GetCompletionItemsAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments);
            Assert.IsTrue(firstItems.Any(item => item.DisplayText == "DraftFirst"));
            Assert.IsFalse(firstItems.Any(item => item.DisplayText == "DraftSecond"));
            const string firstConsumer = "public class Consumer { int Read(ReferenceLibrary.LibraryType item) => item.DraftFirst; }";
            string? firstInfo = await service.GetQuickInfoAsync(
                firstConsumer, consumerPath, firstConsumer.IndexOf("DraftFirst", StringComparison.Ordinal),
                workspace, openDocumentTexts: openDocuments);
            StringAssert.Contains(firstInfo, "int ReferenceLibrary.LibraryType.DraftFirst");
            StringAssert.Contains(firstInfo, "First buffer version.");

            openDocuments[libraryPath] = secondDraft;
            var secondItems = await service.GetCompletionItemsAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments);
            Assert.IsTrue(secondItems.Any(item => item.DisplayText == "DraftSecond"));
            Assert.IsFalse(secondItems.Any(item => item.DisplayText == "DraftFirst"));
            const string secondConsumer = "public class Consumer { string Read(ReferenceLibrary.LibraryType item) => item.DraftSecond; }";
            string? secondInfo = await service.GetQuickInfoAsync(
                secondConsumer, consumerPath, secondConsumer.IndexOf("DraftSecond", StringComparison.Ordinal),
                workspace, openDocumentTexts: openDocuments);
            StringAssert.Contains(secondInfo, "string ReferenceLibrary.LibraryType.DraftSecond");
            StringAssert.Contains(secondInfo, "Second buffer version.");
            Assert.IsNull(await service.GetQuickInfoAsync(
                firstConsumer, consumerPath, firstConsumer.IndexOf("DraftFirst", StringComparison.Ordinal),
                workspace, openDocumentTexts: openDocuments));

            openDocuments.Remove(libraryPath);
            var removedItems = await service.GetCompletionItemsAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments);
            Assert.IsFalse(removedItems.Any(item => item.DisplayText is "DraftFirst" or "DraftSecond"));
            Assert.IsNull(await service.GetQuickInfoAsync(
                secondConsumer, consumerPath, secondConsumer.IndexOf("DraftSecond", StringComparison.Ordinal),
                workspace, openDocumentTexts: openDocuments));
            Assert.AreEqual(savedConsumer, await File.ReadAllTextAsync(consumerPath));
            Assert.AreEqual(savedLibrary, await File.ReadAllTextAsync(libraryPath));
            Assert.AreEqual(savedConsumerProject, await File.ReadAllTextAsync(consumerProject));
            Assert.AreEqual(savedLibraryProject, await File.ReadAllTextAsync(libraryProject));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static async Task RestoreAsync(string projectPath)
    {
        ProcessStartInfo start = new("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(projectPath)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.ArgumentList.Add("restore");
        start.ArgumentList.Add(projectPath);
        start.ArgumentList.Add("--verbosity");
        start.ArgumentList.Add("quiet");
        foreach (string variable in new[]
        {
            "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32", "MSBuildToolsPath",
            "DOTNET_MSBUILD_SDK_RESOLVER_SDKS_DIR", "DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR",
        })
        {
            start.Environment.Remove(variable);
        }

        using Process process = Process.Start(start) ?? throw new InvalidOperationException("Could not start fixture restore.");
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        Assert.AreEqual(0, process.ExitCode, $"Fixture restore failed: {await output}{await error}");
    }
}
