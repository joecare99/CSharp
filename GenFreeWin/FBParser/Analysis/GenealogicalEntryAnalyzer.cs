using System;
using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Provides parser-independent analysis helpers for genealogical entry fragments.
/// </summary>
internal sealed class GenealogicalEntryAnalyzer : IGenealogicalEntryAnalyzer
{
    private readonly GenealogicalEntryAnalyzerConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalEntryAnalyzer"/> class.
    /// </summary>
    /// <param name="configuration">The immutable analysis configuration.</param>
    public GenealogicalEntryAnalyzer(GenealogicalEntryAnalyzerConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public ParserEventType GetEntryType(string subString, out string date, out string data)
    {
        var result = ParserEventType.evt_Last;
        data = string.Empty;
        date = string.Empty;

        if (TestEntry(subString, _configuration.BaptismMarkers, out date))
        {
            result = ParserEventType.evt_Baptism;
        }
        else if (TestEntry(subString, _configuration.BirthMarker, out date))
        {
            result = ParserEventType.evt_Birth;
        }
        else if (TestEntry(subString, _configuration.BurialMarkers, out date))
        {
            result = ParserEventType.evt_Burial;
        }
        else if (TestEntry(subString, _configuration.MarriageMarkers, out date))
        {
            result = ParserEventType.evt_Marriage;
        }
        else if (TestEntry(subString, _configuration.DeathMarkers, out date))
        {
            result = ParserEventType.evt_Death;
        }
        else if (TestEntry(subString, _configuration.StillbornMarkers, out date))
        {
            result = ParserEventType.evt_Stillborn;
            data = "totgeboren";
        }
        else if (TestEntry(subString, _configuration.FallenMarker, out date))
        {
            result = ParserEventType.evt_fallen;
            data = _configuration.FallenMarker;
        }
        else if (TestEntry(subString, _configuration.DivorceMarker, out date))
        {
            result = ParserEventType.evt_Divorce;
        }
        else if (TestEntry(subString, _configuration.MissingMarkers, out date))
        {
            result = ParserEventType.evt_missing;
            data = _configuration.MissingMarkers[0];
        }
        else if (subString.Contains(_configuration.EmigrationMarkers[0], StringComparison.Ordinal)
            || subString.Contains(_configuration.EmigrationMarkers[1], StringComparison.Ordinal))
        {
            result = ParserEventType.evt_AddEmigration;
            var subString2 = subString.StartsWith("ist", StringComparison.Ordinal) ? RemoveStart(subString, 4) : subString;
            data = Left(subString2, subString2.Length - _configuration.EmigrationMarkers[0].Length - 1);
        }
        else if (subString.EndsWith(" " + _configuration.AgeMarker, StringComparison.Ordinal))
        {
            result = ParserEventType.evt_Age;
            data = subString;
        }
        else if (TestEntry(subString, _configuration.IndefiniteArticles, out data))
        {
            result = ParserEventType.evt_Description;
            data = subString;
        }
        else if (subString.Contains(_configuration.AkaMarker, StringComparison.Ordinal))
        {
            result = ParserEventType.evt_AKA;
            var subString2 = subString.StartsWith(_configuration.BecameMarker, StringComparison.Ordinal)
                ? RemoveStart(subString, _configuration.BecameMarker.Length).Trim()
                : subString;
            var pp = subString2.IndexOf(_configuration.AkaMarker, StringComparison.Ordinal);
            data = (Left(subString2, pp).Trim() + " " + Copy(subString2, pp + _configuration.AkaMarker.Length + 1).Trim()).Trim();
        }
        else if (TestEntry(subString, _configuration.DefiniteArticles, out data))
        {
            result = ParserEventType.evt_AKA;
            data = subString;
        }
        else if (FBEntryParser.TestFor(subString, 1, _configuration.DescriptionMarkers, out var found) && _configuration.DescriptionMarkers[found] == subString)
        {
            result = ParserEventType.evt_Description;
            data = subString;
        }
        else if (FBEntryParser.TestFor(subString, 1, _configuration.ResidenceMarkers, out found))
        {
            result = ParserEventType.evt_Residence;
            data = _configuration.ResidenceMarkers[found];
            date = subString[data.Length..].Trim();
            if (date != string.Empty
                && !date.StartsWith(_configuration.InPlaceMarker + " ", StringComparison.Ordinal)
                && !date.StartsWith(_configuration.SinceDateModifier + " ", StringComparison.Ordinal)
                && !Ziffern.Contains(date[0]))
            {
                data += " " + date;
                date = string.Empty;
            }
        }
        else if (IndexOfAny(subString, _configuration.PropertyMarkers) > 0)
        {
            result = ParserEventType.evt_Property;
            data = subString;
        }
        else if (IndexOfAny(subString, _configuration.AddressMarkers) > 0
            && ((subString.Length > 0 && UpperCharset.Contains(subString[0])) || FBEntryParser.TestFor(subString, 1, _configuration.UpperUmlautMarkers))
            && CountChar(subString, ' ') < 2)
        {
            result = ParserEventType.evt_Residence;
            data = subString;
        }
        else if (FBEntryParser.TestFor(subString + '.', 1, _configuration.ReligionMarkers, out found))
        {
            result = ParserEventType.evt_Religion;
            data = _configuration.ReligionMarkers[found];
        }

        return result;
    }

    /// <inheritdoc />
    public void AnalyseEntry(ref string subString, string defaultPlace, int currentMode, out ParserEventType entryType, out string data, out string place, out string date)
    {
        entryType = ParserEventType.evt_Anull;
        date = string.Empty;
        data = string.Empty;
        place = string.Empty;

        var dPos = LastIndexOfAny(subString, Ziffern);
        if (dPos >= 0)
        {
            dPos++;
        }

        var placeDescription = false;
        var pp2 = -1;
        var found = -1;

        if (FBEntryParser.TestFor(subString, dPos + 2, _configuration.PlaceMarkers, out found))
        {
            place = Copy(subString, dPos + 2 + _configuration.PlaceMarkers[found].Length).Trim();
            placeDescription = place == string.Empty || LowerCharset.Contains(place[0]);
            if (placeDescription)
            {
                place = string.Empty;
                data = subString;
                if (dPos < 0)
                {
                    entryType = ParserEventType.evt_Residence;
                }
            }
            else if (dPos < 0)
            {
                entryType = ParserEventType.evt_Residence;
                subString = string.Empty;
            }
            else
            {
                subString = Copy(subString, 1, dPos + 1).Trim();
            }
        }
        else
        {
            var pp = (" " + subString).IndexOf(" " + _configuration.InPlaceMarker + " ", StringComparison.Ordinal);
            if (pp < 0)
            {
                pp = (" " + subString).IndexOf(" " + _configuration.InMonthPlaceMarker + " ", StringComparison.Ordinal);
                found = 5;
                if (pp > 5)
                {
                    pp = -1;
                }
            }
            else
            {
                found = 0;
            }

            var protectSpaceLength = 1;
            if (pp < 0)
            {
                pp = (_configuration.ProtectSpace + subString).IndexOf(_configuration.ProtectSpace + _configuration.InPlaceMarker + " ", StringComparison.Ordinal);
                if (pp >= 0)
                {
                    protectSpaceLength = _configuration.ProtectSpace.Length;
                    found = 0;
                }
            }

            pp2 = IndexOfAny(subString, Ziffern);
            if (pp >= 0 && pp2 < pp && (subString.Length < pp + _configuration.InPlaceMarker.Length + 3 || subString[pp + _configuration.InPlaceMarker.Length + 2] != 'd'))
            {
                place = Copy(subString, pp + 4);
                subString = Copy(subString, 1, pp - protectSpaceLength);
                if (subString == string.Empty)
                {
                    entryType = ParserEventType.evt_Residence;
                }
            }
            else if (pp >= 0 && pp2 > pp)
            {
                place = Copy(subString, pp + 4, pp2 - pp - 4).Trim();
                if (subString.Length - pp2 < 7)
                {
                    TrimPlaceByMonth(ref place);
                }

                TrimPlaceByModif(ref place);
                subString = Copy(subString, 1, pp) + Copy(subString, pp + 4 + place.Length);
                pp2 = IndexOfAny(subString, Ziffern);
            }
            else
            {
                pp = Pos(" " + _configuration.FromPlaceMarker + " ", " " + subString);
                if (pp != 0)
                {
                    found = 1;
                    place = Copy(subString, pp + 4);
                    subString = Copy(subString, 1, pp - 1);
                    if (subString == string.Empty)
                    {
                        entryType = ParserEventType.evt_Residence;
                    }
                }
                else
                {
                    place = string.Empty;
                }
            }
        }

        if (entryType != ParserEventType.evt_Residence)
        {
            entryType = GetEntryType(subString, out date, out data);
        }

        if (entryType == ParserEventType.evt_Last && found >= 0)
        {
            entryType = GetEntryType(subString + " " + _configuration.PlaceMarkers[found] + " " + place, out date, out data);
        }

        if (entryType == ParserEventType.evt_Description && place != string.Empty && FBEntryParser.TestFor(subString, 1, _configuration.IndefiniteArticles, out var found2))
        {
            entryType = ParserEventType.evt_Occupation;
            subString = Copy(subString, _configuration.IndefiniteArticles[found2].Length + 1).Trim();
        }

        var pp3Text = data.IndexOf(_configuration.ToPlaceMarker, StringComparison.Ordinal);
        if (pp3Text >= 0 && entryType == ParserEventType.evt_AddEmigration)
        {
            pp2 = IndexOfAny(data, Ziffern);
            place = Copy(data, pp3Text + _configuration.ToPlaceMarker.Length + 2).Trim();
            if (pp2 < 0)
            {
                date = string.Empty;
            }
            else
            {
                var rest = Copy(data, 1, pp2).Trim();
                TrimPlaceByMonth(ref rest);
                TrimPlaceByModif(ref rest);
                date = Copy(data, rest.Length + 1, pp3Text - rest.Length).Trim();
            }

            if (data.Length < subString.Length)
            {
                data = subString;
            }
            else
            {
                data += " " + _configuration.EmigrationMarkers[0];
            }
        }

        if (entryType == ParserEventType.evt_Property && !_configuration.IsValidPlace(place))
        {
            data = data + " " + _configuration.PlaceMarkers[found] + " " + place;
            place = string.Empty;
        }

        if ((place.Trim() == string.Empty || !_configuration.IsValidDate(date) || date.Length > 14)
            && entryType is >= ParserEventType.evt_Birth and <= ParserEventType.evt_Burial or ParserEventType.evt_Stillborn or ParserEventType.evt_fallen or ParserEventType.evt_missing
            && date.Length > 1
            && (UpperCharset.Contains(date[0]) || FBEntryParser.TestFor(date, 1, _configuration.UpperUmlautMarkers) || FBEntryParser.TestFor(date, 1, _configuration.UnknownMarkers)))
        {
            var originalPlace = place.Trim() != string.Empty ? place : string.Empty;
            pp2 = IndexOfAny(date, Ziffern);
            if (FBEntryParser.TestFor(date, 1, _configuration.UnknownMarkers, out found2))
            {
                place = _configuration.UnknownMarkers[found2];
                date = Copy(date, place.Length + 1).Trim();
            }
            else if (pp2 > 1 && dPos > date.Length)
            {
                place = Left(date, pp2).Trim();
                if (date.Length - pp2 < 7)
                {
                    TrimPlaceByMonth(ref place);
                }

                TrimPlaceByModif(ref place);
                date = Copy(date, place.Length + 1).Trim();
                if (place == string.Empty)
                {
                    place = originalPlace;
                }
                else if (originalPlace.Trim() != string.Empty)
                {
                    place = place + " " + _configuration.PlaceMarkers[found] + " " + originalPlace;
                }
            }
            else if (pp2 == -1 && originalPlace == string.Empty)
            {
                place = date;
                date = string.Empty;
            }
            else
            {
                var pos = date.Length > 12 ? date.IndexOf(' ', date.Length - 12) : date.LastIndexOf(' ');
                if (pos > 0)
                {
                    place = Left(date, pos);
                    date = Copy(date, pos + 2);
                }
            }
        }
        else if (pp2 >= 0 && data == string.Empty && date == string.Empty)
        {
            var pp3 = LastIndexOfAny(subString, Ziffern);
            if (!((pp3 == pp2) || (pp3 == pp2 + 1) || (pp3 == pp2 + 2))
                && pp3 >= 2
                && In(subString[pp3 - 2], Ziffern)
                && In(subString[pp3 - 1], Ziffern)
                && In(subString[pp3], Ziffern)
                && !Copy(subString, pp2 + 3, pp3 - pp2 - 9).Contains(' ')
                && !(pp2 == 0 && subString.Contains("Jahre", StringComparison.Ordinal)))
            {
                var rest = Left(subString, pp2).Trim();
                if (rest.Length - pp2 < 7)
                {
                    TrimPlaceByMonth(ref rest);
                }

                TrimPlaceByModif(ref rest);
                date = Copy(subString, rest.Length + 1, pp3 - rest.Length + 1).Trim();
                subString = (rest + Copy(subString, pp3 + 2)).Trim();
            }
        }

        if (entryType == ParserEventType.evt_Marriage)
        {
            if (date.StartsWith(_configuration.MarriagePartnerMarker + " ", StringComparison.Ordinal) && pp2 < 0)
            {
                subString = date;
                date = string.Empty;
            }

            if (date.Contains(" " + _configuration.MarriagePartnerMarker + " ", StringComparison.Ordinal) && pp2 >= 0)
            {
                subString = Copy(date, date.IndexOf(_configuration.MarriagePartnerMarker, StringComparison.Ordinal) + 1);
                date = Copy(date, 1, date.Length - subString.Length).Trim();
            }
        }

        if (date.StartsWith(_configuration.InMonthPlaceMarker + " ", StringComparison.Ordinal)
            || date.StartsWith(_configuration.OnDatePlaceMarker + " ", StringComparison.Ordinal))
        {
            date = Copy(date, _configuration.OnDatePlaceMarker.Length + 2);
        }

        if (place.Trim() == string.Empty && !new[] { 55, 56, 57 }.Contains(currentMode) && !placeDescription && entryType != ParserEventType.evt_Divorce)
        {
            place = defaultPlace;
        }

        if (!_configuration.IsValidPlace(place))
        {
            _configuration.Warning("Misspelled Place \"" + place + "\"");
            while (place != string.Empty && !Charset.Contains(place[0]))
            {
                place = RemoveStart(place, 1);
            }
        }

        if (entryType is ParserEventType.evt_fallen or ParserEventType.evt_missing)
        {
            entryType = ParserEventType.evt_Death;
        }
    }

    /// <inheritdoc />
    public void TrimPlaceByMonth(ref string place)
    {
        var flag = false;
        for (var index = 1; index < _configuration.MonthNames.Length; index++)
        {
            if (place.EndsWith(_configuration.MonthNames[index], StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - _configuration.MonthNames[index].Length).Trim();
                flag = true;
                break;
            }

            var shortMonth = Copy(_configuration.MonthNames[index], 1, 3) + ".";
            if (place.EndsWith(shortMonth, StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - 4).Trim();
                flag = true;
                break;
            }
        }

        if (flag && (" " + place).EndsWith(" " + _configuration.InMonthPlaceMarker, StringComparison.Ordinal))
        {
            place = Copy(place, 1, place.Length - _configuration.InMonthPlaceMarker.Length).Trim();
        }
    }

    /// <inheritdoc />
    public void TrimPlaceByModif(ref string place)
    {
        for (var index = 1; index < _configuration.DateModifiers.Length; index++)
        {
            if ((" " + place).EndsWith(" " + _configuration.DateModifiers[index], StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - _configuration.DateModifiers[index].Length).Trim();
                break;
            }
        }
    }

    private bool TestEntry(string subString, string testString, out string data)
    {
        data = string.Empty;
        var result = Copy(subString, 1, testString.Length) == testString;
        if (!result)
        {
            return false;
        }

        if (subString == testString)
        {
            data = string.Empty;
        }
        else if (FBEntryParser.TestFor(subString, testString.Length + 1, _configuration.ProtectSpace))
        {
            data = Copy(subString, testString.Length + _configuration.ProtectSpace.Length + 1).Trim();
        }
        else if (FBEntryParser.TestFor(subString, testString.Length + 1, " "))
        {
            data = Copy(subString, testString.Length + 2).Trim();
        }
        else if (!char.IsLetterOrDigit((testString + " ")[testString.Length - 1])
            && (((Copy(subString, testString.Length + 1) + " ")[0] is >= '0' and <= '9' or >= 'A' and <= 'Z' or >= 'a' and <= 'z')
                || FBEntryParser.TestFor(subString, testString.Length, _configuration.UpperUmlautMarkers)))
        {
            data = Copy(subString, testString.Length + 1).Trim();
        }
        else
        {
            result = false;
        }

        return result;
    }

    private bool TestEntry(string subString, string[] testStrings, out string data)
    {
        foreach (var testString in testStrings)
        {
            if (TestEntry(subString, testString, out data))
            {
                return true;
            }
        }

        data = string.Empty;
        return false;
    }
}
