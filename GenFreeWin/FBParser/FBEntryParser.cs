using System;
using FBParser.Analysis;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using static FBParser.PascalCompat;
using System.Collections;

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
    private static readonly string[] CLedigKn = [$"{CsLedig}er", $"{CsLedig}e", CsLedigAbb, CsLedig + "."];
    private static readonly string[] CArtikelU = [CsArtikelU2, CsArtikelU1];
    private static readonly string[] CDeathEntries = [CsDeathEntr2, CsDeathEntr];

    private string _defaultPlace = string.Empty;
    private string _lastErr = string.Empty;
    private string _mainRef = string.Empty;
    private int _mode;
    private string[] _umlauts;
    private string[] _akkaTitel;
    private readonly IGenealogicalEventEmitter _eventEmitter;
    private readonly IGenealogicalEntryAnalyzer _entryAnalyzer;
    private readonly IGenealogicalFactHandler _factHandler;
    private readonly IGenealogicalPersonNameHandler _personNameHandler;
    private readonly IGenealogicalNameTokenBuilder _nameTokenBuilder;
    private readonly IGenealogicalGcHelper _gcHelper;
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
        _eventEmitter = CreateEventEmitter();
        _entryAnalyzer = CreateEntryAnalyzer();
        _factHandler = CreateFactHandler();
        _personNameHandler = CreatePersonNameHandler();
        _nameTokenBuilder = CreateNameTokenBuilder();
        _gcHelper = CreateGcHelper();
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
                    var localGcMarriageWithoutSeparator = TestFor(text, localOffset, CsMarriageGc) && localSubstring.Length > 0 && In(localSubstring[^1], Ziffern);
                    if (In(CharAt(text, localOffset), WhiteSpaceChars.Concat([':']))
                        || localGcMarriageWithoutSeparator)
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

                        if (localGcMarriageWithoutSeparator)
                        {
                            localOffset--;
                        }
                    }
                    else
                    {
                        localSubstring += CharAt(text, localOffset);
                    }

                    break;
                case 2:
                    if (TestFor(text, localOffset, CsMarriageGc))
                    {
                        localSubstring = string.Empty;
                        localMode = 3;
                        localFamName = string.Empty;
                        localPersonType = 'U';
                        localOffset--;
                    }
                    else if (localOffset > 2
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
                        else if (TestFor(localSubstring, 1, CDeathEntries))
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

                    if (BuildName(text, ref localOffset, ref localSubstring, ref localData, ref localCharCount, ref localAka, ref localAddEvent))
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

                    if (BuildData(localIndId, text, ref localOffset, ref localSubstring, ref localData, localMainFamRef, localLastZiffCount, ref localFamDatFlag, ref localEntryEndFlag, ref localEntryType))
                    {
                        if (Right(localSubstring.Trim(), 4) == " alt" && localSubstring.Trim().Length > 0 && In(localSubstring.Trim()[0], Ziffern) && localEntryType == ParserEventType.evt_Death)
                        {
                            SetIndiData(localIndId, localEntryType, localSubstring.Trim());
                            localSubstring = string.Empty;
                            continue;
                        }

                        localEntryType = HandleNonPersonEntry(localSubstring, localIndId, previousEntryType: localEntryType);
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
                            HandleNonPersonEntry(localSubstring, localIndId, previousEntryType: localEntryType);
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
                case 9:
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

                    if (localSubstring == string.Empty && localFirstEntry)
                    {
                        localAka = string.Empty;
                        localLastZiffCount = 0;
                    }

                    if (localFirstEntry && BuildName(text, ref localOffset, ref localSubstring, ref localData, ref localCharCount, ref localAka, ref localAddEvent))
                    {
                        localPersonGName = localSubstring.Trim();
                        localPersonName = localSubstring.Trim() + " " + localFamName;
                        localPersonSex = GuessSexOfGivnName(localSubstring.Trim());

                        if (CharAt(text, localOffset) == '<')
                        {
                            localRetMode = localMode;
                            localMode = 50;
                            localChRef = string.Empty;
                            localPos = localOffset + 1;
                            localPersonSex = GuessSexOfGivnName(localPersonGName);
                            if (!TestFor(text, localOffset + 1, ["vgl", "s.a"]))
                            {
                                while (localPos <= text.Length && (In(CharAt(text, localPos), Ziffern) || CharAt(text, localPos) is 'a' or 'b'))
                                {
                                    localChRef += CharAt(text, localPos);
                                    localPos++;
                                }

                                if (!TestReferenz(localChRef))
                                {
                                    localIndId = "I" + localMainFamRef + "C" + localChildCount;
                                }
                                else if (localPersonSex is 'M' or 'F')
                                {
                                    localIndId = "I" + localChRef + localPersonSex;
                                }
                                else
                                {
                                    localIndId = "I" + localChRef + "_";
                                }
                            }
                            else
                            {
                                localIndId = "I" + localMainFamRef + "C" + localChildCount;
                            }
                        }
                        else
                        {
                            localIndId = "I" + localMainFamRef + "C" + localChildCount;
                        }

                        SetIndiName(localIndId, 0, localPersonName);
                        SetFamilyMember(localMainFamRef, localIndId, 2 + localChildCount);
                        if (localPersonSex is 'M' or 'F')
                        {
                            SetIndiData(localIndId, ParserEventType.evt_Sex, localPersonSex.ToString());
                        }

                        if (localAka != string.Empty)
                        {
                            SetIndiName(localIndId, 3, localAka);
                        }

                        localAdditional = string.Empty;
                        localChildCount++;
                        localSubstring = string.Empty;
                        if (In(CharAt(text, localOffset), Ziffern))
                        {
                            localSubstring = localDefaultBirthPlace != string.Empty
                                ? CsBirth + " " + localDefaultBirthPlace + " " + CharAt(text, localOffset)
                                : CsBirth + " " + CharAt(text, localOffset);
                        }
                        else if (!WhiteSpaceChars.Concat(SatzZeichen).Contains(CharAt(text, localOffset)))
                        {
                            localOffset--;
                        }

                        localFirstEntry = false;
                    }
                    else if (!localFirstEntry && BuildData(localIndId, text, ref localOffset, ref localSubstring, ref localData, localMainFamRef, localLastZiffCount, ref localFamDatFlag, ref localEntryEndFlag, ref localEntryType))
                    {
                        localEntryType = HandleNonPersonEntry(localSubstring, localIndId);
                        if (localData != string.Empty)
                        {
                            SetIndiData(localIndId, localEntryType, localData);
                            localData = string.Empty;
                        }

                        localSubstring = string.Empty;
                        localLastZiffCount = 0;
                    }

                    if (CharAt(text, localOffset) == '<')
                    {
                        localRetMode = localMode;
                        localMode = 50;
                    }

                    if (CharAt(text, localOffset) == '-' && localOffset > 1 && WhiteSpaceChars.Contains(CharAt(text, localOffset - 1)))
                    {
                        if (localData != string.Empty)
                        {
                            SetIndiData(localIndId, ParserEventType.evt_FreeFact, localData);
                            localData = string.Empty;
                        }

                        if (!localEntryEndFlag)
                        {
                            Error(this, "Child (" + (localChildCount - 1) + ") entry not ended with .");
                        }

                        localSubstring = string.Empty;
                        localAdditional = string.Empty;
                        localAka = string.Empty;
                        localFirstEntry = true;
                        localEntryEndFlag = false;
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
                            else if (CharAt(text, localOffset) != ':')
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

                    if (BuildData(localIndId, text, ref localOffset, ref localSubstring, ref localData, localMainFamRef, localLastZiffCount, ref localFamDatFlag, ref localEntryEndFlag, ref localEntryType))
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
                case 20:
                    if (In(CharAt(text, localOffset), Ziffern.Concat(['.'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else
                    {
                        if (localSubstring.Length > 2)
                        {
                            localEventDate = localSubstring;
                            localSubstring = string.Empty;
                            localPlace = _defaultPlace;
                            localMode = 11;
                        }

                        if (CharAt(text, localOffset) == ':')
                        {
                            localPlace = _defaultPlace;
                            localMode = 5;
                        }
                    }

                    break;
                case 25:
                    if (In(CharAt(text, localOffset), Ziffern.Concat(['.'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else
                    {
                        if (localSubstring.Length > 2)
                        {
                            localEventDate = localSubstring;
                            localSubstring = string.Empty;
                            localPlace = _defaultPlace;
                            localMode = 11;
                        }

                        if (CharAt(text, localOffset) == ':')
                        {
                            localPlace = _defaultPlace;
                            localMode = 5;
                        }
                    }

                    break;
                case 30:
                    if (In(CharAt(text, localOffset), Ziffern.Concat(['.'])))
                    {
                        localSubstring += CharAt(text, localOffset);
                    }
                    else
                    {
                        if (localSubstring.Length > 2)
                        {
                            localEventDate = localSubstring;
                            localSubstring = string.Empty;
                            localEntryType = ParserEventType.evt_Partner;
                            localPlace = string.Empty;
                            localMode = 11;
                        }

                        if (CharAt(text, localOffset) == ':')
                        {
                            localMode = 5;
                            localEntryType = ParserEventType.evt_Partner;
                            if (localEventDate == string.Empty)
                            {
                                localEventDate = ScanForEvDate(text, localOffset);
                            }

                            if (localEventDate != string.Empty)
                            {
                                SetFamilyDate(localMainFamRef, localEntryType, localEventDate);
                            }
                        }
                    }

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

                    if (localFirstEntry && TestFor(text, localOffset, CDeathEntries, out localFound))
                    {
                        localSubstring += Copy(text, localOffset, CDeathEntries[localFound].Length );
                        localOffset += CDeathEntries[localFound].Length;
                    }
                    else if (localFirstEntry && BuildName(text, ref localOffset, ref localSubstring, ref localData, ref localCharCount, ref localAka, ref localAddEvent))
                    {
                        localPersonName = localSubstring.Trim();
                        if (TestFor(localPersonName, 1, CDeathEntries , out localFound))
                        {
                            localPersonName = Copy(localPersonName, CDeathEntries[localFound].Length + 1, 200).Trim();
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
                    else if (!localFirstEntry && BuildData(localIndId2, text, ref localOffset, ref localSubstring, ref localData, localMainFamRef, localLastZiffCount, ref localFamDatFlag, ref localEntryEndFlag, ref localEntryType))
                    {
                        localEntryType2 = HandleNonPersonEntry(localSubstring.Trim(), localIndId2, previousEntryType: localEntryType2);
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
                       
                        if (TestFor(localPersonName, 1, CDeathEntries, out localFound))
                        {
                            localPersonName = Copy(localPersonName, CDeathEntries[localFound].Length + 1, 200).Trim();
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
                                HandleNonPersonEntry(localSubstring, localIndId2, previousEntryType: localEntryType2);
                            }
                            else
                            {
                                foreach (var entry in localSubstring.Split(','))
                                {
                                    localEntryType2 = HandleNonPersonEntry(entry, localIndId2, previousEntryType: localEntryType2);
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
                                HandleNonPersonEntry(localSubstring[..localPos], localIndId, previousEntryType: localEntryType);
                                localSubstring = localSubstring[(localPos + 1)..];
                            }
                        }

                        HandleNonPersonEntry(localSubstring, localIndId, localFamRef, localEntryType);
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
                            SetIndiRef(localIndId, localEntryType, localSubstring);
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

    private bool BuildName(string innerText, ref int innerOffset, ref string innerSubstring, ref string innerData, ref int localCharCount, ref string? localAka, ref ParserEventType localAddEvent)
        => _nameTokenBuilder.BuildName(innerText, ref innerOffset, ref innerSubstring, ref innerData, ref localCharCount, ref localAka, ref localAddEvent);

    private bool BuildData(string individualId, string innerText, ref int innerOffset, ref string innerSubstring, ref string innerData, string? localMainFamRef, int localLastZiffCount, ref bool localFamDatFlag, ref bool localEntryEndFlag, ref ParserEventType localEntryType)
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
        => _eventEmitter.SetFamilyMember(famRef, individualId, famMember);

    private void SetFamilyDate(string famRef, ParserEventType eventType, string date)
        => _eventEmitter.SetFamilyDate(famRef, eventType, date);

    private void SetFamilyPlace(string famRef, ParserEventType eventType, string place)
        => _eventEmitter.SetFamilyPlace(famRef, eventType, place);

    private void SetFamilyData(string famRef, ParserEventType eventType, string data)
        => _eventEmitter.SetFamilyData(famRef, eventType, data);

    private void SetIndiName(string individualId, int nameType, string name)
        => _eventEmitter.SetIndiName(individualId, nameType, name);

    private void SetIndiData(string individualId, ParserEventType eventType, string data)
        => _eventEmitter.SetIndiData(individualId, eventType, data);

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
        => _eventEmitter.SetIndiDate(individualId, eventType, date);

    private void SetIndiPlace(string individualId, ParserEventType eventType, string place)
        => _eventEmitter.SetIndiPlace(individualId, eventType, place);

    private void SetIndiOccu(string individualId, ParserEventType eventType, string occu)
        => _eventEmitter.SetIndiOccu(individualId, eventType, occu);

    private void SetIndiRelat(string individualId, string famRef, int relType)
        => _eventEmitter.SetIndiRelat(individualId, famRef, relType, _mainRef);

    private void SetIndiRef(string individualId, ParserEventType eventType, string reference)
        => _eventEmitter.SetIndiRef(individualId, eventType, reference);

    private void StartFamily(string famRef)
        => _eventEmitter.StartFamily(famRef);

    private void SetFamilyType(string famRef, int famType, string data = "")
        => _eventEmitter.SetFamilyType(famRef, famType, data);

    private void EndOfEntry(string famRef)
        => _eventEmitter.EndOfEntry(famRef);

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
        => _entryAnalyzer.TrimPlaceByMonth(ref place);

    /// <summary>
    /// Removes a trailing date modifier from a place fragment when needed.
    /// </summary>
    public void TrimPlaceByModif(ref string place)
        => _entryAnalyzer.TrimPlaceByModif(ref place);

    /// <summary>
    /// Determines the event type encoded by a free-text entry prefix.
    /// </summary>
    public ParserEventType GetEntryType(string subString, out string date, out string data)
        => _entryAnalyzer.GetEntryType(subString, out date, out data);

    /// <summary>
    /// Analyses a generic entry and splits it into event type, data, place, and date fragments.
    /// </summary>
    public void AnalyseEntry(ref string subString, out ParserEventType entryType, out string data, out string place, out string date)
        => _entryAnalyzer.AnalyseEntry(ref subString, _defaultPlace, _mode, out entryType, out data, out place, out date);

    private IGenealogicalEntryAnalyzer CreateEntryAnalyzer()
        => new GenealogicalEntryAnalyzer(
            new GenealogicalEntryAnalyzerConfiguration
            {
                ProtectSpace = CsProtectSpace,
                BirthMarker = CsBirth,
                BaptismMarkers = [CsBaptism, CsBaptism2],
                BurialMarkers = [CsBurial, CsBurial2],
                MarriageMarkers = CMarriageKn,
                DeathMarkers = [CsDeathEntr, CsDeathEntr2],
                StillbornMarkers = [CsDeathEntr + CsBirth, CsDeathEntr2 + CsBirth],
                FallenMarker = CsDeathGefEntr,
                DivorceMarker = CsDivorce,
                MissingMarkers = [CsDeathVermEntr, CsDeathVermEntr2],
                EmigrationMarkers = [CsEmigration, CsEmigration2],
                DateModifiers = CDateModif,
                SinceDateModifier = CsDateModif4,
                AgeMarker = CsAge,
                IndefiniteArticles = CArtikelU,
                DefiniteArticles = CArtikelB,
                AkaMarker = CsGenannt,
                BecameMarker = CsWurde,
                DescriptionMarkers = CDescriptKn,
                ResidenceMarkers = CResidenceKn,
                PlaceMarkers = CPlaceKn,
                UnknownMarkers = CUnknownKn,
                PropertyMarkers = CPropertyKn,
                AddressMarkers = CAddressKn,
                ReligionMarkers = CReligions,
                MonthNames = CMonthKn,
                UpperUmlautMarkers = CUmLautsU,
                InPlaceMarker = CsPlaceKenn,
                ToPlaceMarker = CsPlaceKenn3,
                FromPlaceMarker = CsPlaceKenn2,
                InMonthPlaceMarker = CsPlaceKenn6,
                OnDatePlaceMarker = CsPlaceKenn4,
                MarriagePartnerMarker = CsMarrPartKn,
                IsValidDate = IsValidDate,
                IsValidPlace = IsValidPlace,
                Warning = message => Warning(this, message),
            });

    private IGenealogicalEventEmitter CreateEventEmitter()
        => new GenealogicalEventEmitter(
            new GenealogicalEventEmitterConfiguration
            {
                IsValidDate = IsValidDate,
                IsValidPlace = IsValidPlace,
                IsValidReference = TestReferenz,
                Error = message => Error(this, message),
                OnStartFamily = famRef => OnStartFamily?.Invoke(this, famRef, string.Empty, 0),
                OnEntryEnd = famRef => OnEntryEnd?.Invoke(this, string.Empty, famRef, -1),
                OnFamilyDate = (data, reference, subType) => OnFamilyDate?.Invoke(this, data, reference, subType),
                OnFamilyType = (data, reference, subType) => OnFamilyType?.Invoke(this, data, reference, subType),
                OnFamilyData = (data, reference, subType) => OnFamilyData?.Invoke(this, data, reference, subType),
                OnFamilyPlace = (data, reference, subType) => OnFamilyPlace?.Invoke(this, data, reference, subType),
                OnFamilyIndiv = (data, reference, subType) => OnFamilyIndiv?.Invoke(this, data, reference, subType),
                OnIndiName = (data, reference, subType) => OnIndiName?.Invoke(this, data, reference, subType),
                OnIndiDate = (data, reference, subType) => OnIndiDate?.Invoke(this, data, reference, subType),
                OnIndiPlace = (data, reference, subType) => OnIndiPlace?.Invoke(this, data, reference, subType),
                OnIndiOccu = (data, reference, subType) => OnIndiOccu?.Invoke(this, data, reference, subType),
                OnIndiRel = (data, reference, subType) => OnIndiRel?.Invoke(this, data, reference, subType),
                OnIndiRef = (data, reference, subType) => OnIndiRef?.Invoke(this, data, reference, subType),
                OnIndiData = (data, reference, subType) => OnIndiData?.Invoke(this, data, reference, subType),
            });

    /// <summary>
    /// Handles an entry that is not a person name and emits the corresponding callbacks.
    /// </summary>
    public ParserEventType HandleNonPersonEntry(string subString, string individualId, string famRef = "", ParserEventType previousEntryType = ParserEventType.evt_Anull)
    {
        Debug(this, "HNPE: \"" + subString + "\"");
        return _factHandler.HandleNonPersonEntry(subString, individualId, _defaultPlace, famRef, previousEntryType);
    }

    /// <summary>
    /// Handles a family fact entry.
    /// </summary>
    public void HandleFamilyFact(string mainFamRef, string famEntry)
    {
        Debug(this, "HFF: \"" + famEntry + "\"");
        _factHandler.HandleFamilyFact(mainFamRef, famEntry);
    }

    private IGenealogicalFactHandler CreateFactHandler()
        => new GenealogicalFactHandler(
            new GenealogicalFactHandlerConfiguration
            {
                LedigMarkers = CLedigKn,
                IndefiniteArticles = CArtikelU,
                TitleMarkers = CTitel,
                LedigText = CsLedig,
                MarriagePartnerMarker = CsMarrPartKn,
                AnalyseEntry = AnalyseEntry,
                StartFamily = StartFamily,
                SetFamilyMember = SetFamilyMember,
                SetFamilyDate = SetFamilyDate,
                SetFamilyPlace = SetFamilyPlace,
                SetFamilyData = SetFamilyData,
                SetIndiDate = SetIndiDate,
                SetIndiPlace = SetIndiPlace,
                SetIndiData = SetIndiData,
                SetIndiOccu = SetIndiOccu,
                SetIndiName = SetIndiName,
                SetFamilyType = SetFamilyType,
                HandleAkPersonEntry = HandleAKPersonEntry,
                TryConsumeLeadingEntry = TryConsumeLeadingEntry,
                TestFor = TestFor,
                Warning = message => Warning(this, message),
            });

    /// <summary>
    /// Handles a GC date entry.
    /// </summary>
    public bool HandleGCDateEntry(string text, ref int position, string individualId, ref int mode, ref int retMode, ref ParserEventType entryType)
    {
#if DEBUG
        Debug(this, "HGDE: \"" + Copy(text, position, 30) + "\"");
#endif
        return _gcHelper.HandleGcDateEntry(text, ref position, individualId, ref mode, ref retMode, ref entryType);
    }

    /// <summary>
    /// Handles a GC non-person entry.
    /// </summary>
    public bool HandleGCNonPersonEntry(string subString, char actChar, string individualId)
        => _gcHelper.HandleGcNonPersonEntry(subString, actChar, individualId);

    /// <summary>
    /// Builds a name token fragment from the current text position.
    /// </summary>
    public bool BuildName2(string text, ref int offset, ref int charCount, ref string subString, out string additional)
        => _nameTokenBuilder.BuildNameToken(text, ref offset, ref charCount, ref subString, out additional);

    /// <summary>
    /// Handles a person-name entry including title, maiden name, family membership, and inferred sex.
    /// </summary>
    public string HandleAKPersonEntry(string personEntry, string mainFamRef, char personType, int mode, out string lastName, out char personSex, string aka = "", string famName = "")
    {
        Debug(this, "HANE: \"" + personEntry + "\"");
        return _personNameHandler.HandleAkPersonEntry(personEntry, mainFamRef, personType, mode, out lastName, out personSex, aka, famName);
    }

    private IGenealogicalPersonNameHandler CreatePersonNameHandler()
        => new GenealogicalPersonNameHandler(
            new GenealogicalPersonNameHandlerConfiguration
            {
                UnknownShortMarker = CsUnknown2,
                MaidenNameMarker = CsMaidenNameKn,
                AcademicTitleMarkers = _akkaTitel,
                TestFor = TestFor,
                GuessSexOfGivenName = GuessSexOfGivnName,
                LearnSexOfGivenName = LearnSexOfGivnName,
                SetIndiName = SetIndiName,
                SetIndiData = SetIndiData,
                SetFamilyMember = SetFamilyMember,
                SetFamilyType = SetFamilyType,
            });

    private IGenealogicalNameTokenBuilder CreateNameTokenBuilder()
        => new GenealogicalNameTokenBuilder(
            new GenealogicalNameTokenBuilderConfiguration
            {
                ProtectSpaceMarker = CsProtectSpace,
                UnknownMarker = CsUnknown,
                TwinMarker = CsTwin,
                Separator2Marker = CsSeparator2,
                UmlautMarkers = _umlauts,
                TestFor = TestFor,
                ParseAdditional = ParseAdditional,
                Warning = message => Warning(this, message),
            });

    /// <summary>
    /// Scans for a child event date following a <c>Kd:</c> or <c>Kdr:</c> marker.
    /// </summary>
    public string ScanForEvDate(string text, int offset)
        => _gcHelper.ScanForEventDate(text, offset);

    private IGenealogicalGcHelper CreateGcHelper()
        => new GenealogicalGcHelper(
            new GenealogicalGcHelperConfiguration
            {
                BirthMarker = CsBirth,
                BaptismMarker = CsBaptism2,
                DeathMarker = CsDeathEntr,
                BurialMarker = CsBurial2,
                FallenMarker = CsDeathGefEntr,
                MissingMarker = CsDeathVermEntr,
                ChildDateMarker = "Kd:",
                ChildDateMarkerAlternate = "Kdr:",
                GetDefaultPlace = () => _defaultPlace,
                UmlautMarkers = _umlauts,
                TitleMarkers = CTitel,
                TestFor = TestFor,
                TestForCaseInsensitive = TestForC,
                SetIndiData = SetIndiData,
                SetIndiPlace = SetIndiPlace,
                SetIndiOccu = SetIndiOccu,
                SetIndiName = SetIndiName,
            });
}
