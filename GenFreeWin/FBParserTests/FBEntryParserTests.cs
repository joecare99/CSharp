using FBParser;
using Mono.Cecil.Rocks;

namespace FBParserTests;

[TestClass]
public sealed class FBEntryParserTests
{
    private static readonly string SampleDataPath = ResolveSampleDataPath();
    private static readonly string GNameFilePath = Path.Combine(SampleDataPath, "GNameFile.txt");

    [TestMethod]
    public void Constructor_InitializesDefaults()
    {
        using var sut = new FBEntryParser();

        Assert.IsNotNull(sut.GNameHandler);
        Assert.AreEqual(string.Empty, sut.DefaultPlace);
        Assert.AreEqual(string.Empty, sut.LastErr);
        Assert.AreEqual(string.Empty, sut.MainRef);
        Assert.AreEqual(0, sut.LastMode);
    }

    [TestMethod]
    public void DebugSetMsg_UpdatesState()
    {
        using var sut = new FBEntryParser();

        sut.DebugSetMsg("msg", "123", 7);

        Assert.AreEqual("msg", sut.LastErr);
        Assert.AreEqual("123", sut.MainRef);
        Assert.AreEqual(7, sut.LastMode);
    }

    [TestMethod]
    public void Error_RaisesDetailedMessageEvent()
    {
        using var sut = new FBEntryParser();
        ParseMessageKind? kind = null;
        string? message = null;
        sut.OnParseMessage += (_, k, m, _, _) =>
        {
            kind = k;
            message = m;
        };

        sut.Error(this, "broken");

        Assert.AreEqual(ParseMessageKind.Error, kind);
        Assert.AreEqual("broken", message);
        Assert.AreEqual("broken", sut.LastErr);
    }

    [TestMethod]
    public void Warning_UsesFallbackParseErrorEventWhenNoDetailedHandlerExists()
    {
        using var sut = new FBEntryParser();
        var raised = false;
        sut.OnParseError += (_, _) => raised = true;

        sut.Warning(this, "warn");

        Assert.IsTrue(raised);
        Assert.AreEqual("warn", sut.LastErr);
    }

    [TestMethod]
    public void Debug_RaisesDebugMessage()
    {
        using var sut = new FBEntryParser();
        ParseMessageKind? kind = null;
        sut.OnParseMessage += (_, k, _, _, _) => kind = k;

        sut.Debug(this, "trace");

        Assert.AreEqual(ParseMessageKind.Debug, kind);
        Assert.AreEqual("trace", sut.LastErr);
    }

    [TestMethod]
    [DataRow("12a", true)]
    [DataRow("1234", true)]
    [DataRow("a123", false)]
    [DataRow("12x", false)]
    [DataRow("", false)]
    public void TestReferenz_ValidatesExpectedFormats(string reference, bool expected)
    {
        using var sut = new FBEntryParser();
        Assert.AreEqual(expected, sut.TestReferenz(reference));
    }

    [TestMethod]
    [DataRow("12.03.1900", true)]
    [DataRow("vor 1900", true)]
    [DataRow("12,03,1900", false)]
    [DataRow("12:03", false)]
    public void IsValidDate_ReturnsExpectedResult(string value, bool expected)
    {
        using var sut = new FBEntryParser();
        Assert.AreEqual(expected, sut.IsValidDate(value));
    }

    [TestMethod]
    [DataRow("Berlin", true)]
    [DataRow("\"Berlin\"", true)]
    [DataRow("...", true)]
    [DataRow("berlin", false)]
    public void IsValidPlace_ReturnsExpectedResult(string value, bool expected)
    {
        using var sut = new FBEntryParser();
        Assert.AreEqual(expected, sut.IsValidPlace(value));
    }

    [TestMethod]
    public void TestEntry_WithSingleToken_ReturnsPayload()
    {
        using var sut = new FBEntryParser();

        var result = sut.TestEntry("* 12.03.1900", "*", out var data);

        Assert.IsTrue(result);
        Assert.AreEqual("12.03.1900", data);
    }

    [TestMethod]
    public void TestEntry_WithArray_FindsMatchingPrefix()
    {
        using var sut = new FBEntryParser();

        var result = sut.TestEntry("† 12.03.1900", ["*", "†"], out var data);

        Assert.IsTrue(result);
        Assert.AreEqual("12.03.1900", data);
    }

    [TestMethod]
    [DataRow("* 12.03.1900", ParserEventType.evt_Birth)]
    [DataRow("† 12.03.1900", ParserEventType.evt_Death)]
    [DataRow("⚭ 12.03.1900", ParserEventType.evt_Marriage)]
    [DataRow("o/o 12.03.1900", ParserEventType.evt_Divorce)]
    [DataRow("rk.", ParserEventType.evt_Religion)]
    public void GetEntryType_DetectsKnownEntryMarkers(string text, ParserEventType expected)
    {
        using var sut = new FBEntryParser();

        var result = sut.GetEntryType(text, out _, out _);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TrimPlaceByMonth_RemovesTrailingMonth()
    {
        using var sut = new FBEntryParser();
        var place = "Berlin März";

        sut.TrimPlaceByMonth(ref place);

        Assert.AreEqual("Berlin", place);
    }

    [TestMethod]
    public void TrimPlaceByModif_RemovesTrailingDateModifier()
    {
        using var sut = new FBEntryParser();
        var place = "Berlin vor";

        sut.TrimPlaceByModif(ref place);

        Assert.AreEqual("Berlin", place);
    }

    [TestMethod]
    public void ParseAdditional_ReadsTextInsideParentheses()
    {
        using var sut = new FBEntryParser();
        var position = 1;

        var result = sut.ParseAdditional("(Zw)", ref position, out var output);

        Assert.IsTrue(result);
        Assert.AreEqual("Zw", output);
        Assert.AreEqual(4, position);
    }

    [TestMethod]
    public void HandleGCDateEntry_RecognizesDeathKeyword()
    {
        using var sut = new FBEntryParser();
        var messages = new List<(string Text, string Reference, int SubType)>();
        sut.OnIndiData += (_, text, reference, subType) => messages.Add((text, reference, subType));
        var position = 1;
        var mode = 112;
        var retMode = 0;
        var eventType = ParserEventType.evt_Anull;

        var result = sut.HandleGCDateEntry("gefallen", ref position, "I1M", ref mode, ref retMode, ref eventType);

        Assert.IsTrue(result);
        Assert.AreEqual(101, mode);
        Assert.AreEqual(112, retMode);
        Assert.AreEqual(ParserEventType.evt_Death, eventType);
        Assert.HasCount(1, messages);
        Assert.AreEqual("gefallen", messages[0].Text);
    }

    [TestMethod]
    public void HandleGCNonPersonEntry_MapsReligionEntry()
    {
        using var sut = new FBEntryParser();
        var messages = new List<(string Text, int SubType)>();
        sut.OnIndiData += (_, text, _, subType) => messages.Add((text, subType));

        var result = sut.HandleGCNonPersonEntry("rk.", '.', "I1M");

        Assert.IsTrue(result);
        Assert.HasCount(1, messages);
        Assert.AreEqual("rk..", messages[0].Text);
        Assert.AreEqual((int)ParserEventType.evt_Religion, messages[0].SubType);
    }

    [TestMethod]
    public void HandleAKPersonEntry_EmitsNameSexAndFamilyMembership()
    {
        using var sut = new FBEntryParser();
        var names = new List<(string Text, string Reference, int SubType)>();
        var data = new List<(string Text, string Reference, int SubType)>();
        var familyMembers = new List<(string Text, string Reference, int SubType)>();
        sut.OnIndiName += (_, text, reference, subType) => names.Add((text, reference, subType));
        sut.OnIndiData += (_, text, reference, subType) => data.Add((text, reference, subType));
        sut.OnFamilyIndiv += (_, text, reference, subType) => familyMembers.Add((text, reference, subType));

        var id = sut.HandleAKPersonEntry("Anna Müller", "123", 'U', 5, out var lastName, out var personSex);

        Assert.AreEqual("I123U", id);
        Assert.AreEqual("Müller", lastName);
        Assert.AreEqual('U', personSex);
        Assert.IsTrue(names.Any(n => n.Text == "Anna Müller" && n.Reference == id && n.SubType == 0));
        Assert.IsTrue(data.Any(d => d.Reference == id && d.SubType == (int)ParserEventType.evt_Sex));
        Assert.IsTrue(familyMembers.Any(m => m.Reference == "123"));
    }

    [TestMethod]
    public void HandleNonPersonEntry_MapsOccupationWithDateAndPlace()
    {
        using var sut = new FBEntryParser();
        sut.DefaultPlace = "Bern";
        var occu = new List<(string Text, int SubType)>();
        var dates = new List<(string Text, int SubType)>();
        var places = new List<(string Text, int SubType)>();
        sut.OnIndiOccu += (_, text, _, subType) => occu.Add((text, subType));
        sut.OnIndiDate += (_, text, _, subType) => dates.Add((text, subType));
        sut.OnIndiPlace += (_, text, _, subType) => places.Add((text, subType));

        var result = sut.HandleNonPersonEntry("Schneider 12.03.1900", "I1M");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        Assert.IsTrue(occu.Any(o => o.Text == "Schneider"));
        Assert.IsTrue(dates.Any(d => d.Text == "12.03.1900"));
        Assert.IsTrue(places.Any(p => p.Text == "Bern"));
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithLedigOccupation_EmitsSeparatedDescriptionAndOccupation()
    {
        using var sut = new FBEntryParser();
        sut.DefaultPlace = "Meißenheim";
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        var result = sut.HandleNonPersonEntry("led.Rentnerin", "I61F");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        CollectionAssert.AreEqual(
        new List<ParseResult>()
        {
            new ParseResult("ParserIndiData", "ledig", "I61F", (int)ParserEventType.evt_Description),
            new ParseResult("ParserIndiOccu", "Rentnerin", "I61F", (int)ParserEventType.evt_Occupation),
            new ParseResult("ParserIndiPlace", "Meißenheim", "I61F", (int)ParserEventType.evt_Occupation),
        },
        filteredResults.ToList());
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithLedigOnly_EmitsDescriptionPlaceBeforeDescription()
    {
        using var sut = new FBEntryParser();
        sut.DefaultPlace = "Meißenheim";
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        var result = sut.HandleNonPersonEntry("ledig.", "I3657C4");

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        Assert.AreEqual(2, filteredResults.Count);
        Assert.AreEqual(new ParseResult("ParserIndiPlace", "Meißenheim", "I3657C4", (int)ParserEventType.evt_Description), filteredResults[0]);
        Assert.AreEqual(new ParseResult("ParserIndiData", "ledig", "I3657C4", (int)ParserEventType.evt_Description), filteredResults[1]);
    }

    [TestMethod]
    public void HandleNonPersonEntry_WithLedigOnlyAfterOccupation_EmitsOccupationPlaceThenDescription()
    {
        using var sut = new FBEntryParser();
        sut.DefaultPlace = "Meißenheim";
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        var result = sut.HandleNonPersonEntry("led.", "I100M", previousEntryType: ParserEventType.evt_Occupation);

        Assert.AreEqual(ParserEventType.evt_Occupation, result);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        Assert.AreEqual(2, filteredResults.Count);
        Assert.AreEqual(new ParseResult("ParserIndiPlace", "Meißenheim", "I100M", (int)ParserEventType.evt_Occupation), filteredResults[0]);
        Assert.AreEqual(new ParseResult("ParserIndiData", "ledig", "I100M", (int)ParserEventType.evt_Description), filteredResults[1]);
    }

    [TestMethod]
    public void HandleFamilyFact_EmitsResidenceFallbackData()
    {
        using var sut = new FBEntryParser();
        sut.DefaultPlace = "Bern";
        var familyData = new List<(string Text, int SubType)>();
        sut.OnFamilyData += (_, text, _, subType) => familyData.Add((text, subType));

        sut.HandleFamilyFact("123", "Unbekannte Angabe");

        Assert.IsNotEmpty(familyData);
    }

    [TestMethod]
    public void Feed_WithSimpleFamilyReferenceAndMissingMarker_StartsFamilyAndUpdatesMainRef()
    {
        using var sut = new FBEntryParser();
        var startedFamilies = new List<string>();
        sut.OnStartFamily += (_, text, _, _) => startedFamilies.Add(text);

        sut.Feed("12: fehlt" + Environment.NewLine);

        Assert.AreEqual("12", sut.MainRef);
        CollectionAssert.Contains(startedFamilies, "12");
    }

    [TestMethod]
    public void Feed_WithOriginalPascalGcSample_AtLeastStartsExpectedFamily()
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(UntTestFbDataSamples.TestEntryGc5065);

        Assert.AreEqual("5065", sut.MainRef);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        CollectionAssert.AreEqual(UntTestFbDataRegressionResults.Gc5065CurrentPrefix.ToList(), filteredResults.Take(UntTestFbDataRegressionResults.Gc5065CurrentPrefix.Count).ToList());
    }

    [TestMethod]
    public void Feed_WithOriginalPascalAkSample_AtLeastStartsExpectedFamily()
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(UntTestFbDataSamples.TestEntryAk2421);

        Assert.AreEqual("2421", sut.MainRef);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        CollectionAssert.AreEqual(UntTestFbDataRegressionResults.Ak2421CurrentPrefix.ToList(), filteredResults.Take(UntTestFbDataRegressionResults.Ak2421CurrentPrefix.Count).ToList());
    }

    [TestMethod]
    [DataRow("OsBM0061.entTxt")]
    [DataRow("OsBM0062.entTxt")]
    [DataRow("OsBM0101.entTxt")]
    [DataRow("OsBM0746.entTxt")]
    public void Feed_SelectedParitySamples_MatchExpectedResults(string filename)
        => AssertSampleMatchesExpectedResults(filename);

    [TestMethod]
    public void Feed_ParitySample_OsBM0061_MatchesExpectedResults()
        => AssertSampleMatchesExpectedResults("OsBM0061.entTxt");

    [TestMethod]
    public void Feed_ParitySample_OsBM0062_MatchesExpectedResults()
        => AssertSampleMatchesExpectedResults("OsBM0062.entTxt");

    [TestMethod]
    public void Feed_ParitySample_OsBM0101_MatchesExpectedResults()
        => AssertSampleMatchesExpectedResults("OsBM0101.entTxt");

    [TestMethod]
    public void Feed_ParitySample_OsBM0746_MatchesExpectedResults()
        => AssertSampleMatchesExpectedResults("OsBM0746.entTxt");

    private static void AssertSampleMatchesExpectedResults(string filename)
    {
        if (!Directory.Exists(SampleDataPath))
        {
            Assert.Inconclusive($"Sample path not found: {SampleDataPath}");
        }

        var entryPath = Path.Combine(SampleDataPath, filename);
        var expectedPath = Path.ChangeExtension(entryPath, ".entexp");
        Assert.IsTrue(File.Exists(entryPath), $"Missing sample entry file: {entryPath}");
        Assert.IsTrue(File.Exists(expectedPath), $"Missing sample expected file: {expectedPath}");

        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        sut.DefaultPlace = "Meißenheim";
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(File.ReadAllText(entryPath));

        var expectedResults = ParseExpectedResultsForFile(expectedPath);
        var filteredExpectedResults = ParserSequenceComparer.WithoutDebugMessages(expectedResults);
        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        var mismatch = ParserSequenceComparer.FindFirstMismatch(filteredExpectedResults, filteredResults);
        if (mismatch is not null)
        {
            Assert.Fail($"Mismatch for {filename} at index {mismatch.Value.Index}. Expected: {mismatch.Value.Expected}; Actual: {mismatch.Value.Actual}");
        }

        CollectionAssert.AreEqual(filteredExpectedResults.ToList(), filteredResults.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(AkSamples), DynamicDataSourceType.Method)]
    public void Feed_AkSamples(string filename,string sEntr, IReadOnlyList<ParseResult> expectedResults )
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        ApplySampleDefaults(sut, filename);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(sEntr);

        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        CollectionAssert.AreEqual(expectedResults.ToList(), filteredResults.ToList());
    }
    
    [TestMethod]
    [DynamicData(nameof(GCSamples), DynamicDataSourceType.Method)]
    public void Feed_GcSamples(string filename,string sEntr, IReadOnlyList<ParseResult> expectedResults )
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(sEntr);

        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        CollectionAssert.AreEqual(expectedResults.ToList(), filteredResults.ToList());
    }

    private static IEnumerable<object[]> AkSamples()
    {
        if (!Directory.Exists(SampleDataPath))
        {
            yield break;
        }

        foreach (var file in Directory.EnumerateFiles(SampleDataPath, "OsBM*.enttxt"))
        {
            if (!File.Exists(Path.ChangeExtension(file,".entexp")))
            {
                continue;
            }
            var content = File.ReadAllText(file);
            var expectedResults = ParseExpectedResultsForFile(Path.ChangeExtension(file, ".entexp"));
            yield return new object[] { Path.GetFileName(file), content, expectedResults };
        }
        yield break;
    }
    private static IEnumerable<object[]> GCSamples()
    {
        if (!Directory.Exists(SampleDataPath))
        {
            yield break;
        }


        foreach (var file in Directory.EnumerateFiles(SampleDataPath, "OsBO*.enttxt")
                       .Union(Directory.EnumerateFiles(SampleDataPath, "EntryGC*.enttxt")))
        {
            if (!File.Exists(Path.ChangeExtension(file,".entexp")))
            {
                continue;
            }
            var content = File.ReadAllText(file);
            var expectedResults = ParseExpectedResultsForFile(Path.ChangeExtension(file, ".entexp"));
            yield return new object[] { Path.GetFileName(file), content, expectedResults };
        }
        yield break;
    }

    private static void LoadSampleGNameList(FBEntryParser parser)
    {
        if (File.Exists(GNameFilePath))
        {
            parser.GNameHandler.LoadGNameList(GNameFilePath);
        }
    }

    private static void ApplySampleDefaults(FBEntryParser parser, string filename)
    {
        if (filename.StartsWith("OsBM", StringComparison.OrdinalIgnoreCase))
        {
            parser.DefaultPlace = "Meißenheim";
        }
        else if (filename.StartsWith("OsBObr", StringComparison.OrdinalIgnoreCase))
        {
            parser.DefaultPlace = "Obrigheim";
        }
    }

    private static string ResolveSampleDataPath()
    {
        var candidatePaths = new[]
        {
            "C:\\Projekte\\Delphi\\Daten\\ParseFB",
            "C:\\Projekte\\Delphi\\Data\\ParseFB",
        };

        return candidatePaths.FirstOrDefault(Directory.Exists) ?? candidatePaths[0];
    }

    private static IReadOnlyList<ParseResult> ParseExpectedResultsForFile(string v)
    {
        var FileContent = File.ReadAllLines(v);
        var expectedResults = new List<ParseResult>();
        foreach (var line in FileContent.Skip(1))
        {
            var parts = line.Split('\t');
            if (parts.Length == 4)
            {
                expectedResults.Add(new ParseResult
                {
                    EventType = parts[0],
                    Data = parts[1],
                    Reference = parts[2],
                    SubType = int.TryParse(parts[3], out var subType) ? subType : 0
                });
            }
        }

        return expectedResults;
    }

    [TestMethod]
    public void Feed_WithOriginalPascalGcSample_DocumentsCurrentGapToPascalExpectedSequence()
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(UntTestFbDataSamples.TestEntryGc5065);

        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        var mismatch = ParserSequenceComparer.FindFirstMismatch(UntTestFbDataPascalExpectedResults.ResultEntryGc5065, filteredResults);

        Assert.IsNotNull(mismatch);
        Assert.AreEqual(63, mismatch.Value.Index);
    }

    [TestMethod]
    public void Feed_WithOriginalPascalAkSample_DocumentsCurrentGapToPascalExpectedSequence()
    {
        using var sut = new FBEntryParser();
        LoadSampleGNameList(sut);
        var collector = new ParserResultCollector();
        collector.Attach(sut);

        sut.Feed(UntTestFbDataSamples.TestEntryAk2421);

        var filteredResults = ParserSequenceComparer.WithoutDebugMessages(collector.Results);
        var mismatch = ParserSequenceComparer.FindFirstMismatch(UntTestFbDataPascalExpectedResults.ResultEntryAk2421, filteredResults);

        Assert.IsNotNull(mismatch);
        Assert.AreEqual(3, mismatch.Value.Index);
        Assert.AreEqual(new ParseResult("ParserFamilyPlace", "Meißenheim", "2421", 3), mismatch.Value.Expected);
        Assert.AreEqual(new ParseResult("ParserIndiName", "Andreas Rosewich", "I2421M", 0), mismatch.Value.Actual);
    }

    [TestMethod]
    public void ScanForEvDate_ExtractsChildEventDate()
    {
        using var sut = new FBEntryParser();

        var result = sut.ScanForEvDate("Text Kd: Anna * 12.03.1900 weitere", 1);

        Assert.AreEqual(" 12.03.1900 ", result);
    }
}
