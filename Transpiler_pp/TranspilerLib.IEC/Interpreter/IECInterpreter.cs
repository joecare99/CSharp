using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Ast;
using TranspilerLib.IEC.Models.Scanner;
using TranspilerLib.Interfaces;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Interpreter;

/// <summary>
/// Einfache Interpreter-Implementierung für einen Teil des IEC AST.
/// Aktuell unterstützt: Funktionsaufrufe (aus <see cref="systemfunctions"/>) und Zuweisungen innerhalb eines CodeBlock-Graphen.
/// </summary>
public class IECInterpreter(ICodeBlock codeBlock) : InterpreterBase, IInterpreter
{
   /// <summary>
   /// Wurzel-Codeblock (z.B. Funktion / Programm) ab dem interpretiert wird.
   /// </summary>
   private readonly ICodeBlock _codeBlock = codeBlock;

   /// <summary>
   /// Abbildung von IEC-Reserviertwort (Funktionsname) auf die .NET Methoden-Signaturen.
   /// Mehrere Overloads werden gesammelt; beim Aufruf wird anhand der Argumentanzahl ausgewählt.
   /// </summary>
   public static IDictionary<Enum, IEnumerable<MethodInfo>> systemfunctions = 
        new Dictionary<Enum, IEnumerable<MethodInfo>>() {
            {IECResWords.rw_ABS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Abs)) },
            {IECResWords.rw_ACOS,typeof(Math).GetMethods().Where(m=>m.Name==nameof(Math.Acos)) },
            {IECResWords.rw_ASIN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Asin)) },
            {IECResWords.rw_ATAN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan)) },
         //   {IECResWords.rw_ATAN2,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Atan2)) },
            {IECResWords.rw_CONCAT,typeof(string).GetMethods().Where(m=>m.Name==nameof(string.Concat)) },
            {IECResWords.rw_COS,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Cos)) },
            {IECResWords.rw_DIV,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.DivRem)) },
            {IECResWords.rw_EXP,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Exp)) },
            {IECResWords.rw_INT,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Floor)) },
            {IECResWords.rw_LEN,[GetRequiredMethod(nameof(GetStringLength))] },
            {IECResWords.rw_LN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Log)) },
            {IECResWords.rw_LOG,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Log10)) },
             {IECResWords.rw_MOD,[
                GetRequiredMethod(nameof(GetModuloInt32)),
                GetRequiredMethod(nameof(GetModuloInt64))] },
        //    {IECResWords.rw_POW,typeof(Math).GetMethod(nameof(Math.Pow)) },
        //??    {IECResWords.rw_ROUND,typeof(Math).GetMethod(nameof(Math.Round)) },
            {IECResWords.rw_SIN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Sin)) },
            {IECResWords.rw_SQRT,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Sqrt)) },
            {IECResWords.rw_TO_STRING,typeof(Convert).GetMethods().Where(m=>m.Name==nameof(Convert.ToString)) },
            {IECResWords.rw_TAN,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Tan)) },
            {IECResWords.rw_TRUNC,typeof(Math).GetMethods().Where(m => m.Name == nameof(Math.Truncate)) },
        };

    private static MethodInfo GetRequiredMethod(string methodName)
        => typeof(IECInterpreter).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException(typeof(IECInterpreter).FullName, methodName);

    /// <summary>
    /// Hilfsfunktion für LEN.
    /// </summary>
    private static int GetStringLength(string s) => s.Length;

    /// <summary>
    /// Implements IEC integer modulo semantics for 32-bit values.
    /// </summary>
    private static int GetModuloInt32(int left, int right) => left % right;

    /// <summary>
    /// Implements IEC integer modulo semantics for 64-bit values.
    /// </summary>
    private static long GetModuloInt64(long left, long right) => left % right;

    /// <summary>
    /// Callback zur Auflösung unbekannter Identifikatoren (z.B. Variablen / Funktionen), falls nicht in <see cref="systemfunctions"/>.
    /// Default: wirft <see cref="NotImplementedException"/>.
    /// </summary>
    public Func<string, object> ResolveUnknown { get; set; }
        = (s => throw new NotImplementedException());

   /// <summary>
   /// Liefert ein Dictionary aller im Wurzelblock definierten Entitäten (Noch nicht implementiert).
   /// </summary>
   public IDictionary<string, ICodeBlock> Entitys => GetEntitys(_codeBlock);

    private IDictionary<string, ICodeBlock> GetEntitys(ICodeBlock codeBlock)
    {
        // TODO: Sammeln von Deklarationen / Labels / etc.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Interpretiert den angegebenen Codeblock sequentiell.
    /// Erwartet einen Ablauf: Deklarationen ... 'BEGIN' ... Statements.
    /// Unterstützt aktuell nur Zuweisungen mit optionalem Funktionsaufruf rechts.
    /// </summary>
    /// <param name="cb">Startblock (üblicherweise Wurzel einer Funktion).</param>
    /// <param name="parameters">Variablen-Map (Name -&gt; Wert), wird in-place aktualisiert.</param>
    /// <returns>Optionales Ergebnis (derzeit immer <c>null</c>).</returns>
    public object? Interpret(ICodeBlock cb, IDictionary<string,object> parameters)
    {
        var code = cb.Next;
        // Vorlaufen bis BEGIN gefunden wurde
        while (code != null && code.Code != "BEGIN")
            code = code.Next;
        if (code == null)
            throw new InvalidOperationException("BEGIN Block nicht gefunden.");

        var ipd = new InterpData(code.Next);
        // Hauptschleife: sequentielles Abarbeiten der Next-Kette
        while (ipd.pc != null)
        {
            switch (ipd.pc.Type)
            {
                case CodeBlockType.Assignment:
                    ExecuteAssignment(ipd.pc, parameters);
                    break;
                default:
                    throw new NotImplementedException($"Interpreter unterstützt den Blocktyp '{ipd.pc.Type}' noch nicht.");
            }
            ipd.pc = ipd.pc.Next;
        }

        return null;
    }

    /// <summary>
    /// Führt eine einfache Zuweisung aus. Rechts kann optional ein Funktionsaufruf (Name(Arg1,Arg2,...)) stehen.
    /// </summary>
    private static void ExecuteAssignment(ICodeBlock block, IDictionary<string, object> parameters)
    {
        if (IecAstMapper.TryGetAssignmentStatement(block, out var assignment))
        {
            parameters[assignment.Target.Identifier] = EvaluateExpression(assignment.Value, parameters) ?? string.Empty;
            return;
        }

        var parts = block.Code.Split('=');
        if (parts.Length != 2)
            throw new FormatException($"Ungültige Assignment-Syntax: '{block.Code}'");
        var left = parts[0].Trim();
        var right = parts[1].Trim();

        if (right.Contains('('))
        {
            var func = right.Split('(')[0];
            var args = right.Split('(')[1].Split(')')[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var values = args.Select(a => parameters[a.Trim()]).ToArray();
            var result = InvokeSystemFunction(func, values);
            parameters[left] = result ?? string.Empty;
        }
        else
        {
            parameters[left] = parameters[right];
        }
    }

    /// <summary>
    /// Evaluates a typed IEC expression for the currently supported AST subset.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    /// <param name="parameters">The current variable map.</param>
    /// <returns>The evaluated expression value.</returns>
    private static object? EvaluateExpression(IecExpression expression, IDictionary<string, object> parameters)
    {
        return expression switch
        {
            IecIdentifierExpression identifier => parameters[identifier.Identifier],
            IecLiteralExpression literal => literal.Value,
            IecFunctionCallExpression functionCall => ExecuteFunctionCall(functionCall, parameters),
            IecUnaryExpression unary => EvaluateUnaryExpression(unary, parameters),
            IecBinaryExpression binary => EvaluateBinaryExpression(binary, parameters),
            _ => throw new NotImplementedException($"Interpreter unterstützt den IEC-Ausdruck '{expression.GetType().Name}' noch nicht."),
        };
    }

    /// <summary>
    /// Executes a typed IEC function call for the currently supported AST subset.
    /// </summary>
    /// <param name="functionCall">The typed function call.</param>
    /// <param name="parameters">The current variable map.</param>
    /// <returns>The function call result.</returns>
    private static object? ExecuteFunctionCall(IecFunctionCallExpression functionCall, IDictionary<string, object> parameters)
    {
        var values = functionCall.Arguments.Select(argument => EvaluateExpression(argument, parameters)).ToArray();
        return InvokeSystemFunction(functionCall.FunctionName, values);
    }

    /// <summary>
    /// Resolves and invokes the best matching IEC system function overload.
    /// </summary>
    /// <param name="functionName">The IEC function name.</param>
    /// <param name="values">The runtime arguments.</param>
    /// <returns>The invocation result.</returns>
    private static object? InvokeSystemFunction(string functionName, object?[] values)
    {
        var methods = systemfunctions
            .First(m => string.Equals(m.Key.ToString(), functionName, StringComparison.OrdinalIgnoreCase))
            .Value;

        var (bestMethod, bestConvertedValues) = ResolveBestMethod(methods, values);

        if (bestMethod != null)
        {
            return bestMethod.Invoke(null, bestConvertedValues);
        }

        throw new InvalidOperationException($"Keine kompatible Funktionssignatur für '{functionName}' gefunden.");
    }

    /// <summary>
    /// Selects the best matching overload for the supplied argument values.
    /// </summary>
    /// <param name="methods">The candidate methods.</param>
    /// <param name="values">The runtime arguments.</param>
    /// <returns>The selected method and its converted arguments.</returns>
    private static (MethodInfo? Method, object?[]? ConvertedValues) ResolveBestMethod(IEnumerable<MethodInfo> methods, object?[] values)
    {
        MethodInfo? bestMethod = null;
        object?[]? bestConvertedValues = null;
        var bestScore = int.MinValue;

        foreach (var method in methods.Where(m => values.Length == m.GetParameters().Length))
        {
            var parametersInfo = method.GetParameters();
            var convertedValues = new object?[values.Length];
            var xMatches = true;
            var score = 0;

            for (var i = 0; i < values.Length; i++)
            {
                if (!TryConvertArgument(values[i], parametersInfo[i].ParameterType, out var convertedValue, out var conversionScore))
                {
                    xMatches = false;
                    break;
                }

                convertedValues[i] = convertedValue;
                score += conversionScore;
            }

            if (xMatches && score > bestScore)
            {
                bestMethod = method;
                bestConvertedValues = convertedValues;
                bestScore = score;
            }
        }

        return (bestMethod, bestConvertedValues);
    }

    /// <summary>
    /// Tries to convert a runtime argument value to the target parameter type.
    /// </summary>
    /// <param name="value">The runtime value.</param>
    /// <param name="targetType">The desired parameter type.</param>
    /// <param name="convertedValue">Receives the converted value when conversion succeeds.</param>
    /// <returns><c>true</c> when the conversion succeeds; otherwise <c>false</c>.</returns>
    private static bool TryConvertArgument(object? value, Type targetType, out object? convertedValue, out int score)
    {
        var effectiveTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        score = int.MinValue;

        if (value == null)
        {
            convertedValue = null;
            var xNullable = !effectiveTargetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null;
            if (xNullable)
            {
                score = 1;
            }
            return xNullable;
        }

        if (effectiveTargetType.IsInstanceOfType(value))
        {
            convertedValue = value;
            score = 100;
            return true;
        }

        try
        {
            convertedValue = Convert.ChangeType(value, effectiveTargetType, System.Globalization.CultureInfo.InvariantCulture);
            score = value.GetType() == effectiveTargetType ? 100 : 10;
            return true;
        }
        catch
        {
            convertedValue = null;
            score = int.MinValue;
            return false;
        }
    }

    /// <summary>
    /// Evaluates a typed IEC unary expression for the currently supported AST subset.
    /// </summary>
    /// <param name="unary">The typed unary expression.</param>
    /// <param name="parameters">The current variable map.</param>
    /// <returns>The evaluated unary result.</returns>
    private static object EvaluateUnaryExpression(IecUnaryExpression unary, IDictionary<string, object> parameters)
    {
        var operand = EvaluateExpression(unary.Operand, parameters);
        return unary.OperatorType switch
        {
            IecUnaryOperator.Plus => operand ?? throw new InvalidOperationException("Unary plus requires a value."),
            IecUnaryOperator.Negate => -(dynamic)(operand ?? throw new InvalidOperationException("Unary negation requires a value.")),
            IecUnaryOperator.Not => !(bool)(operand ?? throw new InvalidOperationException("Logical negation requires a value.")),
            _ => throw new NotImplementedException($"Interpreter unterstützt den unären Operator '{unary.OperatorType}' noch nicht."),
        };
    }

    /// <summary>
    /// Evaluates a typed IEC binary expression for the currently supported AST subset.
    /// </summary>
    /// <param name="binary">The typed binary expression.</param>
    /// <param name="parameters">The current variable map.</param>
    /// <returns>The evaluated binary result.</returns>
    private static object EvaluateBinaryExpression(IecBinaryExpression binary, IDictionary<string, object> parameters)
    {
        var left = EvaluateExpression(binary.Left, parameters);
        var right = EvaluateExpression(binary.Right, parameters);

        return binary.OperatorType switch
        {
            IecBinaryOperator.Add => (dynamic)(left ?? throw new InvalidOperationException("Binary addition requires a left operand.")) + (dynamic)(right ?? throw new InvalidOperationException("Binary addition requires a right operand.")),
            IecBinaryOperator.Subtract => (dynamic)(left ?? throw new InvalidOperationException("Binary subtraction requires a left operand.")) - (dynamic)(right ?? throw new InvalidOperationException("Binary subtraction requires a right operand.")),
            IecBinaryOperator.Multiply => (dynamic)(left ?? throw new InvalidOperationException("Binary multiplication requires a left operand.")) * (dynamic)(right ?? throw new InvalidOperationException("Binary multiplication requires a right operand.")),
            IecBinaryOperator.Divide => (dynamic)(left ?? throw new InvalidOperationException("Binary division requires a left operand.")) / (dynamic)(right ?? throw new InvalidOperationException("Binary division requires a right operand.")),
            IecBinaryOperator.Equal => Equals(left, right),
            IecBinaryOperator.NotEqual => !Equals(left, right),
            IecBinaryOperator.LessThan => (dynamic)(left ?? throw new InvalidOperationException("Binary comparison requires a left operand.")) < (dynamic)(right ?? throw new InvalidOperationException("Binary comparison requires a right operand.")),
            IecBinaryOperator.LessThanOrEqual => (dynamic)(left ?? throw new InvalidOperationException("Binary comparison requires a left operand.")) <= (dynamic)(right ?? throw new InvalidOperationException("Binary comparison requires a right operand.")),
            IecBinaryOperator.GreaterThan => (dynamic)(left ?? throw new InvalidOperationException("Binary comparison requires a left operand.")) > (dynamic)(right ?? throw new InvalidOperationException("Binary comparison requires a right operand.")),
            IecBinaryOperator.GreaterThanOrEqual => (dynamic)(left ?? throw new InvalidOperationException("Binary comparison requires a left operand.")) >= (dynamic)(right ?? throw new InvalidOperationException("Binary comparison requires a right operand.")),
            IecBinaryOperator.And => (bool)(left ?? throw new InvalidOperationException("Logical conjunction requires a left operand.")) && (bool)(right ?? throw new InvalidOperationException("Logical conjunction requires a right operand.")),
            IecBinaryOperator.Or => (bool)(left ?? throw new InvalidOperationException("Logical disjunction requires a left operand.")) || (bool)(right ?? throw new InvalidOperationException("Logical disjunction requires a right operand.")),
            _ => throw new NotImplementedException($"Interpreter unterstützt den binären Operator '{binary.OperatorType}' noch nicht."),
        };
    }
}
