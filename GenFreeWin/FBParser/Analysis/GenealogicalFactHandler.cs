using System;
using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Handles genealogical fact fragments after the parser has identified the surrounding person or family context.
/// </summary>
internal sealed class GenealogicalFactHandler : IGenealogicalFactHandler
{
    private readonly GenealogicalFactHandlerConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalFactHandler"/> class.
    /// </summary>
    /// <param name="configuration">The immutable configuration used to delegate parser-specific callbacks.</param>
    public GenealogicalFactHandler(GenealogicalFactHandlerConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public ParserEventType HandleNonPersonEntry(string subString, string individualId, string defaultPlace, string famRef = "", ParserEventType previousEntryType = ParserEventType.evt_Anull)
    {
        var localSubString = subString.Trim();
        var localFamRef = famRef;
        if (localSubString == string.Empty || localSubString == ".")
        {
            return ParserEventType.evt_Anull;
        }

        _configuration.AnalyseEntry(ref localSubString, out var entryType, out var data, out var place, out var date);
        if (entryType == ParserEventType.evt_Stillborn)
        {
            _configuration.SetIndiDate(individualId, ParserEventType.evt_Birth, date);
            _configuration.SetIndiData(individualId, ParserEventType.evt_Birth, "totgeboren");
            if (place != string.Empty)
            {
                _configuration.SetIndiPlace(individualId, ParserEventType.evt_Birth, place);
            }

            _configuration.SetIndiDate(individualId, ParserEventType.evt_Death, date);
            if (place != string.Empty)
            {
                _configuration.SetIndiPlace(individualId, ParserEventType.evt_Death, place);
            }
        }
        else if (entryType is ParserEventType.evt_Marriage or ParserEventType.evt_Divorce)
        {
            if (localFamRef == string.Empty)
            {
                localFamRef = Ziffern.Contains(individualId[^1]) ? individualId[1..] + "P0" : individualId[1..] + "0";
            }

            _configuration.StartFamily(localFamRef);
            char sex;
            if (localSubString.StartsWith(_configuration.MarriagePartnerMarker + " ", StringComparison.Ordinal))
            {
                _ = _configuration.HandleAkPersonEntry(Copy(localSubString, _configuration.MarriagePartnerMarker.Length + 2), localFamRef, 'U', 57, out _, out sex);
            }
            else
            {
                sex = 'F';
            }

            _configuration.SetFamilyMember(localFamRef, individualId, sex == 'M' ? 2 : 1);
            if (date != string.Empty)
            {
                _configuration.SetFamilyDate(localFamRef, entryType, date);
            }

            if (place != string.Empty)
            {
                _configuration.SetFamilyPlace(localFamRef, entryType, place);
            }

            if (data != string.Empty || (date == string.Empty && place == string.Empty))
            {
                _configuration.SetFamilyData(localFamRef, entryType, data);
            }
        }
        else if (entryType > ParserEventType.evt_ID && entryType != ParserEventType.evt_Last && entryType != ParserEventType.evt_Occupation)
        {
            if (date != string.Empty)
            {
                _configuration.SetIndiDate(individualId, entryType, date);
            }

            if (place != string.Empty)
            {
                _configuration.SetIndiPlace(individualId, entryType, place);
            }

            if (data != string.Empty)
            {
                _configuration.SetIndiData(individualId, entryType, data);
            }
        }
        else
        {
            var hasLedig = _configuration.TryConsumeLeadingEntry(ref localSubString, _configuration.LedigMarkers, out var rest);
            if (hasLedig)
            {
                localSubString = rest;
            }

            if (_configuration.TryConsumeLeadingEntry(ref localSubString, _configuration.IndefiniteArticles, out rest))
            {
                localSubString = rest;
            }

            var hasOccupationText = localSubString.Trim() != string.Empty;
            var emitLedigOccupationPlace = hasLedig && hasOccupationText;
            if (hasLedig && !hasOccupationText)
            {
                if (previousEntryType == ParserEventType.evt_Occupation && defaultPlace != string.Empty)
                {
                    _configuration.SetIndiPlace(individualId, ParserEventType.evt_Occupation, defaultPlace);
                }
                else
                {
                    var descriptionPlace = place != string.Empty ? place : defaultPlace;
                    if (descriptionPlace != string.Empty)
                    {
                        _configuration.SetIndiPlace(individualId, ParserEventType.evt_Description, descriptionPlace);
                    }
                }

                place = string.Empty;
            }

            if (hasLedig)
            {
                _configuration.SetIndiData(individualId, ParserEventType.evt_Description, _configuration.LedigText);
            }

            entryType = ParserEventType.evt_Occupation;
            if (_configuration.TestFor(localSubString.Trim(), 1, _configuration.TitleMarkers, out _))
            {
                _configuration.SetIndiName(individualId, 4, localSubString.Trim());
            }
            else if (localSubString != string.Empty)
            {
                _configuration.SetIndiOccu(individualId, entryType, localSubString.Trim());
            }
            else if (date != string.Empty)
            {
                _configuration.Warning("Entry contains no Marker, only a Date");
            }

            if (date != string.Empty)
            {
                _configuration.SetIndiDate(individualId, entryType, date);
            }

            if (place != string.Empty)
            {
                _configuration.SetIndiPlace(individualId, entryType, place);
            }
            else if (emitLedigOccupationPlace && defaultPlace != string.Empty)
            {
                _configuration.SetIndiPlace(individualId, entryType, defaultPlace);
            }
        }

        return entryType;
    }

    /// <inheritdoc />
    public void HandleFamilyFact(string mainFamRef, string famEntry)
    {
        var subString = famEntry.Trim();
        if (subString == string.Empty || subString == ".")
        {
            return;
        }

        _configuration.AnalyseEntry(ref subString, out var entryType, out var data, out var place, out var date);
        if (date != string.Empty)
        {
            _configuration.SetFamilyDate(mainFamRef, entryType, date);
        }

        if (place != string.Empty && entryType != ParserEventType.evt_Last)
        {
            _configuration.SetFamilyPlace(mainFamRef, entryType, place);
        }

        if (data != string.Empty || (date == string.Empty && place == string.Empty && entryType != ParserEventType.evt_Last))
        {
            _configuration.SetFamilyData(mainFamRef, entryType, data);
        }
        else if (subString != string.Empty && entryType == ParserEventType.evt_Last)
        {
            if (place != string.Empty)
            {
                _configuration.SetFamilyPlace(mainFamRef, ParserEventType.evt_Residence, place);
            }

            _configuration.SetFamilyData(mainFamRef, ParserEventType.evt_Residence, subString);
        }
    }
}
