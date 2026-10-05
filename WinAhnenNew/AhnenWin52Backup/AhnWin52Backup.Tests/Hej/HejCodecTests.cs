using System;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Tests.Hej;

[TestClass]
public sealed class HejCodecTests
{
    private readonly HejCodec _codec = new();

    [TestMethod]
    public void WriteAndRead_PreservesOrderedSectionsCp1252AndEmbeddedLineBreaks()
    {
        HejDocument source = CreateDocument();
        using MemoryStream output = new();

        _codec.Write(source, output);

        byte[] serialized = output.ToArray();
        CollectionAssert.AreEqual(
            Encoding.ASCII.GetBytes("1\u000F0\u000F0\u000FCare\u000FG"),
            serialized.Take(12).ToArray());
        Assert.IsTrue(serialized.Contains((byte)0xF6));
        Assert.IsTrue(serialized.Contains((byte)0x10));

        output.Position = 0;
        HejDocument restored = _codec.Read(output);

        Assert.AreEqual("Care", restored.Individuals[0].Fields[3]);
        Assert.AreEqual("Götz", restored.Individuals[0].Fields[4]);
        Assert.AreEqual("first line\nsecond line", restored.Individuals[0].Fields[31]);
        Assert.AreEqual("mrg", restored.Marriages[0].Fields[0]);
        Assert.AreEqual("adop", restored.Adoptions[0].Fields[0]);
        Assert.AreEqual("Adelsheim", restored.Places[0].Fields[0]);
        Assert.AreEqual("Geburtsurkunde", restored.Sources[0].Fields[0]);
    }

    [TestMethod]
    public void Read_RejectsRecordWithWrongFieldCountAndReportsSectionAndLine()
    {
        using MemoryStream input = new(Encoding.ASCII.GetBytes("a\r\nmrg\r\n"));

        HejFormatException exception = Assert.Throws<HejFormatException>(() => _codec.Read(input));

        Assert.AreEqual(HejSection.Individuals, exception.Section);
        Assert.AreEqual(1, exception.LineNumber);
        StringAssert.Contains(exception.Message, "expected between 50 and 51");
    }

    [TestMethod]
    public void Read_RejectsOutOfOrderSections()
    {
        using MemoryStream input = new(Encoding.ASCII.GetBytes("mrg\r\nortv\r\n"));

        HejFormatException exception = Assert.Throws<HejFormatException>(() => _codec.Read(input));

        Assert.AreEqual(HejSection.Places, exception.Section);
        StringAssert.Contains(exception.Message, "out-of-order");
    }

    [TestMethod]
    public void Read_AcceptsMarriageRecordsWithTheVerifiedFieldCount()
    {
        string individualLine = string.Join('\u000F', Enumerable.Repeat(string.Empty, 50));
        string marriageLine = string.Join('\u000F', Enumerable.Repeat(string.Empty, 22));
        using MemoryStream input = new(Encoding.ASCII.GetBytes(
            $"{individualLine}\r\nmrg\r\n{marriageLine}\r\nadop\r\nortv\r\nquellv\r\n"));

        HejDocument document = _codec.Read(input);

        Assert.AreEqual(22, document.Marriages[0].Fields.Count);
    }

    [TestMethod]
    public void Read_RejectsMarriageRecordsWithAnUnverifiedExtraField()
    {
        string individualLine = string.Join('\u000F', Enumerable.Repeat(string.Empty, 50));
        string marriageLine = string.Join('\u000F', Enumerable.Repeat(string.Empty, 23));
        using MemoryStream input = new(Encoding.ASCII.GetBytes(
            $"{individualLine}\r\nmrg\r\n{marriageLine}\r\nadop\r\nortv\r\nquellv\r\n"));

        HejFormatException exception = Assert.Throws<HejFormatException>(() => _codec.Read(input));

        Assert.AreEqual(HejSection.Marriages, exception.Section);
        Assert.AreEqual(3, exception.LineNumber);
        StringAssert.Contains(exception.Message, "expected between 22 and 22");
    }

    [TestMethod]
    public void Read_RejectsBareLineFeed()
    {
        using MemoryStream input = new(Encoding.ASCII.GetBytes("mrg\n"));

        HejFormatException exception = Assert.Throws<HejFormatException>(() => _codec.Read(input));

        StringAssert.Contains(exception.Message, "expected CRLF");
    }

    [TestMethod]
    public void Read_NormalizesLegacyTrailingFieldOmissionAndReportsDiscardedControls()
    {
        string individualLine = "\t" + string.Concat(Enumerable.Repeat("\u000F", 49));
        byte[] data = Encoding.ASCII.GetBytes(
            individualLine + "\r\nmrg\r\nadop\r\nortv\r\nquellv\r\n");
        using MemoryStream input = new(data);

        HejDocument document = _codec.Read(input);

        Assert.AreEqual(51, document.Individuals[0].Fields.Count);
        Assert.AreEqual(string.Empty, document.Individuals[0].Fields[0]);
        Assert.AreEqual(1, document.Warnings.Count);
        StringAssert.Contains(document.Warnings[0], "0x09");
    }

    [TestMethod]
    public void Write_RejectsFieldWithUnrepresentableControlByte()
    {
        HejRecord[] individuals = [Record(HejSection.Individuals, 0, "\t")];
        HejDocument document = new(individuals, [], [], [], []);
        using MemoryStream output = new();

        Assert.Throws<ArgumentException>(() => _codec.Write(document, output));
    }

    private static HejDocument CreateDocument()
    {
        string[] individualFields = Enumerable.Repeat(string.Empty, HejSchema.GetFieldCount(HejSection.Individuals)).ToArray();
        individualFields[0] = "1";
        individualFields[1] = "0";
        individualFields[2] = "0";
        individualFields[3] = "Care";
        individualFields[4] = "Götz";
        individualFields[31] = "first line\r\nsecond line";

        return new HejDocument(
            [new HejRecord(individualFields)],
            [Record(HejSection.Marriages, 0, "mrg")],
            [Record(HejSection.Adoptions, 0, "adop")],
            [Record(HejSection.Places, 0, "Adelsheim")],
            [Record(HejSection.Sources, 0, "Geburtsurkunde")]);
    }

    private static HejRecord Record(HejSection section, int fieldIndex, string value)
    {
        string[] fields = Enumerable.Repeat(string.Empty, HejSchema.GetFieldCount(section)).ToArray();
        fields[fieldIndex] = value;
        return new HejRecord(fields);
    }
}
