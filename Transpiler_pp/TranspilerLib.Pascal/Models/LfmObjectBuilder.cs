using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.Pascal.Models;

/// <summary>
/// Builds an object tree from LFM tokens.
/// </summary>
public class LfmObjectBuilder
{
    private IEnumerator<LfmToken>? _tokenEnumerator;
    private LfmToken _currentToken = new(LfmTokenType.EOF, "", 0, 0);

    /// <summary>
    /// Builds an LfmObject tree from the given token stream.
    /// </summary>
    /// <param name="tokens">The token stream from LfmTokenizer</param>
    /// <returns>The root LfmObject, or null if parsing fails</returns>
    public LfmObject? Build(IEnumerable<LfmToken> tokens)
    {
        _tokenEnumerator = tokens.GetEnumerator();
        Advance();

        if (_currentToken.Type == LfmTokenType.OBJECT || 
            _currentToken.Type == LfmTokenType.INHERITED ||
            _currentToken.Type == LfmTokenType.INLINE)
        {
            return ParseObject();
        }

        return null;
    }

    private void Advance()
    {
        if (_tokenEnumerator != null && _tokenEnumerator.MoveNext())
        {
            _currentToken = _tokenEnumerator.Current;
        }
        else
        {
            _currentToken = new LfmToken(LfmTokenType.EOF, "", 0, 0);
        }
    }

    private LfmToken Expect(LfmTokenType type)
    {
        if (_currentToken.Type != type)
        {
            throw new InvalidOperationException(
                $"Expected {type} but found {_currentToken.Type} at line {_currentToken.Line}, column {_currentToken.Column}");
        }
        var token = _currentToken;
        Advance();
        return token;
    }

    private LfmObject ParseObject()
    {
        var obj = new LfmObject
        {
            IsInherited = _currentToken.Type == LfmTokenType.INHERITED,
            IsInline = _currentToken.Type == LfmTokenType.INLINE
        };

        Advance(); // consume 'object', 'inherited', or 'inline'

        // Parse object name
        var nameToken = Expect(LfmTokenType.IDENTIFIER);
        obj.Name = nameToken.Value;

        // Expect colon
        Expect(LfmTokenType.COLON);

        // Parse type name (may include dots like TForm1.Panel1)
        obj.TypeName = ParseTypeName();

        // Parse properties and nested objects until 'end'
        while (_currentToken.Type != LfmTokenType.END && _currentToken.Type != LfmTokenType.EOF)
        {
            if (_currentToken.Type == LfmTokenType.OBJECT || 
                _currentToken.Type == LfmTokenType.INHERITED ||
                _currentToken.Type == LfmTokenType.INLINE)
            {
                var childObject = ParseObject();
                if (childObject != null)
                {
                    obj.Children.Add(childObject);
                }
            }
            else if (_currentToken.Type == LfmTokenType.IDENTIFIER)
            {
                var property = ParseProperty();
                if (property != null)
                {
                    obj.Properties.Add(property);
                }
            }
            else
            {
                // Unexpected token, skip
                Advance();
            }
        }

        // Consume 'end'
        if (_currentToken.Type == LfmTokenType.END)
        {
            Advance();
        }

        return obj;
    }

    private string ParseTypeName()
    {
        var sb = new StringBuilder();
        var nameToken = Expect(LfmTokenType.IDENTIFIER);
        sb.Append(nameToken.Value);

        // Handle qualified names like TForm1.Panel1
        while (_currentToken.Type == LfmTokenType.DOT)
        {
            sb.Append('.');
            Advance(); // consume dot
            nameToken = Expect(LfmTokenType.IDENTIFIER);
            sb.Append(nameToken.Value);
        }

        // Handle generic-like syntax <Item>
        if (_currentToken.Type == LfmTokenType.LANGLE)
        {
            sb.Append('<');
            Advance();
            if (_currentToken.Type == LfmTokenType.IDENTIFIER)
            {
                sb.Append(_currentToken.Value);
                Advance();
            }
            if (_currentToken.Type == LfmTokenType.RANGLE)
            {
                sb.Append('>');
                Advance();
            }
        }

        return sb.ToString();
    }

    private LfmProperty? ParseProperty()
    {
        var property = new LfmProperty();

        // Property name (may be qualified like Font.Name)
        var nameBuilder = new StringBuilder();
        var nameToken = Expect(LfmTokenType.IDENTIFIER);
        nameBuilder.Append(nameToken.Value);

        while (_currentToken.Type == LfmTokenType.DOT)
        {
            nameBuilder.Append('.');
            Advance();
            nameToken = Expect(LfmTokenType.IDENTIFIER);
            nameBuilder.Append(nameToken.Value);
        }

        property.Name = nameBuilder.ToString();

        // Expect equals
        Expect(LfmTokenType.EQUALS);

        // Parse value
        property.Value = ParseValue(property);

        return property;
    }

    private object? ParseValue(LfmProperty property)
    {
        switch (_currentToken.Type)
        {
            case LfmTokenType.STRING:
                var stringValue = _currentToken.Value;
                Advance();
                // Handle string continuation with +
                while (_currentToken.Type == LfmTokenType.PLUS)
                {
                    Advance(); // consume +
                    if (_currentToken.Type == LfmTokenType.STRING)
                    {
                        stringValue += _currentToken.Value;
                        Advance();
                    }
                }
                property.PropertyType = LfmPropertyType.Simple;
                return stringValue;

            case LfmTokenType.NUMBER:
                var numValue = _currentToken.Value;
                Advance();
                property.PropertyType = LfmPropertyType.Simple;
                if (int.TryParse(numValue, out int intResult))
                    return intResult;
                if (double.TryParse(numValue, out double doubleResult))
                    return doubleResult;
                return numValue;

            case LfmTokenType.MINUS:
                Advance(); // consume -
                if (_currentToken.Type == LfmTokenType.NUMBER)
                {
                    var negNumValue = "-" + _currentToken.Value;
                    Advance();
                    property.PropertyType = LfmPropertyType.Simple;
                    if (int.TryParse(negNumValue, out int negIntResult))
                        return negIntResult;
                    if (double.TryParse(negNumValue, out double negDoubleResult))
                        return negDoubleResult;
                    return negNumValue;
                }
                return null;

            case LfmTokenType.BOOLEAN:
                var boolValue = _currentToken.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
                Advance();
                property.PropertyType = LfmPropertyType.Simple;
                return boolValue;

            case LfmTokenType.IDENTIFIER:
                var identValue = _currentToken.Value;
                Advance();
                // Handle qualified identifiers like clBtnFace
                while (_currentToken.Type == LfmTokenType.DOT)
                {
                    identValue += ".";
                    Advance();
                    if (_currentToken.Type == LfmTokenType.IDENTIFIER)
                    {
                        identValue += _currentToken.Value;
                        Advance();
                    }
                }
                property.PropertyType = LfmPropertyType.Simple;
                return identValue;

            case LfmTokenType.LBRACKET:
                property.PropertyType = LfmPropertyType.Set;
                return ParseSet();

            case LfmTokenType.LBRACE:
                property.PropertyType = LfmPropertyType.Binary;
                var binaryData = _currentToken.Value;
                Advance();
                return binaryData;

            case LfmTokenType.LPAREN:
                property.PropertyType = LfmPropertyType.Collection;
                return ParseCollection();

            case LfmTokenType.LANGLE:
                property.PropertyType = LfmPropertyType.ItemList;
                return ParseItemList();

            default:
                Advance();
                return null;
        }
    }

    private List<string> ParseSet()
    {
        var items = new List<string>();
        Advance(); // consume [

        while (_currentToken.Type != LfmTokenType.RBRACKET && _currentToken.Type != LfmTokenType.EOF)
        {
            if (_currentToken.Type == LfmTokenType.IDENTIFIER)
            {
                items.Add(_currentToken.Value);
                Advance();
            }
            else if (_currentToken.Type == LfmTokenType.COMMA)
            {
                Advance();
            }
            else
            {
                Advance();
            }
        }

        if (_currentToken.Type == LfmTokenType.RBRACKET)
        {
            Advance();
        }

        return items;
    }

    private List<object?> ParseCollection()
    {
        var items = new List<object?>();
        Advance(); // consume (

        while (_currentToken.Type != LfmTokenType.RPAREN && _currentToken.Type != LfmTokenType.EOF)
        {
            var dummyProp = new LfmProperty();
            var value = ParseValue(dummyProp);
            items.Add(value);
        }

        if (_currentToken.Type == LfmTokenType.RPAREN)
        {
            Advance();
        }

        return items;
    }

    /// <summary>
    /// Parses an item list like: &lt;item ... end item ... end&gt;
    /// </summary>
    private List<LfmItem> ParseItemList()
    {
        var items = new List<LfmItem>();
        Advance(); // consume <

        while (_currentToken.Type != LfmTokenType.RANGLE && _currentToken.Type != LfmTokenType.EOF)
        {
            // Expect 'item' keyword (as IDENTIFIER with value "item")
            if (_currentToken.Type == LfmTokenType.IDENTIFIER && 
                _currentToken.Value.Equals("item", StringComparison.OrdinalIgnoreCase))
            {
                var item = ParseItem();
                items.Add(item);
            }
            else
            {
                // Skip unexpected tokens
                Advance();
            }
        }

        if (_currentToken.Type == LfmTokenType.RANGLE)
        {
            Advance();
        }

        return items;
    }

    /// <summary>
    /// Parses a single item block: item PropertyName = Value ... end
    /// </summary>
    private LfmItem ParseItem()
    {
        var item = new LfmItem();
        Advance(); // consume 'item'

        // Parse properties until 'end'
        while (_currentToken.Type != LfmTokenType.END && _currentToken.Type != LfmTokenType.EOF)
        {
            if (_currentToken.Type == LfmTokenType.IDENTIFIER)
            {
                var property = ParseProperty();
                if (property != null)
                {
                    item.Properties.Add(property);
                }
            }
            else if (_currentToken.Type == LfmTokenType.RANGLE)
            {
                // End of item list reached without explicit 'end' - stop parsing this item
                break;
            }
            else
            {
                Advance();
            }
        }

        // Consume 'end' if present
        if (_currentToken.Type == LfmTokenType.END)
        {
            Advance();
        }

        return item;
    }
}
