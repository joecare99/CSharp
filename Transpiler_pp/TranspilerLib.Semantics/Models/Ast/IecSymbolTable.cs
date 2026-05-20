using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a minimal symbol table for the currently extracted IEC declarations.
/// </summary>
public sealed class IecSymbolTable
{
    private readonly IReadOnlyDictionary<string, IecVariableDeclaration> _symbols;

    /// <summary>
    /// Initializes a new instance of the <see cref="IecSymbolTable"/> class.
    /// </summary>
    /// <param name="declarations">The declarations that should be indexed by identifier.</param>
    public IecSymbolTable(IEnumerable<IecVariableDeclaration> declarations)
    {
        _symbols = declarations
            .GroupBy(declaration => declaration.Identifier, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets all indexed symbols.
    /// </summary>
    public IReadOnlyDictionary<string, IecVariableDeclaration> Symbols => _symbols;

    /// <summary>
    /// Tries to get a declaration by identifier.
    /// </summary>
    /// <param name="identifier">The identifier to look up.</param>
    /// <param name="declaration">Receives the declaration when found.</param>
    /// <returns><c>true</c> when the declaration exists; otherwise <c>false</c>.</returns>
    public bool TryGet(string identifier, out IecVariableDeclaration? declaration)
    {
        var result = _symbols.TryGetValue(identifier, out var actualDeclaration);
        declaration = actualDeclaration;
        return result;
    }
}
