using System.Linq;
using TranspilerLib.CSharp.Models.Generation;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.Integration.TestData;

namespace TranspilerLib.Integration.Models.Generation.Tests;

/// <summary>
/// Validates the current end-to-end typed IEC to C# path across frontend and backend project boundaries.
/// </summary>
[TestClass]
public class IecToCSharpIntegrationTests
{
    [TestMethod]
    public void EmitMethod_BuildsCSharp_FromExportSourceTextCompilationUnit()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();
        var compilationUnit = IecFrontendCompilationUnitFactory.CreateFromSourceText(
            fixture.DeclarationText,
            fixture.ImplementationText);

        var result = CSharpBackend.EmitMethod(compilationUnit, "ComputeAlignment");

        Assert.IsTrue(compilationUnit.Statements.OfType<IecIfStatement>().Any(), "The export-based source-text bridge should provide at least one typed IF statement for backend emission.");
        Assert.IsTrue(compilationUnit.Statements.OfType<IecReturnStatement>().Any(), "The source-text bridge should now infer a typed return statement from the exported method header and function-result assignment pattern.");
        StringAssert.Contains(result, "protected static class GeneratedIecProgram");
        StringAssert.Contains(result, "protected static double ComputeAlignment(");
        StringAssert.Contains(result, "private static double _lrAngleAct;");
        StringAssert.Contains(result, "_lrAngleSetpDiff = ");
        StringAssert.Contains(result, "_lrSetpVelocity = Math.Clamp((-_lrAngleVelAlign), (_lrAngleSetpDiff / lrCycleTime), _lrAngleVelAlign);");
        StringAssert.Contains(result, "_lrAligmentVel4 = Math.Clamp((-_lrAngleVelAlign), (_lrAligmentVel3 + ((_lrKdFactor * _lrAngleSetpDiff) / lrCycleTime)), _lrAngleVelAlign);");
        StringAssert.Contains(result, "_lrAxisVelTarget = Math.Clamp((_lrLastValue - (_lrActualCfgDec * lrCycleTime)), _lrAligmentVel4, (_lrLastValue + (_lrActualCfgDec * lrCycleTime)));");
        StringAssert.Contains(result, "_result = ((Math.Abs(_lrAxisVelTarget) < _lrDeadBandThreshold) ? _lrAxisVelTarget : ((_lrAxisVelTarget * Math.Abs(_lrAxisVelTarget)) / _lrDeadBandThreshold));");
        StringAssert.Contains(result, "if (");
        StringAssert.Contains(result, "return _result;");
    }
}
