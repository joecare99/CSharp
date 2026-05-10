using Microsoft.Extensions.Configuration;
using RnzTrauer.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Security;
using BaseLib.Helper;

namespace RnzTrauer.Console.Configuration;

/// <summary>
/// Loads RNZ console configuration from application settings and overlays secrets from the user secrets store.
/// </summary>
public sealed class RnzConsoleConfigurationLoader
{
    private const string SecretSectionName = "RnzConfig";

    /// <summary>
    /// Loads the effective RNZ configuration.
    /// </summary>
    public RnzConfig Load()
    {
        var xBaseConfig = new RnzConfig
        {
            Browser = ParseBrowser(GetRequiredAppSetting(nameof(RnzConfig.Browser))),
            Url = GetRequiredAppSetting(nameof(RnzConfig.Url)),
            Title = GetRequiredAppSetting(nameof(RnzConfig.Title)),
            User = GetOptionalAppSetting(nameof(RnzConfig.User)),
            Password = GetOptionalAppSetting(nameof(RnzConfig.Password)).ToSecureString(),
            LocalPath = GetRequiredAppSetting(nameof(RnzConfig.LocalPath)),
            DBuser = GetRequiredAppSetting(nameof(RnzConfig.DBuser)),
            DBpass = GetOptionalAppSetting(nameof(RnzConfig.DBpass)).ToSecureString(),
            DBhost = GetRequiredAppSetting(nameof(RnzConfig.DBhost)),
            DB = GetRequiredAppSetting(nameof(RnzConfig.DB))
        };

        ApplySecrets(xBaseConfig, BuildSecretsConfiguration());
        Validate(xBaseConfig);
        return xBaseConfig;
    }

    public static void ApplySecrets(RnzConfig xConfig, IConfiguration xSecrets)
    {
        ArgumentNullException.ThrowIfNull(xConfig);
        ArgumentNullException.ThrowIfNull(xSecrets);

        xConfig.User = GetOverlayValue(xSecrets, nameof(RnzConfig.User), xConfig.User);
        xConfig.Password = GetOverlayValue(xSecrets, nameof(RnzConfig.Password),"").ToSecureString();
        xConfig.DBuser = GetOverlayValue(xSecrets, nameof(RnzConfig.DBuser), xConfig.DBuser);
        xConfig.DBpass = GetOverlayValue(xSecrets, nameof(RnzConfig.DBpass),"").ToSecureString();
        xConfig.DBhost = GetOverlayValue(xSecrets, nameof(RnzConfig.DBhost), xConfig.DBhost);
        xConfig.DB = GetOverlayValue(xSecrets, nameof(RnzConfig.DB), xConfig.DB);
    }

    internal static IConfiguration BuildSecretsConfiguration()
    {
        return new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
            .Build();
    }

    public static void Validate(RnzConfig xConfig)
    {
        ArgumentNullException.ThrowIfNull(xConfig);

        var arrMissingValues = new List<string>();
        AddMissing(arrMissingValues, nameof(RnzConfig.Url), xConfig.Url);
        AddMissing(arrMissingValues, nameof(RnzConfig.Title), xConfig.Title);
        AddMissing(arrMissingValues, nameof(RnzConfig.User), xConfig.User);
        AddMissing(arrMissingValues, nameof(RnzConfig.Password), xConfig.Password == null ? string.Empty : new System.Net.NetworkCredential(string.Empty, xConfig.Password).Password);
        AddMissing(arrMissingValues, nameof(RnzConfig.LocalPath), xConfig.LocalPath);
        AddMissing(arrMissingValues, nameof(RnzConfig.DBuser), xConfig.DBuser);
        AddMissing(arrMissingValues, nameof(RnzConfig.DBhost), xConfig.DBhost);
        AddMissing(arrMissingValues, nameof(RnzConfig.DB), xConfig.DB);

        if (arrMissingValues.Count > 0)
        {
            throw new InvalidOperationException($"Missing RNZ configuration values: {string.Join(", ", arrMissingValues)}");
        }
    }

    private static string GetOverlayValue(IConfiguration xSecrets, string sKey, string sFallback)
    {
        var sSectionValue = xSecrets[$"{SecretSectionName}:{sKey}"];
        if (!string.IsNullOrWhiteSpace(sSectionValue))
        {
            return sSectionValue;
        }

        var sDirectValue = xSecrets[sKey];
        return string.IsNullOrWhiteSpace(sDirectValue) ? sFallback : sDirectValue;
    }

    private static BrowserType ParseBrowser(string sBrowser)
    {
        if (Enum.TryParse<BrowserType>(sBrowser, true, out var xBrowser))
        {
            return xBrowser;
        }

        throw new InvalidOperationException($"Invalid RNZ configuration value for {nameof(RnzConfig.Browser)}: {sBrowser}");
    }

    private static void AddMissing(List<string> arrMissingValues, string sName, string sValue)
    {
        if (string.IsNullOrWhiteSpace(sValue))
        {
            arrMissingValues.Add(sName);
        }
    }

    private static string GetRequiredAppSetting(string sKey)
    {
        var sValue = GetOptionalAppSetting(sKey);
        if (string.IsNullOrWhiteSpace(sValue))
        {
            throw new InvalidOperationException($"Missing RNZ appSetting '{sKey}' in App.config.");
        }

        return sValue;
    }

    private static string GetOptionalAppSetting(string sKey)
    {
        return System.Configuration.ConfigurationManager.AppSettings[sKey] ?? string.Empty;
    }
}


