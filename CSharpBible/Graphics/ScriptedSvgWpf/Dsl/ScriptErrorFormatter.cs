using System;

namespace ScriptedSvgWpf.Dsl;

public static class ScriptErrorFormatter
{
    public static string Format(string source, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(exception);

        var root = exception is ScriptException
            ? exception
            : exception.InnerException as ScriptException ?? exception;

        if (root is ScriptSyntaxException syntax)
        {
            return FormatLocated(source, "Syntax error", syntax.Message, syntax.Line, syntax.Column);
        }

        if (root is ScriptRuntimeException runtime)
        {
            return $"Runtime error:\n{runtime.Message}";
        }

        var message = root.Message;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = root.GetType().Name;
        }

        return $"Script error ({root.GetType().Name}):\n{message}";
    }

    private static string FormatLocated(string source, string category, string message, int line, int column)
    {
        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var lineIndex = Math.Clamp(line - 1, 0, Math.Max(0, lines.Length - 1));
        if (lineIndex > 0 && string.IsNullOrEmpty(lines[lineIndex]) && column == 1)
        {
            lineIndex--;
            line = lineIndex + 1;
            column = lines[lineIndex].Length + 1;
        }

        var sourceLine = lines.Length == 0 ? string.Empty : lines[lineIndex];
        var safeColumn = Math.Clamp(column, 1, Math.Max(1, sourceLine.Length + 1));
        var caret = new string(' ', safeColumn - 1) + "^";

        return $"{category} at line {line}, column {column}:\n" +
               $"{line,4} | {sourceLine}\n" +
               $"     | {caret}\n" +
               message;
    }
}

public abstract class ScriptException : Exception
{
    protected ScriptException(string message)
        : base(message)
    {
    }
}
