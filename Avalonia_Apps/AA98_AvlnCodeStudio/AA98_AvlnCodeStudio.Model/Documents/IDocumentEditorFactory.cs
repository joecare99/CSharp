namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>Creates a document editor for paths supported by this factory.</summary>
public interface IDocumentEditorFactory<out TEditor>
{
    string Id { get; }

    int Priority { get; }

    bool CanOpen(string filePath);

    TEditor Create(string filePath);
}
