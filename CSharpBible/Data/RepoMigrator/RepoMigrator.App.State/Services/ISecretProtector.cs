namespace RepoMigrator.App.State.Services;

/// <summary>
/// Protects and unprotects persisted secret values.
/// </summary>
public interface ISecretProtector
{
    string? Protect(string? value);

    string? Unprotect(string? value);
}