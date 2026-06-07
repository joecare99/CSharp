using RepoMigrator.App.State.Services;
using RepoMigrator.App.State.Settings;

namespace RepoMigrator.Tests;

[TestClass]
public sealed class AppInputStateStoreTests
{
    [TestMethod]
    public void Save_ProtectsSecrets_AndLoad_UnprotectsSecretsThroughInjectedProtector()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"RepoMigrator.AppInputStateStoreTests.{Guid.NewGuid():N}.json");
        var protector = new RecordingSecretProtector();
        var store = new AppInputStateStore(new FixedAppStatePathProvider(filePath), protector);

        try
        {
            store.Save(new AppInputState
            {
                SourceUrl = "svn://example/source",
                SourcePassword = "source-secret",
                TargetUrl = "git://example/target",
                TargetPassword = "target-secret"
            });

            var json = File.ReadAllText(filePath);
            StringAssert.Contains(json, "protected:source-secret");
            StringAssert.Contains(json, "protected:target-secret");

            var state = store.Load();

            Assert.AreEqual("source-secret", state.SourcePassword);
            Assert.AreEqual("target-secret", state.TargetPassword);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    private sealed class RecordingSecretProtector : ISecretProtector
    {
        public string? Protect(string? value)
            => string.IsNullOrEmpty(value) ? null : $"protected:{value}";

        public string? Unprotect(string? value)
            => string.IsNullOrEmpty(value) ? null : value.Replace("protected:", string.Empty, StringComparison.Ordinal);
    }

    private sealed class FixedAppStatePathProvider(string filePath) : IAppStatePathProvider
    {
        public string GetStateFilePath()
            => filePath;
    }
}