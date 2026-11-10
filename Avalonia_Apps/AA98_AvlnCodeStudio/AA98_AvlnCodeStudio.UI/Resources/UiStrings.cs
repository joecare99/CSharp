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

    public static string PlanningExplorerTitle => GetString(nameof(PlanningExplorerTitle));

    public static string EditorRegionTitle => GetString(nameof(EditorRegionTitle));

    public static string StatusRegionTitle => GetString(nameof(StatusRegionTitle));

    public static string NotificationRegionTitle => GetString(nameof(NotificationRegionTitle));

    public static string DocumentStatusLabel => GetString(nameof(DocumentStatusLabel));

    public static string EditorStatusLabel => GetString(nameof(EditorStatusLabel));

    public static string FileMenuHeader => GetString(nameof(FileMenuHeader));

    public static string ViewMenuHeader => GetString(nameof(ViewMenuHeader));

    public static string WorkspaceSidebarMenuText => GetString(nameof(WorkspaceSidebarMenuText));

    public static string NewCommandText => GetString(nameof(NewCommandText));

    public static string OpenCommandText => GetString(nameof(OpenCommandText));

    public static string SaveCommandText => GetString(nameof(SaveCommandText));

    public static string SaveAsCommandText => GetString(nameof(SaveAsCommandText));

    public static string CompleteCommandText => GetString(nameof(CompleteCommandText));

    public static string ApplyCompletionCommandText => GetString(nameof(ApplyCompletionCommandText));

    public static string GoToDefinitionCommandText => GetString(nameof(GoToDefinitionCommandText));

    public static string RunTestsCommandText => GetString(nameof(RunTestsCommandText));

    public static string RunTestsUnavailableText => GetString(nameof(RunTestsUnavailableText));

    public static string OpenWorkspaceCommandText => GetString(nameof(OpenWorkspaceCommandText));

    public static string CreateConsoleProjectCommandText => GetString(nameof(CreateConsoleProjectCommandText));

    public static string CreateClassLibraryProjectCommandText => GetString(nameof(CreateClassLibraryProjectCommandText));

    public static string CreateTestProjectCommandText => GetString(nameof(CreateTestProjectCommandText));

    public static string BuildWorkspaceCommandText => GetString(nameof(BuildWorkspaceCommandText));

    public static string BuildUnavailableText => GetString(nameof(BuildUnavailableText));

    public static string ProjectWorkflowTabText => GetString(nameof(ProjectWorkflowTabText));

    public static string ProjectWorkflowProjectText => GetString(nameof(ProjectWorkflowProjectText));

    public static string ProjectWorkflowConfigurationText => GetString(nameof(ProjectWorkflowConfigurationText));

    public static string ProjectWorkflowFrameworkText => GetString(nameof(ProjectWorkflowFrameworkText));

    public static string ProjectWorkflowTrustWarningText => GetString(nameof(ProjectWorkflowTrustWarningText));

    public static string InspectProjectCommandText => GetString(nameof(InspectProjectCommandText));

    public static string BuildSelectedProjectCommandText => GetString(nameof(BuildSelectedProjectCommandText));

    public static string TestSelectedProjectCommandText => GetString(nameof(TestSelectedProjectCommandText));

    public static string DebugSelectedProjectCommandText => GetString(nameof(DebugSelectedProjectCommandText));

    public static string CancelProjectWorkflowCommandText => GetString(nameof(CancelProjectWorkflowCommandText));

    public static string StartDebuggingCommandText => GetString(nameof(StartDebuggingCommandText));

    public static string StopDebuggingCommandText => GetString(nameof(StopDebuggingCommandText));

    public static string ToggleBreakpointCommandText => GetString(nameof(ToggleBreakpointCommandText));

    public static string ContinueDebuggingCommandText => GetString(nameof(ContinueDebuggingCommandText));

    public static string PauseDebuggingCommandText => GetString(nameof(PauseDebuggingCommandText));

    public static string StepIntoCommandText => GetString(nameof(StepIntoCommandText));

    public static string StepOverCommandText => GetString(nameof(StepOverCommandText));

    public static string StepOutCommandText => GetString(nameof(StepOutCommandText));

    public static string RefreshDebugStackCommandText => GetString(nameof(RefreshDebugStackCommandText));

    public static string LoadDebugVariablesCommandText => GetString(nameof(LoadDebugVariablesCommandText));

    public static string DebugStackFramesTitle => GetString(nameof(DebugStackFramesTitle));

    public static string DebugVariablesTitle => GetString(nameof(DebugVariablesTitle));

    public static string DebugTargetPlaceholderText => GetString(nameof(DebugTargetPlaceholderText));

    public static string NoWorkspaceOpenText => GetString(nameof(NoWorkspaceOpenText));

    public static string WorkspaceOpenStatusFormat => GetString(nameof(WorkspaceOpenStatusFormat));

    public static string NoRepositoryStatusText => GetString(nameof(NoRepositoryStatusText));

    public static string RepositoryStatusFormat => GetString(nameof(RepositoryStatusFormat));

    public static string UnknownReferenceText => GetString(nameof(UnknownReferenceText));

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
