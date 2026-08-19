using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ScriptedSvgWpf.Dsl;

public sealed class ScriptLexer
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private int _position;
    private int _line = 1;
    private int _column = 1;

    public ScriptLexer(string source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public IReadOnlyList<Token> Lex()
    {
        while (!IsAtEnd)
        {
            SkipWhitespaceAndComments();
            if (IsAtEnd)
            {
                break;
            }

            var start = _position;
            var line = _line;
            var column = _column;
            var character = Advance();
            if (char.IsLetter(character) || character == '_')
            {
                LexIdentifier(start, line, column);
            }
            else if (char.IsDigit(character))
            {
                LexNumber(start, line, column);
            }
            else if (character == '"')
            {
                LexString(start, line, column);
            }
            else
            {
                LexPunctuation(character, start, line, column);
            }
        }

        _tokens.Add(new Token(TokenKind.EndOfFile, string.Empty, _position, _line, _column));
        return _tokens;
    }

    private bool IsAtEnd => _position >= _source.Length;

    private char Advance()
    {
        var character = _source[_position++];
        if (character == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        return character;
    }

    private char Peek() => IsAtEnd ? '\0' : _source[_position];

    private char PeekNext() => _position + 1 >= _source.Length ? '\0' : _source[_position + 1];

    private bool Match(char expected)
    {
        if (Peek() != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd)
        {
            if (char.IsWhiteSpace(Peek()))
            {
                Advance();
                continue;
            }

            if (Peek() == '/' && PeekNext() == '/')
            {
                while (!IsAtEnd && Advance() != '\n')
                {
                }

                continue;
            }

            if (Peek() == '/' && PeekNext() == '*')
            {
                Advance();
                Advance();
                while (!IsAtEnd && !(Peek() == '*' && PeekNext() == '/'))
                {
                    Advance();
                }

                if (!IsAtEnd)
                {
                    Advance();
                    Advance();
                }

                continue;
            }

            break;
        }
    }

    private void LexIdentifier(int start, int line, int column)
    {
        while (char.IsLetterOrDigit(Peek()) || Peek() == '_')
        {
            Advance();
        }

        _tokens.Add(new Token(TokenKind.Identifier, _source[start.._position], start, line, column));
    }

    private void LexNumber(int start, int line, int column)
    {
        while (char.IsDigit(Peek()))
        {
            Advance();
        }

        var isFloat = false;
        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            isFloat = true;
            Advance();
            while (char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        if (Peek() is 'e' or 'E')
        {
            isFloat = true;
            Advance();
            if (Peek() is '+' or '-')
            {
                Advance();
            }

            while (char.IsDigit(Peek()))
            {
                Advance();
            }
        }

        var lexeme = _source[start.._position];
        if (!double.TryParse(lexeme, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            throw new ScriptSyntaxException($"Invalid number '{lexeme}'.", line, column);
        }

        _tokens.Add(new Token(isFloat ? TokenKind.Float : TokenKind.Integer, lexeme, start, line, column));
    }

    private void LexString(int start, int line, int column)
    {
        var builder = new StringBuilder();
        while (!IsAtEnd && Peek() != '"')
        {
            var character = Advance();
            if (character == '\\')
            {
                if (IsAtEnd)
                {
                    throw new ScriptSyntaxException("Unterminated string literal.", line, column);
                }

                builder.Append(Advance() switch
                {
                    'n' => '\n',
                    'r' => '\r',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    _ => throw new ScriptSyntaxException("Unsupported string escape.", _line, _column)
                });
            }
            else
            {
                builder.Append(character);
            }
        }

        if (IsAtEnd)
        {
            throw new ScriptSyntaxException("Unterminated string literal.", line, column);
        }

        Advance();
        _tokens.Add(new Token(TokenKind.String, builder.ToString(), start, line, column));
    }

    private void LexPunctuation(char character, int start, int line, int column)
    {
        var kind = character switch
        {
            '+' when Match('+') => TokenKind.PlusPlus,
            '+' when Match('=') => TokenKind.PlusEqual,
            '+' => TokenKind.Plus,
            '-' when Match('-') => TokenKind.MinusMinus,
            '-' when Match('=') => TokenKind.MinusEqual,
            '-' => TokenKind.Minus,
            '*' when Match('=') => TokenKind.StarEqual,
            '*' => TokenKind.Star,
            '/' when Match('=') => TokenKind.SlashEqual,
            '/' => TokenKind.Slash,
            '%' => TokenKind.Percent,
            '(' => TokenKind.LeftParen,
            ')' => TokenKind.RightParen,
            '{' => TokenKind.LeftBrace,
            '}' => TokenKind.RightBrace,
            '[' => TokenKind.LeftBracket,
            ']' => TokenKind.RightBracket,
            ',' => TokenKind.Comma,
            ';' => TokenKind.Semicolon,
            '.' => TokenKind.Dot,
            '!' when Match('=') => TokenKind.BangEqual,
            '!' => TokenKind.Bang,
            '=' when Match('=') => TokenKind.EqualEqual,
            '=' => TokenKind.Equal,
            '<' when Match('=') => TokenKind.LessEqual,
            '<' => TokenKind.Less,
            '>' when Match('=') => TokenKind.GreaterEqual,
            '>' => TokenKind.Greater,
            '&' when Match('&') => TokenKind.AndAnd,
            '|' when Match('|') => TokenKind.OrOr,
            _ => throw new ScriptSyntaxException($"Unexpected character '{character}'.", line, column)
        };

        _tokens.Add(new Token(kind, _source[start.._position], start, line, column));
    }
}

public sealed class ScriptSyntaxException : ScriptException
{
    public ScriptSyntaxException(string message, int line, int column)
        : base(message)
    {
        Line = line;
        Column = column;
    }

    public int Line { get; }
    public int Column { get; }
}
