namespace BaseLib.Show;

/// <summary>
/// Represents a single learning example inside a showcase module.
/// </summary>
internal sealed class DemoExample
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DemoExample"/> class.
    /// </summary>
    /// <param name="titleKey">The localization key for the example title.</param>
    /// <param name="sourceCode">The source code shown to the user.</param>
    /// <param name="execute">The action that renders the example output.</param>
    public DemoExample(string titleKey, string sourceCode, Action execute)
    {
        TitleKey = titleKey;
        SourceCode = sourceCode;
        Execute = execute;
    }

    /// <summary>
    /// Gets the localization key for the example title.
    /// </summary>
    public string TitleKey { get; }

    /// <summary>
    /// Gets the source code shown to the user.
    /// </summary>
    public string SourceCode { get; }

    /// <summary>
    /// Gets the action that renders the example output.
    /// </summary>
    public Action Execute { get; }
}
