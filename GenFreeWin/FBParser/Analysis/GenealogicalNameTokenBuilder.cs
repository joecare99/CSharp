using System;
using System.Linq;
using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Builds genealogical person-name tokens independently from the surrounding parser state machine.
/// </summary>
internal sealed class GenealogicalNameTokenBuilder : IGenealogicalNameTokenBuilder
{
    private readonly GenealogicalNameTokenBuilderConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalNameTokenBuilder"/> class.
    /// </summary>
    /// <param name="configuration">The immutable configuration used for parser callbacks and marker lookup.</param>
    public GenealogicalNameTokenBuilder(GenealogicalNameTokenBuilderConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public bool BuildNameToken(string text, ref int offset, ref int charCount, ref string subString, out string additional)
    {
        additional = string.Empty;
        var currentChar = CharAt(text, offset);
        if (In(currentChar, Charset))
        {
            subString += currentChar;
            charCount++;
        }
        else if (currentChar == ' ')
        {
            subString += currentChar;
            charCount = 0;
        }
        else if (_configuration.TestFor(text, offset, [_configuration.ProtectSpaceMarker], out _))
        {
            subString += _configuration.ProtectSpaceMarker;
            offset += _configuration.ProtectSpaceMarker.Length - 1;
            charCount = 0;
        }
        else if (Copy(text, offset, _configuration.UnknownMarker.Length) == _configuration.UnknownMarker)
        {
            subString += _configuration.UnknownMarker;
            offset += _configuration.UnknownMarker.Length - 1;
            charCount = 0;
        }
        else if (currentChar == '.'
            && ((offset + 1 <= text.Length && new[] { ',', '.', ' ', '>' }.Contains(CharAt(text, offset + 1)))
                || _configuration.TestFor(text, offset + 1, [_configuration.Separator2Marker], out _)
                || (offset > 1
                    && (Charset.Contains(CharAt(text, offset - 1)) || CharAt(text, offset - 1) == '.')
                    && (charCount <= 3 || (offset + 1 <= text.Length && In(CharAt(text, offset + 1), UpperCharset))))))
        {
            subString += currentChar;
        }
        else if (_configuration.TestFor(text, offset, ["-"], out var found)
            && offset > 1
            && offset + 1 + found <= text.Length
            && In(CharAt(text, offset - 1), LowerCharset)
            && In(CharAt(text, offset + 1 + found), UpperCharset))
        {
            subString += currentChar;
            charCount = 0;
        }
        else if (_configuration.TestFor(text, offset, ["-", "­"], out found)
            && offset > 1
            && offset + 1 + found <= text.Length
            && In(CharAt(text, offset - 1), LowerCharset)
            && In(CharAt(text, offset + 1 + found), LowerCharset))
        {
            if (found == 0)
            {
                _configuration.Warning("Hyphen in Name Ignored");
            }

            charCount = 0;
            offset += found;
        }
        else if (_configuration.TestFor(text, offset, _configuration.UmlautMarkers, out found))
        {
            subString += _configuration.UmlautMarkers[found];
            charCount++;
            offset += _configuration.UmlautMarkers[found].Length - 1;
        }
        else if (_configuration.TestFor(text, offset, [_configuration.Separator2Marker], out _))
        {
            offset += _configuration.Separator2Marker.Length - 1;
            return false;
        }
        else
        {
            return _configuration.TestFor(text, offset, ["("], out _) && _configuration.ParseAdditional(text, ref offset, out additional);
        }

        return true;
    }

    /// <inheritdoc />
    public bool BuildName(string text, ref int offset, ref string subString, ref string data, ref int charCount, ref string? aka, ref ParserEventType addEvent)
    {
        var result = false;
        if (BuildNameToken(text, ref offset, ref charCount, ref subString, out var additional))
        {
            if (additional == _configuration.TwinMarker)
            {
                data = data == string.Empty ? "Zwilling" : data + "; Zwilling";
            }
            else if (additional != string.Empty)
            {
                aka = additional;
                addEvent = ParserEventType.evt_AKA;
                charCount = 0;
            }
        }
        else
        {
            result = subString.Trim().Length > 0;
        }

        return result;
    }
}
