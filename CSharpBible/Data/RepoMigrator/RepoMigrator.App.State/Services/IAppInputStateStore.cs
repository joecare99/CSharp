using RepoMigrator.App.State.Settings;

namespace RepoMigrator.App.State.Services;

/// <summary>
/// Loads and saves the persisted RepoMigrator application input state.
/// </summary>
public interface IAppInputStateStore
{
    AppInputState Load();

    void Save(AppInputState state);
}