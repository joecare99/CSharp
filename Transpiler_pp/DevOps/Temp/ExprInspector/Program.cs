using TranspilerLib.Interfaces.Code;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.Models.Scanner;
using TranspilerLib.IEC.TestData;

Dump("_lrSetpVelocity := LIMIT(-_lrAngleVelAlign, _lrAngleSetpDiff/lrCycleTime, _lrAngleVelAlign);");
DumpWithFrontendNormalization("_lrSetpVelocity := LIMIT(-_lrAngleVelAlign, _lrAngleSetpDiff/lrCycleTime, _lrAngleVelAlign);");
Console.WriteLine("----");
Dump("_lrAngleSetpDiff := SEL(ABS(_lrAngleSetpDiff)< _lrMaxAngleVel*lrCycleTime,0.0,_lrAngleSetpDiff);");
Console.WriteLine("----");
Dump("_lrAligmentVel3 := LIMIT(LREAL#0,_lrAligmentVel2 * SEW_MKC_Math2D.SIGN(_lrAligmentVel),ABS(_lrAligmentVel)) * SEW_MKC_Math2D.SIGN(_lrAligmentVel);");
Console.WriteLine("----");
DumpWithFrontendNormalization("_lrAligmentVel4 := LIMIT(-_lrAngleVelAlign, _lrAligmentVel3 + _lrKdFactor*_lrAngleSetpDiff/lrCycleTime, _lrAngleVelAlign);");
Console.WriteLine("---- full fixture ----");
DumpFixtureStatements();

static void Dump(string text)
{
    var parser = new IECCode { OriginalCode = text };
    var root = parser.Parse();
    Show(root, 0);
    Console.WriteLine("Statements:");
    foreach (var statement in IecAstMapper.ExtractStatements(root.SubBlocks))
    {
        Console.WriteLine(statement.GetType().Name);
        if (statement is IecAssignmentStatement assignment)
        {
            Console.WriteLine($"  target={assignment.Target.Identifier} value={assignment.Value.GetType().Name}");
        }
    }
}

static void DumpWithFrontendNormalization(string implementation)
{
    var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText("VAR\n_lrSetpVelocity : LREAL;\n_lrAngleVelAlign : LREAL;\n_lrAngleSetpDiff : LREAL;\n_lrAligmentVel3 : LREAL;\n_lrAligmentVel4 : LREAL;\n_lrKdFactor : LREAL;\nlrCycleTime : LREAL;\nEND_VAR", implementation);
    Console.WriteLine("Frontend statements:");
    foreach (var statement in compilationUnit.Statements)
    {
        Console.WriteLine(statement.GetType().Name);
        if (statement is IecAssignmentStatement assignment)
        {
            Console.WriteLine($"  target={assignment.Target.Identifier} value={assignment.Value.GetType().Name}");
        }
    }
}

static void DumpFixtureStatements()
{
    var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();
    var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(fixture.DeclarationText, fixture.ImplementationText);
    foreach (var statement in compilationUnit.Statements)
    {
        Console.WriteLine(statement.GetType().Name);
        if (statement is IecAssignmentStatement assignment)
        {
            Console.WriteLine($"  target={assignment.Target.Identifier} value={assignment.Value.GetType().Name}");
        }
        else if (statement is IecIfStatement ifStatement)
        {
            Console.WriteLine($"  if then={ifStatement.ThenStatements.Count} else={ifStatement.ElseStatements.Count}");
        }
        else if (statement is IecReturnStatement returnStatement)
        {
            Console.WriteLine($"  return={returnStatement.Expression.GetType().Name}");
        }
    }
}

static void Show(ICodeBlock block, int depth)
{
    Console.WriteLine($"{new string(' ', depth * 2)}[{block.Type}] '{block.Code}' sub={block.SubBlocks.Count}");
    foreach (var child in block.SubBlocks)
    {
        Show(child, depth + 1);
    }
}
