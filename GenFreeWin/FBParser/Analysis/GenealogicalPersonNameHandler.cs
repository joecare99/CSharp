using System;
using System.Globalization;
using static FBParser.PascalCompat;

namespace FBParser.Analysis;

/// <summary>
/// Handles genealogical person-name entries independently from the surrounding parser state machine.
/// </summary>
internal sealed class GenealogicalPersonNameHandler : IGenealogicalPersonNameHandler
{
    private readonly GenealogicalPersonNameHandlerConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenealogicalPersonNameHandler"/> class.
    /// </summary>
    /// <param name="configuration">The immutable configuration used for callbacks and marker lookup.</param>
    public GenealogicalPersonNameHandler(GenealogicalPersonNameHandlerConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public string HandleAkPersonEntry(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "")
    {
        var localAka = aka;
        var personName = !personEntry.Contains(_configuration.UnknownShortMarker, StringComparison.Ordinal)
            ? personEntry.Replace("  ", " ", StringComparison.Ordinal).Replace(". ", ".", StringComparison.Ordinal).Replace(".", ". ", StringComparison.Ordinal)
            : (personEntry + " ").Replace("  ", " ", StringComparison.Ordinal);
        var names = personName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var title = string.Empty;
        if (_configuration.TestFor(personName, 1, _configuration.AcademicTitleMarkers, out var found))
        {
            title = _configuration.AcademicTitleMarkers[found];
            personName = Copy(personName, title.Length + 2);
            names = personName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        if (names.Length > 0
            && !names[^1].EndsWith(".", StringComparison.Ordinal)
            && _configuration.GuessSexOfGivenName(names[^1], false) != '_'
            && localAka.StartsWith("?", StringComparison.Ordinal)
            && localAka.Length > 3)
        {
            lastName = Copy(localAka, 2).Trim().Replace(".", ". ", StringComparison.Ordinal);
            if (lastName == Copy(famName, 1, lastName.Length - 2) + ". ")
            {
                lastName = famName;
            }

            personName += " " + lastName;
            localAka = "? " + lastName;
            names = personName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        var marriageFlag = false;
        for (var index = 1; index < names.Length - 1; index++)
        {
            if (names[index] == _configuration.MaidenNameMarker)
            {
                if (index > 1 && mode != 56)
                {
                    var spouseLastName = names[index - 1];
                    personName = personName.Replace(" " + spouseLastName, string.Empty, StringComparison.Ordinal);
                }

                personName = personName.Replace(" " + _configuration.MaidenNameMarker, string.Empty, StringComparison.Ordinal);
                names = personName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                marriageFlag = true;
                break;
            }
        }

        lastName = names.Length > 0 ? names[^1] : string.Empty;
        if (names.Length > 2 && names[^2] != string.Empty && LowerCharset.Contains(names[^2][0]))
        {
            lastName = names[^2] + " " + lastName;
        }

        if (!lastName.EndsWith(".", StringComparison.Ordinal)
            && _configuration.GuessSexOfGivenName(lastName, false) != '_'
            && localAka.EndsWith(".", StringComparison.Ordinal)
            && localAka.Length < 4
            && localAka == Copy(famName, 1, localAka.Length - 1) + ".")
        {
            lastName = famName;
            personName += " " + lastName;
            localAka = "? " + lastName;
        }

        if (lastName != string.Empty
            && (UpperCharset.Contains(lastName[0]) || lastName[0] == 'Ü')
            && lastName.EndsWith(".", StringComparison.Ordinal)
            && lastName.Length <= 4
            && Copy(famName, 1, lastName.Length - 1) + "." == lastName)
        {
            personName = personName.Replace(lastName + " ", famName, StringComparison.Ordinal);
            lastName = famName;
        }
        else
        {
            personName = personName.Trim();
        }

        if (personType == 'U' && Copy(personName, 1, personName.Length - lastName.Length - 1) != _configuration.UnknownShortMarker)
        {
            personSex = _configuration.GuessSexOfGivenName(Copy(personName, 1, personName.Length - lastName.Length - 1));
        }
        else
        {
            personSex = personType;
        }

        if (personSex is 'M' or 'F')
        {
            _configuration.LearnSexOfGivenName(Copy(personName, 1, personName.Length - lastName.Length - 1), personType);
        }

        var result = "I" + mainFamRef + personSex;
        if (marriageFlag)
        {
            _configuration.SetFamilyType(mainFamRef, 1, string.Empty);
        }

        _configuration.SetIndiName(result, 0, personName);
        if (title != string.Empty)
        {
            _configuration.SetIndiName(result, 4, title);
        }

        _configuration.SetFamilyMember(mainFamRef, result, personSex == 'F' ? 2 : 1);
        _configuration.SetIndiData(result, ParserEventType.evt_Sex, personSex.ToString(CultureInfo.InvariantCulture));
        if (localAka != string.Empty)
        {
            _configuration.SetIndiName(result, 3, localAka.Trim());
        }

        return result;
    }
}
