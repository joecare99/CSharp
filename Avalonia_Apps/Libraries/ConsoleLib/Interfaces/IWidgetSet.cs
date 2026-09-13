namespace ConsoleLib.Interfaces;

/// <summary>
/// Defines a pluggable widget set that can render ConsoleLib controls without changing their public API.
/// </summary>
public interface IWidgetSet
{
    /// <summary>
    /// Initializes widget hosting for a ConsoleLib application root.
    /// </summary>
    /// <param name="application">The application root to initialize.</param>
    void InitializeApplication(IApplication application);

    /// <summary>
    /// Runs the specified application using the active widget set host loop.
    /// </summary>
    /// <param name="application">The application to run.</param>
    void RunApplication(IApplication application);

    /// <summary>
    /// Stops the specified application in the active widget set host loop.
    /// </summary>
    /// <param name="application">The application to stop.</param>
    void StopApplication(IApplication application);

    /// <summary>
    /// Attaches a control to the active widget host so a native widget can be materialized if needed.
    /// </summary>
    /// <param name="control">The control that became part of the visual tree.</param>
    void AttachControl(IControl control);

    /// <summary>
    /// Detaches a control from the active widget host and releases native widget resources if needed.
    /// </summary>
    /// <param name="control">The control that left the visual tree.</param>
    void DetachControl(IControl control);

    /// <summary>
    /// Synchronizes the latest control state with the active widget host.
    /// </summary>
    /// <param name="control">The control whose state changed.</param>
    void SynchronizeControl(IControl control);

    /// <summary>
    /// Draws a generic control using the default ConsoleLib appearance.
    /// </summary>
    /// <param name="control">The control to draw.</param>
    void DrawControl(IControl control);

    /// <summary>
    /// Draws a label control.
    /// </summary>
    /// <param name="label">The label to draw.</param>
    void DrawLabel(IControl label);

    /// <summary>
    /// Draws a pixel-like text fragment control.
    /// </summary>
    /// <param name="pixel">The pixel control to draw.</param>
    void DrawPixel(IControl pixel);

    /// <summary>
    /// Draws a panel control and its visible children.
    /// </summary>
    /// <param name="panel">The panel to draw.</param>
    void DrawPanel(IGroupControl panel);

    /// <summary>
    /// Redraws a clipped panel region and its visible children.
    /// </summary>
    /// <param name="panel">The panel to redraw.</param>
    /// <param name="dimension">The clipping rectangle in panel coordinates.</param>
    void RedrawPanel(IGroupControl panel, System.Drawing.Rectangle dimension);

    /// <summary>
    /// Draws a menu item control.
    /// </summary>
    /// <param name="menuItem">The menu item to draw.</param>
    void DrawMenuItem(IControl menuItem);

    /// <summary>
    /// Draws a menu bar control.
    /// </summary>
    /// <param name="menuBar">The menu bar to draw.</param>
    void DrawMenuBar(IGroupControl menuBar);

    /// <summary>
    /// Draws a list box control.
    /// </summary>
    /// <param name="listBox">The list box to draw.</param>
    void DrawListBox(IControl listBox);

    /// <summary>
    /// Draws a scroll bar control.
    /// </summary>
    /// <param name="scrollBar">The scroll bar to draw.</param>
    void DrawScrollBar(IControl scrollBar);

    /// <summary>
    /// Draws a text box control.
    /// </summary>
    /// <param name="textBox">The text box to draw.</param>
    void DrawTextBox(IControl textBox);

    /// <summary>
    /// Draws a terminal control.
    /// </summary>
    /// <param name="terminal">The terminal to draw.</param>
    void DrawTerminal(IControl terminal);

    /// <summary>
    /// Redraws a clipped terminal region.
    /// </summary>
    /// <param name="terminal">The terminal to redraw.</param>
    /// <param name="dimension">The clipping rectangle in terminal coordinates.</param>
    void RedrawTerminal(IControl terminal, System.Drawing.Rectangle dimension);
    void SetTitle(string v);

    System.Drawing.Rectangle ClipRect { get; }
}
