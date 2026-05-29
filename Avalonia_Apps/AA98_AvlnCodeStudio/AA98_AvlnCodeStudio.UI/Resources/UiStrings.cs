using System.Globalization;
using System.Reflection;
using System.Resources;

namespace AA98_AvlnCodeStudio.UI.Resources;

/// <summary>
/// Provides access to localized user interface strings for the UI project.
/// </summary>
public static class UiStrings
{
    private static readonly ResourceManager _resourceManager = new("AA98_AvlnCodeStudio.UI.Resources.UiStrings", Assembly.GetExecutingAssembly());

    /// <summary>
    /// Gets or sets the culture used for resource lookups.
    /// </summary>
    public static CultureInfo? Culture { get; set; }

    public static string ApplicationTitle => GetString(nameof(ApplicationTitle));

    public static string WindowTitleDirtySuffix => GetString(nameof(WindowTitleDirtySuffix));

    public static string NavigationTitle => GetString(nameof(NavigationTitle));

    public static string NavigationPlaceholder => GetString(nameof(NavigationPlaceholder));

    public static string EditorRegionTitle => GetString(nameof(EditorRegionTitle));

    public static string StatusRegionTitle => GetString(nameof(StatusRegionTitle));

    public static string NotificationRegionTitle => GetString(nameof(NotificationRegionTitle));

    public static string FileMenuHeader => GetString(nameof(FileMenuHeader));

    public static string ViewMenuHeader => GetString(nameof(ViewMenuHeader));

    public static string NewCommandText => GetString(nameof(NewCommandText));

    public static string OpenCommandText => GetString(nameof(OpenCommandText));

    public static string SaveCommandText => GetString(nameof(SaveCommandText));

    public static string SaveAsCommandText => GetString(nameof(SaveAsCommandText));

    public static string NavigationPlaceholderMenuText => GetString(nameof(NavigationPlaceholderMenuText));

    public static string StatusPlaceholderMenuText => GetString(nameof(StatusPlaceholderMenuText));

    public static string ReadyStatusText => GetString(nameof(ReadyStatusText));

    public static string ModifiedStatusText => GetString(nameof(ModifiedStatusText));

    public static string DocumentModifiedNotificationText => GetString(nameof(DocumentModifiedNotificationText));

    public static string DocumentSynchronizedNotificationText => GetString(nameof(DocumentSynchronizedNotificationText));

    public static string NewDocumentStatusText => GetString(nameof(NewDocumentStatusText));

    public static string CreatedNewDocumentNotificationText => GetString(nameof(CreatedNewDocumentNotificationText));

    public static string OpenCanceledStatusText => GetString(nameof(OpenCanceledStatusText));

    public static string OpenCanceledNotificationText => GetString(nameof(OpenCanceledNotificationText));

    public static string LoadedDocumentNotificationFormat => GetString(nameof(LoadedDocumentNotificationFormat));

    public static string OpenedDocumentStatusFormat => GetString(nameof(OpenedDocumentStatusFormat));

    public static string SaveCanceledStatusText => GetString(nameof(SaveCanceledStatusText));

    public static string SaveCanceledNotificationText => GetString(nameof(SaveCanceledNotificationText));

    public static string SaveAsCanceledStatusText => GetString(nameof(SaveAsCanceledStatusText));

    public static string SaveAsCanceledNotificationText => GetString(nameof(SaveAsCanceledNotificationText));

    public static string SavedDocumentStatusFormat => GetString(nameof(SavedDocumentStatusFormat));

    public static string SavedDocumentNotificationFormat => GetString(nameof(SavedDocumentNotificationFormat));

    private static string GetString(string name)
    {
        return _resourceManager.GetString(name, Culture) ?? name;
    }
}
