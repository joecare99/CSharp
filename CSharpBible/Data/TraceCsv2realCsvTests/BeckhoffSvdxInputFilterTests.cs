using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Filter.BeckhoffSvdx.Filters;
using TraceAnalysis.Filter.CSV.Filters;
using TraceAnalysis.Filter.CSV.Model;

namespace TraceCsv2realCsv.Tests;

[TestClass]
public class BeckhoffSvdxInputFilterTests
{
    [TestMethod]
    public void Read_WithVerifiedReal64Layout_ReturnsAllSamplesAndUtcTimestamps()
    {
        var filter = new BeckhoffSvdxInputFilter();
        using var stream = new MemoryStream(CreateSvdxFile("REAL64"));

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.svdx", ".svdx"));

        Assert.AreEqual(1, dataSet.Metadata.Fields.Count);
        Assert.AreEqual("TestChannel", dataSet.Metadata.Fields[0].sName);
        Assert.AreEqual(typeof(double), dataSet.Metadata.Fields[0].FieldType);
        Assert.AreEqual(3575, dataSet.Records.Count);
        Assert.AreEqual(0, dataSet.ParseErrors.Count);
        Assert.AreEqual(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), dataSet.Records[0].Timestamp);
        Assert.AreEqual(new DateTime(2025, 1, 1, 0, 0, 35, 740, DateTimeKind.Utc), dataSet.Records[3574].Timestamp);
        Assert.AreEqual(0d, dataSet.Records[0].Values["TestChannel"]);
        Assert.AreEqual(3574d, dataSet.Records[3574].Values["TestChannel"]);
    }

    [TestMethod]
    [DataRow("INT16")]
    [DataRow("BIT")]
    public void Read_WithDiscreteChannelType_ReturnsNormalizedValues(string dataType)
    {
        var filter = new BeckhoffSvdxInputFilter();
        using var stream = new MemoryStream(CreateSvdxFile(dataType));

        var dataSet = filter.Read(stream, new FilterSourceDescriptor("sample.svdx", ".svdx"));

        Assert.AreEqual(3575, dataSet.Records.Count);
        Assert.AreEqual(0, dataSet.ParseErrors.Count);
        if (dataType == "INT16")
        {
            Assert.AreEqual(typeof(short), dataSet.Metadata.Fields[0].FieldType);
            Assert.AreEqual((short)3574, dataSet.Records[3574].Values["TestChannel"]);
        }
        else
        {
            Assert.AreEqual(typeof(bool), dataSet.Metadata.Fields[0].FieldType);
            Assert.AreEqual(false, dataSet.Records[0].Values["TestChannel"]);
            Assert.AreEqual(false, dataSet.Records[3574].Values["TestChannel"]);
        }
    }

    [TestMethod]
    public void Analyze_WithVerifiedLayout_RestoresStreamPosition()
    {
        var filter = new BeckhoffSvdxInputFilter();
        using var stream = new MemoryStream(CreateSvdxFile("REAL64"));
        stream.Position = 7;

        var result = filter.Analyze(stream, new FilterSourceDescriptor("sample.svdx", ".svdx"));

        Assert.IsTrue(result.CanHandle);
        Assert.IsTrue(result.IsExactExtensionMatch);
        Assert.AreEqual(7, stream.Position);
    }

    [TestMethod]
    public void Analyze_WithInvalidXmlOffset_DoesNotClaimSupport()
    {
        var filter = new BeckhoffSvdxInputFilter();
        var bytes = CreateSvdxFile("REAL64");
        BitConverter.GetBytes((uint)1).CopyTo(bytes, 0);
        using var stream = new MemoryStream(bytes);

        var result = filter.Analyze(stream, new FilterSourceDescriptor("invalid.svdx", ".svdx"));

        Assert.IsFalse(result.CanHandle);
    }

    [TestMethod]
    public void Convert_WithVerifiedSvdxInput_WritesCsvRows()
    {
        var inputFilter = new BeckhoffSvdxInputFilter();
        var service = new TraceCsvConversionService(
            new InputFilterSelector(),
            new IAnalyzableInputFilter[] { inputFilter },
            new IOutputFilter[] { new CsvOutputFilter() });
        using var input = new MemoryStream(CreateSvdxFile("REAL64"));
        using var output = new NonClosingMemoryStream();

        service.Convert(input, "sample.svdx", output, "sample.csv");

        output.Position = 0;
        var csvModel = new CsvModel();
        csvModel.ReadCsv(output);
        Assert.AreEqual(3575, csvModel.Rows.Count);
        Assert.AreEqual(2, csvModel.Header.Count);
    }

    private static byte[] CreateSvdxFile(string dataType)
    {
        const int xmlOffsetInBlock = 0x123;
        const int leadingRecordCount = 15;
        const int indexCount = 224;
        const int chunkCount = 224;
        const int fileHeaderSize = 20;
        const int extentDescriptorSize = 20;
        var valueSize = dataType == "REAL64" ? 8 : dataType == "INT16" ? 2 : 1;
        var recordSize = 8 + 2 * valueSize;
        var chunkSize = 16 + valueSize + 15 * (4 + valueSize);
        var indexStart = xmlOffsetInBlock + leadingRecordCount * recordSize;
        var detailStart = indexStart + indexCount * recordSize;
        var blockLength = detailStart + chunkCount * chunkSize;
        var xml = $"<ScopeProject><AdsAcquisition><FileHandle>1</FileHandle><SymbolName>TestChannel</SymbolName><DataType>{dataType}</DataType><BaseSampleTime>100000</BaseSampleTime><Oversample>0</Oversample></AdsAcquisition></ScopeProject>";
        var xmlBytes = Encoding.UTF8.GetBytes(xml);
        var xmlOffset = fileHeaderSize + extentDescriptorSize + blockLength;
        var file = new byte[xmlOffset + xmlBytes.Length];
        BitConverter.GetBytes((uint)xmlOffset).CopyTo(file, 0);
        BitConverter.GetBytes((uint)xmlBytes.Length).CopyTo(file, 8);
        BitConverter.GetBytes((uint)1).CopyTo(file, 16);
        BitConverter.GetBytes((ulong)(fileHeaderSize + extentDescriptorSize)).CopyTo(file, fileHeaderSize);
        BitConverter.GetBytes((ulong)blockLength).CopyTo(file, fileHeaderSize + 8);
        BitConverter.GetBytes((uint)1).CopyTo(file, fileHeaderSize + 16);
        xmlBytes.CopyTo(file, xmlOffset);

        var blockOffset = fileHeaderSize + extentDescriptorSize;
        var baseFileTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToFileTimeUtc();
        for (var index = 0; index < indexCount; index++)
        {
            var chunkBaseTime = baseFileTime + index * 16L * 100000;
            var sampleCount = index == chunkCount - 1 ? 7 : 16;
            var minimum = double.PositiveInfinity;
            var maximum = double.NegativeInfinity;
            for (var sample = 0; sample < sampleCount; sample++)
            {
                var value = GetSampleValue(index, sample, dataType);
                minimum = Math.Min(minimum, value);
                maximum = Math.Max(maximum, value);
            }

            var indexOffset = blockOffset + indexStart + index * recordSize;
            BitConverter.GetBytes(chunkBaseTime).CopyTo(file, indexOffset);
            WriteValue(file, indexOffset + 8, minimum, dataType);
            WriteValue(file, indexOffset + 8 + valueSize, maximum, dataType);

            var chunkOffset = blockOffset + detailStart + index * chunkSize;
            BitConverter.GetBytes(chunkBaseTime).CopyTo(file, chunkOffset);
            BitConverter.GetBytes((uint)sampleCount).CopyTo(file, chunkOffset + 8);
            for (var sample = 0; sample < sampleCount; sample++)
            {
                var value = GetSampleValue(index, sample, dataType);
                if (sample == 0)
                {
                    WriteValue(file, chunkOffset + 16, value, dataType);
                }
                else
                {
                    var deltaOffset = chunkOffset + 16 + valueSize + (sample - 1) * (4 + valueSize);
                    BitConverter.GetBytes((uint)(sample * 100000)).CopyTo(file, deltaOffset);
                    WriteValue(file, deltaOffset + 4, value, dataType);
                }
            }
        }

        return file;
    }

    private static void WriteValue(byte[] file, int offset, double value, string dataType)
    {
        if (dataType == "REAL64")
            BitConverter.GetBytes(value).CopyTo(file, offset);
        else if (dataType == "INT16")
            BitConverter.GetBytes(checked((short)value)).CopyTo(file, offset);
        else
            file[offset] = checked((byte)value);
    }

    private static double GetSampleValue(int index, int sample, string dataType)
    {
        var value = index * 16d + sample;
        return dataType == "BIT" ? value % 2 : value;
    }

    private sealed class NonClosingMemoryStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(false);
        }
    }
}
