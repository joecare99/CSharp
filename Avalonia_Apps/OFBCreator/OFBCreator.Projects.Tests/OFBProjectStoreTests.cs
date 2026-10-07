using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Projects.Tests;

[TestClass]
public sealed class OFBProjectStoreTests
{
    [TestMethod]
    public void LoadOrCreate_CreatesPortableProjectAndTracksCatalog()
    {
        using var testDirectory = new TestDirectory();
        var store = new OFBProjectStore(testDirectory.Path);

        var created = store.LoadOrCreate("Obrigheim");
        var reopened = store.Open(created.ProjectPath);

        Assert.AreEqual(OFBProject.CurrentSchemaVersion, created.Project.SchemaVersion);
        Assert.AreEqual("Obrigheim", reopened.Project.Name);
        Assert.AreEqual(Path.Combine(testDirectory.Path, "Obrigheim.ofbproject"), created.ProjectPath);
        CollectionAssert.Contains(store.GetRecentProjects().ToArray(), created.ProjectPath);
        Assert.IsTrue(File.Exists(Path.Combine(testDirectory.Path, "recent.json")));
    }

    [TestMethod]
    public void SaveAndOpen_SupportsPortableFilesOutsideTheCatalogDirectory()
    {
        using var testDirectory = new TestDirectory();
        var catalogDirectory = Path.Combine(testDirectory.Path, "catalog");
        var portableDirectory = Path.Combine(testDirectory.Path, "portable");
        var portableFile = Path.Combine(portableDirectory, "FamilyBook.ofbproject");
        var store = new OFBProjectStore(catalogDirectory);
        var project = new OFBProject
        {
            Name = "FamilyBook",
            Title = "Ortsfamilienbuch",
            InputPath = "data\\input.ged",
            OutputPath = "output\\book.docx",
            EntryTemplate = "ak",
            Preface = "Vorwort",
            Legend = "Legende"
        };

        store.Save(project, portableFile);
        var reopened = store.Open(portableFile).Project;

        Assert.AreEqual("data\\input.ged", reopened.InputPath);
        Assert.AreEqual("output\\book.docx", reopened.OutputPath);
        Assert.AreEqual("ak", reopened.EntryTemplate);
        Assert.AreEqual(Path.Combine(portableDirectory, "data", "input.ged"),
            OFBProjectStore.ResolveProjectPath(portableFile, reopened.InputPath));
    }

    [TestMethod]
    public void Open_MigratesLegacyVersionOneInMemory_AndRequiresExplicitSave()
    {
        using var testDirectory = new TestDirectory();
        var legacyPath = Path.Combine(testDirectory.Path, "Legacy.json");
        var migratedPath = Path.Combine(testDirectory.Path, "portable", "Legacy.ofbproject");
        File.WriteAllText(legacyPath,
            "{\"SchemaVersion\":1,\"Name\":\"Legacy\",\"Title\":\"Old title\",\"EntryTemplate\":\"AK\"}");
        var store = new OFBProjectStore(testDirectory.Path);

        var loaded = store.Open("Legacy");

        Assert.IsTrue(loaded.RequiresMigrationSave);
        Assert.AreEqual(1, loaded.SourceSchemaVersion);
        Assert.AreEqual("Old title", loaded.Project.Title);
        Assert.AreEqual("ak", loaded.Project.EntryTemplate);
        Assert.AreEqual(OFBProject.CurrentSchemaVersion, loaded.Project.SchemaVersion);
        Assert.IsFalse(File.Exists(migratedPath));

        var savedPath = store.SaveMigration(loaded, migratedPath);

        Assert.AreEqual(Path.GetFullPath(migratedPath), savedPath);
        Assert.AreEqual(1, ReadSchemaVersion(legacyPath));
        Assert.AreEqual(OFBProject.CurrentSchemaVersion, ReadSchemaVersion(migratedPath));
        Assert.IsFalse(store.Open(migratedPath).RequiresMigrationSave);
    }

    [TestMethod]
    public void Open_MigratesSchemaTwoProjectInMemoryToCurrentSchema()
    {
        using var testDirectory = new TestDirectory();
        var path = Path.Combine(testDirectory.Path, "SchemaTwo.ofbproject");
        var id = Guid.NewGuid();
        File.WriteAllText(path,
            $"{{\"SchemaVersion\":2,\"Id\":\"{id:D}\",\"Name\":\"SchemaTwo\",\"Title\":\"Schema Two\",\"EntryTemplate\":\"gc\"}}");

        var loaded = new OFBProjectStore(testDirectory.Path).Open(path);

        Assert.IsTrue(loaded.RequiresMigrationSave);
        Assert.AreEqual(2, loaded.SourceSchemaVersion);
        Assert.AreEqual(OFBProject.CurrentSchemaVersion, loaded.Project.SchemaVersion);
        Assert.AreEqual(0, loaded.Project.ExportRules.Count);
        Assert.AreEqual(2, ReadSchemaVersion(path));
    }

    [TestMethod]
    public void Open_MigratesSchemaThreeProjectInMemoryToCurrentSchema()
    {
        using var testDirectory = new TestDirectory();
        var path = Path.Combine(testDirectory.Path, "SchemaThree.ofbproject");
        var id = Guid.NewGuid();
        File.WriteAllText(path,
            $"{{\"SchemaVersion\":3,\"Id\":\"{id:D}\",\"Name\":\"SchemaThree\",\"Title\":\"Schema Three\",\"EntryTemplate\":\"gc\",\"ExportRules\":[]}}");

        var loaded = new OFBProjectStore(testDirectory.Path).Open(path);

        Assert.IsTrue(loaded.RequiresMigrationSave);
        Assert.AreEqual(3, loaded.SourceSchemaVersion);
        Assert.AreEqual(OFBProject.CurrentSchemaVersion, loaded.Project.SchemaVersion);
        Assert.AreEqual(85, loaded.Project.GroupingPolicy.AutoAcceptThreshold);
        Assert.AreEqual(0, loaded.Project.GroupingDecisions.Count);
        Assert.AreEqual(3, ReadSchemaVersion(path));
    }

    [TestMethod]
    public void LoadOrCreate_FindsLegacyNamedProjectInsteadOfCreatingASecondDefault()
    {
        using var testDirectory = new TestDirectory();
        var legacyPath = Path.Combine(testDirectory.Path, "Existing.json");
        File.WriteAllText(legacyPath,
            "{\"SchemaVersion\":1,\"Name\":\"Existing\",\"Title\":\"Saved title\",\"EntryTemplate\":\"ak\"}");

        var loaded = new OFBProjectStore(testDirectory.Path).LoadOrCreate("Existing");

        Assert.IsTrue(loaded.RequiresMigrationSave);
        Assert.AreEqual("Saved title", loaded.Project.Title);
        Assert.AreEqual("ak", loaded.Project.EntryTemplate);
        Assert.IsFalse(File.Exists(Path.Combine(testDirectory.Path, "Existing.ofbproject")));
    }

    [TestMethod]
    public void LoadOrCreate_FromTemplateCopiesLayoutSettingsButNotSourceInput()
    {
        using var testDirectory = new TestDirectory();
        var store = new OFBProjectStore(testDirectory.Path);
        var source = new OFBProject
        {
            Name = "Source",
            Title = "Source Title",
            EntryTemplate = "ak",
            InputPath = "private\\input.ged",
            OutputPath = "old-output.docx",
            PlaceId = "P1",
            IncludeDescendants = true,
            Preface = "Vorwort",
            Legend = "Legende",
            ExportRules =
            [
                new OFBExportRule
                {
                    Order = 2,
                    TargetKind = "person",
                    TargetId = OFBExportRuleTarget.Person("gedcom", "I1"),
                    Action = "redact",
                    Field = "birthPlace"
                }
            ],
            GroupingPolicy = new OFBGroupingPolicy { AutoAcceptThreshold = 90 },
            GroupingDecisions =
            [
                new OFBGroupingDecision
                {
                    Order = 1,
                    LeftFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F1"),
                    RightFamilyTargetId = OFBExportRuleTarget.Family("gedcom", "F2"),
                    Action = "acceptMerge"
                }
            ]
        };
        store.Save(source, Path.Combine(testDirectory.Path, "Source.ofbproject"));

        var created = store.LoadOrCreate("NewBook", "Source").Project;

        Assert.AreEqual("NewBook", created.Name);
        Assert.AreEqual("NewBook", created.Title);
        Assert.AreEqual("ak", created.EntryTemplate);
        Assert.IsNull(created.InputPath);
        Assert.IsNull(created.OutputPath);
        Assert.AreEqual("P1", created.PlaceId);
        Assert.IsTrue(created.IncludeDescendants);
        Assert.AreEqual("Vorwort", created.Preface);
        Assert.AreEqual("Legende", created.Legend);
        Assert.AreEqual(1, created.ExportRules.Count);
        Assert.AreNotEqual(source.ExportRules[0].Id, created.ExportRules[0].Id);
        Assert.AreEqual(source.ExportRules[0].TargetId, created.ExportRules[0].TargetId);
        Assert.AreEqual(90, created.GroupingPolicy.AutoAcceptThreshold);
        Assert.AreEqual(1, created.GroupingDecisions.Count);
        Assert.AreNotEqual(source.GroupingDecisions[0].Id, created.GroupingDecisions[0].Id);
    }

    [TestMethod]
    public void ResolveEntryTemplate_UsesProjectDirectoryForRelativeExternalPaths()
    {
        using var testDirectory = new TestDirectory();
        var projectFile = Path.Combine(testDirectory.Path, "project", "book.ofbproject");

        Assert.AreEqual("ak", OFBProjectStore.ResolveEntryTemplate(projectFile, "AK"));
        Assert.AreEqual(
            Path.Combine(testDirectory.Path, "project", "templates", "custom.json"),
            OFBProjectStore.ResolveEntryTemplate(projectFile, "templates\\custom.json"));
    }

    [TestMethod]
    public void Open_RejectsFutureSchemaAndInvalidProjectIdentity()
    {
        using var testDirectory = new TestDirectory();
        var futurePath = Path.Combine(testDirectory.Path, "Future.ofbproject");
        var invalidIdPath = Path.Combine(testDirectory.Path, "Invalid.ofbproject");
        File.WriteAllText(futurePath, "{\"schemaVersion\":99,\"name\":\"Future\"}");
        File.WriteAllText(invalidIdPath,
            "{\"schemaVersion\":2,\"id\":\"not-a-guid\",\"name\":\"Invalid\",\"title\":\"Invalid\",\"entryTemplate\":\"gc\"}");
        var store = new OFBProjectStore(testDirectory.Path);

        Assert.ThrowsExactly<InvalidDataException>(() => store.Open(futurePath));
        Assert.ThrowsExactly<InvalidDataException>(() => store.Open(invalidIdPath));
    }

    [TestMethod]
    public void Save_RequiresPortableExtensionAndCleansTemporaryFiles()
    {
        using var testDirectory = new TestDirectory();
        var store = new OFBProjectStore(testDirectory.Path);
        var project = new OFBProject { Name = "Book", Title = "Book" };

        Assert.ThrowsExactly<ArgumentException>(() => store.Save(project, Path.Combine(testDirectory.Path, "Book.json")));
        store.Save(project, Path.Combine(testDirectory.Path, "Book.ofbproject"));
        CollectionAssert.AreEqual(
            new[] { "Book.ofbproject", "recent.json" }.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            Directory.GetFiles(testDirectory.Path).Select(Path.GetFileName).OrderBy(value => value, StringComparer.Ordinal).ToArray());
    }

    private static int ReadSchemaVersion(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("SchemaVersion").GetInt32();
    }

    private sealed class TestDirectory : IDisposable
    {
        public TestDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ofb-projects-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path, recursive: true);
        }
    }
}
