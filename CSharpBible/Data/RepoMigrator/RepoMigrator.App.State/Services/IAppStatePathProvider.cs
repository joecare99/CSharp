namespace RepoMigrator.App.State.Services;

/// <summary>
/// Resolves the storage path for persisted RepoMigrator application state.
/// </summary>
public interface IAppStatePathProvider
{
    string GetStateFilePath();
}