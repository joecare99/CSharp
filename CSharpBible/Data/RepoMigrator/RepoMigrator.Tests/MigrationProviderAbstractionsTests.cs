using RepoMigrator.Core;
using RepoMigrator.Core.Abstractions;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class MigrationProviderAbstractionsTests
{
    [TestMethod]
    public void MigrationSourcePlan_Defaults_AreInitialized()
    {
        var plan = new MigrationSourcePlan();

        Assert.IsNotNull(plan.Source);
        Assert.AreEqual(MigrationSourceKind.Repository, plan.Source.Kind);
        Assert.AreEqual(0, plan.Items.Count);
    }

    [TestMethod]
    public void MigrationSourcePlanItem_Defaults_AreInitialized()
    {
        var item = new MigrationSourcePlanItem();

        Assert.AreEqual(string.Empty, item.ItemId);
        Assert.AreEqual(string.Empty, item.SnapshotId);
        Assert.AreEqual(string.Empty, item.SourceIdentifier);
        Assert.IsNull(item.DisplayName);
    }

    [TestMethod]
    public void MigrationProviderInterfaces_AreAssignable_FromTestDoubles()
    {
        IMigrationSourceProvider sourceProvider = new TestMigrationSourceProvider();
        IMigrationDestinationProvider destinationProvider = new TestMigrationDestinationProvider();
        IMigrationSourceProviderFactory sourceFactory = new TestMigrationSourceProviderFactory(sourceProvider);
        IMigrationDestinationProviderFactory destinationFactory = new TestMigrationDestinationProviderFactory(destinationProvider);

        Assert.AreEqual("Test source", sourceProvider.Name);
        Assert.AreEqual("Test destination", destinationProvider.Name);
        Assert.AreSame(sourceProvider, sourceFactory.Create(new MigrationSourceDefinition()));
        Assert.AreSame(destinationProvider, destinationFactory.Create(new MigrationDestinationDefinition()));
    }

    private sealed class TestMigrationSourceProvider : IMigrationSourceProvider
    {
        public string Name => "Test source";

        public bool CanHandle(MigrationSourceDefinition source) => true;

        public Task<MigrationSourcePlan> PrepareAsync(MigrationSourceDefinition source, CancellationToken ct)
            => Task.FromResult(new MigrationSourcePlan { Source = source });
    }

    private sealed class TestMigrationDestinationProvider : IMigrationDestinationProvider
    {
        public string Name => "Test destination";

        public bool CanHandle(MigrationDestinationDefinition destination) => true;

        public Task InitializeAsync(MigrationDestinationDefinition destination, CancellationToken ct)
            => Task.CompletedTask;

        public Task WriteSnapshotAsync(string workDir, MigrationDestinationCommit metadata, IMigrationProgress progress, CancellationToken ct)
            => Task.CompletedTask;

        public Task FinalizeAsync(CancellationToken ct)
            => Task.CompletedTask;

        public ValueTask DisposeAsync()
            => ValueTask.CompletedTask;
    }

    private sealed class TestMigrationSourceProviderFactory(IMigrationSourceProvider provider) : IMigrationSourceProviderFactory
    {
        public IMigrationSourceProvider Create(MigrationSourceDefinition source) => provider;
    }

    private sealed class TestMigrationDestinationProviderFactory(IMigrationDestinationProvider provider) : IMigrationDestinationProviderFactory
    {
        public IMigrationDestinationProvider Create(MigrationDestinationDefinition destination) => provider;
    }
}
