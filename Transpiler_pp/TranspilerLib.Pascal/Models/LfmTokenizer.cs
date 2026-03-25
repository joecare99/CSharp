using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Pascal.Models;

public class LfmTokenizer
{
    private string _input;
    private int _position;
    private int _line;
    private int _column;

    public LfmTokenizer()
    {
        _input = string.Empty;
        _position = 0;
        _line = 1;
        _column = 1;
    }

    public void SetInput(string input)
    {
        _input = input;
        _position = 0;
        _line = 1;
        _column = 1;
    }


    public IEnumerable<LfmToken> Tokenize()
    {
        while (_position < _input.Length)
        {
            char current = _input[_position];
            if (char.IsWhiteSpace(current))
            {
                if (current == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
                _position++;
                continue;
            }

            switch (current)
            {
                case ':':
                    yield return new LfmToken(LfmTokenType.COLON, ":", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '=':
                    yield return new LfmToken(LfmTokenType.EQUALS, "=", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '.':
                    yield return new LfmToken(LfmTokenType.DOT, ".", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '<':
                    yield return new LfmToken(LfmTokenType.LANGLE, "<", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '>':
                    yield return new LfmToken(LfmTokenType.RANGLE, ">", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '(':
                    yield return new LfmToken(LfmTokenType.LPAREN, "(", _line, _column);
                    _column++;
                    _position++;
                    break;
                case ')':
                    yield return new LfmToken(LfmTokenType.RPAREN, ")", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '[':
                    yield return new LfmToken(LfmTokenType.LBRACKET, "[", _line, _column);
                    _column++;
                    _position++;
                    break;
                case ']':
                    yield return new LfmToken(LfmTokenType.RBRACKET, "]", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '{':
                    yield return ReadBinaryData();
                    break;
                case ',':
                    yield return new LfmToken(LfmTokenType.COMMA, ",", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '+':
                    yield return new LfmToken(LfmTokenType.PLUS, "+", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '-':
                    yield return new LfmToken(LfmTokenType.MINUS, "-", _line, _column);
                    _column++;
                    _position++;
                    break;
                case '\'':
                    yield return ReadString();
                    break;
                default:
                    if (char.IsLetter(current) || current == '_')
                    {
                        yield return ReadIdentifierOrKeyword();
                    }
                    else if (char.IsDigit(current))
                    {
                        yield return ReadNumber();
                    }
                    else
                    {
                        // Unknown character, skip or error
                        _column++;
                        _position++;
                    }
                    break;
            }
        }
        yield return new LfmToken(LfmTokenType.EOF, "", _line, _column);
    }

    private LfmToken ReadString()
    {
        int startColumn = _column;
        _position++; // skip opening '
        _column++;
        StringBuilder sb = new StringBuilder();
        while (_position < _input.Length && _input[_position] != '\'')
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }
        if (_position < _input.Length)
        {
            _position++; // skip closing '
            _column++;
        }
        return new LfmToken(LfmTokenType.STRING, sb.ToString(), _line, startColumn);
    }

    private LfmToken ReadIdentifierOrKeyword()
    {
        int startColumn = _column;
        StringBuilder sb = new StringBuilder();
        while (_position < _input.Length && (char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }
        string value = sb.ToString();
        LfmTokenType type = GetKeywordType(value);
        return new LfmToken(type, value, _line, startColumn);
    }

    private LfmToken ReadNumber()
    {
        int startColumn = _column;
        StringBuilder sb = new StringBuilder();
        while (_position < _input.Length && char.IsDigit(_input[_position]))
        {
            sb.Append(_input[_position]);
            _position++;
            _column++;
        }
        return new LfmToken(LfmTokenType.NUMBER, sb.ToString(), _line, startColumn);
    }

    private LfmToken ReadBinaryData()
    {
        int startColumn = _column;
        int startLine = _line;
        _position++; // skip opening {
        _column++;
        StringBuilder sb = new StringBuilder();
        while (_position < _input.Length && _input[_position] != '}')
        {
            char c = _input[_position];
            if (c == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            if (!char.IsWhiteSpace(c))
            {
                sb.Append(c);
            }
            _position++;
        }
        if (_position < _input.Length)
        {
            _position++; // skip closing }
            _column++;
        }
        return new LfmToken(LfmTokenType.LBRACE, sb.ToString(), startLine, startColumn);
    }

    private LfmTokenType GetKeywordType(string value)
    {
        switch (value.ToLower())
        {
            case "object": return LfmTokenType.OBJECT;
            case "end": return LfmTokenType.END;
            case "inherited": return LfmTokenType.INHERITED;
            case "inline": return LfmTokenType.INLINE;
            case "true":
            case "false": return LfmTokenType.BOOLEAN;
            default: return LfmTokenType.IDENTIFIER;
        }
    }
}