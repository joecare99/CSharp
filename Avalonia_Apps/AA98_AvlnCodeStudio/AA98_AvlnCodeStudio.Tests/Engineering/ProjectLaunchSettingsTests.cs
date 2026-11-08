using AA98_AvlnCodeStudio.Model.Workspaces;
using Config.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Property.Editor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class ProjectLaunchSettingsTests
{
    [TestMethod]
    public async Task ReadAsync_InspectsProfilesWithoutExposingArgumentsOrEnvironmentValues()
    {
        await WithProjectAsync(async (root, projectPath) =>
        {
            const string source = """
                {
                  // Comments and trailing commas are allowed in launch settings.
                  "profiles": {
                    "Local": {
                      "commandName": "Project",
                      "commandLineArgs": "--token synthetic-argument-secret",
                      "environmentVariables": { "API_KEY": "synthetic-environment-secret" },
                      "customSetting": { "unchanged": true },
                    },
                    "External": { "commandName": "Executable", "executablePath": "tool.exe" },
                  },
                }
                """;
            string path = CreateLaunchSettings(projectPath, source);
            ProjectLaunchSettingsReader reader = new();
            ProjectLaunchSettingsSnapshot snapshot = await reader.ReadAsync(projectPath);

            Assert.IsTrue(snapshot.FileExists);
            Assert.IsTrue(snapshot.HasProfilesObject);
            Assert.AreEqual(path, snapshot.FilePath);
            CollectionAssert.AreEqual(new[] { "Local", "External" }, snapshot.Profiles.Select(profile => profile.Name).ToArray());
            Assert.AreEqual("Project", snapshot.Profiles[0].CommandName);
            Assert.IsTrue(snapshot.Profiles[0].HasCommandLineArguments);
            Assert.IsTrue(snapshot.Profiles[0].HasEnvironmentVariables);
            Assert.AreEqual("Executable", snapshot.Profiles[1].CommandName);
            Assert.IsFalse(snapshot.Profiles[1].HasEnvironmentVariables);

            ProjectLaunchProfilesProvider provider = new(projectPath);
            PropertyProviderConfigUiSection section = new("LaunchProfiles", "Launch profiles", provider);
            await section.LoadAsync();
            Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
            Assert.IsTrue(section.IsReadOnly);
            Assert.AreEqual(PropertyScope.Project, section.Context.Scope);
            Assert.AreEqual(path, section.Context.StorageLocation);
            Assert.IsTrue(section.Properties.All(property => !property.IsEditable));
            Assert.AreEqual("launchSettings.json",
                section.Properties.Single(property => property.DisplayName == "Local").ValueContext.Origin);
            string displayed = string.Join("|", section.Properties.Select(property => $"{property.DisplayName}:{property.Value}"));
            StringAssert.Contains(displayed, "Local:commandName: Project");
            StringAssert.Contains(displayed, "arguments specified");
            StringAssert.Contains(displayed, "environment specified");
            Assert.IsFalse(displayed.Contains("synthetic-argument-secret", StringComparison.Ordinal));
            Assert.IsFalse(displayed.Contains("synthetic-environment-secret", StringComparison.Ordinal));
            Assert.AreEqual(source, await File.ReadAllTextAsync(path));
        });
    }

    [TestMethod]
    public async Task MissingLaunchSettings_AreReportedWithoutCreatingFilesOrAllowingChanges()
    {
        await WithProjectAsync(async (root, projectPath) =>
        {
            ProjectLaunchProfilesProvider provider = new(projectPath);
            await provider.LoadAsync();

            Assert.IsTrue(provider.IsReadOnly);
            StringAssert.Contains((string)provider.Properties.Single().Value!, "nothing was created");
            Assert.IsFalse(Directory.Exists(Path.Combine(root, "Properties")));
            Assert.IsFalse(provider.Properties.Single().TrySetValue("invented profile"));
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => provider.ApplyAsync());
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            Assert.ThrowsExactly<OperationCanceledException>(() => provider.ApplyAsync(cancellation.Token));
            Assert.IsFalse(File.Exists(ProjectLaunchSettingsReader.GetSettingsPath(projectPath)));
        });
    }

    [TestMethod]
    public async Task ReadAsync_ReportsAmbiguousAndInvalidProfilesWithoutChoosingOne()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            CreateLaunchSettings(projectPath, """
                {
                  "profiles": {
                    "Local": { "commandName": "Project" },
                    "LOCAL": { "commandName": "IISExpress" },
                    "DuplicateCommand": { "commandName": "Project", "CommandName": "Executable" },
                    "MissingCommand": {},
                    "InvalidObject": 42
                  }
                }
                """);
            ProjectLaunchSettingsSnapshot snapshot = await new ProjectLaunchSettingsReader().ReadAsync(projectPath);

            Assert.AreEqual(5, snapshot.Profiles.Count);
            Assert.IsTrue(snapshot.Profiles[0].IsAmbiguous);
            Assert.IsTrue(snapshot.Profiles[1].IsAmbiguous);
            StringAssert.Contains(snapshot.Profiles[0].Diagnostic, "ambiguous");
            Assert.AreEqual("IISExpress", snapshot.Profiles[1].CommandName);
            Assert.IsNull(snapshot.Profiles[2].CommandName);
            StringAssert.Contains(snapshot.Profiles[2].Diagnostic, "single string");
            StringAssert.Contains(snapshot.Profiles[3].Diagnostic, "missing");
            StringAssert.Contains(snapshot.Profiles[4].Diagnostic, "not a JSON object");
        });
    }

    [TestMethod]
    public async Task InvalidLaunchSettings_AreReportedWithoutFallbackOrSecretText()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            string path = CreateLaunchSettings(projectPath, """{"profiles":{"Before":{"commandName":"Project"}}}""");
            ProjectLaunchProfilesProvider provider = new(projectPath);
            PropertyProviderConfigUiSection section = new("LaunchProfiles", "Launch profiles", provider);
            await section.LoadAsync();
            Assert.AreEqual(ConfigUiSectionState.Ready, section.State);
            Assert.IsTrue(section.Properties.Any(property => property.DisplayName == "Before"));

            await File.WriteAllTextAsync(path, """{"profiles":{"Bad":{"environmentVariables":{"KEY":"synthetic-secret",}}""");
            await section.LoadAsync();

            Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
            StringAssert.Contains(section.ErrorMessage, "Invalid project launch settings JSON");
            Assert.IsFalse(section.ErrorMessage!.Contains("synthetic-secret", StringComparison.Ordinal));
            Assert.AreEqual(0, section.Properties.Count);

            await File.WriteAllTextAsync(path, """{"profiles":[] }""");
            await section.LoadAsync();
            Assert.AreEqual(ConfigUiSectionState.Failed, section.State);
            StringAssert.Contains(section.ErrorMessage, "profiles object");
        });
    }

    [TestMethod]
    public async Task ReadAsync_DistinguishesMissingEmptyAndPresentFields()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            string path = CreateLaunchSettings(projectPath, "{}");
            ProjectLaunchSettingsReader reader = new();
            ProjectLaunchSettingsSnapshot snapshot = await reader.ReadAsync(projectPath);
            Assert.IsTrue(snapshot.FileExists);
            Assert.IsFalse(snapshot.HasProfilesObject);
            Assert.AreEqual(0, snapshot.Profiles.Count);

            await File.WriteAllTextAsync(path, """{"profiles":{}}""");
            snapshot = await reader.ReadAsync(projectPath);
            Assert.IsTrue(snapshot.HasProfilesObject);
            Assert.AreEqual(0, snapshot.Profiles.Count);

            await File.WriteAllTextAsync(path, """{"profiles":{"NullFields":{"commandName":"Project","commandLineArgs":null,"environmentVariables":null}}}""");
            snapshot = await reader.ReadAsync(projectPath);
            Assert.IsTrue(snapshot.Profiles.Single().HasCommandLineArguments);
            Assert.IsTrue(snapshot.Profiles.Single().HasEnvironmentVariables);
        });
    }

    [TestMethod]
    public async Task ReadAsync_RejectsAmbiguousSectionsAndUnboundedProfileNamesOrCounts()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            string path = CreateLaunchSettings(projectPath, "{}");
            string tooManyProfiles = string.Join(",", Enumerable.Range(0, 257)
                .Select(index => $"\"Profile{index}\":{{\"commandName\":\"Project\"}}"));
            string[] invalidContents =
            [
                "[]",
                """{"profiles":null}""",
                """{"profiles":{},"Profiles":{}}""",
                """{"profiles":{" ":{"commandName":"Project"}}}""",
                """{"profiles":{"Bad\u0001Name":{"commandName":"Project"}}}""",
                $"{{\"profiles\":{{\"{new string('x', 257)}\":{{\"commandName\":\"Project\"}}}}}}",
                $"{{\"profiles\":{{{tooManyProfiles}}}}}",
            ];
            foreach (string content in invalidContents)
            {
                await File.WriteAllTextAsync(path, content);
                await Assert.ThrowsExactlyAsync<InvalidDataException>(
                    () => new ProjectLaunchSettingsReader().ReadAsync(projectPath), content);
            }
        });
    }

    [TestMethod]
    public async Task ReadAsync_RejectsMissingProjectAndHonorsCancellation()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            string path = CreateLaunchSettings(projectPath, """{"profiles":{}}""");
            ProjectLaunchSettingsReader reader = new();
            using CancellationTokenSource cancellation = new();
            cancellation.Cancel();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(
                () => reader.ReadAsync(projectPath, cancellation.Token));

            File.Delete(projectPath);
            await Assert.ThrowsExactlyAsync<FileNotFoundException>(() => reader.ReadAsync(projectPath));
            Assert.AreEqual("""{"profiles":{}}""", await File.ReadAllTextAsync(path));
        });
    }

    [TestMethod]
    public async Task ReadAsync_AcceptsUtf8BomAndRejectsOversizedFiles()
    {
        await WithProjectAsync(async (_, projectPath) =>
        {
            string path = CreateLaunchSettings(projectPath, "{}");
            await File.WriteAllTextAsync(path, """{"profiles":{"Bom":{"commandName":"Project"}}}""", new UTF8Encoding(true));
            ProjectLaunchSettingsSnapshot snapshot = await new ProjectLaunchSettingsReader().ReadAsync(projectPath);
            Assert.AreEqual("Bom", snapshot.Profiles.Single().Name);

            await File.WriteAllTextAsync(path, new string(' ', ProjectLaunchSettingsReader.MaximumFileSizeBytes + 1));
            InvalidDataException failure = await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => new ProjectLaunchSettingsReader().ReadAsync(projectPath));
            StringAssert.Contains(failure.Message, "inspection limit");
        });
    }

    [TestMethod]
    public async Task DiscardAsync_ReloadsOnlyTheSelectedProjectsReadOnlySnapshot()
    {
        await WithProjectAsync(async (root, firstProject) =>
        {
            string firstPath = CreateLaunchSettings(firstProject, """{"profiles":{"First":{"commandName":"Project"}}}""");
            string otherDirectory = Path.Combine(root, "Other");
            Directory.CreateDirectory(otherDirectory);
            string secondProject = Path.Combine(otherDirectory, "Other.csproj");
            await File.WriteAllTextAsync(secondProject, "<Project />");
            string secondPath = CreateLaunchSettings(secondProject, """{"profiles":{"Other":{"commandName":"Executable"}}}""");
            ProjectLaunchProfilesProvider first = new(firstProject);
            ProjectLaunchProfilesProvider second = new(secondProject);
            await first.LoadAsync();
            await second.LoadAsync();

            await File.WriteAllTextAsync(firstPath, """{"profiles":{"Changed":{"commandName":"Project"}}}""");
            await first.DiscardAsync();

            Assert.IsTrue(first.Properties.Any(property => property.DisplayName == "Changed"));
            Assert.IsFalse(first.Properties.Any(property => property.DisplayName == "First"));
            Assert.IsTrue(second.Properties.Any(property => property.DisplayName == "Other"));
            Assert.AreEqual("""{"profiles":{"Other":{"commandName":"Executable"}}}""", await File.ReadAllTextAsync(secondPath));
        });
    }

    private static string CreateLaunchSettings(string projectPath, string content)
    {
        string path = ProjectLaunchSettingsReader.GetSettingsPath(projectPath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    private static async Task WithProjectAsync(Func<string, string, Task> action)
    {
        string root = Path.Combine(Path.GetTempPath(), $"AvlnLaunchSettings-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            string projectPath = Path.Combine(root, "Sample.csproj");
            await File.WriteAllTextAsync(projectPath, "<Project />");
            await action(root, projectPath);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
