using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Local.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies the extracted local planning driver behavior.
/// </summary>
[TestClass]
public class LocalPlanningProviderTests
{
    /// <summary>
    /// Verifies that embedded templates are exposed for the supported planning kinds.
    /// </summary>
    [TestMethod]
    public async Task GetTemplatesAsync_ReturnsEmbeddedTemplates()
    {
        LocalPlanningProvider provider = new();

        var templates = await provider.GetTemplatesAsync().ConfigureAwait(false);

        Assert.IsTrue(templates.Any(static template => template.Kind == PlanningItemKind.Epic));
        Assert.IsTrue(templates.Any(static template => template.Kind == PlanningItemKind.Feature));
        Assert.IsTrue(templates.Any(static template => template.Kind == PlanningItemKind.BacklogItem));
        Assert.IsTrue(templates.Any(static template => template.Kind == PlanningItemKind.Task));
    }

    /// <summary>
    /// Verifies that markdown documents can be written through the local planning provider.
    /// </summary>
    [TestMethod]
    public async Task WriteAsync_CreatesMarkdownDocumentFromTemplate()
    {
        string repositoryRootPath = Path.Combine(Path.GetTempPath(), "AA98_LocalPlanningProviderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(repositoryRootPath);

        try
        {
            LocalPlanningProvider provider = new();
            PlanningWriteRequest request = new()
            {
                RepositoryRootPath = repositoryRootPath,
                PlanningRootPath = "DevOps",
            };

            request.Items.Add(new PlanningItem
            {
                Id = "AA98-T999",
                Title = "Write Through Local Driver",
                Kind = PlanningItemKind.Task,
                Status = PlanningItemStatus.Completed,
                Parent = new PlanningItemLink
                {
                    ItemId = "AA98-Bl010",
                    Kind = PlanningItemKind.BacklogItem,
                    SourcePath = "DevOps/BacklogItems/AA98-Bl010-Component-Registration-Baseline.md",
                },
            });

            PlanningWriteResult result = await provider.WriteAsync(request).ConfigureAwait(false);

            Assert.AreEqual(1, result.WrittenSourcePaths.Count);
            string writtenPath = Path.Combine(repositoryRootPath, result.WrittenSourcePaths.Single());
            Assert.IsTrue(File.Exists(writtenPath));

            string content = await File.ReadAllTextAsync(writtenPath).ConfigureAwait(false);
            StringAssert.Contains(content, "# AA98-T999 Write Through Local Driver");
            StringAssert.Contains(content, "- Done");
            StringAssert.Contains(content, "DevOps/BacklogItems/AA98-Bl010-Component-Registration-Baseline.md");
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that saving an existing document preserves its content outside editable metadata.
    /// </summary>
    [TestMethod]
    public async Task WriteAsync_ExistingDocument_PreservesCustomMarkdownAndUpdatesMetadata()
    {
        string repositoryRootPath = Path.Combine(Path.GetTempPath(), "AA98_LocalPlanningProviderTests", Guid.NewGuid().ToString("N"));
        string relativeSourcePath = Path.Combine("DevOps", "Tasks", "AA98-T064-Add-Planning-UI-Tests.md");
        string sourcePath = Path.Combine(repositoryRootPath, relativeSourcePath);
        Directory.CreateDirectory(Path.GetDirectoryName(sourcePath)!);
        await File.WriteAllTextAsync(sourcePath, """
            # AA98-T064 Previous Title

            ## Goal
            Preserve this custom content.

            ## Status
            - Proposed

            ## Notes
            Keep this section unchanged.
            """).ConfigureAwait(false);

        try
        {
            LocalPlanningProvider provider = new();
            PlanningWriteRequest request = new()
            {
                RepositoryRootPath = repositoryRootPath,
            };
            request.Items.Add(new PlanningItem
            {
                Id = "AA98-T064",
                Title = "Updated Planning UI Tests",
                Kind = PlanningItemKind.Task,
                Status = PlanningItemStatus.Completed,
                SourcePath = relativeSourcePath,
                DocumentText = await File.ReadAllTextAsync(sourcePath).ConfigureAwait(false),
            });

            PlanningWriteResult result = await provider.WriteAsync(request).ConfigureAwait(false);

            Assert.AreEqual(1, result.WrittenSourcePaths.Count);
            string content = await File.ReadAllTextAsync(sourcePath).ConfigureAwait(false);
            StringAssert.Contains(content, "# AA98-T064 Updated Planning UI Tests");
            StringAssert.Contains(content, "- Done");
            StringAssert.Contains(content, "Preserve this custom content.");
            StringAssert.Contains(content, "Keep this section unchanged.");
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that external changes prevent an existing planning document from being overwritten.
    /// </summary>
    [TestMethod]
    public async Task WriteAsync_ExistingDocumentChangedAfterLoad_ReportsConflictAndPreservesFile()
    {
        string repositoryRootPath = Path.Combine(Path.GetTempPath(), "AA98_LocalPlanningProviderTests", Guid.NewGuid().ToString("N"));
        string relativeSourcePath = Path.Combine("DevOps", "Tasks", "AA98-T067-Persist-Local-Planning-Document-Edits.md");
        string sourcePath = Path.Combine(repositoryRootPath, relativeSourcePath);
        string loadedDocumentText = "# AA98-T067 Persist Local Planning Document Edits\n\n## Status\n- Proposed\n";
        string externallyChangedDocumentText = "# AA98-T067 Persist Local Planning Document Edits\n\n## Status\n- In Progress\n\n## Notes\nExternal change.\n";
        Directory.CreateDirectory(Path.GetDirectoryName(sourcePath)!);
        await File.WriteAllTextAsync(sourcePath, externallyChangedDocumentText).ConfigureAwait(false);

        try
        {
            LocalPlanningProvider provider = new();
            PlanningWriteRequest request = new()
            {
                RepositoryRootPath = repositoryRootPath,
            };
            request.ExpectedDocumentTexts[relativeSourcePath] = loadedDocumentText;
            request.Items.Add(new PlanningItem
            {
                Id = "AA98-T067",
                Title = "Changed Title",
                Kind = PlanningItemKind.Task,
                Status = PlanningItemStatus.Completed,
                SourcePath = relativeSourcePath,
                DocumentText = loadedDocumentText,
            });

            PlanningWriteResult result = await provider.WriteAsync(request).ConfigureAwait(false);

            Assert.AreEqual(0, result.WrittenSourcePaths.Count);
            Assert.AreEqual(1, result.Diagnostics.Count);
            Assert.AreEqual("PLW002", result.Diagnostics.Single().Code);
            Assert.AreEqual(externallyChangedDocumentText, await File.ReadAllTextAsync(sourcePath).ConfigureAwait(false));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }
}
