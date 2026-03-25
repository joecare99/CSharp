// Copyright (c) JC-Soft
// Integer2D point with basic vector math for grid-based algorithms
using System;
using System.Diagnostics.CodeAnalysis;

namespace MathLibrary.TwoDim;

/// <summary>
/// Immutable integer2D point/vector.
/// Provides basic vector arithmetic and norms used by labyrinth engine.
/// </summary>
public readonly record struct IntPoint(int X, int Y)
{
    public static readonly IntPoint Zero = new(0, 0);
    public static readonly IntPoint EX = new(1, 0);
    public static readonly IntPoint EY = new(0, 1);

    public IntPoint Add(IntPoint other) => new(X + other.X, Y + other.Y);
    public IntPoint Sub(IntPoint other) => new(X - other.X, Y - other.Y);
    public IntPoint Neg() => new(-X, -Y);
    public IntPoint Scale(int k) => new(X * k, Y * k);

    /// <summary>Manhattan length |x|+|y|</summary>
    public int GLen() => Math.Abs(X) + Math.Abs(Y);
    /// <summary>Max norm max(|x|,|y|)</summary>
    public int MLen() => Math.Max(Math.Abs(X), Math.Abs(Y));

    public int Dot(IntPoint other) => X * other.X + Y * other.Y;

    public override string ToString() => $"<{X},{Y}>";
}
