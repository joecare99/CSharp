using System.Globalization;
using System.Resources;

namespace BaseLib.Show;

/// <summary>
/// Resolves localized user-facing text for the showcase application.
/// </summary>
internal sealed class ShowcaseText
{
    private static readonly ResourceManager ResourceManager = new("BaseLib.Show.Resources.Strings", typeof(ShowcaseText).Assembly);

    /// <summary>
    /// Gets a localized string for the current UI culture.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized value or the key when no resource is found.</returns>
    public string Get(string key) => Get(key, CultureInfo.CurrentUICulture);

    /// <summary>
    /// Gets and formats a localized string for the current UI culture.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    public string Format(string key, params object?[] args) => Format(CultureInfo.CurrentUICulture, key, args);

    /// <summary>
    /// Gets a localized string for the specified culture.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="culture">The target culture.</param>
    /// <returns>The localized value or the key when no resource is found.</returns>
    internal string Get(string key, CultureInfo? culture)
    {
        return ResourceManager.GetString(key, culture) ?? ResourceManager.GetString(key, CultureInfo.InvariantCulture) ?? key;
    }

    /// <summary>
    /// Gets and formats a localized string for the specified culture.
    /// </summary>
    /// <param name="culture">The target culture.</param>
    /// <param name="key">The resource key.</param>
    /// <param name="args">The format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    internal string Format(CultureInfo? culture, string key, params object?[] args)
    {
        string format = Get(key, culture);
        return args.Length == 0 ? format : string.Format(culture ?? CultureInfo.CurrentCulture, format, args);
    }
}
