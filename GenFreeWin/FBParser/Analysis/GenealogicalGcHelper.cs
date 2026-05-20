using System.Linq;
using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Provides isolated helper logic for genealogical GC parsing branches.
/// </summary>
internal sealed class GenealogicalGcHelper : IGenealogicalGcHelper
{
    private readonly GenealogicalGcHelperConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalGcHelper"/> class.
    /// </summary>
    /// <param name="configuration">The immutable configuration used by the GC helper logic.</param>
    public GenealogicalGcHelper(GenealogicalGcHelperConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public bool HandleGcDateEntry(string text, ref int position, string individualId, ref int mode, ref int retMode, ref ParserEventType entryType)
    {
        if (_configuration.TestFor(text, position, [_configuration.BirthMarker], out _)
            || _configuration.TestFor(text, position, [_configuration.BaptismMarker], out _)
            || _configuration.TestFor(text, position, [_configuration.DeathMarker], out _)
            || _configuration.TestFor(text, position, [_configuration.BurialMarker], out _))
        {
            retMode = mode;
            mode = 101;
            if (_configuration.TestFor(text, position, [_configuration.BirthMarker], out _))
            {
                entryType = ParserEventType.evt_Birth;
            }
            else if (_configuration.TestFor(text, position, [_configuration.BaptismMarker], out _))
            {
                entryType = ParserEventType.evt_Baptism;
            }
            else if (_configuration.TestFor(text, position, [_configuration.DeathMarker], out _))
            {
                entryType = ParserEventType.evt_Death;
            }
            else
            {
                entryType = ParserEventType.evt_Burial;
            }

            return true;
        }

        if (_configuration.TestForCaseInsensitive(text, position, _configuration.FallenMarker))
        {
            retMode = mode;
            mode = 101;
            position += _configuration.FallenMarker.Length - 1;
            _configuration.SetIndiData(individualId, ParserEventType.evt_Death, _configuration.FallenMarker);
            entryType = ParserEventType.evt_Death;
            return true;
        }

        if (_configuration.TestForCaseInsensitive(text, position, _configuration.MissingMarker))
        {
            retMode = mode;
            mode = 101;
            position += _configuration.MissingMarker.Length - 1;
            _configuration.SetIndiData(individualId, ParserEventType.evt_Death, _configuration.MissingMarker);
            entryType = ParserEventType.evt_Death;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool HandleGcNonPersonEntry(string subString, char actChar, string individualId)
    {
        var localSubstring = subString;
        if (((localSubstring.Length < 4) && localSubstring.EndsWith(".") && localSubstring.Length > 0 && char.IsLower(localSubstring[0]))
            || localSubstring.Trim().Length == 2)
        {
            if (actChar == '.')
            {
                localSubstring += '.';
            }

            _configuration.SetIndiData(individualId, ParserEventType.evt_Religion, localSubstring.Trim());
            return true;
        }

        if (localSubstring.Length <= 2)
        {
            return false;
        }

        var pp = Pos(" in ", localSubstring);
        string place;
        if (pp != 0)
        {
            place = Copy(localSubstring, pp + 4, 255);
            localSubstring = Copy(localSubstring, 1, pp - 1);
        }
        else
        {
            place = _configuration.GetDefaultPlace();
        }

        foreach (var data in localSubstring.Split(','))
        {
            var trimmedData = data.Trim();
            if (trimmedData == "Bürger")
            {
                _configuration.SetIndiData(individualId, ParserEventType.evt_Residence, trimmedData);
                if (place != string.Empty)
                {
                    _configuration.SetIndiPlace(individualId, ParserEventType.evt_Residence, place.Trim());
                }
            }
            else if (trimmedData == "ausgewandert")
            {
                _configuration.SetIndiData(individualId, ParserEventType.evt_AddEmigration, string.Empty);
                if (place != string.Empty)
                {
                    _configuration.SetIndiPlace(individualId, ParserEventType.evt_AddEmigration, place.Trim());
                }
            }
            else if (_configuration.TestFor(trimmedData, 1, _configuration.TitleMarkers, out _))
            {
                _configuration.SetIndiName(individualId, 4, trimmedData);
                if (place != string.Empty)
                {
                    _configuration.SetIndiPlace(individualId, ParserEventType.evt_Occupation, place.Trim());
                }
            }
            else
            {
                _configuration.SetIndiOccu(individualId, ParserEventType.evt_Occupation, trimmedData);
                if (place != string.Empty)
                {
                    _configuration.SetIndiPlace(individualId, ParserEventType.evt_Occupation, place.Trim());
                }
            }
        }

        return true;
    }

    /// <inheritdoc />
    public string ScanForEventDate(string text, int offset)
    {
        var result = string.Empty;
        var pos = Pos(_configuration.ChildDateMarker, text, offset);
        if (pos > 0)
        {
            pos--;
        }
        else
        {
            pos = Pos(_configuration.ChildDateMarkerAlternate, text, offset);
        }

        if (pos <= 0)
        {
            return result;
        }

        pos += 4;
        var offs = pos;
        var found = -1;
        var ziffCount = 0;
        while (offs < text.Length && ((In(CharAt(text, offs), Charset.Concat(WhiteSpaceChars).Concat([',']))) || _configuration.TestFor(text, offs, _configuration.UmlautMarkers, out found)))
        {
            if (found >= 0)
            {
                offs++;
            }

            offs++;
            found = -1;
        }

        if (_configuration.TestFor(text, offs, [_configuration.DeathMarker], out _))
        {
            offs += _configuration.DeathMarker.Length;
        }

        if (_configuration.TestFor(text, offs, [_configuration.BirthMarker], out _))
        {
            offs += 1;
        }

        while (offs < text.Length && (In(CharAt(text, offs), Ziffern.Concat([' ', 'u', 'm', 'v', 'o', 'r'])) || (CharAt(text, offs) == '.' && ziffCount < 4)))
        {
            if (In(CharAt(text, offs), Ziffern))
            {
                ziffCount++;
            }
            else
            {
                ziffCount = 0;
            }

            result += CharAt(text, offs);
            offs++;
        }

        return result;
    }
}
