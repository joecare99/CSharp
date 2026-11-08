using System;
using System.Collections.Generic;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>Selects a compatible editor deterministically without depending on a UI framework.</summary>
public sealed class DocumentEditorRegistry<TEditor>
{
    private readonly IDocumentEditorFactory<TEditor>[] _factories;

    public DocumentEditorRegistry(IEnumerable<IDocumentEditorFactory<TEditor>> factories)
    {
        ArgumentNullException.ThrowIfNull(factories);
        _factories = factories.OrderByDescending(static factory => factory.Priority)
            .ThenBy(static factory => factory.Id, StringComparer.Ordinal)
            .ToArray();
        if (_factories.Any(static factory => string.IsNullOrWhiteSpace(factory.Id))
            || _factories.Select(static factory => factory.Id).Distinct(StringComparer.Ordinal).Count() != _factories.Length)
        {
            throw new ArgumentException("Editor factories must have unique, nonempty identifiers.", nameof(factories));
        }
    }

    public IDocumentEditorFactory<TEditor>? FindFactory(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        return _factories.FirstOrDefault(factory => factory.CanOpen(filePath));
    }
}
