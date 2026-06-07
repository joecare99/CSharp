using RepoMigrator.App.State.Services;

namespace RepoMigrator.App.Wpf.Services;

public sealed class WindowsAppStatePathProvider : IAppStatePathProvider
{
    public string GetStateFilePath()
        => System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RepoMigrator",
            "inputs.json");
}