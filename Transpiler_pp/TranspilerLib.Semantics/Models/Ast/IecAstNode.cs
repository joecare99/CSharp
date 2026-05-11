using TranspilerLib.Models.Scanner;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents the base type for all typed IEC semantic nodes.
/// The node reuses <see cref="CodeBlock"/> as the structural shell so semantic
/// models can participate in the same full-loop block infrastructure while still
/// exposing typed semantic members.
/// </summary>
public abstract class IecAstNode : CodeBlock
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecAstNode"/> class.
    /// </summary>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    protected IecAstNode(int sourcePos)
    {
        SourcePos = sourcePos;
    }
}
