using System;
using System.Collections.Generic;
using System.Globalization;
using ScriptedSvgWpf.Models;

namespace ScriptedSvgWpf.Dsl;

public enum ScriptValueKind
{
    Null,
    Int,
    Float,
    Bool,
    String,
    Point,
    Rect,
    Array
}

public readonly struct ScriptValue
{
    public ScriptValue(ScriptValueKind kind, object? value)
    {
        Kind = kind;
        Value = value;
    }

    public ScriptValueKind Kind { get; }
    public object? Value { get; }

    public int AsInt() => Kind switch
    {
        ScriptValueKind.Int => (int)Value!,
        ScriptValueKind.Float => checked((int)(double)Value!),
        _ => throw new ScriptRuntimeException($"Expected int, got {Kind}.")
    };

    public double AsNumber() => Kind switch
    {
        ScriptValueKind.Int => (int)Value!,
        ScriptValueKind.Float => (double)Value!,
        _ => throw new ScriptRuntimeException($"Expected a number, got {Kind}.")
    };

    public bool AsBool() => Kind == ScriptValueKind.Bool
        ? (bool)Value!
        : throw new ScriptRuntimeException($"Expected bool, got {Kind}.");

    public string AsString() => Kind == ScriptValueKind.String
        ? (string)Value!
        : throw new ScriptRuntimeException($"Expected string, got {Kind}.");

    public ScriptPoint AsPoint() => Kind == ScriptValueKind.Point
        ? (ScriptPoint)Value!
        : throw new ScriptRuntimeException($"Expected Point, got {Kind}.");

    public ScriptRect AsRect() => Kind == ScriptValueKind.Rect
        ? (ScriptRect)Value!
        : throw new ScriptRuntimeException($"Expected Rect, got {Kind}.");

    public IReadOnlyList<ScriptValue> AsArray() => Kind == ScriptValueKind.Array
        ? (IReadOnlyList<ScriptValue>)Value!
        : throw new ScriptRuntimeException($"Expected an array, got {Kind}.");

    public override string ToString() => Kind switch
    {
        ScriptValueKind.Null => "null",
        ScriptValueKind.Int => AsInt().ToString(CultureInfo.InvariantCulture),
        ScriptValueKind.Float => AsNumber().ToString(CultureInfo.InvariantCulture),
        ScriptValueKind.Bool => AsBool().ToString().ToLowerInvariant(),
        ScriptValueKind.String => AsString(),
        ScriptValueKind.Point => $"{AsPoint().X.ToString(CultureInfo.InvariantCulture)},{AsPoint().Y.ToString(CultureInfo.InvariantCulture)}",
        ScriptValueKind.Rect => $"{AsRect().X.ToString(CultureInfo.InvariantCulture)},{AsRect().Y.ToString(CultureInfo.InvariantCulture)},{AsRect().Width.ToString(CultureInfo.InvariantCulture)},{AsRect().Height.ToString(CultureInfo.InvariantCulture)}",
        ScriptValueKind.Array => $"[{string.Join(", ", AsArray())}]",
        _ => string.Empty
    };

    public static ScriptValue From(object? value) => value switch
    {
        null => new ScriptValue(ScriptValueKind.Null, null),
        int number => new ScriptValue(ScriptValueKind.Int, number),
        double number => new ScriptValue(ScriptValueKind.Float, number),
        float number => new ScriptValue(ScriptValueKind.Float, (double)number),
        bool boolean => new ScriptValue(ScriptValueKind.Bool, boolean),
        string text => new ScriptValue(ScriptValueKind.String, text),
        ScriptPoint point => new ScriptValue(ScriptValueKind.Point, point),
        ScriptRect rectangle => new ScriptValue(ScriptValueKind.Rect, rectangle),
        IReadOnlyList<ScriptValue> array => new ScriptValue(ScriptValueKind.Array, array),
        _ => throw new ScriptRuntimeException($"Unsupported value type '{value.GetType().Name}'.")
    };
}

public sealed class ScriptRuntimeException : ScriptException
{
    public ScriptRuntimeException(string message)
        : base(message)
    {
    }
}
