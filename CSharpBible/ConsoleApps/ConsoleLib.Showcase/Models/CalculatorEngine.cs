using System;
using System.Globalization;
using System.Linq;

namespace ConsoleLib.Showcase.Models;

/// <summary>
/// Pure, UI-free calculator state machine (decimal, four operations).
/// All state transitions are synchronous and fully unit-testable.
/// </summary>
public sealed class CalculatorEngine
{
    private const int MaxDigits = 15;

    private decimal _accumulator;
    private CalculatorOperation _pending = CalculatorOperation.None;
    private bool _typingNumber;
    private bool _hasError;

    /// <summary>Current display text (always non-empty).</summary>
    public string Display { get; private set; } = "0";

    /// <summary>One-character indicator of the pending operation ("" when none).</summary>
    public string Indicator { get; private set; } = string.Empty;

    /// <summary>True after a fatal error (e.g., division by zero); Clear or a digit resets.</summary>
    public bool HasError => _hasError;

    /// <summary>Resets the engine to its initial state.</summary>
    public void Clear()
    {
        _accumulator = 0m;
        _pending = CalculatorOperation.None;
        _typingNumber = false;
        _hasError = false;
        Display = "0";
        Indicator = string.Empty;
    }

    /// <summary>Appends a decimal digit (ignored when the display is full).</summary>
    public void Digit(int digit)
    {
        if (digit is < 0 or > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(digit), digit, "Digit must be between 0 and 9.");
        }

        if (_hasError)
        {
            Clear();
        }

        if (!_typingNumber)
        {
            _typingNumber = true;
            if (_pending == CalculatorOperation.None)
            {
                _accumulator = 0m;
            }

            Display = digit.ToString(CultureInfo.InvariantCulture);
            return;
        }

        if (DigitCount() >= MaxDigits)
        {
            return;
        }

        Display += digit.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>Starts a fractional entry or appends the decimal point once.</summary>
    public void DecimalPoint()
    {
        if (_hasError)
        {
            Clear();
        }

        if (Display.Contains('.', StringComparison.Ordinal))
        {
            return;
        }

        if (!_typingNumber)
        {
            _typingNumber = true;
            if (_pending == CalculatorOperation.None)
            {
                _accumulator = 0m;
            }
        }

        Display += ".";
    }

    /// <summary>
    /// Registers a pending binary operation. Chains the previous operation
    /// first when a value was just typed.
    /// </summary>
    public void SetOperation(CalculatorOperation operation)
    {
        if (_hasError)
        {
            return;
        }

        if (_pending != CalculatorOperation.None && _typingNumber)
        {
            ApplyPending();
            if (_hasError)
            {
                return;
            }
        }

        if (_pending == CalculatorOperation.None)
        {
            _accumulator = ParseDisplay();
        }

        _pending = operation;
        _typingNumber = false;
        Indicator = Symbol(operation);
    }

    /// <summary>Applies the pending operation, if any.</summary>
    public void Equals()
    {
        if (_hasError || _pending == CalculatorOperation.None)
        {
            return;
        }

        ApplyPending();
    }

    /// <summary>Removes the last typed character of the current number.</summary>
    public void Backspace()
    {
        if (_hasError)
        {
            Clear();
            return;
        }

        if (!_typingNumber)
        {
            return;
        }

        Display = Display.Length <= 1 ? "0" : Display[..^1];
        if (Display == "." || Display.Length == 0)
        {
            Display = "0";
        }
    }

    /// <summary>Negates the current display value.</summary>
    public void ToggleSign()
    {
        if (_hasError || Display == "0")
        {
            return;
        }

        Display = Display.StartsWith("-", StringComparison.Ordinal)
            ? Display[1..]
            : "-" + Display;
    }

    private void ApplyPending()
    {
        var rhs = ParseDisplay();
        var lhs = _accumulator;

        try
        {
            _accumulator = _pending switch
            {
                CalculatorOperation.Add => lhs + rhs,
                CalculatorOperation.Subtract => lhs - rhs,
                CalculatorOperation.Multiply => lhs * rhs,
                CalculatorOperation.Divide => rhs == 0m
                    ? throw new DivideByZeroException()
                    : lhs / rhs,
                _ => rhs
            };
        }
        catch (DivideByZeroException)
        {
            _hasError = true;
            _accumulator = 0m;
            _pending = CalculatorOperation.None;
            _typingNumber = false;
            Display = "Cannot divide by zero";
            Indicator = string.Empty;
            return;
        }

        _pending = CalculatorOperation.None;
        _typingNumber = false;
        Indicator = string.Empty;
        Display = Format(_accumulator);
    }

    private decimal ParseDisplay()
    {
        if (decimal.TryParse(Display, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        return 0m;
    }

    private int DigitCount() =>
        Display.Count(c => c != '-' && c != '.');

    private static string Format(decimal value) =>
        value.ToString("0.##########", CultureInfo.InvariantCulture);

    private static string Symbol(CalculatorOperation operation) => operation switch
    {
        CalculatorOperation.Add => "+",
        CalculatorOperation.Subtract => "-",
        CalculatorOperation.Multiply => "*",
        CalculatorOperation.Divide => "/",
        _ => string.Empty
    };
}
