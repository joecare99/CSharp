using RepoMigrator.Core;
using RepoMigrator.Tools.PipelinedMigration;

namespace RepoMigrator.Tests;

[TestClass]
[DoNotParallelize]
public sealed class ConsoleMigrationProgressTests
{
    private static readonly object _consoleSync = new();

    [TestMethod]
    public void Report_WritesExpectedMessage_ForSourceOpening()
    {
        var progress = new ConsoleMigrationProgress();

        var writer = new StringWriter();
        string output;

        lock (_consoleSync)
        {
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(writer);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening, "SVN");
                output = writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        StringAssert.Contains(output, "Öffne Quelle (SVN)");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForPipelineEnabled()
    {
        var progress = new ConsoleMigrationProgress();

        var writer = new StringWriter();
        string output;

        lock (_consoleSync)
        {
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(writer);
                progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineEnabled, 2, 3, 10);
                output = writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        StringAssert.Contains(output, "Pipeline aktiviert: 2 Worker, Prefetch 3, 10 Changesets.");
    }

    [TestMethod]
    public void Report_Throws_WhenRequiredArgumentIsMissing()
    {
        var progress = new ConsoleMigrationProgress();

        try
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.SourceOpening);
            Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException ex)
        {
            StringAssert.Contains(ex.Message, "Expected argument at index 0");
        }
    }

    [TestMethod]
    public void Report_Throws_WhenArgumentTypeDoesNotMatch()
    {
        var progress = new ConsoleMigrationProgress();

        try
        {
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineEnabled, "bad", 3, 10);
            Assert.Fail("Expected InvalidOperationException.");
        }
        catch (InvalidOperationException ex)
        {
            StringAssert.Contains(ex.Message, "Expected argument at index 0");
        }
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForCommitCompleted_WithShortenedId()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.CommitCompleted, "1234567890abcdef", 2, 5));

        StringAssert.Contains(output, "Commit 2/5 übertragen: 12345678");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForFlushCompleted()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushCompleted, "Git"));

        StringAssert.Contains(output, "Ziel-Synchronisierung (Git) abgeschlossen.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForProjectedBranchCommitted()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchCommitted, "101", "main/feature"));

        StringAssert.Contains(output, "Branch 'main/feature' für Revision 101 übertragen.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForMigrationCompleted()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.MigrationCompleted));

        StringAssert.Contains(output, "Migration abgeschlossen.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForNoChangeSetsFound()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NoChangeSetsFound));

        StringAssert.Contains(output, "Keine Changesets gefunden.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForTargetInitializing()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.TargetInitializing, "Git"));

        StringAssert.Contains(output, "Initialisiere Ziel (Git)");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForPipelineSnapshotReady()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineSnapshotReady, "123"));

        StringAssert.Contains(output, "Snapshot 123 ist in Commit-Reihenfolge bereit");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForGitBranchTransferStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.GitBranchTransferStarting, "main", "release/main"));

        StringAssert.Contains(output, "Übertrage Branch main -> release/main");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForGitTagTransferStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.GitTagTransferStarting, "v1.0", "v1.0-import"));

        StringAssert.Contains(output, "Übertrage Tag v1.0 -> v1.0-import");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForNativeHistoryTransferStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.NativeHistoryTransferStarting, "Git", RepoType.Git));

        StringAssert.Contains(output, "Übertrage Historie nativ (Git -> Git)");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForChangeSetsLoading()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetsLoading));

        StringAssert.Contains(output, "Lese Changesets");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForPipelineCleanupStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineCleanupStarting));

        StringAssert.Contains(output, "Abbruch oder Fehler erkannt");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForChangeSetProcessingStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ChangeSetProcessingStarting, "abc123", 1, 3));

        StringAssert.Contains(output, "[1/3] abc123");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForFlushStarting()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.FlushStarting, "Git"));

        StringAssert.Contains(output, "Starte Ziel-Synchronisierung (Git)");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForExportWorkerCompleted()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ExportWorkerCompleted, 2));

        StringAssert.Contains(output, "Export-Worker 2: keine weiteren Export-Aufträge.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForPipelineSnapshotBuffered()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.PipelineSnapshotBuffered, "abc", 5));

        StringAssert.Contains(output, "Snapshot abc gepuffert");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForProjectedBranchPrepared()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchPrepared, "101", "main/feature", 7));

        StringAssert.Contains(output, "Branch 'main/feature' erhält 7 Pfade aus Revision 101.");
    }

    [TestMethod]
    public void Report_WritesExpectedMessage_ForProjectedBranchEmpty()
    {
        var progress = new ConsoleMigrationProgress();
        var output = CaptureOutput(() =>
            progress.Report(MigrationReportSeverity.Information, MigrationReportMessage.ProjectedBranchEmpty, "101", "main/feature"));

        StringAssert.Contains(output, "als leerer Snapshot fortgeführt");
    }

    [TestMethod]
    public void Report_ThrowsArgumentOutOfRange_ForUnknownMessage()
    {
        var progress = new ConsoleMigrationProgress();

        try
        {
            progress.Report(MigrationReportSeverity.Information, (MigrationReportMessage)999);
            Assert.Fail("Expected ArgumentOutOfRangeException.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            StringAssert.Contains(ex.ParamName ?? string.Empty, "message");
        }
    }

    private static string CaptureOutput(Action action)
    {
        var writer = new StringWriter();

        lock (_consoleSync)
        {
            var originalOut = Console.Out;
            try
            {
                Console.SetOut(writer);
                action();
                return writer.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
