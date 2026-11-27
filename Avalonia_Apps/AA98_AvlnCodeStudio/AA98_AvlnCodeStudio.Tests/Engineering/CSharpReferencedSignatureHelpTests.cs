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
public sealed class CSharpReferencedSignatureHelpTests
{
    [TestMethod]
    public async Task SignatureHelp_FollowsChangesToReferencedOpenBuffer()
    {
        string fixtureRoot = Path.Combine(AppContext.BaseDirectory, "Fixtures", "LanguageReferenceOverlay");
        string root = Path.Combine(
            Environment.GetEnvironmentVariable("AA98_LANGUAGE_TEST_ROOT") ?? AppContext.BaseDirectory,
            $"LanguageReferenceSignature_{Guid.NewGuid():N}");
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
            string[] paths = [consumerProject, libraryProject, consumerPath, libraryPath];
            byte[][] saved = await Task.WhenAll(paths.Select(path => File.ReadAllBytesAsync(path)));
            await RestoreAsync(consumerProject);
            CSharpWorkspace workspace = new CSharpWorkspaceLoader().Load(consumerProject);
            using CSharpSyntaxDiagnosticService service = new();
            const string consumer = """
                public class Consumer
                {
                    void Read(ReferenceLibrary.LibraryType item) { item.Draft(1, "value"); }
                }
                """;
            const string firstDraft = """
                namespace ReferenceLibrary;
                public sealed class LibraryType
                {
                    public void Draft(int number, string firstText) { }
                    public void Draft(double number, string alternative) { }
                }
                """;
            const string secondDraft = """
                namespace ReferenceLibrary;
                public sealed class LibraryType
                {
                    public void Draft(int number, string secondText) { }
                    public void Draft(bool flag, string alternateText) { }
                }
                """;
            int position = consumer.IndexOf("\"value\"", StringComparison.Ordinal) + 3;
            Dictionary<string, string> openDocuments = new() { [libraryPath] = firstDraft };

            Assert.IsNull(await service.GetSignatureHelpAsync(consumer, consumerPath, position, workspace));
            CSharpSignatureHelp? first = await service.GetSignatureHelpAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments);
            Assert.IsNotNull(first);
            Assert.AreEqual(2, first.Signatures.Count);
            CollectionAssert.Contains(first.Signatures.ToArray(), "LibraryType.Draft(int number, string firstText)");
            CollectionAssert.Contains(first.Signatures.ToArray(), "LibraryType.Draft(double number, string alternative)");
            Assert.AreEqual(1, first.ActiveParameterIndex);
            Assert.AreEqual("firstText", first.ActiveParameterName);
            Assert.AreEqual("LibraryType.Draft(int number, string firstText)", first.Signatures[first.SelectedSignatureIndex]);

            openDocuments[libraryPath] = secondDraft;
            CSharpSignatureHelp? second = await service.GetSignatureHelpAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments);
            Assert.IsNotNull(second);
            Assert.AreEqual(2, second.Signatures.Count);
            CollectionAssert.Contains(second.Signatures.ToArray(), "LibraryType.Draft(int number, string secondText)");
            CollectionAssert.Contains(second.Signatures.ToArray(), "LibraryType.Draft(bool flag, string alternateText)");
            Assert.AreEqual(1, second.ActiveParameterIndex);
            Assert.AreEqual("secondText", second.ActiveParameterName);
            Assert.AreEqual("LibraryType.Draft(int number, string secondText)", second.Signatures[second.SelectedSignatureIndex]);

            openDocuments.Remove(libraryPath);
            Assert.IsNull(await service.GetSignatureHelpAsync(
                consumer, consumerPath, position, workspace, openDocumentTexts: openDocuments));
            for (int i = 0; i < paths.Length; i++)
            {
                CollectionAssert.AreEqual(saved[i], await File.ReadAllBytesAsync(paths[i]), paths[i]);
            }
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
