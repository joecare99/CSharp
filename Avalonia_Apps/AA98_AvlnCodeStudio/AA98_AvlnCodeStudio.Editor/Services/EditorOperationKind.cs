namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Identifies the workflow operation that produced a result.
/// </summary>
public enum EditorOperationKind
{
    /// <summary>
    /// No workflow action was performed.
    /// </summary>
    None,

    /// <summary>
    /// A new document was created.
    /// </summary>
    NewDocument,

    /// <summary>
    /// A document was opened.
    /// </summary>
    Open,

    /// <summary>
    /// A document was saved.
    /// </summary>
    Save,

    /// <summary>
    /// A document was saved under a new path.
    /// </summary>
    SaveAs,
}