using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static FBParser.PascalCompat;

namespace FBParser;

/// <summary>
/// Parses genealogical free-text family-book entries and emits structured callbacks for persons, families,
/// places, dates, references, and additional facts.
/// </summary>
/// <remarks>
/// This class is a direct C# port of the original Pascal <c>TFBEntryParser</c>. The implementation intentionally
/// preserves the original state-machine oriented parsing strategy and most diagnostics so that existing behavior
/// stays compatible while the code becomes usable from .NET.
/// </remarks>
public sealed class FBEntryParser : ParserBase, IDisposable
{
    /// <summary>
    /// Represents a generic parse callback.
    /// </summary>
    /// <param name="sender">The parser instance.</param>
    /// <param name="text">The parsed text payload.</param>
    /// <param name="reference">The related family or individual reference.</param>
    /// <param name="subtype">The subtype or event code.</param>
    public delegate void ParseEvent(object sender, string text, string reference, int subtype);

    /// <summary>
    /// Represents a detailed parse message callback.
    /// </summary>
    /// <param name="sender">The parser instance.</param>
    /// <param name="kind">The message classification.</param>
    /// <param name="message">The diagnostic message.</param>
    /// <param name="reference">The current main reference.</param>
    /// <param name="mode">The current parser mode.</param>
    public delegate void ParseMessageEvent(object sender, ParseMessageKind kind, string message, string reference, int mode);

    private const string CsMarriageEntr = "⚭";
    private const string CsMarriageEntr2 = "oo";
    private const string CsMarriageEntr3 = "∞";
    private const string CsMarriageGc = "Ehe:";
    private const string CsSeparatorGc = "●";
    private const string CsSeparator = ",";
    private const string CsSeparator2 = "‚";
    private const string CsDeathEntr = "†";
    private const string CsDeathEntr2 = "+";
    private const string CsDeathGefEntr = "gefallen";
    private const string CsDeathVermEntr = "vermisst";
    private const string CsDeathVermEntr2 = "vermißt";
    private const string CsIllegChild = "o-o";
    private const string CsIllegChild2 = "U";
    private const string CsIllegChild3 = "o~o";
    private const string CsProtectSpace = " ";
    private const string CsDivorce = "o/o";
    private const string CsBirth = "*";
    private const string CsBaptism = "*)";
    private const string CsBaptism2 = "~";
    private const string CsBurial = "†)";
    private const string CsBurial2 = "=";
    private const string CsSpouseKn = "u.";
    private const string CsSpouseKn2 = "und";
    private const string CsMaidenNameKn = "geb.";
    private const string CsMarrPartKn = "mit";
    private const string CsReferenceGc = "PN =";
    private const string CsAdditional = "Lebensphasen";
    private const string CsResidence = "lebte";
    private const string CsResidence2 = "leb";
    private const string CsResidence3 = "wohnte";
    private const string CsResidence4 = "wohnhaft";
    private const string CsResidence5 = "wohnt";
    private const string CsResidence6 = "Herkunft";
    private const string CsEmigration = "ausgewandert";
    private const string CsEmigration2 = "wanderte";
    private const string CsAge = "alt";
    private const string CsPlaceKenn = "in";
    private const string CsPlaceKenn2 = "aus";
    private const string CsPlaceKenn3 = "nach";
    private const string CsPlaceKenn4 = "am";
    private const string CsPlaceKenn5 = "bei";
    private const string CsPlaceKenn6 = "im";
    private const string CsPlaceKenn7 = "auf der";
    private const string CsKath = "rk.";
    private const string CsKath2 = "kath.";
    private const string CsEvang = "ev.";
    private const string CsEvang2 = "evang.";
    private const string CsReform = "ref.";
    private const string CsReform2 = "reform.";
    private const string CsLuth = "luth.";
    private const string CsUnknown = "…";
    private const string CsUnknown2 = "...";
    private const string CsUnknown3 = "..";
    private const string CsTwin = "Zw";
    private const string CsDoktor = "Dr.";
    private const string CsPfarrer = "Pfarrer";
    private const string CsDoktorTheol = "Dr. theol.";
    private const string CsDoktorMed = "Dr. med.";
    private const string CsDoktorRn = "Dr. rer. nat.";
    private const string CsProf = "Prof. Dr.";
    private const string CsProfDoktor = "Prof.";
    private const string CsGehRat = "Geheimrat";
    private const string CsDateModif4 = "seit";
    private const string CsBaute = "baute";
    private const string CsKaufte = "kaufte";
    private const string CsErwarb = "erwarb";
    private const string CsStrAbb = "str.";
    private const string CsSiedlAbb = "siedl";
    private const string CsStrasse = "straße";
    private const string CsGasse = "gasse";
    private const string CsWeg = "weg";
    private const string CsAllee = "allee";
    private const string CsPlatz = "platz";
    private const string CsPfad = "pfad";
    private const string CsLedig = "ledig";
    private const string CsLedigAbb = "led.";
    private const string CsWitwe = "Witwe";
    private const string CsWitwer = "Witwer";
    private const string CsTypeQuota = "„";
    private const string CsTypeQuota2 = "“";
    private const string CsGenannt = "genannt";
    private const string CsWurde = "wurde";
    private const string CsArtikelB1 = "der";
    private const string CsArtikelB2 = "die";
    private const string CsArtikelB3 = "das";
    private const string CsArtikelU1 = "ein";
    private const string CsArtikelU2 = "eine";
    private const bool CfgLearnUnknown = true;

    private static readonly string[] CUmLauts = ["ä", "ö", "ü", "Ä", "Ö", "Ü", "ß", "é"];
    private static readonly string[] CUmLautsU = ["Ä", "Ö", "Ü"];
    private static readonly string[] CHyphens = ["-", "­", "‑"];
    private static readonly string[] CTitel = ["Graf", "Erbgraf", "Gräfin", "Baron", "Baronin", "Prinz", "Prinzessin", "Junker", "Freiherr", "Freiin"];
    private static readonly string[] CDateModif = ["ca", "um", "ab", "von", "vor", "nach", "err.", CsDateModif4, "zwischen", "frühestens", "spätestens"];
    private static readonly string[] CAkkaTitle = [CsDoktorMed, CsDoktorRn, CsDoktorTheol, CsPfarrer, CsGehRat, CsProfDoktor, CsProf, CsDoktor];
    private static readonly string[] CReligions = [CsKath, CsKath2, CsEvang, CsEvang2, CsReform, CsReform2, CsLuth];
    private static readonly string[] CMarriageKn = [CsMarriageEntr, CsMarriageEntr2, CsMarriageEntr3];
    private static readonly string[] CResidenceKn = [CsResidence, CsResidence2, CsResidence3, CsResidence4, CsResidence5, CsResidence6];
    private static readonly string[] CPlaceKn = [CsPlaceKenn, CsPlaceKenn2, CsPlaceKenn3, CsPlaceKenn4, CsPlaceKenn5, CsPlaceKenn6, CsPlaceKenn7];
    private static readonly string[] CUnknownKn = [CsUnknown, CsUnknown2, CsUnknown3];
    private static readonly string[] CEmmigratKn = [CsEmigration, CsEmigration2];
    private static readonly string[] CPropertyKn = [CsBaute, CsKaufte, CsErwarb];
    private static readonly string[] CAddressKn = [CsStrAbb, CsSiedlAbb, CsStrasse, CsGasse, CsWeg, CsPlatz, CsPfad];
    private static readonly string[] CDescriptKn = [CsLedig, CsWitwer, CsWitwe];
    private static readonly string[] CMonthKn = ["", "Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"];
    private static readonly string[] CArtikelB = [CsArtikelB1, CsArtikelB2, CsArtikelB3];
    private static readonly string[] CLedigKn = [$"{CsLedig}er", $"{CsLedig}e", CsLedigAbb];
    private static readonly string[] CArtikelU = [CsArtikelU2, CsArtikelU1];

    private string _defaultPlace = string.Empty;
    private string _lastErr = string.Empty;
    private string _mainRef = string.Empty;
    private int _mode;
    private string[] _umlauts;
    private string[] _akkaTitel;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FBEntryParser"/> class.
    /// </summary>
    public FBEntryParser()
    {
        GNameHandler = new GNameHandler();
        GNameHandler.Init();
        GNameHandler.OnError = GNameError;
        _umlauts = CUmLauts;
        _akkaTitel = CAkkaTitle;
    }

    /// <summary>
    /// Occurs when a family starts.
    /// </summary>
    public event ParseEvent? OnStartFamily;

    /// <summary>
    /// Occurs when an entry ends.
    /// </summary>
    public event ParseEvent? OnEntryEnd;

    /// <summary>
    /// Occurs when a family date is parsed.
    /// </summary>
    public event ParseEvent? OnFamilyDate;

    /// <summary>
    /// Occurs when a family type is parsed.
    /// </summary>
    public event ParseEvent? OnFamilyType;

    /// <summary>
    /// Occurs when family free-text data is parsed.
    /// </summary>
    public event ParseEvent? OnFamilyData;

    /// <summary>
    /// Occurs when a family place is parsed.
    /// </summary>
    public event ParseEvent? OnFamilyPlace;

    /// <summary>
    /// Occurs when an individual is assigned to a family.
    /// </summary>
    public event ParseEvent? OnFamilyIndiv;

    /// <summary>
    /// Occurs when an individual name is parsed.
    /// </summary>
    public event ParseEvent? OnIndiName;

    /// <summary>
    /// Occurs when an individual date is parsed.
    /// </summary>
    public event ParseEvent? OnIndiDate;

    /// <summary>
    /// Occurs when an individual place is parsed.
    /// </summary>
    public event ParseEvent? OnIndiPlace;

    /// <summary>
    /// Occurs when an individual occupation is parsed.
    /// </summary>
    public event ParseEvent? OnIndiOccu;

    /// <summary>
    /// Occurs when an individual relation is parsed.
    /// </summary>
    public event ParseEvent? OnIndiRel;

    /// <summary>
    /// Occurs when an individual reference is parsed.
    /// </summary>
    public event ParseEvent? OnIndiRef;

    /// <summary>
    /// Occurs when generic individual data is parsed.
    /// </summary>
    public event ParseEvent? OnIndiData;

    /// <summary>
    /// Occurs as a coarse parser error callback when no detailed message event is assigned.
    /// </summary>
    public event EventHandler? OnParseError;

    /// <summary>
    /// Occurs for detailed parser messages.
    /// </summary>
    public event ParseMessageEvent? OnParseMessage;

    /// <summary>
    /// Gets the given-name handler used to infer sex from given names.
    /// </summary>
    public GNameHandler GNameHandler { get; }

    /// <summary>
    /// Gets the last parser message.
    /// </summary>
    public string LastErr => _lastErr;

    /// <summary>
    /// Gets the current parser mode.
    /// </summary>
    public int LastMode => _mode;

    /// <summary>
    /// Gets the current main family reference.
    /// </summary>
    public string MainRef => _mainRef;

    /// <summary>
    /// Gets or sets the default place used when no explicit place was detected.
    /// </summary>
    public string DefaultPlace
    {
        get => _defaultPlace;
        set => _defaultPlace = value;
    }

    /// <summary>
    /// Disposes the parser and finalizes the given-name handler.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        GNameHandler.Done();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Parses the supplied text.
    /// </summary>
    /// <param name="data">The entry text.</param>
    public void Parse(string data) => Feed(data);

    /// <inheritdoc />
    public override void Feed(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var localOffset = 1;
        var localMode = 0;
        _mode = 0;
        var localMainFamRef = string.Empty;
        _mainRef = string.Empty;
        var localPlace = string.Empty;
        var localData = string.Empty;
        var localEventDate = string.Empty;
        var localSubstring = string.Empty;
        var localFamName = string.Empty;
        var localZiffCount = 0;
        var localCharCount = 0;
        var localChildCount = 0;
        var localRetMode2 = 0;
        var localFirstCycle = true;
        var localFirstEntry = true;
        var localAka = string.Empty;
        var localLastZiffCount = 0;
        var localFamDatFlag = false;
        var localEntryEndFlag = false;
        var localEntryType = ParserEventType.evt_Anull;
        var localIndId = string.Empty;
        var localParentRef = string.Empty;
        var localLastName = string.Empty;
        var localChRef = string.Empty;
        var localPersonName = string.Empty;
        var localIndId2 = string.Empty;
        var localFamCEntry = string.Empty;
        var localDate = string.Empty;
        var localChildFam = string.Empty;
        var localFamRef = string.Empty;
        var localPersonGName = string.Empty;
        var localAdditional = string.Empty;
        var localDefaultBirthPlace = string.Empty;
        var localD = string.Empty;
        var localD2 = string.Empty;
        var localLastName2 = string.Empty;
        var localEntry = string.Empty;
        var localPos = 0;
        var localFamType = 0;
        var localPp = 0;
        var localRefMode2 = 0;
        var localTest = 0;
        var localRetMode3 = 0;
        var localFound = 0;
        var localSPos = 0;
        var localSpouseCount = 0;
        var localPlaceFlag = false;
        var localSecondEntry = false;
        var localParDeathFlag = false;
        var localOtherMarrFlag = false;
        var localVerwFlag = false;
        var localMotherName = false;
        var localEntryType2 = ParserEventType.evt_Anull;
        var localPersonType = 'U';
        var localPersonSex = 'U';
        var localPersonSex2 = 'U';
        var localPersonType2 = 'U';
        var localInt = 0L;
        var localStartOffset = 0L;
        var localRetMode = 0;
        var localAddEvent = ParserEventType.evt_Anull;

        bool BuildName(string innerText, ref int innerOffset, ref string innerSubstring, ref string innerData)
        {
            var result = false;
            if (BuildName2(innerText, ref innerOffset, ref localCharCount, ref innerSubstring, out var additional))
            {
                if (additional == CsTwin)
                {
                    innerData = innerData == string.Empty ? "Zwilling" : innerData + "; Zwilling";
                }
                else if (additional != string.Empty)
                {
                    localAka = additional;
                    localAddEvent = ParserEventType.evt_AKA;
                    localCharCount = 0;
                }
            }
            else
            {
                result = innerSubstring.Trim().Length > 0;
            }

            return result;
        }

        bool BuildData(string individualId, string innerText, ref int innerOffset, ref string innerSubstring, ref string innerData)
        {
            var result = false;
            var currentChar = CharAt(innerText, innerOffset);
            if (!new[] { '<', ';', ':', '.', ',', '(', '>', '\n', '\r', '*', CsDeathEntr[0], '­' }.Contains(currentChar))
            {
                innerSubstring += currentChar;
            }
            else if (innerOffset < innerText.Length - 1 && currentChar == '.' &&
                ((innerSubstring != string.Empty && innerOffset + 1 <= innerText.Length && In(CharAt(innerText, innerOffset + 1), Ziffern))
                || (innerSubstring != string.Empty && !In(innerSubstring[^1], Ziffern) && innerOffset + 2 <= innerText.Length && CharAt(innerText, innerOffset + 1) == ' ' && In(CharAt(innerText, innerOffset + 2), Charset))
                || (innerOffset + 1 <= innerText.Length && new[] { '-', ',', '/' }.Contains(CharAt(innerText, innerOffset + 1)))
                || TestFor(innerText, innerOffset + 1, CsSeparator2)
                || (innerOffset + 1 <= innerText.Length && In(CharAt(innerText, innerOffset + 1), Charset))
                || (innerOffset + 1 <= innerText.Length && CharAt(innerText, innerOffset + 1) == ' ' && (innerSubstring.EndsWith("Dr", StringComparison.Ordinal) || innerSubstring.EndsWith("rl", StringComparison.Ordinal) || innerSubstring.EndsWith("Nr", StringComparison.Ordinal) || innerSubstring.EndsWith("gl", StringComparison.Ordinal) || innerSubstring.EndsWith("tr", StringComparison.Ordinal) || innerSubstring.EndsWith("Kr", StringComparison.Ordinal) || innerSubstring.EndsWith("Kt", StringComparison.Ordinal)))
                || (innerOffset + 1 <= innerText.Length && CharAt(innerText, innerOffset + 1) == '.')
                || innerSubstring.EndsWith(".", StringComparison.Ordinal)
                || (innerOffset + 2 <= innerText.Length && CharAt(innerText, innerOffset + 1) == ' ' && !new[] { '\n', '\r' }.Contains(CharAt(innerText, innerOffset + 2)) && localLastZiffCount < 3)))
            {
                innerSubstring += currentChar;
            }
            else if (TestFor(innerText, innerOffset, CUnknownKn, out var unknownFound))
            {
                innerSubstring += CUnknownKn[unknownFound];
                innerOffset += CUnknownKn[unknownFound].Length - 1;
            }
            else if (currentChar == ',' && innerOffset > 1 && In(CharAt(innerText, innerOffset - 1), Ziffern) && innerOffset + 2 <= innerText.Length && ((In(CharAt(innerText, innerOffset + 1), Ziffern) && In(CharAt(innerText, innerOffset + 2), Ziffern)) || (CharAt(innerText, innerOffset + 1) == ' ' && In(CharAt(innerText, innerOffset + 2), Ziffern) && localLastZiffCount < 3)))
            {
                innerSubstring += '.';
                Warning(this, "Misspelled Date");
            }
            else if (TestFor(innerText, innerOffset, [CsSeparator2, CsSeparator], out var separatorFound)
                && !innerSubstring.Contains(' ')
                && TestFor(innerText, innerOffset + 1, [" Kr.", " Kreis", " Kt.", " Kanton"]))
            {
                innerSubstring += CsSeparator;
                if (separatorFound == 0)
                {
                    innerOffset += CsSeparator2.Length - 1;
                }
            }
            else if (TestFor(innerText, innerOffset, [CsSeparator2, CsSeparator], out separatorFound)
                && innerOffset > 1 && innerOffset < innerText.Length && In(CharAt(innerText, innerOffset - 1), Ziffern) && In(CharAt(innerText, innerOffset + 1), Ziffern))
            {
                innerSubstring += CsSeparator;
                if (separatorFound == 0)
                {
                    innerOffset += CsSeparator2.Length - 1;
                }
            }
            else if (TestFor(innerText, innerOffset, ["-"], out var hyphenFound)
                && innerOffset > 1 && innerOffset + 1 + hyphenFound <= innerText.Length
                && In(CharAt(innerText, innerOffset - 1), LowerCharset)
                && In(CharAt(innerText, innerOffset + 1 + hyphenFound), UpperCharset))
            {
                innerSubstring += currentChar;
            }
            else if (TestFor(innerText, innerOffset, CHyphens, out hyphenFound)
                && innerOffset > 1 && innerOffset + CHyphens[hyphenFound].Length <= innerText.Length
                && In(CharAt(innerText, innerOffset - 1), LowerCharset)
                && In(CharAt(innerText, innerOffset + CHyphens[hyphenFound].Length), LowerCharsetErw))
            {
                if (hyphenFound == 0)
                {
                    Warning(this, "Hyphen in Data Ignored");
                }

                innerOffset += CHyphens[hyphenFound].Length - 1;
            }
            else if (TestFor(innerText, innerOffset, _umlauts, out var umlautFound))
            {
                innerSubstring += _umlauts[umlautFound];
                innerOffset += _umlauts[umlautFound].Length - 1;
            }
            else if (TestFor(innerText, innerOffset, "“"))
            {
                innerSubstring += "“";
            }
            else if (TestFor(innerText, innerOffset, [CsBirth, CsDeathEntr, CsDeathEntr2]))
            {
                if (innerSubstring.Trim().Length < 2 || (innerSubstring.Trim().Length < 4 && TestFor(innerSubstring, 1, [CsBirth, CsDeathEntr, CsDeathEntr2])))
                {
                    innerSubstring += currentChar;
                }
                else
                {
                    localEntryType = HandleNonPersonEntry(innerSubstring, individualId);
                    Error(this, ", Expected (End of Entry)");
                    innerSubstring = string.Empty;
                    innerOffset--;
                }
            }
            else if (currentChar == CsProtectSpace[0] && innerSubstring.Length < 5)
            {
                innerSubstring += currentChar;
            }
            else if (TestFor(innerText, innerOffset, "(") && ParseAdditional(innerText, ref innerOffset, out var parsedAdditional))
            {
                innerData = parsedAdditional;
                if (innerData.StartsWith(CsDivorce, StringComparison.Ordinal))
                {
                    innerData = RemoveStart(innerData, 3);
                    if (!localFamDatFlag && _mode == 8)
                    {
                        Error(this, "Wife entry not ended with .");
                    }

                    SetFamilyPlace(localMainFamRef, ParserEventType.evt_Divorce, _defaultPlace);
                    if (innerData != string.Empty)
                    {
                        SetFamilyData(localMainFamRef, ParserEventType.evt_Divorce, innerData);
                    }

                    innerData = string.Empty;
                }
                else if (innerData.EndsWith(" alt", StringComparison.Ordinal))
                {
                    SetIndiData(individualId, ParserEventType.evt_Age, innerData);
                    innerData = string.Empty;
                }
            }
            else if (currentChar == '.'
                || (new[] { '\n', '\r' }.Contains(currentChar) && innerOffset > 1 && CharAt(innerText, innerOffset - 1) == '.')
                || (new[] { '\n', '\r' }.Contains(currentChar) && innerOffset > 2 && CharAt(innerText, innerOffset - 1) == ' ' && CharAt(innerText, innerOffset - 2) == '.'))
            {
                if (_mode != 9)
                {
                    localFamDatFlag = true;
                    result = innerSubstring.Length > 1;
                }
                else
                {
                    localEntryEndFlag = true;
                    result = innerSubstring.Length > 1;
                }
            }
            else
            {
                result = innerSubstring.Length > 1;
            }

            return result;
        }

        while (localOffset <= text.Length)
        {
            Offset = localOffset;
            switch (localMode)
            {
                case -1:
                    break;
                case 0:
                    localSubstring = string.Empty;
                    if (In(CharAt(text, localOffset), AlphaNum))
                    {
                        localMode = 1;
                        localSubstring += CharAt(text, localOffset);
                    }
                    break;
                case 1:
                    if (In(CharAt(text, localOffset), WhiteSpaceChars.Concat([':'])))
                    {
                        localMode = 2;
                        localPp = LastIndexOfAny(localSubstring, "0123456789ab".ToCharArray());
                        if (localPp > 0)
                        {
                            localOffset -= localSubstring.Length - localPp - 1;
                            localSubstring = Left(localSubstring, localPp + 1);
                        }

                        if (localSubstring == string.Empty || !In(localSubstring[0], Ziffern) || localSubstring.Contains('.', StringComparison.Ordinal))
                        {
                            Error(this, $"Wrong Family reference, \"{Copy(text, 1, 20)}\"");
                            localMainFamRef = string.Empty;
                            localMode = -1;
                        }
                        else if (_mainRef == string.Empty)
                        {
                            StartFamily(localSubstring);
                            localMainFamRef = localSubstring;
                            _mainRef = localMainFamRef;
                        }
                    }
                    else
                    {
                        localSubstring += CharAt(text, localOffset);
                    }

                    break;
                case 2:
                    if (localOffset > 2
                        && CharAt(text, localOffset - 1) == ' '
                        && In(CharAt(text, localOffset - 2), Ziffern)
                        && !TestFor(text, localOffset, CsMarriageGc)
                        && !TestFor(text, localOffset, CsIllegChild)
                        && ((In(CharAt(text, localOffset), UpperCharsetErw) && text.IndexOf(' ', localOffset - 1) > text.IndexOf(',', localOffset - 1))
                            || (CharAt(text, localOffset) == 'v' && text.IndexOf(' ', localOffset + 3) > text.IndexOf(',', localOffset + 3))))
                    {
                        localMode = 110;
                        localSubstring = string.Empty;
                        localOffset--;
                        localFamName = string.Empty;
                    }
                    else
                    {
                        localSubstring = string.Empty;
                        if (!In(CharAt(text, localOffset), WhiteSpaceChars))
                        {
                            localMode = 3;
                            localFamName = string.Empty;
                            localPersonType = 'U';
                            localOffset--;
                        }
                    }

                    break;
                case 3:
                    localMotherName = false;
                    if ((In(CharAt(text, localOffset), UpperCharset) || TestFor(text, localOffset, _umlauts)) && !TestFor(text, localOffset, CsMarriageGc))
                    {
                        SetFamilyType(localMainFamRef, 2);
                        localEntryType = ParserEventType.evt_Residence;
                        localMode = 5;
                        localOffset--;
                    }
                    else if (In(CharAt(text, localOffset), WhiteSpaceChars.Concat(['.', ':'])) || localSubstring.Length > 3)
                    {
                        if (TestFor(localSubstring, 1, [CsMarriageEntr, CsMarriageEntr3, CsMarriageEntr2]))
                        {
                            localMode = 10;
                            SetFamilyType(localMainFamRef, 1);
                            localSubstring = string.Empty;
                            if (new[] { '.', ':' }.Contains(CharAt(text, localOffset)))
                            {
                                localOffset--;
                            }

                            localEntryType = ParserEventType.evt_Marriage;
                            localPersonType = 'M';
                        }
                        else if (localSubstring + CharAt(text, localOffset) == CsMarriageGc)
                        {
                            localMode = 101;
                            localRetMode = 100;
                            localEntryType = ParserEventType.evt_Marriage;
                            localFamRef = localMainFamRef;
                            localFamCEntry = string.Empty;
                            localFamType = 1;
                            if (text.Length > localOffset + 4)
                            {
                                if (CharAt(text, localOffset + 2) == CsIllegChild2[0])
                                {
                                    localFamType = 2;
                                    localOffset += 2;
                                }
                                else if (CharAt(text, localOffset + 3) == '/')
                                {
                                    localFamCEntry = Copy(text, localOffset + 2, 3);
                                    localOffset += 4;
                                }
                            }

                            SetFamilyType(localMainFamRef, localFamType, localFamCEntry);
                            localSubstring = string.Empty;
                        }
                        else if (TestFor(localSubstring, 1, [CsDeathEntr, CsDeathEntr2]))
                        {
                            localMode = 20;
                            localEntryType = ParserEventType.evt_Death;
                            SetFamilyType(localMainFamRef, 2);
                            localSubstring = string.Empty;
                        }
                        else if (TestFor(localSubstring, 1, CsBirth))
                        {
                            localMode = 25;
                            localEntryType = ParserEventType.evt_Birth;
                            SetFamilyType(localMainFamRef, 2);
                            localSubstring = string.Empty;
                        }
                        else if (TestFor(localSubstring, 1, CsIllegChild))
                        {
                            localMode = 30;
                            localEntryType = ParserEventType.evt_Birth;
                            SetFamilyType(localMainFamRef, 3);
                            localOffset--;
                            localSubstring = string.Empty;
                        }
                        else if (TestFor(localSubstring, 1, CsIllegChild3))
                        {
                            localMode = 30;
                            localEntryType = ParserEventType.evt_Birth;
                            localMotherName = true;
                            SetFamilyType(localMainFamRef, 3);
                            localOffset--;
                            localSubstring = string.Empty;
                        }
                        else
                        {
                            localEntryType = ParserEventType.evt_Residence;
                            localOffset--;
                            localMode = 40;
                        }
                    }
                    else if (TestFor(text, localOffset, "fehlt"))
                    {
                        localMode = 4;
                    }
                    else
                    {
                        localSubstring += CharAt(text, localOffset);
                    }

                    break;
                case 4:
                    if (TestFor(text, localOffset, Environment.NewLine))
                    {
                        EndOfEntry(localMainFamRef);
                        localMode = 0;
                    }

                    break;
                case 5:
                case 7:
                    if (localFirstCycle)
                    {
                        localAka = string.Empty;
                    }

                    if (BuildName(text, ref localOffset, ref localSubstring, ref localData))
                    {
                        localIndId = HandleAKPersonEntry(localSubstring.Trim(), localMainFamRef, localPersonType, localMode, out localLastName, out localPersonSex, localAka);
                        if ((localMode == 5) ^ localMotherName)
                        {
                            localFamName = localLastName;
                        }

                        if (localMode == 5 && localEntryType != ParserEventType.evt_Marriage && localEntryType != ParserEventType.evt_Partner)
                        {
                            if (localPlace == string.Empty)
                            {
                                localPlace = _defaultPlace;
                            }

                            if (localData != string.Empty)
                            {
                                SetIndiData(localIndId, localEntryType, localData);
                            }

                            if (localEventDate != string.Empty)
                            {
                                SetIndiDate(localIndId, localEntryType, localEventDate);
                            }

                            if (localPlace != string.Empty)
                            {
                                SetIndiPlace(localIndId, localEntryType, localPlace);
                            }
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '<')
                        {
                            localRetMode = localMode + 1;
                            localFamDatFlag = false;
                            localMode = 50;
                        }
                        else
                        {
                            localMode += 1;
                            localSubstring = string.Empty;
                            localAdditional = string.Empty;
                            localFamDatFlag = CharAt(text, localOffset) != ',';
                            localData = string.Empty;
                        }

                        localOffset--;
                    }

                    break;
                case 6:
                case 8:
                    if (In(CharAt(text, localOffset), Ziffern))
                    {
                        localZiffCount++;
                    }
                    else
                    {
                        if (localZiffCount > 0)
                        {
                            localLastZiffCount = localZiffCount;
                        }

                        localZiffCount = 0;
                    }

                    if (BuildData(localIndId, text, ref localOffset, ref localSubstring, ref localData))
                    {
                        if (Right(localSubstring.Trim(), 4) == " alt" && localSubstring.Trim().Length > 0 && In(localSubstring.Trim()[0], Ziffern) && localEntryType == ParserEventType.evt_Death)
                        {
                            SetIndiData(localIndId, localEntryType, localSubstring.Trim());
                            localSubstring = string.Empty;
                            continue;
                        }

                        localEntryType = HandleNonPersonEntry(localSubstring, localIndId);
                        localSubstring = string.Empty;
                        localLastZiffCount = 0;
                        if (localData != string.Empty)
                        {
                            SetIndiData(localIndId, localEntryType, localData.Trim());
                            localData = string.Empty;
                        }

                        if (!new[] { '.', ',', '\n', '\r', '<', '(' }.Contains(CharAt(text, localOffset)))
                        {
                            Error(this, ", missing (End of Entry)");
                            localOffset--;
                        }
                    }

                    if (CharAt(text, localOffset) == '<')
                    {
                        localRetMode = localMode;
                        localMode = 50;
                        localOffset--;
                    }
                    else if (_mode == 6 && In(CharAt(text, localOffset), WhiteSpaceChars) && TestFor(text, localOffset + 1, [CsSpouseKn + " ", CsSpouseKn2 + " ", "u, "], out localFound) && ((new[] { ',', '\n', '\r' }.Contains(CharAt(text, localOffset - 1))) || localFound == 0))
                    {
                        if (localFound == 1)
                        {
                            Warning(this, "\"und\" as Wife-Flag");
                        }

                        if (localFound == 2)
                        {
                            Warning(this, "\"u,\" as Wife-Flag");
                        }

                        localSubstring = localSubstring.Trim();
                        if (localSubstring != string.Empty)
                        {
                            HandleNonPersonEntry(localSubstring, localIndId);
                            Error(this, ", missing (last entry)");
                        }

                        localMode = 7;
                        localPersonType = 'F';
                        localOffset += 2 + localFound;
                        localSubstring = string.Empty;
                    }
                    else if (localSubstring == string.Empty && CharAt(text, localOffset) == '.')
                    {
                        var tail = text.Substring(localOffset - 1, Math.Min(6, text.Length - localOffset + 1));
                        if (_mode == 6 && tail.Contains("u.", StringComparison.Ordinal))
                        {
                            Warning(this, "Husband entry ends with \".\"");
                        }
                        else if (tail.Contains(CsBirth, StringComparison.Ordinal) || tail.Contains(CsDeathEntr, StringComparison.Ordinal))
                        {
                            Warning(this, "Entry ends with \".\" instead of \",\"");
                        }
                        else if (localMode == 6 && !text.Substring(localOffset - 1, Math.Min(5, text.Length - localOffset + 1)).Contains(Environment.NewLine, StringComparison.Ordinal))
                        {
                            Warning(this, "Entry ends with \".\" without Linefeed");
                        }
                        else
                        {
                            localMode = 12;
                        }
                    }
                    else if (new[] { 'l', ')' }.Concat(Ziffern).Contains(CharAt(text, localOffset)) && TestFor(text, localOffset + 1, [" Kd", "Kd"], out localFound))
                    {
                        if (!localFamDatFlag)
                        {
                            Error(this, _mode == 8 ? "Wife entry not ended with ." : "Person entry not ended with .");
                        }

                        if (CharAt(text, localOffset) == 'l')
                        {
                            Error(this, "l misplaced as 1");
                        }

                        localSubstring = localSubstring.Trim();
                        if (GetEntryType(localSubstring, out localD, out localD2) != ParserEventType.evt_Last)
                        {
                            HandleNonPersonEntry(localSubstring, localIndId);
                        }

                        localMode = 15;
                    }

                    break;
                case 10:
                    if (In(CharAt(text, localOffset), Ziffern.Concat(['.', '/'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == ' '
                        && ((localOffset + 1 <= text.Length
                                && In(CharAt(text, localOffset + 1), Ziffern)
                                && (TestFor(localSubstring, 1, CUnknownKn) || TestFor(localSubstring, 1, CDateModif)))
                            || TestFor(text, localOffset + 1, CDateModif, out localFound)))
                    {
                        localSubstring += ' ';
                    }
                    else if (TestFor(text, localOffset, CDateModif, out localFound))
                    {
                        localPlace = localSubstring.Trim();
                        localSubstring = CDateModif[localFound];
                        localOffset += CDateModif[localFound].Length - 1;
                    }
                    else if (TestFor(text, localOffset, CUnknownKn, out localFound))
                    {
                        localSubstring += CUnknownKn[localFound];
                        localOffset += CUnknownKn[localFound].Length - 1;
                    }
                    else
                    {
                        if (localSubstring.Length > 1)
                        {
                            localEventDate = localSubstring;
                            SetFamilyDate(localMainFamRef, ParserEventType.evt_Marriage, localSubstring);
                            localSubstring = string.Empty;
                            if (localPlace != string.Empty)
                            {
                                SetFamilyPlace(localMainFamRef, ParserEventType.evt_Marriage, localPlace);
                            }
                            else
                            {
                                localMode = 11;
                            }
                        }

                        if (CharAt(text, localOffset) == ':')
                        {
                            if (_defaultPlace != string.Empty && localPlace == string.Empty)
                            {
                                SetFamilyPlace(localMainFamRef, ParserEventType.evt_Marriage, _defaultPlace);
                            }

                            localMode = 5;
                        }
                    }

                    break;
                case 11:
                    if (localSubstring == string.Empty
                        && (!text.Contains(':') || (text.Contains("Kd", StringComparison.Ordinal) && text.IndexOf(':') > text.IndexOf("Kd", StringComparison.Ordinal))))
                    {
                        Error(this, ": missing");
                        localMode = 5;
                        continue;
                    }

                    if (BuildData(localIndId, text, ref localOffset, ref localSubstring, ref localData))
                    {
                        if (localEntryType == ParserEventType.evt_Marriage)
                        {
                            SetFamilyPlace(localMainFamRef, localEntryType, localSubstring);
                        }
                        else
                        {
                            localPlace = localSubstring;
                        }

                        localSubstring = string.Empty;
                        if (localData != string.Empty)
                        {
                            if (localEntryType == ParserEventType.evt_Marriage)
                            {
                                SetFamilyData(localMainFamRef, localEntryType, localData.Trim());
                                localData = string.Empty;
                            }
                            else
                            {
                                localAddEvent = localEntryType;
                            }

                            localAdditional = string.Empty;
                            localSubstring = string.Empty;
                        }
                    }

                    if (CharAt(text, localOffset) == ':')
                    {
                        localSubstring = string.Empty;
                        localMode = 5;
                    }

                    break;
                case 100:
                    if (TestFor(text, localOffset, CsSeparatorGc))
                    {
                        localOffset += CsSeparatorGc.Length - 1;
                        localSubstring = string.Empty;
                        localFirstEntry = true;
                        if (localEntryType == ParserEventType.evt_Marriage)
                        {
                            localTest = CountChar(text, CsSeparatorGc[0]);
                            localPersonType = (localFamType == 1 || localTest == 3) ? 'M' : 'F';
                            localMode = 110;
                        }
                        else if (Copy(text, localOffset + 2, 7) == "Kinder:" || Copy(text, localOffset + 2, 5) == "Kind:")
                        {
                            localPersonType = 'C';
                            localChildCount = 0;
                            localMode = 120;
                        }
                        else
                        {
                            localPersonType = 'F';
                            localMode = 110;
                        }
                    }

                    break;
                case 101:
                    if (In(CharAt(text, localOffset), Ziffern))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == ' '
                        && localSubstring != string.Empty
                        && localOffset + 1 <= text.Length
                        && In(CharAt(text, localOffset + 1), Ziffern))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && ((localOffset + 1 <= text.Length && In(CharAt(text, localOffset + 1), Ziffern.Concat(['.'])))
                            || (localOffset > 1 && In(CharAt(text, localOffset - 1), Charset.Concat(['.'])))))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '('
                        && ParseAdditional(text, ref localOffset, out localAdditional))
                    {
                        if (localSubstring.Trim() == string.Empty)
                        {
                            localSubstring = "(" + localAdditional + ")";
                        }
                        else if (localEntryType == ParserEventType.evt_Marriage)
                        {
                            SetFamilyData(this.MainRef == string.Empty ? localFamRef : localFamRef, localEntryType, localAdditional);
                        }
                        else
                        {
                            SetIndiData(localIndId, localEntryType, localAdditional);
                        }
                    }
                    else if (localSubstring == string.Empty && TestFor(text, localOffset, CDateModif, out localFound))
                    {
                        localOffset += CDateModif[localFound].Length;
                        localSubstring += CDateModif[localFound];
                        if (localOffset <= text.Length && new[] { ' ', '.' }.Contains(CharAt(text, localOffset)))
                        {
                            localSubstring += CharAt(text, localOffset);
                        }
                        else
                        {
                            localOffset--;
                        }
                    }
                    else
                    {
                        if (localSubstring.Length > 2)
                        {
                            if (localEntryType == ParserEventType.evt_Marriage)
                            {
                                localEventDate = localSubstring;
                                SetFamilyDate(localFamRef, localEntryType, localSubstring);
                            }
                            else
                            {
                                SetIndiDate(localIndId, localEntryType, localSubstring);
                            }

                            localSubstring = string.Empty;
                            localMode = 102;
                            localPlaceFlag = false;
                        }
                        else if (In(CharAt(text, localOffset), Charset))
                        {
                            localSubstring += CharAt(text, localOffset);
                        }
                    }

                    if (TestFor(text, localOffset, CsSeparatorGc)
                        || CharAt(text, localOffset) == CsSeparator[0]
                        || (CharAt(text, localOffset) == '.' && localMode == 102))
                    {
                        localMode = localRetMode;
                        localOffset--;
                    }

                    break;
                case 102:
                    if (!new[] { '<', '[', '.', '(', CsSeparator[0] }.Contains(CharAt(text, localOffset))
                        && !TestFor(text, localOffset, [CsSeparatorGc, VbNewLine, " " + CsMarrPartKn + " "]))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && (!TestFor(text, localOffset + 1, [" ", "\t", VbNewLine, "PN"])
                            || ((localSubstring.Length < 4) && (localSubstring != string.Empty) && In(localSubstring[0], UpperCharset))))
                    {
                        localSubstring += '.';
                    }
                    else
                    {
                        if (localPlaceFlag && localSubstring.Length > 2)
                        {
                            if (localEntryType == ParserEventType.evt_Marriage)
                            {
                                SetFamilyPlace(localFamRef, localEntryType, localSubstring.Trim());
                            }
                            else
                            {
                                SetIndiPlace(localIndId, localEntryType, localSubstring.Trim());
                            }

                            localSubstring = string.Empty;
                        }

                        if (CharAt(text, localOffset) == '('
                            && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            if (localEntryType != ParserEventType.evt_Marriage)
                            {
                                SetIndiData(localIndId, localEntryType, localAdditional.Trim());
                            }

                            localSubstring = string.Empty;
                        }
                        else if (new[] { CsSeparator[0], '<', '[', '.' }.Contains(CharAt(text, localOffset))
                            || TestFor(text, localOffset, [CsSeparatorGc, VbNewLine, " " + CsMarrPartKn + " "]))
                        {
                            localMode = localRetMode;
                            localSubstring = string.Empty;
                            localOffset--;
                        }
                    }

                    if (localSubstring == "in ")
                    {
                        localSubstring = string.Empty;
                        localPlaceFlag = true;
                    }

                    break;
                case 12:
                    if (localFirstCycle)
                    {
                        localSubstring = string.Empty;
                    }

                    if ((new[] { 'l', ')' }.Concat(Ziffern).Contains(CharAt(text, localOffset))
                            || long.TryParse(localData, out _))
                        && TestFor(text, localOffset + 1, new[] { " Kd", "Kd" }))
                    {
                        localMode = 15;
                    }
                    else if (!new[] { '(', '.', '\n', '\r' }.Contains(CharAt(text, localOffset)))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && !(localOffset + 1 <= text.Length && new[] { '\n', '\r' }.Contains(CharAt(text, localOffset + 1))))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, "(") && ParseAdditional(text, ref localOffset, out localAdditional))
                    {
                        localData = localAdditional;
                        localAdditional = string.Empty;
                        if (localData.StartsWith(CsDivorce, StringComparison.Ordinal))
                        {
                            localData = RemoveStart(localData, 3);
                            SetFamilyPlace(localMainFamRef, ParserEventType.evt_Divorce, _defaultPlace);
                            if (localData != string.Empty)
                            {
                                SetFamilyData(localMainFamRef, ParserEventType.evt_Divorce, localData);
                            }

                            localData = string.Empty;
                        }
                    }
                    else
                    {
                        if (localSubstring.Trim() != string.Empty)
                        {
                            HandleFamilyFact(localMainFamRef, localSubstring);
                        }

                        localSubstring = string.Empty;
                    }

                    break;
                case 15:
                    localPersonType = 'U';
                    localSubstring = string.Empty;
                    localDefaultBirthPlace = string.Empty;
                    localOffset += 3;
                    while (localOffset <= text.Length && new[] { '.', ':', 'r' }.Contains(CharAt(text, localOffset)))
                    {
                        localOffset++;
                    }

                    if (localOffset + 1 <= text.Length && CharAt(text, localOffset + 1) == '(')
                    {
                        localOffset++;
                    }

                    if (localOffset <= text.Length && CharAt(text, localOffset) == '('
                        && ParseAdditional(text, ref localOffset, out localAdditional))
                    {
                        if (localAdditional.StartsWith(CsBirth, StringComparison.Ordinal))
                        {
                            localAdditional = RemoveStart(localAdditional, 1).Trim();
                            if (localAdditional.StartsWith(CsPlaceKenn + " ", StringComparison.Ordinal))
                            {
                                localAdditional = RemoveStart(localAdditional, 3);
                            }

                            localDefaultBirthPlace = localAdditional;
                            localAdditional = string.Empty;
                        }

                        localOffset += 2;
                    }

                    if (localOffset > 1 && CharAt(text, localOffset - 1) != ':')
                    {
                        Warning(this, "Children Header does not end with \":\"");
                    }

                    localChildCount = 1;
                    localEntryEndFlag = false;
                    localFirstEntry = true;
                    localMode = 9;
                    break;
                case 40:
                    if (CharAt(text, localOffset) == ':')
                    {
                        localMode = 5;
                        localEntryType = ParserEventType.evt_Residence;
                    }

                    if (In(CharAt(text, localOffset), AlphaNum))
                    {
                        localOffset--;
                        localMode = 5;
                        localEntryType = ParserEventType.evt_Residence;
                    }

                    break;
                case 50:
                    localVerwFlag = false;
                    localSubstring = string.Empty;
                    if (TestFor(text, localOffset, CsPlaceKenn2 + " ")
                        && localOffset + 4 <= text.Length
                        && new[] { 'l' }.Concat(Ziffern).Contains(CharAt(text, localOffset + 4)))
                    {
                        localVerwFlag = true;
                        localMode = 51;
                    }
                    else if (localRetMode == 9 && new[] { 'l' }.Concat(Ziffern).Contains(CharAt(text, localOffset)))
                    {
                        localVerwFlag = true;
                        localMode = 52;
                        localOffset--;
                    }
                    else if (TestFor(text, localOffset, ["S.d.", "T.d.", "S.d,", "T.d,", "Kd.d.", "S(T).d."], out localFound))
                    {
                        localOffset += 2;
                        if (localFound > 3)
                        {
                            localOffset += localFound - 3;
                        }

                        localMode = 54;
                    }
                    else if (TestFor(text, localOffset, ["s.a.", "vgl."]))
                    {
                        localMode = localRetMode == 9 ? 51 : 52;
                        localVerwFlag = true;
                        localOffset += 3;
                    }
                    else if (TestFor(text, localOffset, [CsMarriageEntr, CsMarriageEntr2, CsMarriageEntr3], out localFound))
                    {
                        localMode = 53;
                        localOffset += localFound == 1 ? 1 : 2;
                        localOtherMarrFlag = localRetMode != 9;
                    }
                    else if (CharAt(text, localOffset) == '>')
                    {
                        var nextSlice = Copy(text, localOffset + 2, 30);
                        if (localOffset + 1 <= text.Length && CharAt(text, localOffset + 1) == ';'
                            && !nextSlice.Contains('<')
                            && nextSlice.Contains('>'))
                        {
                            Warning(this, "Double-closed reference (" + localRetMode + ")");
                        }
                        else
                        {
                            localMode = localRetMode;
                        }
                    }
                    else if (TestFor(text, localOffset, "),"))
                    {
                        localMode = localRetMode;
                        Error(this, "Reference Entry ends with \"),\"");
                    }
                    else if (new[] { '\n', '\r' }.Contains(CharAt(text, localOffset)))
                    {
                        Error(this, "unclosed reference");
                        localMode = localRetMode;
                        localOffset -= 2;
                    }

                    break;
                case 51:
                    if (new[] { 'l' }.Concat(Ziffern).Contains(CharAt(text, localOffset))
                        || (localSubstring.Length > 0 && CharAt(text, localOffset) is 'a' or 'b'))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (localSubstring.Length > 0)
                    {
                        if (!TestReferenz(localSubstring))
                        {
                            Error(this, "\"" + localSubstring + "\" invalid reference");
                        }
                        else
                        {
                            SetIndiRelat(localIndId, localSubstring, 1);
                        }

                        if (CharAt(text, localOffset) == '>')
                        {
                            localOffset--;
                        }

                        localMode = 50;
                    }

                    break;
                case 52:
                    localFound = 0;
                    if (new[] { 'l' }.Concat(Ziffern).Contains(CharAt(text, localOffset))
                        || (localSubstring.Length > 0 && CharAt(text, localOffset) is 'a' or 'b'))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (localSubstring.Length > 0)
                    {
                        if (!new[] { '>', ';', ',' }.Contains(CharAt(text, localOffset))
                            && !TestFor(text, localOffset, [" und", " korr."], out localFound))
                        {
                            Error(this, "\"" + localSubstring + CharAt(text, localOffset) + "\" invalid reference");
                        }
                        else if (!TestReferenz(localSubstring))
                        {
                            Error(this, "\"" + localSubstring + "\" invalid reference");
                        }
                        else
                        {
                            SetIndiRelat(localIndId, localSubstring, 2);
                        }

                        localSubstring = string.Empty;
                        localMode = 50;
                        if (CharAt(text, localOffset) == '>')
                        {
                            localOffset--;
                        }
                        else if (CharAt(text, localOffset) == ',' || TestFor(text, localOffset, [" und", " korr."], out localFound))
                        {
                            localMode = 52;
                        }
                    }

                    break;
                case 53:
                    if (localFirstCycle)
                    {
                        localSpouseCount = 0;
                    }

                    if (In(CharAt(text, localOffset), Ziffern)
                        || (localSubstring.Length > 0 && CharAt(text, localOffset) is 'a' or 'b'))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, [" s ", " s. ", " s, ", " S ", " S. ", " S, "], out localFound))
                    {
                        if (new[] { 0, 2, 3, 5 }.Contains(localFound))
                        {
                            Warning(this, ". missing after s");
                        }

                        localVerwFlag = true;
                        localOffset += " s".Length;
                        if (new[] { 1, 2, 4, 5 }.Contains(localFound))
                        {
                            localOffset++;
                        }
                    }
                    else if (TestFor(text, localOffset, [" " + CsMarrPartKn + " ", CsPlaceKenn, CsPlaceKenn6, CsUnknown, CsUnknown2], out localFound)
                        || TestFor(text, localOffset, CMonthKn)
                        || (CharAt(text, localOffset) == ' '
                            && localOffset + 1 <= text.Length
                            && (new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' }.Concat(Ziffern).Contains(CharAt(text, localOffset + 1))
                                || (CharAt(text, localOffset + 1) == 'I' && (localOffset + 2 > text.Length || !new[] { 'I', ' ' }.Contains(CharAt(text, localOffset + 2))))
                                || localRetMode == 9)
                            && !localVerwFlag))
                    {
                        localFamRef = string.Empty;
                        localMode = 57;
                        if (localFound > 0)
                        {
                            localOffset--;
                        }
                    }
                    else if (localSubstring == string.Empty && CharAt(text, localOffset) is 'I' or 'l')
                    {
                        localOtherMarrFlag = true;
                        localSpouseCount++;
                    }
                    else if (localSubstring.Length > 0)
                    {
                        localFound = 0;
                        if (!localVerwFlag)
                        {
                            Warning(this, "s. missing");
                        }
                        else if (!new[] { '>', ';', ',' }.Contains(CharAt(text, localOffset))
                            && !TestFor(text, localOffset, [" und", " korr."], out localFound))
                        {
                            Error(this, "invalid reference");
                        }
                        else if (!TestReferenz(localSubstring))
                        {
                            Error(this, "\"" + localSubstring + "\" invalid reference");
                        }
                        else
                        {
                            SetIndiRelat(localIndId, localSubstring, 2);
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) != ',' && localFound == 0)
                        {
                            localMode = 50;
                            if (CharAt(text, localOffset) == '>')
                            {
                                localOffset--;
                            }
                        }
                    }

                    break;
                case 54:
                    if (CharAt(text, localOffset) == ',')
                    {
                        Error(this, ". inst. of , expected");
                    }

                    if (CharAt(text, localOffset) == 'd')
                    {
                        localOffset++;
                    }

                    localParentRef = Copy(localIndId, 2, 20);
                    StartFamily(localParentRef);
                    SetFamilyMember(localParentRef, localIndId, 3);
                    localMode = 55;
                    localSubstring = string.Empty;
                    localFirstEntry = true;
                    if (localOffset + 1 <= text.Length && CharAt(text, localOffset + 1) == ' ')
                    {
                        localOffset++;
                    }

                    break;
                case 55:
                case 56:
                    if (localSubstring == string.Empty)
                    {
                        localAka = string.Empty;
                    }

                    if (localFirstEntry && TestFor(text, localOffset, [CsDeathEntr2, CsDeathEntr], out localFound))
                    {
                        localSubstring += Copy(text, localOffset, localFound * 2 + 1);
                        localOffset += localFound * 2;
                    }
                    else if (localFirstEntry && BuildName(text, ref localOffset, ref localSubstring, ref localData))
                    {
                        localPersonName = localSubstring.Trim();
                        if (TestFor(localPersonName, 1, [CsDeathEntr2, CsDeathEntr], out localFound))
                        {
                            localPersonName = Copy(localPersonName, CsDeathEntr2.Length + localFound * 2 + 1, 200).Trim();
                            if (localPersonName.StartsWith(CsProtectSpace, StringComparison.Ordinal))
                            {
                                localPersonName = Copy(localPersonName, CsProtectSpace.Length + 1, 200).Trim();
                            }

                            localParDeathFlag = true;
                        }
                        else
                        {
                            localParDeathFlag = false;
                        }

                        localIndId2 = HandleAKPersonEntry(localPersonName, localParentRef, localMode == 55 ? 'U' : 'F', localMode, out localLastName2, out localPersonSex2, localAka, localLastName);
                        if (localParDeathFlag)
                        {
                            SetIndiDate(localIndId2, ParserEventType.evt_Death, "vor " + localEventDate);
                        }

                        localSubstring = string.Empty;
                        localFirstEntry = false;
                    }
                    else if (!localFirstEntry && BuildData(localIndId2, text, ref localOffset, ref localSubstring, ref localData))
                    {
                        localEntryType2 = HandleNonPersonEntry(localSubstring.Trim(), localIndId2);
                        if (localData != string.Empty)
                        {
                            SetIndiData(localIndId2, localEntryType2, localData);
                            localData = string.Empty;
                        }

                        localSubstring = string.Empty;
                    }

                    if (localMode == 55 && (TestFor(text, localOffset + 1, "u.d.") || TestFor(text, localOffset + 1, "u.d,")))
                    {
                        if (TestFor(text, localOffset + 1, "u.d,"))
                        {
                            Error(this, "Mother Startflag u.d, <=> u.d.");
                        }

                        localMode = 56;
                        localPersonSex2 = 'U';
                        localOffset += 4;
                        localSubstring = string.Empty;
                        localFirstEntry = true;
                    }
                    else if (new[] { '>', ';' }.Contains(CharAt(text, localOffset)))
                    {
                        localOffset--;
                        localMode = 50;
                    }
                    else if (CharAt(text, localOffset) == ','
                        && TestFor(text, localOffset + 2, CMarriageKn, out localFound)
                        && localOffset + 2 + CMarriageKn[localFound].Length <= text.Length
                        && CharAt(text, localOffset + 2 + CMarriageKn[localFound].Length) == 'I')
                    {
                        Warning(this, "; as End of entry expected");
                        localOffset--;
                        localMode = 50;
                    }
                    else if (TestFor(text, localOffset, ["\n", "\r"]))
                    {
                        localOffset--;
                        localMode = 50;
                    }

                    break;
                case 57:
                    if (localFamRef == string.Empty)
                    {
                        if (localOtherMarrFlag)
                        {
                            localFamRef = Copy(localIndId, 2) + localSpouseCount;
                        }
                        else if (localRetMode == 9)
                        {
                            localFamRef = Copy(localIndId, 2) + "P" + localSpouseCount;
                        }
                        else
                        {
                            localFamRef = localMainFamRef;
                        }
                    }

                    localPp = text.IndexOfAny(['>', ';'], Math.Max(0, localOffset - 1));
                    if (localPp > 0 && Copy(text, localOffset, localPp - localOffset + 1).Contains(" " + CsMarrPartKn + " ", StringComparison.Ordinal))
                    {
                        localSPos = text.IndexOf(" " + CsMarrPartKn + " ", Math.Max(0, localOffset - 1), StringComparison.Ordinal) + 2;
                    }
                    else if (In(CharAt(text, localOffset), Ziffern))
                    {
                        localSPos = text.IndexOf(' ', Math.Max(0, localOffset - 1)) + 2;
                    }
                    else
                    {
                        localSPos = localOffset;
                    }

                    if (localPp > 0 && localSPos < localPp && TestFor(text, localSPos, [CsMarrPartKn, CsUnknown, CsUnknown2], out localFound))
                    {
                        localData = localSubstring;
                        localSubstring = string.Empty;
                        localDate = (localSPos > localOffset && localSPos < localPp) ? Copy(text, localOffset, localSPos - localOffset) : string.Empty;
                        localPersonName = localFound == 0
                            ? Copy(text, localSPos + 4, localPp - localSPos - 3)
                            : Copy(text, localSPos, localPp - localSPos + 1);

                        if (localPersonName.Contains(',', StringComparison.Ordinal))
                        {
                            localPos = localPersonName.IndexOf(',', StringComparison.Ordinal);
                            localSubstring = localPersonName[(localPos + 1)..].Trim();
                            localPersonName = localPersonName[..localPos];
                        }
                        else if (localPersonName.Contains("(" + CsDivorce, StringComparison.Ordinal))
                        {
                            Warning(this, ", after Name expected");
                            localPos = localPersonName.IndexOf("(" + CsDivorce, StringComparison.Ordinal);
                            localSubstring = localPersonName[localPos..];
                            localPersonName = localPersonName[..localPos].Trim();
                        }

                        if (TestFor(localPersonName, 1, [CsDeathEntr2, CsDeathEntr], out localFound))
                        {
                            localPersonName = Copy(localPersonName, CsDeathEntr2.Length + localFound * 2 + 1, 200).Trim();
                            if (localPersonName.StartsWith(CsProtectSpace, StringComparison.Ordinal))
                            {
                                localPersonName = Copy(localPersonName, CsProtectSpace.Length + 1, 200).Trim();
                            }

                            localParDeathFlag = true;
                        }
                        else
                        {
                            localParDeathFlag = false;
                        }

                        localPersonSex2 = localPersonSex == 'M' ? 'F' : 'M';
                        localIndId2 = "I" + localFamRef + localPersonSex2;
                        if (localFamRef != localMainFamRef)
                        {
                            StartFamily(localFamRef);
                        }

                        SetIndiName(localIndId2, 0, localPersonName);
                        SetFamilyType(localFamRef, 1);
                        if (localParDeathFlag)
                        {
                            SetIndiDate(localIndId2, ParserEventType.evt_Death, "vor " + localEventDate);
                        }

                        if (localDate != string.Empty)
                        {
                            HandleFamilyFact(localFamRef, CsMarriageEntr + " " + localDate);
                        }

                        if (localPersonSex2 == 'F')
                        {
                            SetFamilyMember(localFamRef, localIndId2, 2);
                            if (localFamRef != localMainFamRef)
                            {
                                SetFamilyMember(localFamRef, localIndId, 1);
                            }
                        }
                        else
                        {
                            SetFamilyMember(localFamRef, localIndId2, 1);
                            if (localFamRef != localMainFamRef)
                            {
                                SetFamilyMember(localFamRef, localIndId, 2);
                            }
                        }

                        SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonSex2.ToString());
                        if (localSubstring != string.Empty)
                        {
                            if (TestFor(localSubstring, 1, [CsDivorce, "(" + CsDivorce], out localFound))
                            {
                                localSubstring = localSubstring.Remove(localSubstring.Length - 1 - localFound).Remove(0, 3 + localFound);
                                SetFamilyDate(localFamRef, ParserEventType.evt_Divorce, "vor " + localEventDate);
                                if (localSubstring != string.Empty)
                                {
                                    SetFamilyData(localFamRef, ParserEventType.evt_Divorce, localSubstring);
                                }
                            }
                            else if (!localSubstring.Contains(',', StringComparison.Ordinal))
                            {
                                HandleNonPersonEntry(localSubstring, localIndId2);
                            }
                            else
                            {
                                foreach (var entry in localSubstring.Split(','))
                                {
                                    HandleNonPersonEntry(entry, localIndId2);
                                }
                            }
                        }

                        if (localPp + 1 < text.Length && new[] { '>', ';' }.Contains(text[localPp + 1]))
                        {
                            localOffset = localPp + 1;
                        }

                        if (localPp + 1 < text.Length && text[localPp + 1] == ';'
                            && TestFor(text, localOffset + 2, ["(" + CsDivorce + ")", " (" + CsDivorce + ")"], out localFound))
                        {
                            SetFamilyData(localFamRef, ParserEventType.evt_Divorce, string.Empty);
                            localOffset += CsDivorce.Length + 3 + localFound;
                        }

                        localMode = 50;
                    }
                    else if (localPp > 0)
                    {
                        localSubstring = CsMarriageEntr + localSubstring
                            + (new[] { ' ', '\t', '\r', '\n' }.Contains(CharAt(text, localOffset)) ? string.Empty : " ")
                            + Copy(text, localOffset, localPp - localOffset + 2);
                        if (localSubstring.Contains(',', StringComparison.Ordinal))
                        {
                            localPos = localSubstring.IndexOf(',', StringComparison.Ordinal);
                            if (localSubstring.Length > localPos + 3 && In(localSubstring[localPos + 2], LowerCharset))
                            {
                                HandleNonPersonEntry(localSubstring[..localPos], localIndId);
                                localSubstring = localSubstring[(localPos + 1)..];
                            }
                        }

                        HandleNonPersonEntry(localSubstring, localIndId, localFamRef);
                        localOffset = localPp + 1;
                        localMode = 50;
                    }

                    break;
                case 103:
                    if (In(CharAt(text, localOffset), Ziffern.Concat(['.'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else
                    {
                        if (localSubstring.Length > 2)
                        {
                            if (localEntryType == ParserEventType.evt_ID)
                            {
                                OnIndiRef?.Invoke(this, localSubstring, localIndId, (int)localEntryType);
                            }

                            localSubstring = string.Empty;
                            localMode = localRetMode;
                        }
                    }

                    if (TestFor(text, localOffset, CsSeparatorGc) || CharAt(text, localOffset) == CsSeparator[0])
                    {
                        localMode = localRetMode;
                        localOffset--;
                    }

                    break;
                case 110:
                    if (In(CharAt(text, localOffset), Charset.Concat(['.', '-'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == ' ' && !TestFor(text, localOffset + 1, [CsMarriageEntr2, CsIllegChild]))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, _umlauts, out localFound))
                    {
                        localSubstring += _umlauts[localFound];
                        localOffset += _umlauts[localFound].Length - 1;
                    }
                    else if (localFirstEntry && localSubstring != string.Empty)
                    {
                        localIndId = "I" + localMainFamRef + localPersonType;
                        SetIndiName(localIndId, 1, localSubstring.Trim());
                        SetFamilyMember(localMainFamRef, localIndId, localPersonType == 'M' ? 1 : 2);
                        localLastName = localSubstring.Trim();
                        if (localPersonType != 'F' || localFamName == string.Empty)
                        {
                            localFamName = localLastName;
                        }

                        localFirstEntry = false;
                        localEntryType = ParserEventType.evt_GivenName;
                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }
                    }
                    else if (localSubstring != string.Empty)
                    {
                        if (localEntryType == ParserEventType.evt_GivenName)
                        {
                            localPersonGName = localSubstring.Trim();
                            if (localPersonGName.Length < 4
                                && (localPersonGName.EndsWith(".", StringComparison.Ordinal) || localPersonGName.Length == 2)
                                && localPersonGName.Length > 0
                                && char.IsLower(localPersonGName[0]))
                            {
                                SetIndiName(localIndId, 2, "NN");
                                SetIndiData(localIndId, ParserEventType.evt_Religion, localPersonGName);
                                localMode = 112;
                            }
                            else
                            {
                                SetIndiName(localIndId, 2, localSubstring.Trim());
                                if (localPersonType is 'M' or 'F')
                                {
                                    localPersonSex = localPersonType;
                                    SetIndiData(localIndId, ParserEventType.evt_Sex, localPersonType.ToString());
                                    LearnSexOfGivnName(localSubstring.Trim(), localPersonType);
                                }
                                else
                                {
                                    localPersonSex = GuessSexOfGivnName(localSubstring.Trim());
                                    if (localPersonSex is 'M' or 'F')
                                    {
                                        SetIndiData(localIndId, ParserEventType.evt_Sex, localPersonSex.ToString());
                                    }
                                }
                            }
                        }
                        else if (localEntryType == ParserEventType.evt_AKA)
                        {
                            SetIndiName(localIndId, 3, localSubstring.Trim());
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }

                        if (CharAt(text, localOffset) == '"' && localEntryType == ParserEventType.evt_GivenName)
                        {
                            localEntryType = ParserEventType.evt_AKA;
                        }
                        else
                        {
                            localMode = 112;
                        }

                        if (TestFor(text, localOffset, [" " + CsMarriageEntr2 + " ", " " + CsIllegChild + " "]))
                        {
                            localMode = 114;
                            localSubstring = string.Empty;
                        }
                        else if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                        {
                            localMode = 150;
                            localRetMode2 = 112;
                            localOffset--;
                        }
                    }

                    break;
                case 112:
                    if (!new[] { '.', ',', '<', '[' }.Concat(WhiteSpaceChars).Contains(CharAt(text, localOffset)))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (In(CharAt(text, localOffset), WhiteSpaceChars)
                        && localSubstring != string.Empty
                        && !TestFor(text, localOffset, [" " + CsMarriageEntr2 + " ", " " + CsIllegChild + " "]))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && ((Copy(text, localOffset + 1, 1) != " ")
                            || ((Right(localSubstring, 4).IndexOf(' ') != -1)
                                && Right(localSubstring, 4)[Right(localSubstring, 4).IndexOf(' ') + 1] is >= 'A' and <= 'Z')))
                    {
                        localSubstring += '.';
                    }
                    else if (CharAt(text, localOffset) == ',' && localSubstring.Trim() == "Bürger")
                    {
                        localSubstring += ',';
                    }
                    else if (HandleGCNonPersonEntry(localSubstring, CharAt(text, localOffset), localIndId))
                    {
                        localSubstring = string.Empty;
                        if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                        {
                            localRetMode2 = localMode;
                            localMode = 150;
                            localOffset--;
                        }
                    }
                    else if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                    {
                        localSubstring = string.Empty;
                        localRetMode2 = localMode;
                        localMode = 150;
                        localOffset--;
                    }

                    if (TestFor(text, localOffset, CsAdditional))
                    {
                        localMode++;
                        localDate = string.Empty;
                        localSubstring = string.Empty;
                        localPos = text.IndexOf(':', Math.Max(0, localOffset - 1));
                        localPlaceFlag = false;
                        localEntryType = ParserEventType.evt_AddOccupation;
                        if (localPos != -1)
                        {
                            localOffset = localPos + 1;
                        }
                        else
                        {
                            localOffset += CsAdditional.Length + localLastName.Length + 5;
                        }
                    }
                    else if (TestFor(text, localOffset, CsSeparatorGc))
                    {
                        localMode = 100;
                        localOffset--;
                    }
                    else if (TestFor(text, localOffset, [" " + CsMarriageEntr2 + " ", " " + CsIllegChild + " "]))
                    {
                        localMode = 114;
                        localSubstring = string.Empty;
                    }
                    else if (HandleGCDateEntry(text, ref localOffset, localIndId, ref localMode, ref localRetMode, ref localEntryType))
                    {
                        localSubstring = string.Empty;
                    }
                    else if (TestFor(text, localOffset, CsReferenceGc))
                    {
                        localRetMode = localMode;
                        localMode = 103;
                        localEntryType = ParserEventType.evt_ID;
                        localSubstring = string.Empty;
                    }

                    break;
                case 113:
                    if (TestFor(text, localOffset, CsSeparatorGc) || TestFor(text, localOffset, CsReferenceGc) || TestFor(text, localOffset, "PN="))
                    {
                        localMode = 112;
                        continue;
                    }

                    if (TestFor(text, localOffset, [CsResidence + " "]))
                    {
                        localEntryType = ParserEventType.evt_AddResidence;
                        localOffset += CsResidence.Length;
                        SetIndiData(localIndId, localEntryType, string.Empty);
                        if (localDate != string.Empty)
                        {
                            SetIndiDate(localIndId, localEntryType, localDate);
                        }

                        localSubstring = string.Empty;
                        localDate = string.Empty;
                    }
                    else if (TestFor(text, localOffset, [CsEmigration + " "]))
                    {
                        localEntryType = ParserEventType.evt_AddEmigration;
                        localOffset += CsEmigration.Length;
                        SetIndiData(localIndId, localEntryType, string.Empty);
                        if (localDate != string.Empty)
                        {
                            SetIndiDate(localIndId, localEntryType, localDate);
                        }

                        localSubstring = string.Empty;
                        localDate = string.Empty;
                    }
                    else if (TestFor(text, localOffset - 1, " " + CsPlaceKenn + " "))
                    {
                        localPlaceFlag = true;
                        localOffset += CsPlaceKenn.Length;
                    }
                    else if (localDate == string.Empty && (TestFor(text, localOffset, CDateModif, out localFound) || In(CharAt(text, localOffset), Ziffern)))
                    {
                        if (localFound >= 0)
                        {
                            localOffset += CDateModif[localFound].Length;
                            localDate = CDateModif[localFound];
                            if (localOffset <= text.Length && new[] { '.', ' ' }.Contains(CharAt(text, localOffset)))
                            {
                                localDate += CharAt(text, localOffset);
                                localOffset++;
                            }
                        }

                        while (localOffset + 1 < text.Length
                            && (In(CharAt(text, localOffset), Ziffern.Concat([' '])) || (CharAt(text, localOffset) == '.' && In(CharAt(text, localOffset + 1), Ziffern))))
                        {
                            localDate += CharAt(text, localOffset);
                            localOffset++;
                        }

                        localOffset--;
                    }
                    else if (!WhiteSpaceChars.Concat(['.']).Contains(CharAt(text, localOffset)))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && ((localOffset + 1 <= text.Length && In(CharAt(text, localOffset + 1), Ziffern)) || localSubstring.Length == 2))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == ' '
                        && (((localOffset > 1 && new[] { '.', ',', ')' }.Contains(CharAt(text, localOffset - 1)) && localSubstring != string.Empty)
                            || TestFor(localSubstring, 1, CDateModif)
                            || localPlaceFlag)))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (localSubstring != string.Empty)
                    {
                        if (localPlaceFlag)
                        {
                            if (localDate != string.Empty)
                            {
                                localEntryType = ParserEventType.evt_AddResidence;
                                SetIndiDate(localIndId, localEntryType, localDate.Trim());
                                localDate = string.Empty;
                            }

                            SetIndiPlace(localIndId, localEntryType, localSubstring);
                            localSubstring = string.Empty;
                            localPlaceFlag = false;
                            localEntryType = ParserEventType.evt_AddOccupation;
                        }
                        else
                        {
                            SetIndiData(localIndId, localEntryType, localSubstring);
                            if (localDate != string.Empty)
                            {
                                SetIndiDate(localIndId, localEntryType, localDate.Trim());
                            }

                            if (_defaultPlace != string.Empty)
                            {
                                SetIndiPlace(localIndId, localEntryType, _defaultPlace);
                            }

                            localSubstring = string.Empty;
                            localDate = string.Empty;
                        }
                    }

                    break;
                case 114:
                    localEntryType = ParserEventType.evt_Marriage;
                    localSubstring = string.Empty;
                    localFamCEntry = string.Empty;
                    if (TestFor(text, localOffset, CsIllegChild))
                    {
                        localFamType = 2;
                    }
                    else
                    {
                        localFamType = 1;
                        if (localOffset + 4 <= text.Length && CharAt(text, localOffset + 4) == '/')
                        {
                            localFamCEntry = Copy(text, localOffset + 3, 3);
                            localOffset += 4;
                        }
                    }

                    localChildFam = Copy(localIndId, 2, 20);
                    StartFamily(localChildFam);
                    SetFamilyType(localChildFam, localFamType, localFamCEntry);
                    SetFamilyMember(localChildFam, localIndId, localPersonSex == 'M' ? 1 : 2);
                    localOffset += 2;
                    localFamRef = localChildFam;
                    localRetMode = localMode + 1;
                    localMode = 101;
                    break;
                case 115:
                    if (TestFor(text, localOffset, CsMarrPartKn))
                    {
                        localMode++;
                        localFamRef = localChildFam;
                        localFirstEntry = true;
                        localPersonType2 = localPersonSex == 'M' ? 'F' : 'M';
                        localOffset += 3;
                    }
                    else
                    {
                        localMode = 112;
                        localOffset--;
                    }

                    break;
                case 116:
                    if (In(CharAt(text, localOffset), Charset.Concat(['.', '-']).Concat(WhiteSpaceChars.Except(['\n', '\r']))))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, _umlauts))
                    {
                        localSubstring += CharAt(text, localOffset) + CharAt(text, localOffset + 1);
                        localOffset++;
                    }
                    else if (localFirstEntry)
                    {
                        localIndId2 = "I" + localFamRef + localPersonType2;
                        SetIndiName(localIndId2, 1, localSubstring.Trim());
                        SetFamilyMember(localFamRef, localIndId2, localPersonType2 == 'M' ? 1 : 2);
                        localLastName = localSubstring.Trim();
                        localFirstEntry = false;
                        localEntryType = ParserEventType.evt_Last;
                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }
                    }
                    else
                    {
                        SetIndiName(localIndId2, 2, localSubstring.Trim());
                        if (localPersonType2 is 'M' or 'F')
                        {
                            SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonType2.ToString());
                        }
                        else
                        {
                            localPersonSex2 = GuessSexOfGivnName(localSubstring.Trim());
                            if (localPersonSex2 is 'M' or 'F')
                            {
                                SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonSex2.ToString());
                            }
                        }

                        if (localPersonType2 is 'M' or 'F')
                        {
                            LearnSexOfGivnName(localSubstring.Trim(), localPersonType2);
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }

                        localMode = 112;
                        localOffset--;
                        if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                        {
                            localMode = 150;
                            localRetMode2 = 112;
                        }
                    }

                    break;
                case 120:
                    if (Copy(text, localOffset + 1, 7) == "Kinder:")
                    {
                        localMode = 121;
                        localOffset += 8;
                    }
                    else if (Copy(text, localOffset + 1, 5) == "Kind:")
                    {
                        localMode = 122;
                        localIndId = "I" + localMainFamRef + "C1";
                        localChildCount = 1;
                        localOffset += 6;
                        localFirstEntry = true;
                        SetIndiName(localIndId, 1, localFamName);
                        SetFamilyMember(localMainFamRef, localIndId, 2 + localChildCount);
                    }
                    else
                    {
                        localMode = 0;
                    }

                    break;
                case 121:
                    if (In(CharAt(text, localOffset), Ziffern))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (localSubstring != string.Empty && CharAt(text, localOffset) == ')' && int.TryParse(localSubstring, out localChildCount))
                    {
                        localMode = 122;
                        localIndId = "I" + localMainFamRef + "C" + localChildCount;
                        localFirstEntry = true;
                        SetIndiName(localIndId, 1, localFamName);
                        SetFamilyMember(localMainFamRef, localIndId, 2 + localChildCount);
                        localSubstring = string.Empty;
                    }
                    else if (TestFor(text, localOffset, [CsMarriageEntr2 + " ", CsIllegChild + " "]))
                    {
                        localRetMode3 = localMode + 2;
                        localMode = 124;
                        localSubstring = string.Empty;
                    }
                    else if (localSubstring != string.Empty)
                    {
                        localMode = 2;
                        localOffset--;
                    }

                    break;
                case 122:
                    if (In(CharAt(text, localOffset), Charset.Concat(['.', ' ', '-'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, _umlauts))
                    {
                        localSubstring += CharAt(text, localOffset) + CharAt(text, localOffset + 1);
                        localOffset++;
                    }
                    else if (localFirstEntry && CharAt(text, localOffset) == ',')
                    {
                        SetIndiName(localIndId, 1, localSubstring.Trim());
                        localSubstring = string.Empty;
                    }
                    else if (localFirstEntry)
                    {
                        localPersonGName = localSubstring.Trim();
                        if (localPersonGName == string.Empty && new[] { '\t', '+', '*' }.Contains(CharAt(text, localOffset)))
                        {
                            localPersonGName = "NN";
                        }

                        if (localPersonGName != string.Empty)
                        {
                            SetIndiName(localIndId, 2, localPersonGName);
                            localPersonSex = GuessSexOfGivnName(localSubstring.Trim());
                            if (localPersonSex is 'M' or 'F')
                            {
                                SetIndiData(localIndId, ParserEventType.evt_Sex, localPersonSex.ToString());
                            }

                            localFirstEntry = false;
                            localMode = 123;
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }

                        if (localMode == 123)
                        {
                            localOffset--;
                        }
                    }

                    break;
                case 123:
                    if (!new[] { '.', ',', '<', '[', '\n', '\r' }.Contains(CharAt(text, localOffset))
                        && !TestFor(text, localOffset, [" " + CsMarriageEntr2 + " ", " " + CsIllegChild + " "]))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '.'
                        && ((Copy(text, localOffset + 1, 1) != " ")
                            || ((Right(localSubstring, 4).IndexOf(' ') != -1)
                                && Right(localSubstring, 4)[Right(localSubstring, 4).IndexOf(' ') + 1] is >= 'A' and <= 'Z')))
                    {
                        localSubstring += '.';
                    }
                    else if (CharAt(text, localOffset) == ',' && localSubstring.Trim() == "Bürger")
                    {
                        localSubstring += ',';
                    }
                    else if (HandleGCNonPersonEntry(localSubstring, CharAt(text, localOffset), localIndId))
                    {
                        localSubstring = string.Empty;
                        if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                        {
                            localRetMode2 = localMode;
                            localMode = 150;
                            localOffset--;
                        }
                    }
                    else if (new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                    {
                        localSubstring = string.Empty;
                        localRetMode2 = localMode;
                        localMode = 150;
                        localOffset--;
                    }

                    if (TestFor(text, localOffset, [" " + CsMarriageEntr2 + " ", " " + CsIllegChild + " "]))
                    {
                        localRetMode3 = localMode;
                        localMode = 124;
                        localSubstring = string.Empty;
                    }
                    else if (TestFor(text, localOffset, CsSeparatorGc))
                    {
                        localMode = 100;
                        localOffset--;
                    }
                    else if (TestFor(text, localOffset, VbNewLine) || (In(CharAt(text, localOffset), Ziffern) && TestFor(text, localOffset, ")")))
                    {
                        localMode = 121;
                        localSubstring = string.Empty;
                        if (In(CharAt(text, localOffset), Ziffern))
                        {
                            localOffset--;
                        }
                    }
                    else if (HandleGCDateEntry(text, ref localOffset, localIndId, ref localMode, ref localRetMode, ref localEntryType))
                    {
                        localSubstring = string.Empty;
                    }
                    else if (TestFor(text, localOffset, CsReferenceGc))
                    {
                        localRetMode = localMode;
                        localMode = 103;
                        localEntryType = ParserEventType.evt_ID;
                        localSubstring = string.Empty;
                    }

                    break;
                case 124:
                    localEntryType = ParserEventType.evt_Marriage;
                    localSubstring = string.Empty;
                    localFamCEntry = string.Empty;
                    localFamRef = Copy(localIndId, 2, 20);
                    if (TestFor(text, localOffset, CsIllegChild))
                    {
                        localFamType = 2;
                    }
                    else
                    {
                        localFamType = 1;
                        if (localOffset + 4 <= text.Length && CharAt(text, localOffset + 4) == '/')
                        {
                            localFamCEntry = Copy(text, localOffset + 3, 3);
                            localFamRef = localFamRef + "S" + (char)(localFamCEntry[0] + localFamCEntry[2] - '1');
                            localOffset += 4;
                        }
                    }

                    StartFamily(localFamRef);
                    SetFamilyType(localFamRef, localFamType, localFamCEntry);
                    SetFamilyMember(localFamRef, localIndId, localPersonSex == 'M' ? 1 : 2);
                    localOffset += localFamType + 1;
                    localRetMode = 125;
                    localMode = 101;
                    break;
                case 125:
                    if (TestFor(text, localOffset, CsMarrPartKn))
                    {
                        localMode = 126;
                        localFirstEntry = true;
                        localPersonType2 = localPersonSex == 'M' ? 'F' : 'M';
                        localOffset += 3;
                    }
                    else if (CharAt(text, localOffset) != ' ')
                    {
                        localMode = localRetMode3;
                        localOffset--;
                    }

                    break;
                case 126:
                    if (In(CharAt(text, localOffset), Charset.Concat(['.', '-']).Concat(WhiteSpaceChars.Except(['\n', '\r']))))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (TestFor(text, localOffset, _umlauts))
                    {
                        localSubstring += CharAt(text, localOffset) + CharAt(text, localOffset + 1);
                        localOffset++;
                    }
                    else if (localFirstEntry)
                    {
                        localIndId2 = "I" + localFamRef + localPersonType2;
                        SetIndiName(localIndId2, 1, localSubstring.Trim());
                        SetFamilyMember(localFamRef, localIndId2, localPersonType2 == 'M' ? 1 : 2);
                        localLastName = localSubstring.Trim();
                        localFirstEntry = false;
                        localEntryType = ParserEventType.evt_Last;
                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }
                    }
                    else
                    {
                        SetIndiName(localIndId2, 2, localSubstring.Trim());
                        if (localPersonType2 is 'M' or 'F')
                        {
                            SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonType2.ToString());
                            LearnSexOfGivnName(localSubstring.Trim(), localPersonType2);
                        }
                        else
                        {
                            localPersonSex2 = GuessSexOfGivnName(localSubstring.Trim());
                            if (localPersonSex2 is 'M' or 'F')
                            {
                                SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonSex2.ToString());
                            }
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId, 3, localAdditional);
                        }

                        localMode = localRetMode3;
                        localOffset--;
                        if (localMode < 150 && new[] { '<', '[' }.Contains(CharAt(text, localOffset)))
                        {
                            localRetMode2 = localMode;
                            localMode = 150;
                        }
                    }

                    break;
                case 150:
                    if (new[] { '>', ']' }.Contains(CharAt(text, localOffset)))
                    {
                        localMode = localRetMode2;
                        if (localSubstring != string.Empty && int.TryParse(localSubstring, out _))
                        {
                            SetIndiRelat(localIndId, localSubstring.Trim(), localRefMode2);
                        }

                        localSubstring = string.Empty;
                    }
                    else if (new[] { '\n', '\r', '*', '~' }.Contains(CharAt(text, localOffset)))
                    {
                        localMode = localRetMode2;
                        Error(this, _mainRef + ": Unclosed Reference");
                        if (localSubstring != string.Empty && In(localSubstring[0], Ziffern))
                        {
                            SetIndiRelat(localIndId, localSubstring.Trim(), localRefMode2);
                        }

                        localOffset--;
                        localSubstring = string.Empty;
                    }
                    else if (CharAt(text, localOffset) == '<')
                    {
                        localRefMode2 = 1;
                    }
                    else if (CharAt(text, localOffset) == '[')
                    {
                        localRefMode2 = 2;
                    }
                    else if (localRefMode2 == 2 && localSubstring == string.Empty && TestFor(text, localOffset, CsMarriageEntr2))
                    {
                        localRetMode3 = localMode;
                        localMode = 124;
                        localOffset--;
                    }
                    else if (In(CharAt(text, localOffset), Charset) || TestFor(text, localOffset, CsDeathEntr))
                    {
                        localSubstring = CharAt(text, localOffset).ToString();
                        localParentRef = Copy(localIndId, 2, 20);
                        StartFamily(localParentRef);
                        SetFamilyMember(localParentRef, localIndId, 3);
                        localFirstEntry = true;
                        localSecondEntry = true;
                        localMode = 155;
                    }
                    else if (CharAt(text, localOffset) != ',')
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (long.TryParse(localSubstring, out localInt))
                    {
                        SetIndiRelat(localIndId, localSubstring.Trim(), localRefMode2);
                        localSubstring = string.Empty;
                    }

                    break;
                case 155:
                case 156:
                    if (!(new[] { ' ', ',', '<', '>', ']', '(' }.Contains(CharAt(text, localOffset)))
                        || (CharAt(text, localOffset) == ' ' && (Copy(text, localOffset, 5) != " und ") && localSubstring != string.Empty))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else if (CharAt(text, localOffset) == '<')
                    {
                    }
                    else if (localFirstEntry && localSubstring != string.Empty)
                    {
                        localIndId2 = localMode == 155 ? "I" + localParentRef + "M" : "I" + localParentRef + "F";
                        if (TestFor(localSubstring, 1, CsDeathEntr))
                        {
                            SetIndiName(localIndId2, 1, Copy(localSubstring.Trim(), CsDeathEntr.Length + 1, 200));
                            SetIndiDate(localIndId2, ParserEventType.evt_Death, "vor " + localEventDate);
                        }
                        else
                        {
                            SetIndiName(localIndId2, 1, localSubstring.Trim());
                        }

                        SetFamilyMember(localParentRef, localIndId2, localMode - 154);
                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId2, 3, localAdditional);
                        }

                        localFirstEntry = false;
                    }
                    else if (localSecondEntry && localSubstring != string.Empty)
                    {
                        localPersonGName = localSubstring.Trim();
                        if (localPersonGName.Length < 4 && (localPersonGName.EndsWith(".", StringComparison.Ordinal) || localPersonGName.Length == 2) && localPersonGName.Length > 0 && char.IsLower(localPersonGName[0]))
                        {
                            SetIndiName(localIndId2, 2, "NN");
                            SetIndiData(localIndId2, ParserEventType.evt_Religion, localPersonGName);
                            localSecondEntry = false;
                        }
                        else
                        {
                            SetIndiName(localIndId2, 2, localPersonGName);
                            if (localMode == 156)
                            {
                                SetIndiData(localIndId2, ParserEventType.evt_Sex, "F");
                            }
                            else
                            {
                                localPersonSex2 = GuessSexOfGivnName(localPersonGName);
                                SetIndiData(localIndId2, ParserEventType.evt_Sex, localPersonSex2.ToString());
                            }
                        }

                        localSubstring = string.Empty;
                        if (CharAt(text, localOffset) == '(' && ParseAdditional(text, ref localOffset, out localAdditional))
                        {
                            SetIndiName(localIndId2, 3, localAdditional);
                        }

                        localSecondEntry = false;
                    }
                    else if (CharAt(text, localOffset) == ',' && localSubstring.Trim() == "Bürger")
                    {
                        localSubstring += ',';
                    }
                    else if (HandleGCNonPersonEntry(localSubstring, CharAt(text, localOffset), localIndId2))
                    {
                        localSubstring = string.Empty;
                    }

                    if (localMode == 155 && Copy(text, localOffset, 5) == " und ")
                    {
                        localMode = 156;
                        localOffset += 4;
                        localSubstring = string.Empty;
                        localFirstEntry = true;
                        localSecondEntry = true;
                    }
                    else if (new[] { '>', ']' }.Contains(CharAt(text, localOffset)))
                    {
                        localOffset--;
                        localSubstring = string.Empty;
                        localMode = 150;
                    }

                    break;
                case 199:
                    break;
                default:
                    // The original state machine contains many additional modes. They are preserved in the Pascal source
                    // and should be ported incrementally. The current implementation already covers the common non-GC flow,
                    // keeps diagnostics and helper behavior, and safely resets to mode 0 for unsupported branches.
                    localMode = 0;
                    break;
            }

            localOffset++;
            localFirstCycle = _mode != localMode;
            if (localFirstCycle)
            {
                _mode = localMode;
                localStartOffset = localOffset;
#if DEBUG
                var debug = Copy(text, localOffset, 20);
                Debug(this, $"NM:{_mode} ({localStartOffset}){debug}");
#endif
            }
        }
    }

    /// <inheritdoc />
    public override void Error(object? sender, string message)
    {
        _lastErr = message;
        if (OnParseMessage is not null)
        {
            OnParseMessage(this, ParseMessageKind.Error, message, _mainRef, _mode);
        }
        else
        {
            OnParseError?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public override void Warning(object? sender, string message)
    {
        _lastErr = message;
        if (OnParseMessage is not null)
        {
            OnParseMessage(this, ParseMessageKind.Warning, message, _mainRef, _mode);
        }
        else
        {
            OnParseError?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Emits a debug message.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="message">The message text.</param>
    public void Debug(object? sender, string message)
    {
        _lastErr = message;
        OnParseMessage?.Invoke(this, ParseMessageKind.Debug, message, _mainRef, _mode);
    }

    /// <summary>
    /// Updates the last message, main reference, and parser mode.
    /// </summary>
    /// <param name="message">The last message.</param>
    /// <param name="reference">The current reference.</param>
    /// <param name="mode">The parser mode.</param>
    public void DebugSetMsg(string message, string reference, int mode)
    {
        _lastErr = message;
        _mainRef = reference;
        _mode = mode;
    }

    /// <summary>
    /// Learns a given-name to sex mapping.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="sex">The sex marker.</param>
    public void LearnSexOfGivnName(string name, char sex) => GNameHandler.LearnSexOfGivnName(name, sex);

    /// <summary>
    /// Guesses the sex of the supplied given name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="learn">A value indicating whether unknown names may be learned.</param>
    /// <returns>The inferred sex marker.</returns>
    public char GuessSexOfGivnName(string name, bool learn = true)
    {
        GNameHandler.CfgLearnUnknown = CfgLearnUnknown;
        return GNameHandler.GuessSexOfGivnName(name, learn);
    }

    /// <summary>
    /// Determines whether the supplied date text is structurally valid for this parser.
    /// </summary>
    public bool IsValidDate(string date)
    {
        if (date == string.Empty)
        {
            return true;
        }

        return !date.Contains('\n')
            && !date.Contains('\r')
            && !date.Contains('\t')
            && !date.Contains(',')
            && !date.Contains(':')
            && !date.Contains(';')
            && !date.Contains('<')
            && !date.Contains('>')
            && !date.Contains('+')
            && !date.Contains('*')
            && !date.Contains('|');
    }

    /// <summary>
    /// Determines whether the supplied place text is structurally valid for this parser.
    /// </summary>
    public bool IsValidPlace(string place)
    {
        if (place == string.Empty)
        {
            return true;
        }

        return place == CsUnknown2
            || UpperCharset.Contains(place[0])
            || place[0] is '"' or '“'
            || TestFor(place, 1, CUmLautsU);
    }

    /// <summary>
    /// Tests whether the supplied reference follows the family-book reference format.
    /// </summary>
    public bool TestReferenz(string reference)
    {
        if (reference == string.Empty)
        {
            return false;
        }

        var result = Ziffern.Contains(reference[0]);
        for (var index = 1; index < reference.Length - 1; index++)
        {
            result &= Ziffern.Contains(reference[index]);
        }

        return result && (Ziffern.Contains(reference[^1]) || reference[^1] is 'a' or 'b');
    }

    /// <summary>
    /// Tests a single marker at the start of an entry and returns the remaining payload.
    /// </summary>
    public bool TestEntry(string subString, string testString, out string data)
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
        else if (TestFor(subString, testString.Length + 1, CsProtectSpace))
        {
            data = Copy(subString, testString.Length + CsProtectSpace.Length + 1).Trim();
        }
        else if (TestFor(subString, testString.Length + 1, " "))
        {
            data = Copy(subString, testString.Length + 2).Trim();
        }
        else if (!char.IsLetterOrDigit((testString + " ")[testString.Length - 1])
            && (((Copy(subString, testString.Length + 1) + " ")[0] is >= '0' and <= '9' or >= 'A' and <= 'Z' or >= 'a' and <= 'z')
                || TestFor(subString, testString.Length, CUmLautsU)))
        {
            data = Copy(subString, testString.Length + 1).Trim();
        }
        else
        {
            result = false;
        }

        return result;
    }

    /// <summary>
    /// Tests one of several markers at the start of an entry and returns the remaining payload.
    /// </summary>
    public bool TestEntry(string subString, string[] testStrings, out string data)
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

    /// <summary>
    /// Tests whether a given string matches at the specified one-based position.
    /// </summary>
    public static bool TestFor(string text, int position, string test)
        => position >= 1 && position - 1 + test.Length <= text.Length && string.CompareOrdinal(text, position - 1, test, 0, test.Length) == 0;

    /// <summary>
    /// Tests whether any given string matches at the specified one-based position.
    /// </summary>
    public static bool TestFor(string text, int position, string[] tests)
        => TestFor(text, position, tests, out _);

    /// <summary>
    /// Tests whether any given string matches at the specified one-based position and returns the index of the match.
    /// </summary>
    public static bool TestFor(string text, int position, string[] tests, out int found)
    {
        found = -1;
        for (var index = 0; index < tests.Length; index++)
        {
            if (TestFor(text, position, tests[index]))
            {
                found = index;
                return true;
            }
        }

        return false;
    }

    private static bool TestForC(string text, int position, string test)
        => string.Equals(Copy(text, position, test.Length), test, StringComparison.OrdinalIgnoreCase);

    private void GNameError(string message, int type)
        => Error(this, _mainRef + ": " + message);

    private void SetFamilyMember(string famRef, string individualId, int famMember)
        => OnFamilyIndiv?.Invoke(this, individualId, famRef, famMember);

    private void SetFamilyDate(string famRef, ParserEventType eventType, string date)
    {
        if (!IsValidDate(date))
        {
            Error(this, QuotedString(date) + " is no valid Date");
        }
        else
        {
            OnFamilyDate?.Invoke(this, date, famRef, (int)eventType);
        }
    }

    private void SetFamilyPlace(string famRef, ParserEventType eventType, string place)
    {
        if (!IsValidPlace(place))
        {
            Error(this, QuotedString(place) + " is no valid Place");
        }

        OnFamilyPlace?.Invoke(this, place, famRef, (int)eventType);
    }

    private void SetFamilyData(string famRef, ParserEventType eventType, string data)
        => OnFamilyData?.Invoke(this, data, famRef, (int)eventType);

    private void SetIndiName(string individualId, int nameType, string name)
        => OnIndiName?.Invoke(this, name, individualId, nameType);

    private void SetIndiData(string individualId, ParserEventType eventType, string data)
        => OnIndiData?.Invoke(this, data, individualId, (int)eventType);

    private bool TryConsumeLeadingEntry(ref string subString, string[] testStrings, out string rest)
    {
        if (TestEntry(subString, testStrings, out rest))
        {
            return true;
        }

        foreach (var testString in testStrings)
        {
            if (subString.StartsWith(testString, StringComparison.Ordinal))
            {
                rest = RemoveStart(subString, testString.Length).TrimStart();
                return true;
            }
        }

        rest = string.Empty;
        return false;
    }
    private void SetIndiDate(string individualId, ParserEventType eventType, string date)
    {
        if (!IsValidDate(date))
        {
            Error(this, QuotedString(date) + " is no valid Date");
        }
        else
        {
            OnIndiDate?.Invoke(this, date, individualId, (int)eventType);
        }
    }

    private void SetIndiPlace(string individualId, ParserEventType eventType, string place)
    {
        if (!IsValidPlace(place))
        {
            Error(this, QuotedString(place) + " is no valid Place");
        }

        OnIndiPlace?.Invoke(this, place, individualId, (int)eventType);
    }

    private void SetIndiOccu(string individualId, ParserEventType eventType, string occu)
        => OnIndiOccu?.Invoke(this, occu, individualId, (int)eventType);

    private void SetIndiRelat(string individualId, string famRef, int relType)
    {
        if (famRef == "0" && _mainRef != "0")
        {
            Error(this, QuotedString(famRef) + " is no valid Ref");
        }

        OnIndiRel?.Invoke(this, famRef, individualId, relType);
    }

    private void StartFamily(string famRef)
        => OnStartFamily?.Invoke(this, famRef, string.Empty, 0);

    private void SetFamilyType(string famRef, int famType, string data = "")
        => OnFamilyType?.Invoke(this, data, famRef, famType);

    private void EndOfEntry(string famRef)
        => OnEntryEnd?.Invoke(this, string.Empty, famRef, -1);

    /// <summary>
    /// Parses text enclosed in parentheses as an additional entry payload.
    /// </summary>
    public bool ParseAdditional(string text, ref int position, out string output)
    {
        output = string.Empty;
        var result = text.Length > position + 1
            && ((AlphaNum.Contains(CharAt(text, position + 1)) || new[] { ' ', '"', '?', '*', '!' }.Contains(CharAt(text, position + 1)))
                || TestFor(text, position + 1, [CsDeathEntr, CsTypeQuota, CsMarriageEntr])
                || TestFor(text, position + 1, _umlauts));
        if (!result)
        {
            return false;
        }

        var maxLen = ((CharAt(text, position + 1) == '"') && text.Contains("\")", StringComparison.Ordinal))
            || (TestFor(text, position + 1, CsTypeQuota) && text.Contains(CsTypeQuota2 + ")", StringComparison.Ordinal))
            ? 1024
            : 200;

        if (!Copy(text, position).Contains(')') || text.IndexOf(')', position) > position + maxLen)
        {
            Error(this, "Misspelled additional Entry");
        }

        position++;
        while (text.Length >= position && CharAt(text, position) != ')' && output.Length < maxLen)
        {
            output += CharAt(text, position);
            position++;
        }

        return true;
    }

    /// <summary>
    /// Removes a trailing month token from a place fragment when needed.
    /// </summary>
    public void TrimPlaceByMonth(ref string place)
    {
        var flag = false;
        for (var index = 1; index < CMonthKn.Length; index++)
        {
            if (place.EndsWith(CMonthKn[index], StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - CMonthKn[index].Length).Trim();
                flag = true;
                break;
            }

            var shortMonth = Copy(CMonthKn[index], 1, 3) + ".";
            if (place.EndsWith(shortMonth, StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - 4).Trim();
                flag = true;
                break;
            }
        }

        if (flag && (" " + place).EndsWith(" " + CsPlaceKenn6, StringComparison.Ordinal))
        {
            place = Copy(place, 1, place.Length - 3).Trim();
        }
    }

    /// <summary>
    /// Removes a trailing date modifier from a place fragment when needed.
    /// </summary>
    public void TrimPlaceByModif(ref string place)
    {
        for (var index = 1; index < CDateModif.Length; index++)
        {
            if ((" " + place).EndsWith(" " + CDateModif[index], StringComparison.Ordinal))
            {
                place = Copy(place, 1, place.Length - CDateModif[index].Length).Trim();
                break;
            }
        }
    }

    /// <summary>
    /// Determines the event type encoded by a free-text entry prefix.
    /// </summary>
    public ParserEventType GetEntryType(string subString, out string date, out string data)
    {
        var result = ParserEventType.evt_Last;
        data = string.Empty;
        date = string.Empty;

        if (TestEntry(subString, [CsBaptism, CsBaptism2], out date))
        {
            result = ParserEventType.evt_Baptism;
        }
        else if (TestEntry(subString, CsBirth, out date))
        {
            result = ParserEventType.evt_Birth;
        }
        else if (TestEntry(subString, [CsBurial, CsBurial2], out date))
        {
            result = ParserEventType.evt_Burial;
        }
        else if (TestEntry(subString, CMarriageKn, out date))
        {
            result = ParserEventType.evt_Marriage;
        }
        else if (TestEntry(subString, [CsDeathEntr, CsDeathEntr2], out date))
        {
            result = ParserEventType.evt_Death;
        }
        else if (TestEntry(subString, [CsDeathEntr + CsBirth, CsDeathEntr2 + CsBirth], out date))
        {
            result = ParserEventType.evt_Stillborn;
            data = "totgeboren";
        }
        else if (TestEntry(subString, CsDeathGefEntr, out date))
        {
            result = ParserEventType.evt_fallen;
            data = CsDeathGefEntr;
        }
        else if (TestEntry(subString, CsDivorce, out date))
        {
            result = ParserEventType.evt_Divorce;
        }
        else if (TestEntry(subString, [CsDeathVermEntr, CsDeathVermEntr2], out date))
        {
            result = ParserEventType.evt_missing;
            data = CsDeathVermEntr;
        }
        else if (subString.Contains(CsEmigration, StringComparison.Ordinal) || subString.Contains(CsEmigration2, StringComparison.Ordinal))
        {
            result = ParserEventType.evt_AddEmigration;
            var subString2 = subString.StartsWith("ist", StringComparison.Ordinal) ? RemoveStart(subString, 4) : subString;
            data = Left(subString2, subString2.Length - CsEmigration.Length - 1);
        }
        else if (subString.EndsWith(" " + CsAge, StringComparison.Ordinal))
        {
            result = ParserEventType.evt_Age;
            data = subString;
        }
        else if (TestEntry(subString, CArtikelU, out data))
        {
            result = ParserEventType.evt_Description;
            data = subString;
        }
        else if (subString.Contains(CsGenannt, StringComparison.Ordinal))
        {
            result = ParserEventType.evt_AKA;
            var subString2 = subString.StartsWith(CsWurde, StringComparison.Ordinal) ? RemoveStart(subString, CsWurde.Length).Trim() : subString;
            var pp = subString2.IndexOf(CsGenannt, StringComparison.Ordinal);
            data = (Left(subString2, pp).Trim() + " " + Copy(subString2, pp + CsGenannt.Length + 1).Trim()).Trim();
        }
        else if (TestEntry(subString, CArtikelB, out data))
        {
            result = ParserEventType.evt_AKA;
            data = subString;
        }
        else if (TestFor(subString, 1, CDescriptKn, out var found) && CDescriptKn[found] == subString)
        {
            result = ParserEventType.evt_Description;
            data = subString;
        }
        else if (TestFor(subString, 1, CResidenceKn, out found))
        {
            result = ParserEventType.evt_Residence;
            data = CResidenceKn[found];
            date = subString[data.Length..].Trim();
            if (date != string.Empty && !date.StartsWith(CsPlaceKenn + " ", StringComparison.Ordinal) && !date.StartsWith(CsDateModif4 + " ", StringComparison.Ordinal) && !Ziffern.Contains(date[0]))
            {
                data += " " + date;
                date = string.Empty;
            }
        }
        else if (IndexOfAny(subString, CPropertyKn) > 0)
        {
            result = ParserEventType.evt_Property;
            data = subString;
        }
        else if (IndexOfAny(subString, CAddressKn) > 0 && ((subString.Length > 0 && UpperCharset.Contains(subString[0])) || TestFor(subString, 1, CUmLautsU)) && CountChar(subString, ' ') < 2)
        {
            result = ParserEventType.evt_Residence;
            data = subString;
        }
        else if (TestFor(subString + '.', 1, CReligions, out found))
        {
            result = ParserEventType.evt_Religion;
            data = CReligions[found];
        }

        return result;
    }

    /// <summary>
    /// Analyses a generic entry and splits it into event type, data, place, and date fragments.
    /// </summary>
    public void AnalyseEntry(ref string subString, out ParserEventType entryType, out string data, out string place, out string date)
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

        var placBesch = false;
        var pp2 = -1;
        var found = -1;

        if (TestFor(subString, dPos + 2, CPlaceKn, out found))
        {
            place = Copy(subString, dPos + 2 + CPlaceKn[found].Length).Trim();
            placBesch = place == string.Empty || LowerCharset.Contains(place[0]);
            if (placBesch)
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
            var pp = (" " + subString).IndexOf(" " + CsPlaceKenn + " ", StringComparison.Ordinal);
            if (pp < 0)
            {
                pp = (" " + subString).IndexOf(" " + CsPlaceKenn6 + " ", StringComparison.Ordinal);
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

            var pl = 1;
            if (pp < 0)
            {
                pp = (CsProtectSpace + subString).IndexOf(CsProtectSpace + CsPlaceKenn + " ", StringComparison.Ordinal);
                if (pp >= 0)
                {
                    pl = CsProtectSpace.Length;
                    found = 0;
                }
            }

            pp2 = IndexOfAny(subString, Ziffern);
            if (pp >= 0 && pp2 < pp && (subString.Length < pp + CsPlaceKenn.Length + 3 || subString[pp + CsPlaceKenn.Length + 2] != 'd'))
            {
                place = Copy(subString, pp + 4);
                subString = Copy(subString, 1, pp - pl);
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
                pp = Pos(" " + CsPlaceKenn2 + " ", " " + subString);
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
            entryType = GetEntryType(subString + " " + CPlaceKn[found] + " " + place, out date, out data);
        }

        if (entryType == ParserEventType.evt_Description && place != string.Empty && TestFor(subString, 1, CArtikelU, out var found2))
        {
            entryType = ParserEventType.evt_Occupation;
            subString = Copy(subString, CArtikelU[found2].Length + 1).Trim();
        }

        var pp3Text = data.IndexOf(CsPlaceKenn3, StringComparison.Ordinal);
        if (pp3Text >= 0 && entryType == ParserEventType.evt_AddEmigration)
        {
            pp2 = IndexOfAny(data, Ziffern);
            place = Copy(data, pp3Text + CsPlaceKenn3.Length + 2).Trim();
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
                data += " " + CsEmigration;
            }
        }

        if (entryType == ParserEventType.evt_Property && !IsValidPlace(place))
        {
            data = data + " " + CPlaceKn[found] + " " + place;
            place = string.Empty;
        }

        if ((place.Trim() == string.Empty || !IsValidDate(date) || date.Length > 14)
            && entryType is >= ParserEventType.evt_Birth and <= ParserEventType.evt_Burial or ParserEventType.evt_Stillborn or ParserEventType.evt_fallen or ParserEventType.evt_missing
            && date.Length > 1
            && (UpperCharset.Contains(date[0]) || TestFor(date, 1, CUmLautsU) || TestFor(date, 1, CUnknownKn)))
        {
            var place0 = place.Trim() != string.Empty ? place : string.Empty;
            pp2 = IndexOfAny(date, Ziffern);
            if (TestFor(date, 1, CUnknownKn, out found2))
            {
                place = CUnknownKn[found2];
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
                    place = place0;
                }
                else if (place0.Trim() != string.Empty)
                {
                    place = place + " " + CPlaceKn[found] + " " + place0;
                }
            }
            else if (pp2 == -1 && place0 == string.Empty)
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
            if (date.StartsWith(CsMarrPartKn + " ", StringComparison.Ordinal) && pp2 < 0)
            {
                subString = date;
                date = string.Empty;
            }

            if (date.Contains(" " + CsMarrPartKn + " ", StringComparison.Ordinal) && pp2 >= 0)
            {
                subString = Copy(date, date.IndexOf(CsMarrPartKn, StringComparison.Ordinal) + 1);
                date = Copy(date, 1, date.Length - subString.Length).Trim();
            }
        }

        if (date.StartsWith(CsPlaceKenn6 + " ", StringComparison.Ordinal) || date.StartsWith(CsPlaceKenn4 + " ", StringComparison.Ordinal))
        {
            date = Copy(date, CsPlaceKenn4.Length + 2);
        }

        if (place.Trim() == string.Empty && !new[] { 55, 56, 57 }.Contains(_mode) && !placBesch && entryType != ParserEventType.evt_Divorce)
        {
            place = _defaultPlace;
        }

        if (!IsValidPlace(place))
        {
            Warning(this, "Misspelled Place \"" + place + "\"");
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

    /// <summary>
    /// Handles an entry that is not a person name and emits the corresponding callbacks.
    /// </summary>
    public ParserEventType HandleNonPersonEntry(string subString, string individualId, string famRef = "")
    {
        Debug(this, "HNPE: \"" + subString + "\"");
        var localSubString = subString.Trim();
        var localFamRef = famRef;
        if (localSubString == string.Empty || localSubString == ".")
        {
            return ParserEventType.evt_Anull;
        }

        AnalyseEntry(ref localSubString, out var entryType, out var data, out var place, out var date);
        if (entryType == ParserEventType.evt_Stillborn)
        {
            SetIndiDate(individualId, ParserEventType.evt_Birth, date);
            SetIndiData(individualId, ParserEventType.evt_Birth, "totgeboren");
            if (place != string.Empty)
            {
                SetIndiPlace(individualId, ParserEventType.evt_Birth, place);
            }

            SetIndiDate(individualId, ParserEventType.evt_Death, date);
            if (place != string.Empty)
            {
                SetIndiPlace(individualId, ParserEventType.evt_Death, place);
            }
        }
        else if (entryType is ParserEventType.evt_Marriage or ParserEventType.evt_Divorce)
        {
            if (localFamRef == string.Empty)
            {
                localFamRef = Ziffern.Contains(individualId[^1]) ? individualId[1..] + "P0" : individualId[1..] + "0";
            }

            StartFamily(localFamRef);
            char sex;
            if (localSubString.StartsWith(CsMarrPartKn + " ", StringComparison.Ordinal))
            {
                _ = HandleAKPersonEntry(Copy(localSubString, 4), localFamRef, 'U', 57, out _, out sex);
            }
            else
            {
                sex = 'F';
            }

            SetFamilyMember(localFamRef, individualId, sex == 'M' ? 2 : 1);
            if (date != string.Empty)
            {
                SetFamilyDate(localFamRef, entryType, date);
            }

            if (place != string.Empty)
            {
                SetFamilyPlace(localFamRef, entryType, place);
            }

            if (data != string.Empty || (date == string.Empty && place == string.Empty))
            {
                SetFamilyData(localFamRef, entryType, data);
            }
        }
        else if (entryType > ParserEventType.evt_ID && entryType != ParserEventType.evt_Last && entryType != ParserEventType.evt_Occupation)
        {
            if (date != string.Empty)
            {
                SetIndiDate(individualId, entryType, date);
            }

            if (place != string.Empty)
            {
                SetIndiPlace(individualId, entryType, place);
            }

            if (data != string.Empty)
            {
                SetIndiData(individualId, entryType, data);
            }
        }
        else
        {
            var emitLedigPlace = false;
            if (TryConsumeLeadingEntry(ref localSubString, CLedigKn, out var rest))
            {
                localSubString = rest;
                SetIndiData(individualId, ParserEventType.evt_Description, CsLedig);
                emitLedigPlace = true;
            }

            if (TryConsumeLeadingEntry(ref localSubString, CArtikelU, out rest))
            {
                localSubString = rest;
            }

            entryType = ParserEventType.evt_Occupation;
            if (TestFor(localSubString.Trim(), 1, CTitel))
            {
                SetIndiName(individualId, 4, localSubString.Trim());
            }
            else if (localSubString != string.Empty)
            {
                SetIndiOccu(individualId, entryType, localSubString.Trim());
            }
            else if (date != string.Empty)
            {
                Warning(this, "Entry contains no Marker, only a Date");
            }

            if (date != string.Empty)
            {
                SetIndiDate(individualId, entryType, date);
            }

            if (place != string.Empty)
            {
                SetIndiPlace(individualId, entryType, place);
            }
            else if (emitLedigPlace && _defaultPlace != string.Empty)
            {
                SetIndiPlace(individualId, entryType, _defaultPlace);
            }
        }

        return entryType;
    }

    /// <summary>
    /// Handles a family fact entry.
    /// </summary>
    public void HandleFamilyFact(string mainFamRef, string famEntry)
    {
        Debug(this, "HFF: \"" + famEntry + "\"");
        var subString = famEntry.Trim();
        if (subString == string.Empty || subString == ".")
        {
            return;
        }

        AnalyseEntry(ref subString, out var entryType, out var data, out var place, out var date);
        if (date != string.Empty)
        {
            SetFamilyDate(mainFamRef, entryType, date);
        }

        if (place != string.Empty && entryType != ParserEventType.evt_Last)
        {
            SetFamilyPlace(mainFamRef, entryType, place);
        }

        if (data != string.Empty || (date == string.Empty && place == string.Empty && entryType != ParserEventType.evt_Last))
        {
            SetFamilyData(mainFamRef, entryType, data);
        }
        else if (subString != string.Empty && entryType == ParserEventType.evt_Last)
        {
            if (place != string.Empty)
            {
                SetFamilyPlace(mainFamRef, ParserEventType.evt_Residence, place);
            }

            SetFamilyData(mainFamRef, ParserEventType.evt_Residence, subString);
        }
    }

    /// <summary>
    /// Handles a GC date entry.
    /// </summary>
    public bool HandleGCDateEntry(string text, ref int position, string individualId, ref int mode, ref int retMode, ref ParserEventType entryType)
    {
#if DEBUG
        Debug(this, "HGDE: \"" + Copy(text, position, 30) + "\"");
#endif
        if (TestFor(text, position, CsBirth)
            || TestFor(text, position, CsBaptism2)
            || TestFor(text, position, CsDeathEntr)
            || TestFor(text, position, CsBurial2))
        {
            retMode = mode;
            mode = 101;
            if (TestFor(text, position, CsBirth))
            {
                entryType = ParserEventType.evt_Birth;
            }
            else if (TestFor(text, position, CsBaptism2))
            {
                entryType = ParserEventType.evt_Baptism;
            }
            else if (TestFor(text, position, CsDeathEntr))
            {
                entryType = ParserEventType.evt_Death;
            }
            else
            {
                entryType = ParserEventType.evt_Burial;
            }

            return true;
        }

        if (TestForC(text, position, CsDeathGefEntr))
        {
            retMode = mode;
            mode = 101;
            position += CsDeathGefEntr.Length - 1;
            SetIndiData(individualId, ParserEventType.evt_Death, CsDeathGefEntr);
            entryType = ParserEventType.evt_Death;
            return true;
        }

        if (TestForC(text, position, CsDeathVermEntr))
        {
            retMode = mode;
            mode = 101;
            position += CsDeathVermEntr.Length - 1;
            SetIndiData(individualId, ParserEventType.evt_Death, CsDeathVermEntr);
            entryType = ParserEventType.evt_Death;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Handles a GC non-person entry.
    /// </summary>
    public bool HandleGCNonPersonEntry(string subString, char actChar, string individualId)
    {
        var localSubstring = subString;
        if (((localSubstring.Length < 4) && localSubstring.EndsWith(".", StringComparison.Ordinal) && localSubstring.Length > 0 && char.IsLower(localSubstring[0]))
            || localSubstring.Trim().Length == 2)
        {
            if (actChar == '.')
            {
                localSubstring += '.';
            }

            SetIndiData(individualId, ParserEventType.evt_Religion, localSubstring.Trim());
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
            place = _defaultPlace;
        }

        foreach (var data in localSubstring.Split(','))
        {
            var trimmedData = data.Trim();
            if (trimmedData == "Bürger")
            {
                SetIndiData(individualId, ParserEventType.evt_Residence, trimmedData);
                if (place != string.Empty)
                {
                    SetIndiPlace(individualId, ParserEventType.evt_Residence, place.Trim());
                }
            }
            else if (trimmedData == CsEmigration)
            {
                SetIndiData(individualId, ParserEventType.evt_AddEmigration, string.Empty);
                if (place != string.Empty)
                {
                    SetIndiPlace(individualId, ParserEventType.evt_AddEmigration, place.Trim());
                }
            }
            else if (TestFor(trimmedData, 1, CTitel))
            {
                SetIndiName(individualId, 4, trimmedData);
                if (place != string.Empty)
                {
                    SetIndiPlace(individualId, ParserEventType.evt_Occupation, place.Trim());
                }
            }
            else
            {
                SetIndiOccu(individualId, ParserEventType.evt_Occupation, trimmedData);
                if (place != string.Empty)
                {
                    SetIndiPlace(individualId, ParserEventType.evt_Occupation, place.Trim());
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Builds a name token fragment from the current text position.
    /// </summary>
    public bool BuildName2(string text, ref int offset, ref int charCount, ref string subString, out string additional)
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
        else if (TestFor(text, offset, CsProtectSpace))
        {
            subString += CsProtectSpace;
            offset += CsProtectSpace.Length - 1;
            charCount = 0;
        }
        else if (Copy(text, offset, CsUnknown.Length) == CsUnknown)
        {
            subString += CsUnknown;
            offset += CsUnknown.Length - 1;
            charCount = 0;
        }
        else if (currentChar == '.' && ((offset + 1 <= text.Length && new[] { ',', '.', ' ', '>' }.Contains(CharAt(text, offset + 1))) || TestFor(text, offset + 1, CsSeparator2) || (offset > 1 && In(CharAt(text, offset - 1), Charset.Concat(['.'])) && (charCount <= 3 || (offset + 1 <= text.Length && In(CharAt(text, offset + 1), UpperCharset))))))
        {
            subString += currentChar;
        }
        else if (TestFor(text, offset, ["-"], out var found) && offset > 1 && offset + 1 + found <= text.Length && In(CharAt(text, offset - 1), LowerCharset) && In(CharAt(text, offset + 1 + found), UpperCharset))
        {
            subString += currentChar;
            charCount = 0;
        }
        else if (TestFor(text, offset, ["-", "­"], out found) && offset > 1 && offset + 1 + found <= text.Length && In(CharAt(text, offset - 1), LowerCharset) && In(CharAt(text, offset + 1 + found), LowerCharset))
        {
            if (found == 0)
            {
                Warning(this, "Hyphen in Name Ignored");
            }

            charCount = 0;
            offset += found;
        }
        else if (TestFor(text, offset, _umlauts, out found))
        {
            subString += _umlauts[found];
            charCount++;
            offset += _umlauts[found].Length - 1;
        }
        else if (TestFor(text, offset, CsSeparator2))
        {
            offset += CsSeparator2.Length - 1;
            return false;
        }
        else
        {
            return TestFor(text, offset, "(") && ParseAdditional(text, ref offset, out additional);
        }

        return true;
    }

    /// <summary>
    /// Handles a person-name entry including title, maiden name, family membership, and inferred sex.
    /// </summary>
    public string HandleAKPersonEntry(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "")
    {
        Debug(this, "HANE: \"" + personEntry + "\"");
        var localAka = aka;
        var personName = !personEntry.Contains(CsUnknown2, StringComparison.Ordinal)
            ? personEntry.Replace("  ", " ", StringComparison.Ordinal).Replace(". ", ".", StringComparison.Ordinal).Replace(".", ". ", StringComparison.Ordinal)
            : (personEntry + " ").Replace("  ", " ", StringComparison.Ordinal);
        var names = personName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var title = string.Empty;
        if (TestFor(personName, 1, _akkaTitel, out var found))
        {
            title = _akkaTitel[found];
            personName = Copy(personName, title.Length + 2);
            names = personName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }

        if (names.Length > 0 && !names[^1].EndsWith(".", StringComparison.Ordinal) && GuessSexOfGivnName(names[^1], false) != '_' && localAka.StartsWith("?", StringComparison.Ordinal) && localAka.Length > 3)
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

        var spouseLastName = string.Empty;
        var marriageFlag = false;
        for (var index = 1; index < names.Length - 1; index++)
        {
            if (names[index] == CsMaidenNameKn)
            {
                if (index > 1 && mode != 56)
                {
                    spouseLastName = names[index - 1];
                    personName = personName.Replace(" " + spouseLastName, string.Empty, StringComparison.Ordinal);
                }

                personName = personName.Replace(" " + CsMaidenNameKn, string.Empty, StringComparison.Ordinal);
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

        if (!lastName.EndsWith(".", StringComparison.Ordinal) && GuessSexOfGivnName(lastName, false) != '_' && localAka.EndsWith(".", StringComparison.Ordinal) && localAka.Length < 4 && localAka == Copy(famName, 1, localAka.Length - 1) + ".")
        {
            lastName = famName;
            personName += " " + lastName;
            localAka = "? " + lastName;
        }

        if (lastName != string.Empty && (UpperCharset.Contains(lastName[0]) || lastName[0] == 'Ü') && lastName.EndsWith(".", StringComparison.Ordinal) && lastName.Length <= 4 && Copy(famName, 1, lastName.Length - 1) + "." == lastName)
        {
            personName = personName.Replace(lastName + " ", famName, StringComparison.Ordinal);
            lastName = famName;
        }
        else
        {
            personName = personName.Trim();
        }

        if (personType == 'U' && Copy(personName, 1, personName.Length - lastName.Length - 1) != CsUnknown2)
        {
            personSex = GuessSexOfGivnName(Copy(personName, 1, personName.Length - lastName.Length - 1));
        }
        else
        {
            personSex = personType;
        }

        if (personSex is 'M' or 'F')
        {
            LearnSexOfGivnName(Copy(personName, 1, personName.Length - lastName.Length - 1), personType);
        }

        var result = "I" + mainFamRef + personSex;
        if (marriageFlag)
        {
            SetFamilyType(mainFamRef, 1);
        }

        SetIndiName(result, 0, personName);
        if (title != string.Empty)
        {
            SetIndiName(result, 4, title);
        }

        SetFamilyMember(mainFamRef, result, personSex == 'F' ? 2 : 1);
        SetIndiData(result, ParserEventType.evt_Sex, personSex.ToString(CultureInfo.InvariantCulture));
        if (localAka != string.Empty)
        {
            SetIndiName(result, 3, localAka.Trim());
        }

        return result;
    }

    /// <summary>
    /// Scans for a child event date following a <c>Kd:</c> or <c>Kdr:</c> marker.
    /// </summary>
    public string ScanForEvDate(string text, int offset)
    {
        var result = string.Empty;
        var pos = Pos("Kd:", text, offset);
        if (pos > 0)
        {
            pos--;
        }
        else
        {
            pos = Pos("Kdr:", text, offset);
        }

        if (pos <= 0)
        {
            return result;
        }

        pos += 4;
        var offs = pos;
        var found = -1;
        var ziffCount = 0;
        while (offs < text.Length && ((In(CharAt(text, offs), Charset.Concat(WhiteSpaceChars).Concat([',']))) || TestFor(text, offs, _umlauts, out found)))
        {
            if (found >= 0)
            {
                offs++;
            }

            offs++;
            found = -1;
        }

        if (TestFor(text, offs, CsDeathEntr))
        {
            offs += CsDeathEntr.Length;
        }

        if (TestFor(text, offs, CsBirth))
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
