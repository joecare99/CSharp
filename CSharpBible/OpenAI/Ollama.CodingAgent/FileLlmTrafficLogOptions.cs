using Ollama.CodingAgent.Models;
using Ollama.CodingAgent.Interfaces;
using System;
using System.IO;

namespace Ollama.CodingAgent;

/// <summary>
/// Configures the central location and naming of LLM traffic session logs.
/// </summary>
public sealed class FileLlmTrafficLogOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileLlmTrafficLogOptions"/> class.
    /// </summary>
    /// <param name="baseDirectory">The directory below which vendor and application folders are created.</param>
    /// <param name="vendorName">The vendor folder name.</param>
    /// <param name="applicationName">The application folder name.</param>
    /// <param name="sessionStartTimestamp">The timestamp used for the session file name.</param>
    public FileLlmTrafficLogOptions(
        string baseDirectory,
        string vendorName = "Ollama",
        string applicationName = "CodingAgent",
        DateTimeOffset? sessionStartTimestamp = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(vendorName);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationName);
        ValidateDirectoryName(vendorName, nameof(vendorName));
        ValidateDirectoryName(applicationName, nameof(applicationName));

        BaseDirectory = Path.GetFullPath(baseDirectory);
        VendorName = vendorName;
        ApplicationName = applicationName;
        SessionStartTimestamp = sessionStartTimestamp ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets the root directory used for application data.
    /// </summary>
    public string BaseDirectory { get; }

    /// <summary>
    /// Gets the vendor folder name.
    /// </summary>
    public string VendorName { get; }

    /// <summary>
    /// Gets the application folder name.
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    /// Gets the program or session start timestamp used in the file name.
    /// </summary>
    public DateTimeOffset SessionStartTimestamp { get; }

    /// <summary>
    /// Creates default configuration below the current user's application-data directory.
    /// </summary>
    public static FileLlmTrafficLogOptions CreateDefault()
    {
        string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrWhiteSpace(appDataDirectory))
        {
            appDataDirectory = Environment.GetEnvironmentVariable("APPDATA");
        }

        if (string.IsNullOrWhiteSpace(appDataDirectory))
        {
            appDataDirectory = Path.Combine(Path.GetTempPath(), "ApplicationData");
        }

        return new FileLlmTrafficLogOptions(appDataDirectory);
    }

    private static void ValidateDirectoryName(string value, string parameterName)
    {
        if (value.IndexOfAny(Path.GetInvalidPathChars()) >= 0
            || value.Contains(Path.DirectorySeparatorChar)
            || value.Contains(Path.AltDirectorySeparatorChar)
            || value is "." or "..")
        {
            throw new ArgumentException("The directory name must be a single safe path segment.", parameterName);
        }
    }
}
