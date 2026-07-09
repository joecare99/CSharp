namespace BaseLib.Show;

/// <summary>
/// Describes a showcase module that can be selected from the main menu.
/// </summary>
internal interface IDemoModule
{
    /// <summary>
    /// Gets the key the user can press to run the module.
    /// </summary>
    char SelectionKey { get; }

    /// <summary>
    /// Gets the resource key for the localized module title.
    /// </summary>
    string TitleKey { get; }

    /// <summary>
    /// Gets the resource key for the localized menu description.
    /// </summary>
    string MenuDescriptionKey { get; }

    /// <summary>
    /// Gets the examples exposed by the module.
    /// </summary>
    IReadOnlyList<DemoExample> Examples { get; }

    /// <summary>
    /// Runs the module.
    /// </summary>
    void Run();
}
