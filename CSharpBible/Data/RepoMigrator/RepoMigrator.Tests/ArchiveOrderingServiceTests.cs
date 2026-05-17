using RepoMigrator.Providers.Archive;
using RepoMigrator.Providers.Archive.Services;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class ArchiveOrderingServiceTests
{
    [TestMethod]
    public void Order_WhenManualOrderIsPresent_PrefersManualOrderBeforeOtherSignals()
    {
        var service = new ArchiveOrderingService();
        var snapshots = new[]
        {
            CreateSnapshot("release-2.0.zip", discoveryIndex: 1, manualOrderIndex: 2, newestEntryTimestamp: new DateTimeOffset(2024, 1, 3, 0, 0, 0, TimeSpan.Zero)),
            CreateSnapshot("release-1.0.zip", discoveryIndex: 0, manualOrderIndex: 1, newestEntryTimestamp: new DateTimeOffset(2024, 1, 2, 0, 0, 0, TimeSpan.Zero))
        };

        var result = service.Order(snapshots);

        CollectionAssert.AreEqual(new[] { "release-1.0", "release-2.0" }, result.Select(item => item.SnapshotId).ToArray());
        Assert.AreEqual(ArchiveOrderingSignalKind.ManualOrder, result[0].Evidence.Signals[0].Kind);
    }

    [TestMethod]
    public void Order_WhenManualOrderIsMissing_UsesDetectedVersionBeforeTimestamp()
    {
        var service = new ArchiveOrderingService();
        var snapshots = new[]
        {
            CreateSnapshot("release-2.0.zip", discoveryIndex: 0, newestEntryTimestamp: new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            CreateSnapshot("release-10.0.zip", discoveryIndex: 1, newestEntryTimestamp: new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)),
            CreateSnapshot("release-1.5.zip", discoveryIndex: 2, newestEntryTimestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero))
        };

        var result = service.Order(snapshots);

        CollectionAssert.AreEqual(new[] { "release-1.5", "release-2.0", "release-10.0" }, result.Select(item => item.SnapshotId).ToArray());
        Assert.IsTrue(result[0].Evidence.Signals.Any(signal => signal.Kind == ArchiveOrderingSignalKind.DetectedVersion));
    }

    [TestMethod]
    public void Order_WhenVersionIsMissing_UsesNewestEntryTimestampThenStableFileName()
    {
        var service = new ArchiveOrderingService();
        var snapshots = new[]
        {
            CreateSnapshot("beta.zip", discoveryIndex: 1, archiveBaseName: "beta", newestEntryTimestamp: new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero), detectedVersionText: string.Empty),
            CreateSnapshot("alpha.zip", discoveryIndex: 0, archiveBaseName: "alpha", newestEntryTimestamp: new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero), detectedVersionText: string.Empty)
        };

        var result = service.Order(snapshots);

        CollectionAssert.AreEqual(new[] { "alpha", "beta" }, result.Select(item => item.SnapshotId).ToArray());
        Assert.IsTrue(result[0].Evidence.Signals.Any(signal => signal.Kind == ArchiveOrderingSignalKind.ArchiveFileName));
    }

    private static ArchiveSnapshotDescriptor CreateSnapshot(
        string archiveFileName,
        int discoveryIndex,
        int? manualOrderIndex = null,
        DateTimeOffset? newestEntryTimestamp = null,
        string? archiveBaseName = null,
        string? detectedVersionText = null)
    {
        var baseName = archiveBaseName ?? Path.GetFileNameWithoutExtension(archiveFileName);
        return new ArchiveSnapshotDescriptor
        {
            SnapshotId = baseName,
            ArchivePathOrUrl = archiveFileName,
            ArchiveFileName = archiveFileName,
            ArchiveBaseName = baseName,
            ArchiveExtension = Path.GetExtension(archiveFileName),
            DriverId = "zip",
            DetectedVersionText = detectedVersionText,
            DiscoveryIndex = discoveryIndex,
            ManualOrderIndex = manualOrderIndex,
            NewestEntryTimestamp = newestEntryTimestamp
        };
    }
}
