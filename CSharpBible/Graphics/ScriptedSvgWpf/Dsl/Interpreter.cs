using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ScriptedSvgWpf.Models;
using ScriptedSvgWpf.Rendering;

namespace ScriptedSvgWpf.Dsl;

public sealed class InterpreterOptions
{
    public int MaxSteps { get; init; } = 250_000;
    public int MaxLoopIterations { get; init; } = 25_000;
    public int MaxDrawCommands { get; init; } = 100_000;
}

public sealed class ScriptInterpreter
{
    public RenderDocument Execute(string source, InterpreterOptions? options = null)
    {
        var tokens = new ScriptLexer(source).Lex();
        var program = new ScriptParser(tokens).Parse();
        return Execute(program, options);
    }

    public RenderDocument Execute(ScriptProgram program, InterpreterOptions? options = null)
    {
        var state = new ExecutionState(options ?? new InterpreterOptions());
        state.ExecuteBlock(program.Statements, createScope: false);
        return state.Document;
    }

    private sealed class ExecutionState
    {
        private readonly InterpreterOptions _options;
        private readonly List<Dictionary<string, ScriptValue>> _scopes = new() { new(StringComparer.OrdinalIgnoreCase) };
        private readonly List<Dictionary<string, string>> _types = new() { new(StringComparer.OrdinalIgnoreCase) };
        private int _steps;
        private int _loopIterations;

        public ExecutionState(InterpreterOptions options)
        {
            _options = options;
            Document = new RenderDocument(640, 480, "white");
        }

        public RenderDocument Document { get; private set; }

        public void ExecuteBlock(IReadOnlyList<Statement> statements, bool createScope)
        {
            if (createScope)
            {
                PushScope();
            }

            try
            {
                foreach (var statement in statements)
                {
                    ExecuteStatement(statement);
                }
            }
            finally
            {
                if (createScope)
                {
                    PopScope();
                }
            }
        }

        private void ExecuteStatement(Statement statement)
        {
            Step();
            switch (statement)
            {
                case BlockStatement block:
                    ExecuteBlock(block.Statements, createScope: true);
                    break;
                case VariableDeclarationStatement declaration:
                    Declare(declaration);
                    break;
                case AssignmentStatement assignment:
                    Assign(assignment.Target, Evaluate(assignment.Value));
                    break;
                case ExpressionStatement expression:
                    Evaluate(expression.Expression);
                    break;
                case IfStatement conditional:
                    if (Evaluate(conditional.Condition).AsBool())
                    {
                        ExecuteStatement(conditional.ThenBranch);
                    }
                    else if (conditional.ElseBranch is not null)
                    {
                        ExecuteStatement(conditional.ElseBranch);
                    }
                    break;
                case WhileStatement loop:
                    ExecuteWhile(loop);
                    break;
                case ForStatement loop:
                    ExecuteFor(loop);
                    break;
                default:
                    throw new ScriptRuntimeException($"Unsupported statement '{statement.GetType().Name}'.");
            }
        }

        private void ExecuteWhile(WhileStatement loop)
        {
            var iterations = 0;
            while (Evaluate(loop.Condition).AsBool())
            {
                GuardLoop(++iterations);
                ExecuteStatement(loop.Body);
            }
        }

        private void ExecuteFor(ForStatement loop)
        {
            PushScope();
            try
            {
                if (loop.Initializer is not null)
                {
                    ExecuteStatement(loop.Initializer);
                }

                var iterations = 0;
                while (loop.Condition is null || Evaluate(loop.Condition).AsBool())
                {
                    GuardLoop(++iterations);
                    ExecuteStatement(loop.Body);
                    if (loop.Increment is not null)
                    {
                        Evaluate(loop.Increment);
                    }
                }
            }
            finally
            {
                PopScope();
            }
        }

        private void GuardLoop(int iteration)
        {
            if (iteration > _options.MaxLoopIterations)
            {
                throw new ScriptRuntimeException($"Loop limit of {_options.MaxLoopIterations} iterations exceeded.");
            }

            _loopIterations++;
            if (_loopIterations > _options.MaxLoopIterations * 4)
            {
                throw new ScriptRuntimeException("Combined loop execution limit exceeded.");
            }
        }

        private void Declare(VariableDeclarationStatement declaration)
        {
            var value = Evaluate(declaration.Initializer);
            SetCurrent(declaration.Name, ConvertToType(value, declaration.TypeName), declaration.TypeName);
        }

        private ScriptValue Evaluate(Expression expression)
        {
            Step();
            return expression switch
            {
                LiteralExpression literal => ScriptValue.From(literal.Value),
                NameExpression name => Get(name.Name),
                UnaryExpression unary => EvaluateUnary(unary),
                IncrementExpression increment => EvaluateIncrement(increment),
                BinaryExpression binary when binary.Operator == TokenKind.Equal => EvaluateAssignment(binary),
                BinaryExpression binary => EvaluateBinary(binary),
                CallExpression call => EvaluateCall(call),
                MemberExpression member => EvaluateMember(member),
                ArrayExpression array => new ScriptValue(ScriptValueKind.Array, array.Items.Select(Evaluate).ToArray()),
                _ => throw new ScriptRuntimeException($"Unsupported expression '{expression.GetType().Name}'.")
            };
        }

        private ScriptValue EvaluateAssignment(BinaryExpression assignment)
        {
            var value = Evaluate(assignment.Right);
            Assign(assignment.Left, value);
            return value;
        }

        private ScriptValue EvaluateIncrement(IncrementExpression increment)
        {
            var current = Evaluate(increment.Target);
            var updated = current.Kind == ScriptValueKind.Int
                ? new ScriptValue(ScriptValueKind.Int, current.AsInt() + increment.Delta)
                : new ScriptValue(ScriptValueKind.Float, current.AsNumber() + increment.Delta);
            Assign(increment.Target, updated);
            return updated;
        }

        private ScriptValue EvaluateUnary(UnaryExpression unary)
        {
            var operand = Evaluate(unary.Operand);
            return unary.Operator switch
            {
                TokenKind.Bang => new ScriptValue(ScriptValueKind.Bool, !operand.AsBool()),
                TokenKind.Minus when operand.Kind == ScriptValueKind.Int => new ScriptValue(ScriptValueKind.Int, -operand.AsInt()),
                TokenKind.Minus => new ScriptValue(ScriptValueKind.Float, -operand.AsNumber()),
                TokenKind.Plus when operand.Kind == ScriptValueKind.Int => operand,
                TokenKind.Plus => new ScriptValue(ScriptValueKind.Float, operand.AsNumber()),
                _ => throw new ScriptRuntimeException("Unsupported unary operator.")
            };
        }

        private ScriptValue EvaluateBinary(BinaryExpression binary)
        {
            if (binary.Operator == TokenKind.AndAnd)
            {
                var left = Evaluate(binary.Left).AsBool();
                return new ScriptValue(ScriptValueKind.Bool, left && Evaluate(binary.Right).AsBool());
            }

            if (binary.Operator == TokenKind.OrOr)
            {
                var left = Evaluate(binary.Left).AsBool();
                return new ScriptValue(ScriptValueKind.Bool, left || Evaluate(binary.Right).AsBool());
            }

            var leftValue = Evaluate(binary.Left);
            var rightValue = Evaluate(binary.Right);
            if (binary.Operator == TokenKind.Plus && (leftValue.Kind == ScriptValueKind.String || rightValue.Kind == ScriptValueKind.String))
            {
                return new ScriptValue(ScriptValueKind.String, leftValue.ToString() + rightValue);
            }

            return binary.Operator switch
            {
                TokenKind.Plus => Numeric(leftValue, rightValue, (left, right) => left + right, (left, right) => left + right),
                TokenKind.Minus => Numeric(leftValue, rightValue, (left, right) => left - right, (left, right) => left - right),
                TokenKind.Star => Numeric(leftValue, rightValue, (left, right) => left * right, (left, right) => left * right),
                TokenKind.Slash => NumericDivision(leftValue, rightValue),
                TokenKind.Percent => Numeric(leftValue, rightValue, (left, right) => left % right, (left, right) => left % right),
                TokenKind.EqualEqual => new ScriptValue(ScriptValueKind.Bool, AreEqual(leftValue, rightValue)),
                TokenKind.BangEqual => new ScriptValue(ScriptValueKind.Bool, !AreEqual(leftValue, rightValue)),
                TokenKind.Less => Compare(leftValue, rightValue, (left, right) => left < right),
                TokenKind.LessEqual => Compare(leftValue, rightValue, (left, right) => left <= right),
                TokenKind.Greater => Compare(leftValue, rightValue, (left, right) => left > right),
                TokenKind.GreaterEqual => Compare(leftValue, rightValue, (left, right) => left >= right),
                _ => throw new ScriptRuntimeException("Unsupported binary operator.")
            };
        }

        private static ScriptValue Numeric(
            ScriptValue left,
            ScriptValue right,
            Func<int, int, int> integerOperation,
            Func<double, double, double> floatingOperation)
        {
            if (left.Kind == ScriptValueKind.Int && right.Kind == ScriptValueKind.Int)
            {
                return new ScriptValue(ScriptValueKind.Int, integerOperation(left.AsInt(), right.AsInt()));
            }

            return new ScriptValue(ScriptValueKind.Float, floatingOperation(left.AsNumber(), right.AsNumber()));
        }

        private static ScriptValue NumericDivision(ScriptValue left, ScriptValue right)
        {
            var divisor = right.AsNumber();
            if (Math.Abs(divisor) < double.Epsilon)
            {
                throw new ScriptRuntimeException("Division by zero.");
            }

            return new ScriptValue(ScriptValueKind.Float, left.AsNumber() / divisor);
        }

        private static ScriptValue Compare(ScriptValue left, ScriptValue right, Func<double, double, bool> comparison) =>
            new(ScriptValueKind.Bool, comparison(left.AsNumber(), right.AsNumber()));

        private static bool AreEqual(ScriptValue left, ScriptValue right)
        {
            if (left.Kind is ScriptValueKind.Int or ScriptValueKind.Float &&
                right.Kind is ScriptValueKind.Int or ScriptValueKind.Float)
            {
                return Math.Abs(left.AsNumber() - right.AsNumber()) < 0.0000001;
            }

            if (left.Kind != right.Kind)
            {
                return false;
            }

            return Equals(left.Value, right.Value);
        }

        private ScriptValue EvaluateCall(CallExpression call)
        {
            var arguments = call.Arguments.Select(Evaluate).ToArray();
            var functionName = call.Callee switch
            {
                NameExpression name => name.Name,
                MemberExpression member when member.Target is NameExpression target &&
                    string.Equals(target.Name, "Math", StringComparison.OrdinalIgnoreCase) => member.Name,
                _ => throw new ScriptRuntimeException("Only named functions can be called.")
            };

            if (TryEvaluateConstructor(functionName, arguments, out var constructed))
            {
                return constructed;
            }

            if (TryEvaluateMath(functionName, arguments, out var mathResult))
            {
                return mathResult;
            }

            switch (functionName.ToLowerInvariant())
            {
                case "canvas":
                    RequireArguments(functionName, arguments, 3);
                    Document = new RenderDocument(arguments[0].AsNumber(), arguments[1].AsNumber(), arguments[2].AsString());
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "rect":
                case "rectangle":
                    if (arguments.Length is < 5 or > 7)
                    {
                        throw new ScriptRuntimeException("rect expects x, y, width, height, fill, and optional scale and rotation.");
                    }

                    AddCommand(new RectangleCommand(
                        arguments[0].AsNumber(),
                        arguments[1].AsNumber(),
                        arguments[2].AsNumber(),
                        arguments[3].AsNumber(),
                        arguments[4].AsString(),
                        arguments.Length >= 6 ? arguments[5].AsNumber() : 1,
                        arguments.Length >= 7 ? arguments[6].AsNumber() : 0));
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "circle":
                    RequireArguments(functionName, arguments, 4);
                    AddCommand(new CircleCommand(arguments[0].AsNumber(), arguments[1].AsNumber(), arguments[2].AsNumber(), arguments[3].AsString()));
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "line":
                    if (arguments.Length is < 5 or > 6)
                    {
                        throw new ScriptRuntimeException("line expects x1, y1, x2, y2, stroke, and optional width.");
                    }

                    AddCommand(new LineCommand(
                        arguments[0].AsNumber(),
                        arguments[1].AsNumber(),
                        arguments[2].AsNumber(),
                        arguments[3].AsNumber(),
                        arguments[4].AsString(),
                        arguments.Length == 6 ? arguments[5].AsNumber() : 1));
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "text":
                    if (arguments.Length is < 4 or > 5)
                    {
                        throw new ScriptRuntimeException("text expects x, y, text, fill, and optional font size.");
                    }

                    AddCommand(new TextCommand(
                        arguments[0].AsNumber(),
                        arguments[1].AsNumber(),
                        arguments[2].AsString(),
                        arguments[3].AsString(),
                        arguments.Length == 5 ? arguments[4].AsNumber() : 14));
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "polygon":
                    if (arguments.Length is < 2 or > 4)
                    {
                        throw new ScriptRuntimeException("polygon expects points, fill, and optional stroke and width.");
                    }

                    var points = arguments[0].AsArray().Select(value => value.AsPoint()).ToArray();
                    AddCommand(new PolygonCommand(
                        points,
                        arguments[1].AsString(),
                        arguments.Length >= 3 ? arguments[2].AsString() : null,
                        arguments.Length == 4 ? arguments[3].AsNumber() : 1));
                    return new ScriptValue(ScriptValueKind.Null, null);
                case "path":
                    if (arguments.Length is < 2 or > 4)
                    {
                        throw new ScriptRuntimeException("path expects data, stroke, and optional width and fill.");
                    }

                    AddCommand(new PathCommand(
                        arguments[0].AsString(),
                        arguments[1].AsString(),
                        arguments.Length >= 3 ? arguments[2].AsNumber() : 1,
                        arguments.Length == 4 ? arguments[3].AsString() : null));
                    return new ScriptValue(ScriptValueKind.Null, null);
                default:
                    throw new ScriptRuntimeException($"Unknown function '{functionName}'.");
            }
        }

        private static bool TryEvaluateConstructor(string name, IReadOnlyList<ScriptValue> arguments, out ScriptValue value)
        {
            switch (name.ToLowerInvariant())
            {
                case "point":
                    RequireArguments(name, arguments, 2);
                    value = new ScriptValue(ScriptValueKind.Point, new ScriptPoint(arguments[0].AsNumber(), arguments[1].AsNumber()));
                    return true;
                case "rect":
                    if (arguments.Count == 4)
                    {
                        value = new ScriptValue(ScriptValueKind.Rect, new ScriptRect(
                            arguments[0].AsNumber(),
                            arguments[1].AsNumber(),
                            arguments[2].AsNumber(),
                            arguments[3].AsNumber()));
                        return true;
                    }

                    break;
            }

            value = default;
            return false;
        }

        private static bool TryEvaluateMath(string name, IReadOnlyList<ScriptValue> arguments, out ScriptValue value)
        {
            var normalized = name.ToLowerInvariant();
            if (normalized == "pi" && arguments.Count == 0)
            {
                value = new ScriptValue(ScriptValueKind.Float, Math.PI);
                return true;
            }

            if (normalized is "min" or "max" && arguments.Count == 2)
            {
                var rangeResult = normalized == "min"
                    ? Math.Min(arguments[0].AsNumber(), arguments[1].AsNumber())
                    : Math.Max(arguments[0].AsNumber(), arguments[1].AsNumber());
                value = new ScriptValue(ScriptValueKind.Float, rangeResult);
                return true;
            }

            if (arguments.Count != 1 && normalized is not "pow" and not "atan2")
            {
                value = default;
                return false;
            }

            var number = arguments.Count > 0 ? arguments[0].AsNumber() : 0;
            var mathResult = normalized switch
            {
                "abs" => Math.Abs(number),
                "sqrt" => Math.Sqrt(number),
                "sin" => Math.Sin(number),
                "cos" => Math.Cos(number),
                "tan" => Math.Tan(number),
                "atan" => Math.Atan(number),
                "atan2" when arguments.Count == 2 => Math.Atan2(number, arguments[1].AsNumber()),
                "floor" => Math.Floor(number),
                "ceil" => Math.Ceiling(number),
                "round" => Math.Round(number),
                "pow" when arguments.Count == 2 => Math.Pow(number, arguments[1].AsNumber()),
                _ => double.NaN
            };

            if (double.IsNaN(mathResult))
            {
                value = default;
                return false;
            }

            value = new ScriptValue(ScriptValueKind.Float, mathResult);
            return true;
        }

        private ScriptValue EvaluateMember(MemberExpression member)
        {
            if (member.Target is NameExpression targetName &&
                string.Equals(targetName.Name, "Math", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(member.Name, "PI", StringComparison.OrdinalIgnoreCase))
            {
                return new ScriptValue(ScriptValueKind.Float, Math.PI);
            }

            var target = Evaluate(member.Target);
            if (target.Kind == ScriptValueKind.Point)
            {
                var point = target.AsPoint();
                return member.Name.ToLowerInvariant() switch
                {
                    "x" => new ScriptValue(ScriptValueKind.Float, point.X),
                    "y" => new ScriptValue(ScriptValueKind.Float, point.Y),
                    _ => throw new ScriptRuntimeException($"Point has no member '{member.Name}'.")
                };
            }

            if (target.Kind == ScriptValueKind.Rect)
            {
                var rectangle = target.AsRect();
                return member.Name.ToLowerInvariant() switch
                {
                    "x" => new ScriptValue(ScriptValueKind.Float, rectangle.X),
                    "y" => new ScriptValue(ScriptValueKind.Float, rectangle.Y),
                    "width" => new ScriptValue(ScriptValueKind.Float, rectangle.Width),
                    "height" => new ScriptValue(ScriptValueKind.Float, rectangle.Height),
                    _ => throw new ScriptRuntimeException($"Rect has no member '{member.Name}'.")
                };
            }

            throw new ScriptRuntimeException($"Value of type {target.Kind} has no members.");
        }

        private void AddCommand(RenderCommand command)
        {
            if (Document.Commands.Count >= _options.MaxDrawCommands)
            {
                throw new ScriptRuntimeException($"Draw command limit of {_options.MaxDrawCommands} exceeded.");
            }

            Document.Commands.Add(command);
        }

        private void Assign(Expression target, ScriptValue value)
        {
            if (target is not NameExpression name)
            {
                throw new ScriptRuntimeException("Only variables can be assigned.");
            }

            for (var index = _scopes.Count - 1; index >= 0; index--)
            {
                if (_scopes[index].ContainsKey(name.Name))
                {
                    _scopes[index][name.Name] = ConvertToType(value, _types[index][name.Name]);
                    return;
                }
            }

            throw new ScriptRuntimeException($"Unknown variable '{name.Name}'.");
        }

        private ScriptValue Get(string name)
        {
            for (var index = _scopes.Count - 1; index >= 0; index--)
            {
                if (_scopes[index].TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            throw new ScriptRuntimeException($"Unknown variable '{name}'.");
        }

        private void SetCurrent(string name, ScriptValue value, string typeName)
        {
            if (!_scopes[^1].TryAdd(name, value))
            {
                throw new ScriptRuntimeException($"Variable '{name}' is already declared in this scope.");
            }

            _types[^1].Add(name, typeName);
        }

        private static ScriptValue ConvertToType(ScriptValue value, string typeName)
        {
            return typeName.ToLowerInvariant() switch
            {
                "int" when value.Kind == ScriptValueKind.Int => value,
                "float" when value.Kind is ScriptValueKind.Int or ScriptValueKind.Float => new ScriptValue(ScriptValueKind.Float, value.AsNumber()),
                "bool" when value.Kind == ScriptValueKind.Bool => value,
                "string" when value.Kind == ScriptValueKind.String => value,
                "point" when value.Kind == ScriptValueKind.Point => value,
                "rect" when value.Kind == ScriptValueKind.Rect => value,
                _ => throw new ScriptRuntimeException($"Cannot assign {value.Kind} to {typeName}.")
            };
        }

        private void PushScope()
        {
            _scopes.Add(new Dictionary<string, ScriptValue>(StringComparer.OrdinalIgnoreCase));
            _types.Add(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        private void PopScope()
        {
            _scopes.RemoveAt(_scopes.Count - 1);
            _types.RemoveAt(_types.Count - 1);
        }

        private void Step()
        {
            if (++_steps > _options.MaxSteps)
            {
                throw new ScriptRuntimeException($"Execution step limit of {_options.MaxSteps} exceeded.");
            }
        }

        private static void RequireArguments(string functionName, IReadOnlyList<ScriptValue> arguments, int count)
        {
            if (arguments.Count != count)
            {
                throw new ScriptRuntimeException($"{functionName} expects {count} arguments.");
            }
        }
    }
}
