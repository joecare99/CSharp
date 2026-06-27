using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Terminal.Core;

using System.Diagnostics;

/// <summary>
/// Parses a minimal subset of ANSI and VT escape sequences into a terminal buffer.
/// </summary>
public sealed class TerminalAnsiParser
{
    private const int TabWidth = 4;
    private readonly ITerminalBuffer _buffer;
    private readonly Action<TerminalMouseTrackingMode, TerminalMouseProtocol>? _mouseTrackingChanged;
    private readonly Action<string>? _windowTitleChanged;
    private readonly StringBuilder _sequenceBuilder = new();
    private ParserState _state;
    private TerminalColor _background = TerminalColor.DefaultBackground;
    private TerminalColor _foreground = TerminalColor.DefaultForeground;
    private TerminalMouseTrackingMode _mouseTrackingMode;
    private TerminalMouseProtocol _mouseProtocol;

    /// <summary>
    /// Initializes a new instance of the <see cref="TerminalAnsiParser"/> class.
    /// </summary>
    public TerminalAnsiParser(
        ITerminalBuffer buffer,
        Action<string>? windowTitleChanged = null,
        Action<TerminalMouseTrackingMode, TerminalMouseProtocol>? mouseTrackingChanged = null)
    {
        _buffer = buffer;
        _windowTitleChanged = windowTitleChanged;
        _mouseTrackingChanged = mouseTrackingChanged;
    }

    /// <summary>
    /// Parses terminal output text and applies it to the target buffer.
    /// </summary>
    public void Parse(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        foreach (var character in text)
        {
            Parse(character);
        }
    }

    private void Parse(char character)
    {
        switch (_state)
        {
            case ParserState.Ground:
                ParseGround(character);
                break;
            case ParserState.Escape:
                ParseEscape(character);
                break;
            case ParserState.Csi:
                ParseCsi(character);
                break;
            case ParserState.Osc:
                ParseOsc(character);
                break;
            case ParserState.OscEscape:
                ParseOscEscape(character);
                break;
        }
    }

    private void ParseGround(char character)
    {
        switch (character)
        {
            case '\u001b':
                _state = ParserState.Escape;
                _sequenceBuilder.Clear();
                break;
            case '\r':
                _buffer.CarriageReturn();
                break;
            case '\n':
                _buffer.LineFeed();
                break;
            case '\b':
                _buffer.Backspace();
                break;
            case '\t':
                WriteTab();
                break;
            default:
                WriteCharacter(character);
                break;
        }
    }

    private void ParseEscape(char character)
    {
        if (character == '[')
        {
            _state = ParserState.Csi;
            return;
        }

        if (character == ']')
        {
            _state = ParserState.Osc;
            return;
        }

        Debug.WriteLine($"Ignoring unsupported escape sequence: ESC {DescribeCharacter(character)}");
        _state = ParserState.Ground;
    }

    private void ParseCsi(char character)
    {
        if ((character >= '0' && character <= '9') || character == ';' || character == '?')
        {
            _sequenceBuilder.Append(character);
            return;
        }

        ApplyCsi(_sequenceBuilder.ToString(), character);
        _sequenceBuilder.Clear();
        _state = ParserState.Ground;
    }

    private void ParseOsc(char character)
    {
        switch (character)
        {
            case '\a':
                ApplyOsc(_sequenceBuilder.ToString());
                _sequenceBuilder.Clear();
                _state = ParserState.Ground;
                return;
            case '\u001b':
                _state = ParserState.OscEscape;
                return;
            default:
                _sequenceBuilder.Append(character);
                return;
        }
    }

    private void ParseOscEscape(char character)
    {
        if (character == '\\')
        {
            ApplyOsc(_sequenceBuilder.ToString());
            _sequenceBuilder.Clear();
            _state = ParserState.Ground;
            return;
        }

        _sequenceBuilder.Append('\u001b');
        _sequenceBuilder.Append(character);
        _state = ParserState.Osc;
    }

    private void ApplyCsi(string parameterText, char command)
    {
        switch (command)
        {
            case 'H':
            case 'f':
                ApplyCursorPosition(parameterText);
                break;
            case 'G':
                ApplyCursorHorizontalPosition(parameterText);
                break;
            case 'C':
                ApplyCursorForward(parameterText);
                break;
            case 'J':
                ApplyClear(parameterText);
                break;
            case 'K':
                ApplyLineClear(parameterText);
                break;
            case 'X':
                ApplyEraseCharacter(parameterText);
                break;
            case 'm':
                ApplySgr(parameterText);
                break;
            case 'h':
                ApplyMode(parameterText, true);
                break;
            case 'l':
                ApplyMode(parameterText, false);
                break;
            default:
                Debug.WriteLine($"Ignoring unsupported CSI sequence: ESC[{parameterText}{DescribeCharacter(command)}");
                break;
        }
    }

    private void ApplyCursorPosition(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var row = parameters.Count > 0 ? Math.Max(1, parameters[0]) : 1;
        var column = parameters.Count > 1 ? Math.Max(1, parameters[1]) : 1;
        _buffer.SetCursorPosition(column - 1, row - 1);
    }

    private void ApplyCursorHorizontalPosition(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var column = parameters.Count > 0 ? Math.Max(1, parameters[0]) : 1;
        _buffer.SetCursorPosition(column - 1, _buffer.Cursor.Row);
    }

    private void ApplyCursorForward(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var columns = parameters.Count > 0 ? Math.Max(1, parameters[0]) : 1;
        _buffer.MoveCursorForward(columns);
    }

    private void ApplyClear(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var mode = parameters.Count == 0 ? 0 : parameters[0];
        if (mode == 2)
        {
            _buffer.ClearViewport();
        }
    }

    private void ApplyLineClear(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var mode = parameters.Count == 0 ? 0 : parameters[0];
        if (mode == 2)
        {
            _buffer.ClearCurrentLine();
            return;
        }

        _buffer.ClearToEndOfLine();
    }

    private void ApplyEraseCharacter(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        var characterCount = parameters.Count > 0 ? Math.Max(1, parameters[0]) : 1;
        _buffer.EraseCharacters(characterCount);
    }

    private void ApplySgr(string parameterText)
    {
        var parameters = ParseParameters(parameterText);
        if (parameters.Count == 0)
        {
            ResetColors();
            return;
        }

        for (var index = 0; index < parameters.Count; index++)
        {
            var parameter = parameters[index];
            switch (parameter)
            {
                case 0:
                    ResetColors();
                    break;
                case 1:
                    if (_foreground == TerminalColor.DefaultForeground)
                    {
                        _foreground = TerminalPalette.GetColor(97);
                    }
                    else if (TryPromoteToBrightColor(_foreground, out var brightForeground))
                    {
                        _foreground = brightForeground;
                    }

                    break;
                case 39:
                    _foreground = TerminalColor.DefaultForeground;
                    break;
                case 49:
                    _background = TerminalColor.DefaultBackground;
                    break;
                case >= 30 and <= 37:
                case >= 90 and <= 97:
                    _foreground = TerminalPalette.GetColor(parameter);
                    break;
                case >= 40 and <= 47:
                case >= 100 and <= 107:
                    _background = TerminalPalette.GetColor(parameter);
                    break;
                case 38:
                    ApplyExtendedColor(parameters, ref index, isForeground: true);
                    break;
                case 48:
                    ApplyExtendedColor(parameters, ref index, isForeground: false);
                    break;
            }
        }
    }

    private void ApplyMode(string parameterText, bool enabled)
    {
        if (string.IsNullOrEmpty(parameterText))
        {
            return;
        }

        if (parameterText[0] == '?')
        {
            foreach (var modeText in parameterText[1..].Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                ApplyPrivateMode(modeText, enabled);
            }

            return;
        }

        ApplyStandardMode(parameterText, enabled);
    }

    private void ApplyStandardMode(string parameterText, bool enabled)
    {
        if (parameterText == "25")
        {
            _buffer.SetCursorVisibility(enabled);
        }
    }

    private void ApplyPrivateMode(string parameterText, bool enabled)
    {
        switch (parameterText)
        {
            case "25":
                _buffer.SetCursorVisibility(enabled);
                return;
            case "1000":
                SetMouseTrackingMode(enabled ? TerminalMouseTrackingMode.Button : TerminalMouseTrackingMode.None);
                return;
            case "1002":
                SetMouseTrackingMode(enabled ? TerminalMouseTrackingMode.Drag : TerminalMouseTrackingMode.None);
                return;
            case "1003":
                SetMouseTrackingMode(enabled ? TerminalMouseTrackingMode.Move : TerminalMouseTrackingMode.None);
                return;
            case "1006":
                SetMouseProtocol(enabled ? TerminalMouseProtocol.Sgr : TerminalMouseProtocol.None);
                return;
        }
    }

    private void SetMouseTrackingMode(TerminalMouseTrackingMode mode)
    {
        if (_mouseTrackingMode == mode && (mode != TerminalMouseTrackingMode.None || _mouseProtocol == TerminalMouseProtocol.None))
        {
            return;
        }

        _mouseTrackingMode = mode;
        if (mode == TerminalMouseTrackingMode.None)
        {
            _mouseProtocol = TerminalMouseProtocol.None;
        }

        _mouseTrackingChanged?.Invoke(_mouseTrackingMode, _mouseProtocol);
    }

    private void SetMouseProtocol(TerminalMouseProtocol protocol)
    {
        if (_mouseProtocol == protocol)
        {
            return;
        }

        _mouseProtocol = protocol;
        _mouseTrackingChanged?.Invoke(_mouseTrackingMode, _mouseProtocol);
    }

    private void ApplyOsc(string parameterText)
    {
        if (_windowTitleChanged is null || string.IsNullOrEmpty(parameterText))
        {
            return;
        }

        var separatorIndex = parameterText.IndexOf(';');
        if (separatorIndex <= 0 || separatorIndex == parameterText.Length - 1)
        {
            return;
        }

        var command = parameterText[..separatorIndex];
        if (!string.Equals(command, "0", StringComparison.Ordinal) && !string.Equals(command, "2", StringComparison.Ordinal))
        {
            return;
        }

        _windowTitleChanged(parameterText[(separatorIndex + 1)..]);
    }

    private void ResetColors()
    {
        _foreground = TerminalColor.DefaultForeground;
        _background = TerminalColor.DefaultBackground;
    }

    private void ApplyExtendedColor(IReadOnlyList<int> parameters, ref int index, bool isForeground)
    {
        if (index + 1 >= parameters.Count)
        {
            return;
        }

        var colorMode = parameters[index + 1];
        switch (colorMode)
        {
            case 5 when index + 2 < parameters.Count:
                SetColor(TerminalPalette.GetExtendedColor(parameters[index + 2]), isForeground);
                index += 2;
                break;
            case 2 when index + 4 < parameters.Count:
                var color = new TerminalColor(
                    ClampToByte(parameters[index + 2]),
                    ClampToByte(parameters[index + 3]),
                    ClampToByte(parameters[index + 4]));
                SetColor(color, isForeground);
                index += 4;
                break;
        }
    }

    private void SetColor(TerminalColor color, bool isForeground)
    {
        if (isForeground)
        {
            _foreground = color;
            return;
        }

        _background = color;
    }

    private static byte ClampToByte(int value)
    {
        return (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
    }

    private static bool TryPromoteToBrightColor(TerminalColor color, out TerminalColor brightColor)
    {
        for (var index = 0; index < 8; index++)
        {
            if (TerminalPalette.GetExtendedColor(index) == color)
            {
                brightColor = TerminalPalette.GetExtendedColor(index + 8);
                return true;
            }
        }

        brightColor = default;
        return false;
    }

    private static List<int> ParseParameters(string parameterText)
    {
        List<int> parameters = [];
        if (string.IsNullOrEmpty(parameterText))
        {
            return parameters;
        }

        foreach (var segment in parameterText.Split(';', StringSplitOptions.None))
        {
            if (string.IsNullOrEmpty(segment) || segment[0] == '?')
            {
                continue;
            }

            if (int.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            {
                parameters.Add(value);
            }
        }

        return parameters;
    }

    private void WriteCharacter(char character)
    {
        if (char.IsSurrogate(character))
        {
            Debug.WriteLine($"Ignoring surrogate terminal character {DescribeCharacter(character)}.");
            return;
        }

        if (char.IsControl(character))
        {
            Debug.WriteLine($"Ignoring control terminal character {DescribeCharacter(character)}.");
            return;
        }

        _buffer.Write(character, _foreground, _background);
    }

    private void WriteTab()
    {
        var spaces = TabWidth - (_buffer.Cursor.Column % TabWidth);
        if (spaces <= 0)
        {
            spaces = TabWidth;
        }

        for (var index = 0; index < spaces; index++)
        {
            _buffer.Write(' ', _foreground, _background);
        }
    }

    private static string DescribeCharacter(char character)
    {
        return $"U+{(int)character:X4} ('{FormatPrintableCharacter(character)}')";
    }

    private static string FormatPrintableCharacter(char character)
    {
        if (char.IsControl(character) || char.IsSurrogate(character))
        {
            return "?";
        }

        return character.ToString();
    }

    private enum ParserState
    {
        Ground,
        Escape,
        Csi,
        Osc,
        OscEscape
    }
}
