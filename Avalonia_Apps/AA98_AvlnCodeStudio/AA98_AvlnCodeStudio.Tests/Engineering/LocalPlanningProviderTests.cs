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
}
