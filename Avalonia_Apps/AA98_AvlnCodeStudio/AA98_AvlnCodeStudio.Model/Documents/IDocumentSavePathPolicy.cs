namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>Optionally requires a new destination when saving the current document.</summary>
public interface IDocumentSavePathPolicy
{
    bool RequiresSaveAs { get; }
}
