using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Emits a first deterministic C# representation for the currently supported typed IEC AST subset.
/// The initial implementation keeps the generated structure intentionally small and test-friendly.
/// </summary>
public static class IecCSharpEmitter
{
    /// <summary>
    /// Emits a simple C# method body from a typed IEC compilation unit.
    /// </summary>
    /// <param name="compilationUnit">The compilation unit to emit.</param>
    /// <param name="methodName">The generated method name.</param>
    /// <returns>The generated C# source code.</returns>
    public static string EmitMethod(IecCompilationUnit compilationUnit, string methodName = "Execute")
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("public static class GeneratedIecProgram");
        sb.AppendLine("{");

        foreach (var stateDeclaration in compilationUnit.Declarations.Where(declaration => declaration.Section == IecDeclarationSection.Instance))
        {
            sb.AppendLine($"    {EmitStateDeclaration(stateDeclaration)}");
        }

        if (compilationUnit.Declarations.Any(declaration => declaration.Section == IecDeclarationSection.Instance))
        {
            sb.AppendLine();
        }

        var returnType = DetermineMethodReturnType(compilationUnit);
        sb.AppendLine($"    public static {returnType} {methodName}({EmitParameterList(compilationUnit.Declarations)})");
        sb.AppendLine("    {");

        foreach (var declaration in compilationUnit.Declarations.Where(ShouldEmitAsLocalDeclaration))
        {
            sb.AppendLine($"        {EmitDeclaration(declaration)}");
        }

        if (compilationUnit.Declarations.Any(ShouldEmitAsLocalDeclaration) && compilationUnit.Statements.Count > 0)
        {
            sb.AppendLine();
        }

        foreach (var statement in compilationUnit.Statements)
        {
            sb.AppendLine($"        {EmitStatement(statement)}");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }

    /// <summary>
    /// Emits a typed IEC declaration as a C# state field declaration.
    /// </summary>
    /// <param name="declaration">The state declaration to emit.</param>
    /// <returns>The generated C# field declaration.</returns>
    public static string EmitStateDeclaration(IecVariableDeclaration declaration)
    {
        var typeName = EmitTypeName(declaration.TypeName);
        var initializer = declaration.InitializerText != null
            ? $" = {EmitRawInitializer(declaration.InitializerText, declaration.TypeName)}"
            : string.Empty;
        return $"private static {typeName} {declaration.Identifier}{initializer};";
    }

    /// <summary>
    /// Emits a typed IEC declaration as a C# local variable declaration.
    /// </summary>
    /// <param name="declaration">The declaration to emit.</param>
    /// <returns>The generated C# declaration statement.</returns>
    public static string EmitDeclaration(IecVariableDeclaration declaration)
    {
        var typeName = EmitTypeName(declaration.TypeName);
        var initializer = declaration.InitializerText != null
            ? $" = {EmitRawInitializer(declaration.InitializerText, declaration.TypeName)}"
            : string.Empty;
        return $"{typeName} {declaration.Identifier}{initializer};";
    }

    /// <summary>
    /// Emits a typed IEC statement as a C# statement.
    /// </summary>
    /// <param name="statement">The statement to emit.</param>
    /// <returns>The generated C# statement.</returns>
    public static string EmitStatement(IecStatement statement)
    {
        return statement switch
        {
            IecAssignmentStatement assignment => $"{assignment.Target.Identifier} = {EmitExpression(assignment.Value)};",
            IecIfStatement ifStatement => EmitIfStatement(ifStatement, 0),
            IecReturnStatement returnStatement => $"return {EmitExpression(returnStatement.Expression)};",
            _ => throw new NotSupportedException($"C# emission for IEC statement '{statement.GetType().Name}' is not supported yet."),
        };
    }

    private static string EmitIfStatement(IecIfStatement ifStatement, int indentLevel)
    {
        var indent = new string(' ', indentLevel * 4);
        var innerIndent = new string(' ', (indentLevel + 1) * 4);
        var sb = new StringBuilder();
        sb.AppendLine($"if ({EmitExpression(ifStatement.Condition)})");
        sb.AppendLine($"{indent}{{");

        foreach (var statement in ifStatement.ThenStatements)
        {
            sb.AppendLine($"{innerIndent}{EmitStatement(statement)}");
        }

        sb.Append($"{indent}}}");
        if (ifStatement.ElseStatements.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine($"{indent}else");
            sb.AppendLine($"{indent}{{");
            foreach (var statement in ifStatement.ElseStatements)
            {
                sb.AppendLine($"{innerIndent}{EmitStatement(statement)}");
            }

            sb.Append($"{indent}}}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Emits a typed IEC expression as a C# expression.
    /// </summary>
    /// <param name="expression">The expression to emit.</param>
    /// <returns>The generated C# expression.</returns>
    public static string EmitExpression(IecExpression expression)
    {
        return expression switch
        {
            IecIdentifierExpression identifier => identifier.Identifier,
            IecLiteralExpression literal => EmitLiteral(literal.Value),
            IecUnaryExpression unary => EmitUnaryExpression(unary),
            IecBinaryExpression binary => EmitBinaryExpression(binary),
            IecFunctionCallExpression functionCall => EmitFunctionCall(functionCall),
            _ => throw new NotSupportedException($"C# emission for IEC expression '{expression.GetType().Name}' is not supported yet."),
        };
    }

    private static string EmitTypeName(string? typeName)
    {
        return typeName?.Trim().ToUpperInvariant() switch
        {
            "BOOL" => "bool",
            "INT" => "int",
            "LREAL" => "double",
            "STRING" => "string",
            null or "" => "object",
            _ => "double",
        };
    }

    private static string EmitParameterList(IEnumerable<IecVariableDeclaration> declarations)
    {
        return string.Join(", ", declarations
            .Where(ShouldEmitAsParameter)
            .Select(EmitParameterDeclaration));
    }

    private static string EmitParameterDeclaration(IecVariableDeclaration declaration)
    {
        var modifier = declaration.Section switch
        {
            IecDeclarationSection.InOut => "ref ",
            IecDeclarationSection.Output => "out ",
            _ => string.Empty,
        };

        return $"{modifier}{EmitTypeName(declaration.TypeName)} {declaration.Identifier}";
    }

    private static bool ShouldEmitAsParameter(IecVariableDeclaration declaration)
    {
        return declaration.Section is IecDeclarationSection.Input or IecDeclarationSection.InOut or IecDeclarationSection.Output;
    }

    private static bool ShouldEmitAsLocalDeclaration(IecVariableDeclaration declaration)
    {
        return declaration.Section is IecDeclarationSection.Local or IecDeclarationSection.Unknown;
    }

    private static string EmitRawInitializer(string initializerText, string? declaredType)
    {
        var trimmedInitializer = initializerText.Trim();
        if (string.Equals(declaredType, "BOOL", StringComparison.OrdinalIgnoreCase))
        {
            return trimmedInitializer.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ? "true" : trimmedInitializer.Equals("FALSE", StringComparison.OrdinalIgnoreCase) ? "false" : trimmedInitializer;
        }

        if (string.Equals(declaredType, "LREAL", StringComparison.OrdinalIgnoreCase)
            && double.TryParse(trimmedInitializer, NumberStyles.Float, CultureInfo.InvariantCulture, out var fDouble))
        {
            return fDouble.ToString("R", CultureInfo.InvariantCulture);
        }

        return trimmedInitializer;
    }

    private static string EmitLiteral(object? value)
    {
        return value switch
        {
            null => "null",
            bool xBoolean => xBoolean ? "true" : "false",
            string sValue => $"\"{sValue.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal)}\"",
            double fDouble => fDouble.ToString("R", CultureInfo.InvariantCulture),
            float fFloat => fFloat.ToString("R", CultureInfo.InvariantCulture) + "f",
            decimal fDecimal => fDecimal.ToString(CultureInfo.InvariantCulture) + "m",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
        };
    }

    private static string EmitUnaryExpression(IecUnaryExpression unary)
    {
        var operand = EmitExpression(unary.Operand);
        return unary.OperatorType switch
        {
            IecUnaryOperator.Plus => $"(+{operand})",
            IecUnaryOperator.Negate => $"(-{operand})",
            IecUnaryOperator.Not => $"(!{operand})",
            _ => throw new NotSupportedException($"C# emission for IEC unary operator '{unary.OperatorType}' is not supported yet."),
        };
    }

    private static string EmitBinaryExpression(IecBinaryExpression binary)
    {
        var left = EmitExpression(binary.Left);
        var right = EmitExpression(binary.Right);
        var operatorText = binary.OperatorType switch
        {
            IecBinaryOperator.Add => "+",
            IecBinaryOperator.Subtract => "-",
            IecBinaryOperator.Multiply => "*",
            IecBinaryOperator.Divide => "/",
            IecBinaryOperator.Equal => "==",
            IecBinaryOperator.NotEqual => "!=",
            IecBinaryOperator.LessThan => "<",
            IecBinaryOperator.LessThanOrEqual => "<=",
            IecBinaryOperator.GreaterThan => ">",
            IecBinaryOperator.GreaterThanOrEqual => ">=",
            IecBinaryOperator.And => "&&",
            IecBinaryOperator.Or => "||",
            _ => throw new NotSupportedException($"C# emission for IEC binary operator '{binary.OperatorType}' is not supported yet."),
        };

        return $"({left} {operatorText} {right})";
    }

    private static string EmitFunctionCall(IecFunctionCallExpression functionCall)
    {
        var arguments = string.Join(", ", functionCall.Arguments.Select(EmitExpression));
        return functionCall.FunctionName.ToUpperInvariant() switch
        {
            "ABS" or "SQRT" => $"Math.{functionCall.FunctionName[..1].ToUpperInvariant()}{functionCall.FunctionName[1..].ToLowerInvariant()}({arguments})",
            "LIMIT" => $"Math.Clamp({arguments})",
            "SEL" => EmitSel(functionCall.Arguments),
            "SIGN" => $"Math.Sign({arguments})",
            "RW_ABS" => $"Math.Abs({arguments})",
            _ => $"{functionCall.FunctionName}({arguments})",
        };
    }

    private static string EmitSel(IReadOnlyList<IecExpression> arguments)
    {
        if (arguments.Count != 3)
        {
            return $"SEL({string.Join(", ", arguments.Select(EmitExpression))})";
        }

        return $"({EmitExpression(arguments[0])} ? {EmitExpression(arguments[1])} : {EmitExpression(arguments[2])})";
    }

    private static string DetermineMethodReturnType(IecCompilationUnit compilationUnit)
    {
        foreach (var statement in compilationUnit.Statements.OfType<IecReturnStatement>())
        {
            return DetermineExpressionTypeName(statement.Expression);
        }

        return "void";
    }

    private static string DetermineExpressionTypeName(IecExpression expression)
    {
        return expression switch
        {
            IecLiteralExpression literal => literal.Value switch
            {
                bool => "bool",
                float or double or decimal => "double",
                short or int or long => "int",
                string => "string",
                _ => "object",
            },
            IecBinaryExpression binary when binary.OperatorType is IecBinaryOperator.Equal
                or IecBinaryOperator.NotEqual
                or IecBinaryOperator.LessThan
                or IecBinaryOperator.LessThanOrEqual
                or IecBinaryOperator.GreaterThan
                or IecBinaryOperator.GreaterThanOrEqual
                or IecBinaryOperator.And
                or IecBinaryOperator.Or => "bool",
            IecFunctionCallExpression functionCall when functionCall.FunctionName.Equals("rw_ABS", StringComparison.OrdinalIgnoreCase) => "int",
            _ => "double",
        };
    }
}
