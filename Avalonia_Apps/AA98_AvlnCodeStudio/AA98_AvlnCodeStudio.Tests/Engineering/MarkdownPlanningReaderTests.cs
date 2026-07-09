using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies deterministic loading behavior for the local markdown planning reader.
/// </summary>
[TestClass]
public class MarkdownPlanningReaderTests
{
    /// <summary>
    /// Verifies that the reader loads representative planning files and resolves parent-child links.
    /// </summary>
    [TestMethod]
    public async Task ReadAsync_LoadsPlanningItemsAndBuildsParentLinks()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Epics", "AA98-E12-DevOps-Planning-Workbench.md"),
                """
                # AA98-E12 DevOps Planning Workbench

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F43-Repository-and-Planning-Workflows.md"),
                """
                # AA98-F43 Repository and Planning Workflows

                ## Parent
                - Epic: `AA98-E12` DevOps Planning Workbench

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "BacklogItems", "AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md"),
                """
                # AA98-Bl043 DevOps Planning Local Model Baseline

                ## Parent
                - Feature: `AA98-F43` Repository and Planning Workflows

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T060-Implement-Markdown-Planning-Reader.md"),
                """
                # AA98-T060 Implement Markdown Planning Reader

                ## Parent
                - Backlog Item: `AA98-Bl043` DevOps Planning Local Model Baseline

                ## Status
                - Proposed
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            Assert.AreEqual(4, result.Items.Count);
            Assert.AreEqual(0, result.Diagnostics.Count);

            PlanningItem epic = result.Items.Single(static item => item.Id == "AA98-E12");
            PlanningItem feature = result.Items.Single(static item => item.Id == "AA98-F43");
            PlanningItem backlogItem = result.Items.Single(static item => item.Id == "AA98-Bl043");
            PlanningItem task = result.Items.Single(static item => item.Id == "AA98-T060");

            Assert.AreEqual(PlanningItemKind.Epic, epic.Kind);
            Assert.AreEqual(PlanningItemStatus.Proposed, epic.Status);
            Assert.AreEqual(PlanningItemKind.Feature, feature.Kind);
            Assert.AreEqual(PlanningItemStatus.InProgress, feature.Status);
            Assert.AreEqual(PlanningItemKind.BacklogItem, backlogItem.Kind);
            Assert.AreEqual(PlanningItemStatus.InProgress, backlogItem.Status);
            Assert.AreEqual(PlanningItemKind.Task, task.Kind);
            Assert.AreEqual(PlanningItemStatus.Proposed, task.Status);

            Assert.IsNotNull(feature.Parent);
            Assert.AreEqual("AA98-E12", feature.Parent.ItemId);
            Assert.AreEqual(PlanningItemKind.Epic, feature.Parent.Kind);
            Assert.AreEqual(epic.SourcePath, feature.Parent.SourcePath);

            Assert.IsNotNull(backlogItem.Parent);
            Assert.AreEqual("AA98-F43", backlogItem.Parent.ItemId);
            Assert.AreEqual(PlanningItemKind.Feature, backlogItem.Parent.Kind);
            Assert.AreEqual(feature.SourcePath, backlogItem.Parent.SourcePath);

            Assert.IsNotNull(task.Parent);
            Assert.AreEqual("AA98-Bl043", task.Parent.ItemId);
            Assert.AreEqual(PlanningItemKind.BacklogItem, task.Parent.Kind);
            Assert.AreEqual(backlogItem.SourcePath, task.Parent.SourcePath);

            Assert.AreEqual("AA98-F43", epic.Children.Single().ItemId);
            Assert.AreEqual("AA98-Bl043", feature.Children.Single().ItemId);
            Assert.AreEqual("AA98-T060", backlogItem.Children.Single().ItemId);
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    /// <summary>
    /// Verifies that duplicate IDs and missing parents are reported without aborting the read.
    /// </summary>
    [TestMethod]
    public async Task ReadAsync_ReportsDuplicateIdsAndMissingParents()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F43-Repository-and-Planning-Workflows.md"),
                """
                # AA98-F43 Repository and Planning Workflows

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T060-Implement-Markdown-Planning-Reader.md"),
                """
                # AA98-T060 Implement Markdown Planning Reader

                ## Parent
                - Backlog Item: `AA98-Bl999` Missing Item

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T060-Duplicate.md"),
                """
                # AA98-T060 Duplicate Planning Reader Task

                ## Status
                - Proposed
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            Assert.AreEqual(3, result.Items.Count);

            PlanningItem[] duplicateItems = result.Items.Where(static item => item.Id == "AA98-T060").ToArray();
            Assert.AreEqual(2, duplicateItems.Length);
            Assert.IsTrue(duplicateItems.All(static item => item.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN030")));

            PlanningItem missingParentItem = duplicateItems.Single(static item => item.Title == "Implement Markdown Planning Reader");
            Assert.IsTrue(missingParentItem.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    /// <summary>
    /// Verifies deterministic diagnostics for malformed headings and invalid status conventions.
    /// </summary>
    [TestMethod]
    public async Task ReadAsync_ReportsHeadingAndStatusConventionDiagnostics()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T061-Malformed-Heading.md"),
                """
                #AA98-T061 Malformed Heading
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T061-Unknown-Status.md"),
                """
                # AA98-T061 Unknown Status Value

                ## Status
                - Started
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T061-Empty-Status.md"),
                """
                # AA98-T061 Empty Status Section

                ## Status
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            PlanningItem malformedHeadingItem = result.Items.Single(static item => item.SourcePath.EndsWith("AA98-T061-Malformed-Heading.md", StringComparison.Ordinal));
            Assert.IsTrue(malformedHeadingItem.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN010"));
            Assert.IsTrue(malformedHeadingItem.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN020"));

            PlanningItem unknownStatusItem = result.Items.Single(static item => item.Title == "Unknown Status Value");
            Assert.AreEqual(PlanningItemStatus.Unknown, unknownStatusItem.Status);
            Assert.IsTrue(unknownStatusItem.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN021"));

            PlanningItem emptyStatusItem = result.Items.Single(static item => item.Title == "Empty Status Section");
            Assert.AreEqual(PlanningItemStatus.Unknown, emptyStatusItem.Status);
            Assert.IsTrue(emptyStatusItem.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN022"));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    /// <summary>
    /// Verifies file-based parent references are resolved while allowing items without parent links.
    /// </summary>
    [TestMethod]
    public async Task ReadAsync_ResolvesPathBasedParentReferencesAndAllowsMissingParentSection()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Epics", "AA98-E12-DevOps-Planning-Workbench.md"),
                """
                # AA98-E12 DevOps Planning Workbench

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F43-Repository-and-Planning-Workflows.md"),
                """
                # AA98-F43 Repository and Planning Workflows

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "BacklogItems", "AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md"),
                """
                # AA98-Bl043 DevOps Planning Local Model Baseline

                ## Parent
                - Feature: `AA98-F43` Repository and Planning Workflows

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T061-Broken-CrossLink.md"),
                """
                # AA98-T061 Broken Cross Link Example

                ## Parent
                - Backlog Item: `../BacklogItems/AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md`

                ## Status
                - Proposed
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            PlanningItem feature = result.Items.Single(static item => item.Id == "AA98-F43");
            Assert.IsNull(feature.Parent);
            Assert.IsFalse(feature.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));

            PlanningItem task = result.Items.Single(static item => item.Id == "AA98-T061");
            Assert.IsNotNull(task.Parent);
            Assert.AreEqual("AA98-Bl043", task.Parent.ItemId);
            Assert.AreEqual(PlanningItemKind.BacklogItem, task.Parent.Kind);
            Assert.AreEqual("DevOps\\BacklogItems\\AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md", task.Parent.SourcePath);
            Assert.IsFalse(task.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task ReadAsync_UsesFirstParentAndStoresAdditionalParentsAsRelatedEntries()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Epics", "AA98-E12-DevOps-Planning-Workbench.md"),
                """
                # AA98-E12 DevOps Planning Workbench

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F43-Repository-and-Planning-Workflows.md"),
                """
                # AA98-F43 Repository and Planning Workflows

                ## Parent
                - Epic: `AA98-E12`
                - Epic: `AA98-E13`

                ## Status
                - In Progress
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            PlanningItem feature = result.Items.Single(static item => item.Id == "AA98-F43");
            Assert.IsFalse(feature.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN032"));
            Assert.IsNotNull(feature.Parent);
            Assert.AreEqual("AA98-E12", feature.Parent.ItemId);
            Assert.AreEqual(1, feature.RelatedParents.Count);
            Assert.AreEqual("AA98-E13", feature.RelatedParents[0].ItemId);
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task ReadAsync_UsesFileNameAndDevOpsRootFallbacksForParentResolution()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Vision.md"),
                """
                # Vision
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Epics", "AA98-E12-DevOps-Planning-Workbench.md"),
                """
                # AA98-E12 DevOps Planning Workbench

                ## Parent
                - Vision: `DevOps/Vision.md`

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F43-Repository-and-Planning-Workflows.md"),
                """
                # AA98-F43 Repository and Planning Workflows

                ## Parent
                - Epic: `AA98-E12-DevOps-Planning-Workbench.md`

                ## Status
                - In Progress
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            PlanningItem epic = result.Items.Single(static item => item.Id == "AA98-E12");
            Assert.IsFalse(epic.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
            Assert.IsNotNull(epic.Parent);
            Assert.AreEqual("DevOps/Vision.md", epic.Parent.SourcePath);

            PlanningItem feature = result.Items.Single(static item => item.Id == "AA98-F43");
            Assert.IsNotNull(feature.Parent);
            Assert.AreEqual("AA98-E12", feature.Parent.ItemId);
            Assert.AreEqual(epic.SourcePath, feature.Parent.SourcePath);
            Assert.IsFalse(feature.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    [TestMethod]
    public async Task ReadAsync_UsesFirstParentForInLineChainAndParsesDoneStatus()
    {
        string repositoryRootPath = CreateRepositoryRoot();

        try
        {
            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "BacklogItems", "AA98-Bl001-Small-Editor-Foundation.md"),
                """
                # AA98-Bl001 Small Editor Foundation

                ## Status
                - Proposed
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "BacklogItems", "AA98-Bl010-Component-Registration-Baseline.md"),
                """
                # AA98-Bl010 Component Registration Baseline

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Features", "AA98-F03-Workbench-Startup-and-Composition.md"),
                """
                # AA98-F03 Workbench Startup and Composition

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Epics", "AA98-E02-Editor-Framework.md"),
                """
                # AA98-E02 Editor Framework

                ## Parent
                - Vision: `DevOps/Vision.md`

                ## Status
                - In Progress
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T001-Implement-Small-Editor.md"),
                """
                # AA98-T001 Implement Small Editor

                ## Parent
                - Backlog Item: `AA98-Bl001 Small Editor Foundation`
                """);

            WritePlanningFile(
                repositoryRootPath,
                Path.Combine("DevOps", "Tasks", "AA98-T001-Extract-Editor-Component-And-DI-Hosted-Avalonia-UI.md"),
                """
                # AA98-T001 Extract Editor Component And DI Hosted Avalonia UI

                ## Parent
                - Backlog Item: `DevOps/BacklogItems/AA98-Bl010-Component-Registration-Baseline.md`
                - Feature: `DevOps/Features/AA98-F03-Workbench-Startup-and-Composition.md`
                - Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`

                ## Status
                - Done
                """);

            MarkdownPlanningReader reader = new();
            PlanningReadResult result = await reader.ReadAsync(new PlanningReadRequest
            {
                RepositoryRootPath = repositoryRootPath,
            }).ConfigureAwait(false);

            PlanningItem extractedTask = result.Items.Single(static item =>
                item.SourcePath.EndsWith("AA98-T001-Extract-Editor-Component-And-DI-Hosted-Avalonia-UI.md", StringComparison.Ordinal));

            Assert.AreEqual(PlanningItemStatus.Completed, extractedTask.Status);
            Assert.IsNotNull(extractedTask.Parent);
            Assert.AreEqual("AA98-Bl010", extractedTask.Parent.ItemId);
            Assert.AreEqual("DevOps\\BacklogItems\\AA98-Bl010-Component-Registration-Baseline.md", extractedTask.Parent.SourcePath);
            Assert.AreEqual(2, extractedTask.RelatedParents.Count);
            Assert.AreEqual("DevOps/Features/AA98-F03-Workbench-Startup-and-Composition.md", extractedTask.RelatedParents[0].ItemId);
            Assert.AreEqual("DevOps/Epics/AA98-E02-Editor-Framework.md", extractedTask.RelatedParents[1].ItemId);

            PlanningItem smallEditorTask = result.Items.Single(static item =>
                item.SourcePath.EndsWith("AA98-T001-Implement-Small-Editor.md", StringComparison.Ordinal));
            Assert.IsNotNull(smallEditorTask.Parent);
            Assert.AreEqual("AA98-Bl001", smallEditorTask.Parent.ItemId);
            Assert.AreEqual("DevOps\\BacklogItems\\AA98-Bl001-Small-Editor-Foundation.md", smallEditorTask.Parent.SourcePath);
            Assert.IsFalse(smallEditorTask.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
        }
        finally
        {
            Directory.Delete(repositoryRootPath, recursive: true);
        }
    }

    private static string CreateRepositoryRoot()
    {
        string repositoryRootPath = Path.Combine(Path.GetTempPath(), "AA98_PlanningReaderTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(repositoryRootPath);
        return repositoryRootPath;
    }

    private static void WritePlanningFile(string repositoryRootPath, string relativePath, string content)
    {
        string filePath = Path.Combine(repositoryRootPath, relativePath);
        string? directoryPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(filePath, content.ReplaceLineEndings(Environment.NewLine));
    }
}