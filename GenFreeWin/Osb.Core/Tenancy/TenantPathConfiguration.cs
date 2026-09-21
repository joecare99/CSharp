using System;
using System.IO;

namespace Osb.Core.Tenancy;

/// <summary>
/// Represents the absolute directory of a Gen_Plus tenant supplied at application startup.
/// </summary>
public sealed class TenantPathConfiguration
{
    private TenantPathConfiguration(string tenantRootPath)
    {
        TenantRootPath = tenantRootPath;
    }

    /// <summary>
    /// Gets the normalized absolute tenant directory with a trailing directory separator.
    /// </summary>
    public string TenantRootPath { get; }

    /// <summary>
    /// Parses the mandatory tenant-path startup argument.
    /// </summary>
    /// <param name="arguments">The command-line arguments passed to the OSB executable.</param>
    /// <returns>A normalized tenant-path configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when the argument syntax is invalid.</exception>
    public static TenantPathConfiguration FromCommandLine(string[] arguments)
    {
        if (arguments == null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        if (arguments.Length == 1 && arguments[0].StartsWith("--tenant=", StringComparison.OrdinalIgnoreCase))
        {
            return FromAbsolutePath(arguments[0].Substring("--tenant=".Length));
        }

        if (arguments.Length == 1)
        {
            return FromAbsolutePath(arguments[0]);
        }

        if (arguments.Length == 2 && string.Equals(arguments[0], "--tenant", StringComparison.OrdinalIgnoreCase))
        {
            return FromAbsolutePath(arguments[1]);
        }

        throw new ArgumentException(
            "Pass the full absolute tenant directory as '--tenant <path>' or as the only startup argument.",
            nameof(arguments));
    }

    /// <summary>
    /// Creates a configuration from an absolute tenant directory without touching the filesystem.
    /// </summary>
    /// <param name="tenantPath">The tenant directory supplied by the caller.</param>
    /// <returns>A normalized configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when the path is missing or relative.</exception>
    public static TenantPathConfiguration FromAbsolutePath(string tenantPath)
    {
        if (string.IsNullOrWhiteSpace(tenantPath))
        {
            throw new ArgumentException("A tenant directory path is required.", nameof(tenantPath));
        }

        if (!Path.IsPathRooted(tenantPath))
        {
            throw new ArgumentException("The tenant directory path must be absolute.", nameof(tenantPath));
        }

        var normalizedPath = Path.GetFullPath(tenantPath);
        if (!normalizedPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) &&
            !normalizedPath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
        {
            normalizedPath += Path.DirectorySeparatorChar;
        }

        return new TenantPathConfiguration(normalizedPath);
    }

    /// <summary>
    /// Verifies that the configured tenant directory exists before database access begins.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when the configured tenant directory does not exist.
    /// </exception>
    public void EnsureDirectoryExists()
    {
        if (!Directory.Exists(TenantRootPath))
        {
            throw new DirectoryNotFoundException(
                string.Format("The tenant directory '{0}' does not exist.", TenantRootPath));
        }
    }
}
