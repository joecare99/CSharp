namespace AA98_AvlnCodeStudio.Editor.Services;

/// <summary>
/// Describes the outcome of a UI-agnostic editor workflow operation.
/// </summary>
public sealed class EditorOperationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditorOperationResult"/> class.
    /// </summary>
    /// <param name="operationKind">The executed operation kind.</param>
    /// <param name="status">The operation status.</param>
    /// <param name="documentDisplayName">The resulting document display name.</param>
    /// <param name="filePath">The resulting file path, if any.</param>
    public EditorOperationResult(
        EditorOperationKind operationKind,
        EditorOperationStatus status,
        string? documentDisplayName,
        string? filePath)
    {
        OperationKind = operationKind;
        Status = status;
        DocumentDisplayName = documentDisplayName;
        FilePath = filePath;
    }

    /// <summary>
    /// Gets the executed operation kind.
    /// </summary>
    public EditorOperationKind OperationKind { get; }

    /// <summary>
    /// Gets the operation status.
    /// </summary>
    public EditorOperationStatus Status { get; }

    /// <summary>
    /// Gets the resulting document display name.
    /// </summary>
    public string? DocumentDisplayName { get; }

    /// <summary>
    /// Gets the resulting file path.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsCompleted => Status == EditorOperationStatus.Completed;

    /// <summary>
    /// Creates a completed workflow result.
    /// </summary>
    /// <param name="operationKind">The executed operation kind.</param>
    /// <param name="documentDisplayName">The resulting document display name.</param>
    /// <param name="filePath">The resulting file path.</param>
    /// <returns>The completed result.</returns>
    public static EditorOperationResult Completed(EditorOperationKind operationKind, string? documentDisplayName, string? filePath)
    {
        return new EditorOperationResult(operationKind, EditorOperationStatus.Completed, documentDisplayName, filePath);
    }

    /// <summary>
    /// Creates a canceled workflow result.
    /// </summary>
    /// <param name="operationKind">The attempted operation kind.</param>
    /// <param name="documentDisplayName">The current document display name.</param>
    /// <param name="filePath">The current file path.</param>
    /// <returns>The canceled result.</returns>
    public static EditorOperationResult Canceled(EditorOperationKind operationKind, string? documentDisplayName, string? filePath)
    {
        return new EditorOperationResult(operationKind, EditorOperationStatus.Canceled, documentDisplayName, filePath);
    }
}