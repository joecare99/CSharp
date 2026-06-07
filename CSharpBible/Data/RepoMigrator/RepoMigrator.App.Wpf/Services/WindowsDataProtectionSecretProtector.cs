using RepoMigrator.App.State.Services;
using System.Security.Cryptography;
using System.Text;

namespace RepoMigrator.App.Wpf.Services;

public sealed class WindowsDataProtectionSecretProtector : ISecretProtector
{
    public string? Protect(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        var bytes = Encoding.UTF8.GetBytes(value);
        var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(protectedBytes);
    }

    public string? Unprotect(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        try
        {
            var protectedBytes = Convert.FromBase64String(value);
            var bytes = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return null;
        }
    }
}