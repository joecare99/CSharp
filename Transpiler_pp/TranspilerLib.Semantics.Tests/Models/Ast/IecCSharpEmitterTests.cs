using System;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.TestData;

namespace TranspilerLib.IEC.Models.Ast.Tests;

/// <summary>
/// Tests the first C# emission slice for the typed IEC AST subset.
/// </summary>
[TestClass]
public class IecCSharpEmitterTests
{
    private sealed class UnsupportedExpression : IecExpression
    {
        public UnsupportedExpression()
            : base(-1)
        {
        }
    }

    private sealed class UnsupportedStatement : IecStatement
    {
        public UnsupportedStatement()
            : base(-1)
        {
        }
    }

    [TestMethod]
    public void EmitExpression_TranslatesArithmeticAndComparisonOperators()
    {
        var expression = new IecBinaryExpression(
            new IecUnaryExpression(IecUnaryOperator.Negate, new IecIdentifierExpression("Left")),
            IecBinaryOperator.LessThan,
            new IecBinaryExpression(new IecIdentifierExpression("Right"), IecBinaryOperator.Add, new IecLiteralExpression(2.5)));

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("((-Left) < (Right + 2.5))", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesSupportedFunctionCalls()
    {
        var expression = new IecFunctionCallExpression(
            "SEL",
            [
                new IecBinaryExpression(new IecIdentifierExpression("Flag"), IecBinaryOperator.Equal, new IecLiteralExpression(true)),
                new IecFunctionCallExpression("ABS", [new IecIdentifierExpression("Input")]),
                new IecFunctionCallExpression("LIMIT", [new IecLiteralExpression(0.0), new IecIdentifierExpression("Value"), new IecLiteralExpression(10.0)]),
            ]);

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("((Flag == true) ? Math.Abs(Input) : Math.Clamp(0, Value, 10))", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesPlusUnaryOperator()
    {
        var expression = new IecUnaryExpression(IecUnaryOperator.Plus, new IecIdentifierExpression("Value"));

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("(+Value)", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesNotUnaryOperator()
    {
        var expression = new IecUnaryExpression(IecUnaryOperator.Not, new IecIdentifierExpression("Flag"));

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("(!Flag)", result);
    }

    [TestMethod]
    [DataRow("SQRT", "Math.Sqrt(Value)")]
    [DataRow("SIGN", "Math.Sign(Value)")]
    [DataRow("rw_ABS", "Math.Abs(Value)")]
    [DataRow("Custom", "Custom(Value)")]
    public void EmitExpression_TranslatesAdditionalFunctionCalls(string functionName, string expected)
    {
        var expression = new IecFunctionCallExpression(functionName, [new IecIdentifierExpression("Value")]);

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void EmitExpression_EscapesStringLiterals()
    {
        var expression = new IecLiteralExpression("A\\B\"C");

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("\"A\\\\B\\\"C\"", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesNullLiteral()
    {
        var result = IecCSharpEmitter.EmitExpression(new IecLiteralExpression(null));

        Assert.AreEqual("null", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesFloatLiteral()
    {
        var result = IecCSharpEmitter.EmitExpression(new IecLiteralExpression(1.25f));

        Assert.AreEqual("1.25f", result);
    }

    [TestMethod]
    public void EmitExpression_TranslatesDecimalLiteral()
    {
        var result = IecCSharpEmitter.EmitExpression(new IecLiteralExpression(1.25m));

        Assert.AreEqual("1.25m", result);
    }

    [TestMethod]
    public void EmitExpression_UsesSelFallback_ForUnexpectedArgumentCount()
    {
        var expression = new IecFunctionCallExpression("SEL", [new IecIdentifierExpression("Flag"), new IecIdentifierExpression("Value")]);

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual("SEL(Flag, Value)", result);
    }

    [TestMethod]
    public void EmitMethod_UsesDeclarationsAndStatements_FromExportBasedCompilationUnit()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();
        var declarations = IecDeclarationExtractor.ExtractDeclarations(fixture.DeclarationText);
        var statement = new IecAssignmentStatement(
            new IecIdentifierExpression("_lrAngleSetpDiff"),
            new IecFunctionCallExpression(
                "SEL",
                [
                    new IecBinaryExpression(
                        new IecFunctionCallExpression("ABS", [new IecIdentifierExpression("_lrAngleSetpDiff")]),
                        IecBinaryOperator.LessThan,
                        new IecBinaryExpression(
                            new IecIdentifierExpression("_lrMaxAngleVel"),
                            IecBinaryOperator.Multiply,
                            new IecIdentifierExpression("lrCycleTime"))),
                    new IecLiteralExpression(0.0),
                    new IecIdentifierExpression("_lrAngleSetpDiff"),
                ]));
        var compilationUnit = new IecCompilationUnit(declarations, [statement]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "public static void Compute(");
        StringAssert.Contains(result, "double lrAngleSetp");
        StringAssert.Contains(result, "double lrDeadTime");
        StringAssert.Contains(result, "bool _xNew;");
        StringAssert.Contains(result, "_lrAngleSetpDiff = ((Math.Abs(_lrAngleSetpDiff) < (_lrMaxAngleVel * lrCycleTime)) ? 0 : _lrAngleSetpDiff);");
    }

    [TestMethod]
    public void EmitMethod_TranslatesIfAndReturnStatements()
    {
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Local),
            new IecVariableDeclaration("Value", "LREAL", IecDeclarationSection.Local),
        ],
        [
            new IecIfStatement(
                new IecIdentifierExpression("Flag"),
                [new IecAssignmentStatement(new IecIdentifierExpression("Value"), new IecLiteralExpression(1.0))],
                [new IecAssignmentStatement(new IecIdentifierExpression("Value"), new IecLiteralExpression(2.0))]),
            new IecReturnStatement(new IecIdentifierExpression("Value")),
        ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "public static double Compute()");
        StringAssert.Contains(result, "if (Flag)");
        StringAssert.Contains(result, "Value = 1;");
        StringAssert.Contains(result, "else");
        StringAssert.Contains(result, "Value = 2;");
        StringAssert.Contains(result, "return Value;");
    }

    [TestMethod]
    public void EmitMethod_MapsInputInOutAndInstanceDeclarations_ToParametersAndState()
    {
        var compilationUnit = new IecCompilationUnit(
        [
            new IecVariableDeclaration("Input", "LREAL", IecDeclarationSection.Input),
            new IecVariableDeclaration("Memory", "LREAL", IecDeclarationSection.InOut),
            new IecVariableDeclaration("Output", "BOOL", IecDeclarationSection.Output),
            new IecVariableDeclaration("_state", "LREAL", IecDeclarationSection.Instance, "0.5"),
            new IecVariableDeclaration("localValue", "INT", IecDeclarationSection.Local),
        ],
        [
            new IecAssignmentStatement(new IecIdentifierExpression("localValue"), new IecLiteralExpression(1)),
            new IecReturnStatement(new IecIdentifierExpression("_state")),
        ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Step");

        StringAssert.Contains(result, "private static double _state = 0.5;");
        StringAssert.Contains(result, "public static double Step(double Input, ref double Memory, out bool Output)");
        StringAssert.Contains(result, "int localValue;");
        Assert.IsFalse(result.Contains("double Input;", StringComparison.Ordinal));
        Assert.IsFalse(result.Contains("double Memory;", StringComparison.Ordinal));
        Assert.IsFalse(result.Contains("bool Output;", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_UsesExportDeclarationSections_ForSignatureAndState()
    {
        var fixture = IecExportFixtureData.LoadMfComputeAlignmentRot();
        var declarations = IecDeclarationExtractor.ExtractDeclarations(fixture.DeclarationText);
        var compilationUnit = new IecCompilationUnit(
            declarations,
            [new IecReturnStatement(new IecIdentifierExpression("_result"))]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "private static double _lrAngleAct;");
        StringAssert.Contains(result, "private static bool _xNew;");
        StringAssert.Contains(result, "public static double Compute(double lrAngleSetp, double lrAngleAct, double lrLastValue, double lrMaxAngleVel, double lrActualCfgDec, double lrAngleVelAlign, double lrDeadTime, double lrDeadBandThreshold, double lrKdFactor, ref double lrMemAngSetp, ref double lrMemAngAct)");
        var methodBodyStart = result.IndexOf("public static double Compute(", StringComparison.Ordinal);
        Assert.IsTrue(methodBodyStart >= 0);
        var methodBody = result[methodBodyStart..];
        Assert.IsFalse(methodBody.Contains($"{Environment.NewLine}        double _lrAngleAct;", StringComparison.Ordinal));
    }

    [TestMethod]
    public void EmitMethod_UsesBoolReturnType_ForComparisonReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(
                    new IecBinaryExpression(
                        new IecLiteralExpression(1),
                        IecBinaryOperator.LessThan,
                        new IecLiteralExpression(2)))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Check");

        StringAssert.Contains(result, "public static bool Check()");
    }

    [TestMethod]
    public void EmitMethod_UsesIntReturnType_ForRwAbsReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(
                    new IecFunctionCallExpression("rw_ABS", [new IecIdentifierExpression("Input")]))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "public static int Compute()");
    }

    [TestMethod]
    public void EmitMethod_UsesStringReturnType_ForStringLiteralReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(new IecLiteralExpression("Done"))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Describe");

        StringAssert.Contains(result, "public static string Describe()");
    }

    [TestMethod]
    public void EmitStateDeclaration_ConvertsBooleanInitializerText()
    {
        var declaration = new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Instance, "TRUE");

        var result = IecCSharpEmitter.EmitStateDeclaration(declaration);

        Assert.AreEqual("private static bool Flag = true;", result);
    }

    [TestMethod]
    public void EmitDeclaration_ConvertsBooleanInitializerText()
    {
        var declaration = new IecVariableDeclaration("Flag", "BOOL", IecDeclarationSection.Local, "FALSE");

        var result = IecCSharpEmitter.EmitDeclaration(declaration);

        Assert.AreEqual("bool Flag = false;", result);
    }

    [TestMethod]
    public void CompilationUnit_StoresArtifactMetadata()
    {
        var compilationUnit = new IecCompilationUnit(
            artifactKind: IecArtifactKind.FunctionBlock,
            accessibility: IecAccessibility.Internal);

        Assert.AreEqual(IecArtifactKind.FunctionBlock, compilationUnit.ArtifactKind);
        Assert.AreEqual(IecAccessibility.Internal, compilationUnit.Accessibility);
    }

    [TestMethod]
    public void CompilationUnit_StoresWrapperTraits()
    {
        var compilationUnit = new IecCompilationUnit(
            declarations: null,
            statements: null,
            artifactKind: IecArtifactKind.Program,
            accessibility: IecAccessibility.Public,
            isStatic: false,
            isPartial: true);

        Assert.IsFalse(compilationUnit.IsStatic);
        Assert.IsTrue(compilationUnit.IsPartial);
    }

    [TestMethod]
    public void CompilationUnit_ExposesArtifactMetadataObject()
    {
        var metadata = new IecArtifactMetadata(
            IecArtifactKind.FunctionBlock,
            IecAccessibility.Internal,
            isStatic: false,
            isPartial: true);
        var compilationUnit = new IecCompilationUnit(null, null, metadata);

        Assert.AreSame(metadata, compilationUnit.ArtifactMetadata);
        Assert.AreEqual(IecArtifactKind.FunctionBlock, compilationUnit.ArtifactMetadata.ArtifactKind);
        Assert.AreEqual(IecAccessibility.Internal, compilationUnit.ArtifactMetadata.Accessibility);
        Assert.IsFalse(compilationUnit.ArtifactMetadata.IsStatic);
        Assert.IsTrue(compilationUnit.ArtifactMetadata.IsPartial);
    }

    [TestMethod]
    public void ArtifactMetadata_InheritsSharedMetadataBase()
    {
        IecMetadata metadata = new IecArtifactMetadata();

        Assert.IsInstanceOfType<IecArtifactMetadata>(metadata);
    }

    [TestMethod]
    public void EmitExpression_ThrowsForUnsupportedExpression()
    {
        var exception = CaptureNotSupportedException(() => IecCSharpEmitter.EmitExpression(new UnsupportedExpression()));

        StringAssert.Contains(exception.Message, "UnsupportedExpression");
    }

    [TestMethod]
    public void EmitStatement_ThrowsForUnsupportedStatement()
    {
        var exception = CaptureNotSupportedException(() => IecCSharpEmitter.EmitStatement(new UnsupportedStatement()));

        StringAssert.Contains(exception.Message, "UnsupportedStatement");
    }

    [TestMethod]
    [DataRow(IecBinaryOperator.Subtract, "(Left - Right)")]
    [DataRow(IecBinaryOperator.Multiply, "(Left * Right)")]
    [DataRow(IecBinaryOperator.Divide, "(Left / Right)")]
    [DataRow(IecBinaryOperator.NotEqual, "(Left != Right)")]
    [DataRow(IecBinaryOperator.LessThanOrEqual, "(Left <= Right)")]
    [DataRow(IecBinaryOperator.GreaterThan, "(Left > Right)")]
    [DataRow(IecBinaryOperator.GreaterThanOrEqual, "(Left >= Right)")]
    [DataRow(IecBinaryOperator.And, "(Left && Right)")]
    [DataRow(IecBinaryOperator.Or, "(Left || Right)")]
    public void EmitExpression_TranslatesRemainingBinaryOperators(IecBinaryOperator operatorType, string expected)
    {
        var expression = new IecBinaryExpression(new IecIdentifierExpression("Left"), operatorType, new IecIdentifierExpression("Right"));

        var result = IecCSharpEmitter.EmitExpression(expression);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void EmitExpression_ThrowsForInvalidUnaryOperatorValue()
    {
        var exception = CaptureNotSupportedException(() => IecCSharpEmitter.EmitExpression(new IecUnaryExpression((IecUnaryOperator)999, new IecIdentifierExpression("Value"))));

        StringAssert.Contains(exception.Message, "999");
    }

    [TestMethod]
    public void EmitExpression_ThrowsForInvalidBinaryOperatorValue()
    {
        var exception = CaptureNotSupportedException(() => IecCSharpEmitter.EmitExpression(new IecBinaryExpression(new IecIdentifierExpression("Left"), (IecBinaryOperator)999, new IecIdentifierExpression("Right"))));

        StringAssert.Contains(exception.Message, "999");
    }

    [TestMethod]
    public void EmitMethod_UsesDoubleReturnType_ForDecimalLiteralReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(new IecLiteralExpression(1.25m))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "public static double Compute()");
    }

    [TestMethod]
    public void EmitMethod_UsesIntReturnType_ForIntegerLiteralReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(new IecLiteralExpression(5))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Compute");

        StringAssert.Contains(result, "public static int Compute()");
    }

    [TestMethod]
    public void EmitMethod_UsesBoolReturnType_ForBooleanLiteralReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(new IecLiteralExpression(true))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Check");

        StringAssert.Contains(result, "public static bool Check()");
    }

    [TestMethod]
    public void EmitMethod_UsesObjectReturnType_ForNullLiteralReturn()
    {
        var compilationUnit = new IecCompilationUnit(
            statements:
            [
                new IecReturnStatement(new IecLiteralExpression(null))
            ]);

        var result = IecCSharpEmitter.EmitMethod(compilationUnit, "Describe");

        StringAssert.Contains(result, "public static object Describe()");
    }

    [TestMethod]
    public void EmitDeclaration_UsesDoubleType_ForUnknownTypeName()
    {
        var declaration = new IecVariableDeclaration("Value", "CUSTOM", IecDeclarationSection.Local);

        var result = IecCSharpEmitter.EmitDeclaration(declaration);

        Assert.AreEqual("double Value;", result);
    }

    [TestMethod]
    public void EmitDeclaration_UsesObjectType_ForMissingTypeName()
    {
        var declaration = new IecVariableDeclaration("Value", null, IecDeclarationSection.Local);

        var result = IecCSharpEmitter.EmitDeclaration(declaration);

        Assert.AreEqual("object Value;", result);
    }

    [TestMethod]
    public void EmitDeclaration_UsesStringType_AndKeepsRawInitializer_ForStringDeclaration()
    {
        var declaration = new IecVariableDeclaration("Text", "STRING", IecDeclarationSection.Local, "SourceText");

        var result = IecCSharpEmitter.EmitDeclaration(declaration);

        Assert.AreEqual("string Text = SourceText;", result);
    }

    private static NotSupportedException CaptureNotSupportedException(Action action)
    {
        try
        {
            action();
        }
        catch (NotSupportedException exception)
        {
            return exception;
        }

        Assert.Fail("Expected NotSupportedException.");
        throw new InvalidOperationException("Unreachable.");
    }
}
