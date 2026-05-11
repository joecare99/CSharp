using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.IEC.Models.Ast.Tests;

/// <summary>
/// Guards the first semantic inheritance slice that reuses <see cref="CodeBlock"/> as the semantic base.
/// </summary>
[TestClass]
public class IecSemanticCodeBlockTests
{
    [TestMethod]
    public void CompilationUnit_ActsAsCodeBlockParent_ForSemanticStatements()
    {
        var unit = new IecCompilationUnit(sourcePos: 3)
        {
            Code = "UNIT",
            Type = CodeBlockType.Block,
        };
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("Result"),
            new IecLiteralExpression(1),
            sourcePos: 8)
        {
            Code = ":=",
            Type = CodeBlockType.Assignment,
        };

        statement.Parent = unit;

        Assert.AreEqual(3, unit.SourcePos);
        Assert.AreEqual(8, statement.SourcePos);
        Assert.AreEqual(1, unit.SubBlocks.Count);
        Assert.AreSame(statement, unit.SubBlocks[0]);
        Assert.AreEqual(CodeBlockType.Assignment, statement.Type);
    }
}
