using System;
using System.Collections.Generic;
using System.Linq;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC root node that groups declarations and executable statements.
/// The class intentionally keeps the first implementation slice small and focused on the
/// structures that are needed for deterministic interpretation and later code generation.
/// </summary>
public sealed class IecCompilationUnit : IecAstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecCompilationUnit"/> class.
    /// </summary>
    /// <param name="declarations">The declarations that belong to the compilation unit.</param>
    /// <param name="statements">The executable statements that belong to the compilation unit.</param>
    /// <param name="artifactKind">The language-independent artifact kind.</param>
    /// <param name="accessibility">The shared artifact accessibility.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecCompilationUnit(
        IEnumerable<IecVariableDeclaration>? declarations = null,
        IEnumerable<IecStatement>? statements = null,
        IecArtifactKind artifactKind = IecArtifactKind.Function,
        IecAccessibility accessibility = IecAccessibility.Public,
        int sourcePos = -1)
        : this(declarations, statements, new IecArtifactMetadata(artifactKind, accessibility), sourcePos)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IecCompilationUnit"/> class with explicit wrapper traits.
    /// </summary>
    /// <param name="declarations">The declarations that belong to the compilation unit.</param>
    /// <param name="statements">The executable statements that belong to the compilation unit.</param>
    /// <param name="artifactKind">The language-independent artifact kind.</param>
    /// <param name="accessibility">The shared artifact accessibility.</param>
    /// <param name="isStatic">The wrapper static intent, or <c>null</c> to use the default derived from the artifact kind.</param>
    /// <param name="isPartial">The wrapper partial intent.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecCompilationUnit(
        IEnumerable<IecVariableDeclaration>? declarations,
        IEnumerable<IecStatement>? statements,
        IecArtifactKind artifactKind,
        IecAccessibility accessibility,
        bool? isStatic,
        bool isPartial,
        int sourcePos = -1)
        : this(declarations, statements, new IecArtifactMetadata(artifactKind, accessibility, isStatic, isPartial), sourcePos)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IecCompilationUnit"/> class with explicit artifact metadata.
    /// </summary>
    /// <param name="declarations">The declarations that belong to the compilation unit.</param>
    /// <param name="statements">The executable statements that belong to the compilation unit.</param>
    /// <param name="artifactMetadata">The shared artifact metadata.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecCompilationUnit(
        IEnumerable<IecVariableDeclaration>? declarations,
        IEnumerable<IecStatement>? statements,
        IecArtifactMetadata? artifactMetadata,
        int sourcePos = -1)
        : base(sourcePos)
    {
        Declarations = declarations?.ToArray() ?? Array.Empty<IecVariableDeclaration>();
        Statements = statements?.ToArray() ?? Array.Empty<IecStatement>();
        ArtifactMetadata = artifactMetadata ?? new IecArtifactMetadata();
        Symbols = new IecSymbolTable(Declarations);
    }

    /// <summary>
    /// Gets the shared artifact metadata.
    /// </summary>
    public IecArtifactMetadata ArtifactMetadata { get; }

    /// <summary>
    /// Gets the language-independent artifact kind.
    /// </summary>
    public IecArtifactKind ArtifactKind => ArtifactMetadata.ArtifactKind;

    /// <summary>
    /// Gets the shared artifact accessibility.
    /// </summary>
    public IecAccessibility Accessibility => ArtifactMetadata.Accessibility;

    /// <summary>
    /// Gets a value indicating whether the generated wrapper has static intent.
    /// </summary>
    public bool IsStatic => ArtifactMetadata.IsStatic;

    /// <summary>
    /// Gets a value indicating whether the generated wrapper has partial intent.
    /// </summary>
    public bool IsPartial => ArtifactMetadata.IsPartial;

    /// <summary>
    /// Gets the declarations contained in the compilation unit.
    /// </summary>
    public IReadOnlyList<IecVariableDeclaration> Declarations { get; }

    /// <summary>
    /// Gets the symbol table derived from the contained declarations.
    /// </summary>
    public IecSymbolTable Symbols { get; }

    /// <summary>
    /// Gets the executable statements contained in the compilation unit.
    /// </summary>
    public IReadOnlyList<IecStatement> Statements { get; }
}
