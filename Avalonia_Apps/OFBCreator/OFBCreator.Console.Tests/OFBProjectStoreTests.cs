using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Projects.Models;
using OFBCreator.Projects.Services;

namespace OFBCreator.Console.Tests;

[TestClass]
public sealed class OFBProjectStoreTests
{
    [TestMethod]
    public void LoadOrCreate_CreatesAndReloadsProjectDefaults()
    {
        var directory = CreateTestDirectory();
        try
        {
            var store = new OFBProjectStore(directory);

            var created = store.LoadOrCreate("Obrigheim");
            var loaded = store.LoadOrCreate("Obrigheim");

            Assert.AreEqual("Obrigheim", created.Project.Name);
            Assert.AreEqual("Obrigheim", created.Project.Title);
            Assert.AreEqual("gc", created.Project.EntryTemplate);
            Assert.AreEqual(created.Project.Name, loaded.Project.Name);
            Assert.AreEqual(created.Project.Title, loaded.Project.Title);
            Assert.IsTrue(File.Exists(Path.Combine(directory, "Obrigheim.ofbproject")));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void LoadOrCreate_InitializesNewProjectFromTemplateDefaults()
    {
        var directory = CreateTestDirectory();
        try
        {
            var templatePath = Path.Combine(directory, "Ak-Vorlage.ofbproject");
            var template = new OFBProject
            {
                Name = "Ak-Vorlage",
                Title = "Altes Buch",
                EntryTemplate = "ak",
                Preface = "Vorwort",
                InputPath = "source.ged"
            };
            var store = new OFBProjectStore(directory);
            store.Save(template, templatePath);

            var project = store.LoadOrCreate("Neues Buch", "Ak-Vorlage");

            Assert.AreEqual("Neues Buch", project.Project.Name);
            Assert.AreEqual("Neues Buch", project.Project.Title);
            Assert.AreEqual("ak", project.Project.EntryTemplate);
            Assert.AreEqual("Vorwort", project.Project.Preface);
            Assert.IsNull(project.Project.InputPath);
            Assert.IsTrue(File.Exists(Path.Combine(directory, "Neues Buch.ofbproject")));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void LoadOrCreate_PreservesExternalTemplatePathAsProjectDefault()
    {
        var directory = CreateTestDirectory();
        try
        {
            var templatePath = Path.Combine(directory, "custom.ofbproject");
            var source = new OFBProject { Name = "Source", Title = "Source", EntryTemplate = "gc" };
            source.EntryTemplate = templatePath;
            var store = new OFBProjectStore(directory);
            store.Save(source, Path.Combine(directory, "Source.ofbproject"));
            var project = store.LoadOrCreate("Destination", "Source");

            Assert.AreEqual(templatePath, project.Project.EntryTemplate);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void LoadOrCreate_RejectsMissingTemplateAndPathTraversal()
    {
        var directory = CreateTestDirectory();
        try
        {
            var store = new OFBProjectStore(directory);

            Assert.ThrowsExactly<FileNotFoundException>(() => store.LoadOrCreate("Neues Buch", "Fehlt"));
            Assert.ThrowsExactly<ArgumentException>(() => store.LoadOrCreate("..\\outside"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void LoadOrCreate_RejectsTemplateOptionForExistingProject()
    {
        var directory = CreateTestDirectory();
        try
        {
            var store = new OFBProjectStore(directory);
            store.LoadOrCreate("Vorhanden");

            Assert.ThrowsExactly<InvalidOperationException>(
                () => store.LoadOrCreate("Vorhanden", "Andere Vorlage"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTestDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"ofb-project-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}
