namespace BaseLib.Show;

/// <summary>
/// Provides the common execution flow for showcase modules.
/// </summary>
internal abstract class ShowcaseDemoBase : IDemoModule
{
    private IReadOnlyList<DemoExample>? _examples;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowcaseDemoBase"/> class.
    /// </summary>
    /// <param name="showcaseConsole">The showcase console renderer.</param>
    /// <param name="text">The localized text provider.</param>
    protected ShowcaseDemoBase(ShowcaseConsole showcaseConsole, ShowcaseText text)
    {
        ShowcaseConsole = showcaseConsole;
        Text = text;
    }

    /// <inheritdoc/>
    public abstract char SelectionKey { get; }

    /// <inheritdoc/>
    public abstract string TitleKey { get; }

    /// <inheritdoc/>
    public abstract string MenuDescriptionKey { get; }

    /// <inheritdoc/>
    public IReadOnlyList<DemoExample> Examples => _examples ??= CreateExamples().ToArray();

    /// <summary>
    /// Gets the formatted console helper.
    /// </summary>
    protected ShowcaseConsole ShowcaseConsole { get; }

    /// <summary>
    /// Gets the localized text provider.
    /// </summary>
    protected ShowcaseText Text { get; }

    /// <summary>
    /// Gets the raw console abstraction when direct access is needed.
    /// </summary>
    protected BaseLib.Interfaces.IConsole RawConsole => ShowcaseConsole.RawConsole;

    /// <inheritdoc/>
    public void Run()
    {
        ShowcaseConsole.PrintHeader(Text.Get(TitleKey));
        foreach (DemoExample example in Examples)
        {
            ShowcaseConsole.PrintSubHeader(Text.Get(example.TitleKey));
            ShowcaseConsole.PrintCode(example.SourceCode);
            example.Execute();
            ShowcaseConsole.BlankLine();
        }
    }

    /// <summary>
    /// Creates the example list for the module.
    /// </summary>
    /// <returns>The examples provided by the module.</returns>
    protected abstract IEnumerable<DemoExample> CreateExamples();

    /// <summary>
    /// Creates a single demo example and normalizes its source formatting.
    /// </summary>
    /// <param name="titleKey">The localization key for the example title.</param>
    /// <param name="sourceCode">The source code displayed to the user.</param>
    /// <param name="execute">The action that prints the runtime output.</param>
    /// <returns>A configured demo example.</returns>
    protected DemoExample Example(string titleKey, string sourceCode, Action execute) =>
        new(titleKey, sourceCode.Trim(), execute);
}
