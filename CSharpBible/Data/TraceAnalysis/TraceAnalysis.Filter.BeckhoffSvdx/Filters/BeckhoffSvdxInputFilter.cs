using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Filter.BeckhoffSvdx.Filters;

/// <summary>
/// Reads the verified TwinCAT Scope SVDX binary/XML layout used by the current
/// forensic sample. Other layouts are rejected until they are verified.
/// </summary>
public sealed class BeckhoffSvdxInputFilter : IAnalyzableInputFilter
{
    private const int HeaderSize = 20;
    private const int ExtentDescriptorSize = 20;
    private const int CommonBlockPrefixSize = 0x8F;
    private const int BlockPreambleSize = 0x123;
    private const int LeadingIndexRecordCount = 15;
    private const int IndexRecordCount = 224;
    private const int SamplesPerChunk = 16;
    private const int FileTimeSize = 8;
    private const int ChunkHeaderSize = 16;
    private const int ExpectedFinalChunkSampleCount = 7;
    private const int MaximumFileSize = 512 * 1024 * 1024;
    private const int MaximumChannelCount = 1024;
    private const int MaximumXmlCharacters = 16 * 1024 * 1024;

    /// <inheritdoc/>
    public string FilterId => "BeckhoffSvdx";

    /// <inheritdoc/>
    public int Priority => 120;

    /// <inheritdoc/>
    public bool CanHandle(Stream _stream, string _sourceId)
    {
        if (_stream == null)
            throw new ArgumentNullException(nameof(_stream));

        return Analyze(_stream, new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId))).CanHandle;
    }

    /// <inheritdoc/>
    public InputFilterAnalysisResult Analyze(Stream stream, FilterSourceDescriptor sourceDescriptor)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        var exactExtension = string.Equals(
            sourceDescriptor.SuggestedExtension ?? Path.GetExtension(sourceDescriptor.SourceId),
            ".svdx",
            StringComparison.OrdinalIgnoreCase);
        if (!stream.CanSeek)
            return new InputFilterAnalysisResult(FilterId, false, 0, exactExtension, new[] { "SVDX analysis requires a seekable stream." });

        var originalPosition = stream.Position;
        try
        {
            var parsed = Parse(stream, sourceDescriptor.SourceId, validateSamples: false);
            return new InputFilterAnalysisResult(
                FilterId,
                true,
                150,
                exactExtension,
                new[]
                {
                    $"Verified SVDX layout detected with {parsed.Channels.Count} channels.",
                    $"Embedded XML offset: 0x{parsed.XmlOffset:X}.",
                    "Binary structure matches the currently supported observed layout."
                });
        }
        catch (Exception exception) when (IsFormatException(exception))
        {
            return new InputFilterAnalysisResult(FilterId, false, 0, exactExtension, new[] { exception.Message });
        }
        catch (Exception exception) when (exception is ArgumentOutOfRangeException || exception is IndexOutOfRangeException)
        {
            return new InputFilterAnalysisResult(FilterId, false, 0, exactExtension, new[] { "SVDX structure contains invalid bounds." });
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    /// <inheritdoc/>
    public ITraceDataSet Read(Stream _stream, string _sourceId)
    {
        if (_stream == null)
            throw new ArgumentNullException(nameof(_stream));

        return Read(_stream, new FilterSourceDescriptor(_sourceId, Path.GetExtension(_sourceId)));
    }

    /// <inheritdoc/>
    public ITraceDataSet Read(Stream stream, FilterSourceDescriptor sourceDescriptor)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        if (sourceDescriptor == null)
            throw new ArgumentNullException(nameof(sourceDescriptor));

        if (!stream.CanSeek || !stream.CanRead)
            return EmptyDataSet(sourceDescriptor.SourceId, "SVDX input must be a readable, seekable stream.");

        var originalPosition = stream.Position;
        try
        {
            var parsed = Parse(stream, sourceDescriptor.SourceId, validateSamples: true);
            return parsed.DataSet!;
        }
        catch (Exception exception) when (IsFormatException(exception))
        {
            return EmptyDataSet(sourceDescriptor.SourceId, $"Failed to read supported SVDX layout: {exception.Message}");
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    private static ParsedSvdx Parse(Stream stream, string sourceId, bool validateSamples)
    {
        if (!stream.CanSeek || !stream.CanRead)
            throw new InvalidDataException("SVDX input must be a readable, seekable stream.");
        if (stream.Length < HeaderSize || stream.Length > MaximumFileSize)
            throw new InvalidDataException("SVDX file size is outside the supported bounds.");

        var fileLength = checked((int)stream.Length);
        if (!BitConverter.IsLittleEndian)
            throw new InvalidDataException("SVDX binary decoding requires a little-endian runtime.");
        var fileBytes = new byte[fileLength];
        stream.Position = 0;
        ReadExactly(stream, fileBytes, 0, fileBytes.Length);

        var xmlOffset = ReadUInt32(fileBytes, 0);
        var xmlLength = ReadUInt32(fileBytes, 8);
        var channelCount = ReadUInt32(fileBytes, 16);
        if (xmlOffset > fileLength || xmlLength != fileLength - xmlOffset)
            throw new InvalidDataException("SVDX XML offset/length fields do not match the file boundary.");
        if (channelCount == 0 || channelCount > MaximumChannelCount)
            throw new InvalidDataException("SVDX channel count is outside the supported bounds.");
        if ((ulong)(HeaderSize + channelCount * ExtentDescriptorSize) > xmlOffset)
            throw new InvalidDataException("SVDX extent table overlaps the XML metadata.");

        var extents = ReadExtents(fileBytes, (int)channelCount, fileLength, (int)xmlOffset);
        ValidateExtentCoverage(extents, (int)xmlOffset);
        ValidateExtentLengths(extents);
        var channels = ReadChannels(fileBytes, (int)xmlOffset, (int)xmlLength, extents);
        var dataSet = validateSamples ? BuildDataSet(fileBytes, channels, sourceId) : null;
        if (!validateSamples)
            ValidateBlockLayout(fileBytes, channels);
        return new ParsedSvdx((int)xmlOffset, channels, dataSet);
    }

    private static void ValidateExtentLengths(IReadOnlyList<Extent> extents)
    {
        foreach (var extent in extents)
        {
            if (extent.Length < CommonBlockPrefixSize + BlockPreambleSize)
                throw new InvalidDataException($"SVDX FileHandle {extent.Handle} extent is too short for the verified block layout.");
        }
    }

    private static List<Extent> ReadExtents(byte[] bytes, int channelCount, int fileLength, int xmlOffset)
    {
        var extents = new List<Extent>(channelCount);
        var handles = new HashSet<uint>();
        for (var index = 0; index < channelCount; index++)
        {
            var descriptorOffset = HeaderSize + index * ExtentDescriptorSize;
            var dataOffset = ReadUInt64(bytes, descriptorOffset);
            var dataLength = ReadUInt64(bytes, descriptorOffset + 8);
            var handle = ReadUInt32(bytes, descriptorOffset + 16);
            if (dataOffset < (ulong)(HeaderSize + channelCount * ExtentDescriptorSize) ||
                dataLength == 0 ||
                dataOffset > (ulong)xmlOffset ||
                dataLength > (ulong)xmlOffset - dataOffset ||
                dataOffset + dataLength > (ulong)fileLength ||
                !handles.Add(handle))
            {
                throw new InvalidDataException($"SVDX extent descriptor {index} has invalid bounds or a duplicate FileHandle.");
            }

            extents.Add(new Extent(handle, checked((int)dataOffset), checked((int)dataLength)));
        }

        return extents;
    }

    private static void ValidateExtentCoverage(IReadOnlyList<Extent> extents, int xmlOffset)
    {
        var ordered = extents.OrderBy(extent => extent.Offset).ToArray();
        var expectedOffset = HeaderSize + extents.Count * ExtentDescriptorSize;
        foreach (var extent in ordered)
        {
            if (extent.Offset != expectedOffset)
                throw new InvalidDataException("SVDX data extents contain a gap or overlap.");
            expectedOffset = checked(extent.Offset + extent.Length);
        }

        if (expectedOffset != xmlOffset)
            throw new InvalidDataException("SVDX data extents do not end at the embedded XML document.");
    }

    private static List<Channel> ReadChannels(byte[] bytes, int xmlOffset, int xmlLength, IReadOnlyList<Extent> extents)
    {
        var xmlBytes = new byte[xmlLength];
        Buffer.BlockCopy(bytes, xmlOffset, xmlBytes, 0, xmlLength);
        XDocument document;
        using (var xmlStream = new MemoryStream(xmlBytes, writable: false))
        using (var xmlReader = XmlReader.Create(xmlStream, new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = MaximumXmlCharacters,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true
        }))
        {
            document = XDocument.Load(xmlReader, LoadOptions.None);
        }

        if (!string.Equals(document.Root?.Name.LocalName, "ScopeProject", StringComparison.Ordinal))
            throw new InvalidDataException("Embedded XML root is not ScopeProject.");

        var acquisitions = document.Descendants()
            .Where(element => string.Equals(element.Name.LocalName, "AdsAcquisition", StringComparison.Ordinal))
            .ToList();
        if (acquisitions.Count != extents.Count)
            throw new InvalidDataException("SVDX acquisition count does not match its extent count.");
        var xmlHandles = acquisitions.Select(element => ReadElementUInt32(element, "FileHandle")).ToArray();
        if (xmlHandles.Distinct().Count() != xmlHandles.Length)
            throw new InvalidDataException("SVDX contains duplicate XML FileHandle values.");

        var channels = new List<Channel>(extents.Count);
        foreach (var extent in extents)
        {
            var acquisition = acquisitions.SingleOrDefault(element => ReadElementUInt32(element, "FileHandle") == extent.Handle);
            if (acquisition == null)
                throw new InvalidDataException($"SVDX FileHandle {extent.Handle} has no XML acquisition.");

            var name = ReadElementText(acquisition, "SymbolName");
            var typeName = ReadElementText(acquisition, "DataType");
            var sampleTime = ReadElementUInt64(acquisition, "BaseSampleTime");
            var oversample = ReadElementUInt32(acquisition, "Oversample");
            if (string.IsNullOrWhiteSpace(name) || oversample != 0 || sampleTime != 100000)
                throw new InvalidDataException($"SVDX acquisition {extent.Handle} has unsupported channel metadata.");

            var kind = ParseDataType(typeName);
            channels.Add(new Channel(extent, name, typeName, kind, sampleTime));
        }

        if (channels.Select(channel => channel.Name).Distinct(StringComparer.Ordinal).Count() != channels.Count)
            throw new InvalidDataException("SVDX contains duplicate channel names.");

        return channels.OrderBy(channel => channel.Extent.Handle).ToList();
    }

    private static double ReadSampleValue(byte[] bytes, int offset, Channel channel)
    {
        return channel.Kind switch
        {
            ChannelKind.Real64 => ReadDouble(bytes, offset),
            ChannelKind.Int16 => ReadInt16(bytes, offset),
            ChannelKind.Bit => bytes[offset],
            _ => throw new InvalidDataException("Unsupported SVDX data type.")
        };
    }

    private static void ValidateBlockLayout(byte[] bytes, IReadOnlyList<Channel> channels)
    {
        foreach (var channel in channels)
        {
            var expectedLength = BlockPreambleSize + LeadingIndexRecordCount * channel.IndexRecordSize +
                IndexRecordCount * channel.IndexRecordSize + IndexRecordCount * ChunkSize(channel.Kind);
            if (channel.Extent.Length != expectedLength)
                throw new InvalidDataException($"SVDX FileHandle {channel.Extent.Handle} extent length does not match the verified chunk layout.");
        }

        var commonPrefix = CopyRange(bytes, channels[0].Extent.Offset, CommonBlockPrefixSize);
        var indexStart = BlockPreambleSize + LeadingIndexRecordCount * channels[0].IndexRecordSize;
        var referenceRecords = new List<SummaryRecord>(IndexRecordCount);
        for (var index = 0; index < IndexRecordCount; index++)
        {
            referenceRecords.Add(ReadSummaryRecord(bytes, channels[0], indexStart + index * channels[0].IndexRecordSize));
            if (index > 0 && referenceRecords[index].Timestamp - referenceRecords[index - 1].Timestamp != SamplesPerChunk * (long)channels[0].SampleTime)
                throw new InvalidDataException("SVDX index timestamp interval does not match the verified chunk cadence.");
        }

        foreach (var channel in channels)
        {
            var extent = channel.Extent;
            var summaryStart = BlockPreambleSize + LeadingIndexRecordCount * channel.IndexRecordSize;
            var detailStart = summaryStart + IndexRecordCount * channel.IndexRecordSize;
            var chunkSize = ChunkSize(channel.Kind);
            if (extent.Length != detailStart + IndexRecordCount * chunkSize)
                throw new InvalidDataException($"SVDX FileHandle {extent.Handle} extent length does not match the verified chunk layout.");
            if (!BytesEqual(commonPrefix, bytes, extent.Offset, commonPrefix.Length))
                throw new InvalidDataException("SVDX channel blocks do not share the verified common prefix.");

            for (var index = 0; index < IndexRecordCount; index++)
            {
                var summary = ReadSummaryRecord(bytes, channel, summaryStart + index * channel.IndexRecordSize);
                var referenceSummary = referenceRecords[index];
                if (summary.Timestamp != referenceSummary.Timestamp)
                    throw new InvalidDataException("SVDX channel index timestamps are not aligned.");
                if (index > 0 && summary.Timestamp - referenceRecords[index - 1].Timestamp != SamplesPerChunk * (long)channel.SampleTime)
                    throw new InvalidDataException("SVDX index timestamp interval does not match the verified chunk cadence.");

                var chunkOffset = detailStart + index * chunkSize;
                ValidateChunk(bytes, channel, chunkOffset, summary, null, index == IndexRecordCount - 1);
            }
        }
    }

    private static SummaryRecord ReadSummaryRecord(byte[] bytes, Channel channel, int relativeOffset)
    {
        var absoluteOffset = channel.Extent.Offset + relativeOffset;
        var timestamp = ReadInt64(bytes, absoluteOffset);
        double first;
        double second;
        switch (channel.Kind)
        {
            case ChannelKind.Real64:
                first = ReadDouble(bytes, absoluteOffset + FileTimeSize);
                second = ReadDouble(bytes, absoluteOffset + FileTimeSize + 8);
                break;
            case ChannelKind.Int16:
                first = ReadInt16(bytes, absoluteOffset + FileTimeSize);
                second = ReadInt16(bytes, absoluteOffset + FileTimeSize + 2);
                break;
            case ChannelKind.Bit:
                first = bytes[absoluteOffset + FileTimeSize];
                second = bytes[absoluteOffset + FileTimeSize + 1];
                if (first > 1 || second > 1)
                    throw new InvalidDataException($"SVDX BIT index values for '{channel.Name}' are not normalized to 0 or 1.");
                break;
            default:
                throw new InvalidDataException("Unsupported SVDX data type.");
        }

        ValidateFileTime(timestamp, channel.Name);
        if (double.IsNaN(first) || double.IsInfinity(first) || double.IsNaN(second) || double.IsInfinity(second))
            throw new InvalidDataException($"SVDX index values for '{channel.Name}' are not finite.");

        return new SummaryRecord(timestamp, first, second);
    }

    private static void ValidateChunk(
        byte[] bytes,
        Channel channel,
        int relativeOffset,
        SummaryRecord summary,
        SortedDictionary<long, Dictionary<string, object?>>? sampleRows,
        bool isFinalChunk)
    {
        var absoluteOffset = channel.Extent.Offset + relativeOffset;
        var chunkTimestamp = ReadInt64(bytes, absoluteOffset);
        var sampleCount = ReadUInt32(bytes, absoluteOffset + 8);
        var expectedSampleCount = isFinalChunk ? ExpectedFinalChunkSampleCount : SamplesPerChunk;
        if (chunkTimestamp != summary.Timestamp || sampleCount != expectedSampleCount)
            throw new InvalidDataException($"SVDX chunk metadata for '{channel.Name}' is invalid.");

        ReadUInt32(bytes, absoluteOffset + 12);
        var firstValueOffset = absoluteOffset + ChunkHeaderSize;
        var summaryMinimum = Math.Min(summary.FirstValue, summary.SecondValue);
        var summaryMaximum = Math.Max(summary.FirstValue, summary.SecondValue);
        var actualMinimum = double.PositiveInfinity;
        var actualMaximum = double.NegativeInfinity;
        var timestamp = chunkTimestamp;

        for (var sampleIndex = 0; sampleIndex < sampleCount; sampleIndex++)
        {
            int valueOffset;
            if (sampleIndex == 0)
            {
                valueOffset = firstValueOffset;
            }
            else
            {
                var deltaOffset = firstValueOffset + channel.ValueSize + (sampleIndex - 1) * (4 + channel.ValueSize);
                var relativeTime = ReadUInt32(bytes, deltaOffset);
                if (relativeTime != (uint)(sampleIndex * channel.SampleTime))
                    throw new InvalidDataException($"SVDX sample interval for '{channel.Name}' does not match BaseSampleTime.");
                timestamp = checked(chunkTimestamp + relativeTime);
                valueOffset = deltaOffset + 4;
            }

            var value = ReadSampleValue(bytes, valueOffset, channel);
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new InvalidDataException($"SVDX sample value for '{channel.Name}' is not finite.");
            if (channel.Kind == ChannelKind.Bit && value != 0d && value != 1d)
                throw new InvalidDataException($"SVDX BIT sample for '{channel.Name}' is not normalized to 0 or 1.");
            actualMinimum = Math.Min(actualMinimum, value);
            actualMaximum = Math.Max(actualMaximum, value);

            if (sampleRows != null)
            {
                if (!sampleRows.TryGetValue(timestamp, out var row))
                {
                    row = new Dictionary<string, object?>(StringComparer.Ordinal);
                    sampleRows.Add(timestamp, row);
                }
                row[channel.Name] = ConvertSampleValue(value, channel.Kind);
            }
        }

        if (Math.Abs(summaryMinimum - actualMinimum) > ValueTolerance(channel.Kind) ||
            Math.Abs(summaryMaximum - actualMaximum) > ValueTolerance(channel.Kind))
        {
            throw new InvalidDataException($"SVDX min/max index values for '{channel.Name}' do not match its detail chunk.");
        }
    }

    private static ITraceDataSet BuildDataSet(byte[] bytes, IReadOnlyList<Channel> channels, string sourceId)
    {
        var sampleRows = new SortedDictionary<long, Dictionary<string, object?>>();
        var fields = channels
            .Select(channel => (ITraceFieldMetadata)new TraceFieldMetadata(channel.Name, ClrType(channel.Kind)))
            .ToArray();
        ValidateBlockLayout(bytes, channels);
        foreach (var channel in channels)
            ReadSamples(bytes, channel, sampleRows);
        var records = sampleRows
            .Select(pair => (ITraceRecord)new TraceRecord(DateTime.FromFileTimeUtc(pair.Key), pair.Value))
            .ToArray();
        return new TraceDataSet(new TraceMetadata(sourceId, fields), records);
    }

    private static void ReadSamples(byte[] bytes, Channel channel, SortedDictionary<long, Dictionary<string, object?>> sampleRows)
    {
        var summaryStart = BlockPreambleSize + LeadingIndexRecordCount * channel.IndexRecordSize;
        var detailStart = summaryStart + IndexRecordCount * channel.IndexRecordSize;
        var chunkSize = ChunkSize(channel.Kind);
        for (var index = 0; index < IndexRecordCount; index++)
        {
            var summary = ReadSummaryRecord(bytes, channel, summaryStart + index * channel.IndexRecordSize);
            ValidateChunk(bytes, channel, detailStart + index * chunkSize, summary, sampleRows, index == IndexRecordCount - 1);
        }
    }

    private static object ConvertSampleValue(double value, ChannelKind kind)
    {
        return kind switch
        {
            ChannelKind.Real64 => value,
            ChannelKind.Int16 => checked((short)value),
            ChannelKind.Bit => checked((byte)value) != 0,
            _ => throw new InvalidDataException("Unsupported SVDX data type.")
        };
    }

    private static Type ClrType(ChannelKind kind)
    {
        return kind switch
        {
            ChannelKind.Real64 => typeof(double),
            ChannelKind.Int16 => typeof(short),
            ChannelKind.Bit => typeof(bool),
            _ => typeof(object)
        };
    }

    private static double ValueTolerance(ChannelKind kind) => kind == ChannelKind.Real64 ? 1e-10 : 0d;

    private static int ChunkSize(ChannelKind kind)
    {
        var size = ValueSize(kind);
        return ChunkHeaderSize + size + (SamplesPerChunk - 1) * (4 + size);
    }

    private static int ValueSize(ChannelKind kind)
    {
        return kind switch
        {
            ChannelKind.Real64 => 8,
            ChannelKind.Int16 => 2,
            ChannelKind.Bit => 1,
            _ => throw new InvalidDataException("Unsupported SVDX data type.")
        };
    }

    private static ChannelKind ParseDataType(string typeName)
    {
        return typeName switch
        {
            "REAL64" => ChannelKind.Real64,
            "INT16" => ChannelKind.Int16,
            "BIT" => ChannelKind.Bit,
            _ => throw new InvalidDataException($"Unsupported SVDX data type '{typeName}'.")
        };
    }

    private static string ReadElementText(XElement element, string localName)
    {
        var value = element.Elements().FirstOrDefault(child => string.Equals(child.Name.LocalName, localName, StringComparison.Ordinal))?.Value;
        return value?.Trim() ?? string.Empty;
    }

    private static uint ReadElementUInt32(XElement element, string localName)
    {
        var text = ReadElementText(element, localName);
        if (!uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            throw new InvalidDataException($"SVDX XML element '{localName}' is not a valid UInt32.");
        return result;
    }

    private static ulong ReadElementUInt64(XElement element, string localName)
    {
        var text = ReadElementText(element, localName);
        if (!ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            throw new InvalidDataException($"SVDX XML element '{localName}' is not a valid UInt64.");
        return result;
    }

    private static void ValidateFileTime(long timestamp, string channelName)
    {
        try
        {
            DateTime.FromFileTimeUtc(timestamp);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new InvalidDataException($"SVDX timestamp for '{channelName}' is outside the supported FILETIME range.");
        }
    }

    private static bool BytesEqual(byte[] expected, byte[] actual, int actualOffset, int length)
    {
        if (expected.Length != length)
            return false;
        for (var index = 0; index < length; index++)
        {
            if (expected[index] != actual[actualOffset + index])
                return false;
        }
        return true;
    }

    private static byte[] CopyRange(byte[] bytes, int offset, int length)
    {
        var result = new byte[length];
        Buffer.BlockCopy(bytes, offset, result, 0, length);
        return result;
    }

    private static ITraceDataSet EmptyDataSet(string sourceId, string error)
    {
        return new TraceDataSet(
            new TraceMetadata(sourceId, Array.Empty<ITraceFieldMetadata>()),
            Array.Empty<ITraceRecord>(),
            new[] { error });
    }

    private static bool IsFormatException(Exception exception)
    {
        return exception is InvalidDataException || exception is IOException || exception is XmlException || exception is ArgumentException || exception is OverflowException;
    }

    private static uint ReadUInt32(byte[] bytes, int offset) => BitConverter.ToUInt32(bytes, offset);

    private static ulong ReadUInt64(byte[] bytes, int offset) => BitConverter.ToUInt64(bytes, offset);

    private static long ReadInt64(byte[] bytes, int offset) => BitConverter.ToInt64(bytes, offset);

    private static short ReadInt16(byte[] bytes, int offset) => BitConverter.ToInt16(bytes, offset);

    private static double ReadDouble(byte[] bytes, int offset) => BitConverter.ToDouble(bytes, offset);

    private static void ReadExactly(Stream stream, byte[] buffer, int offset, int count)
    {
        var totalRead = 0;
        while (totalRead < count)
        {
            var read = stream.Read(buffer, offset + totalRead, count - totalRead);
            if (read == 0)
                throw new EndOfStreamException("Unexpected end of SVDX stream.");
            totalRead += read;
        }
    }

    private sealed class ParsedSvdx
    {
        public ParsedSvdx(int xmlOffset, IReadOnlyList<Channel> channels, ITraceDataSet? dataSet)
        {
            XmlOffset = xmlOffset;
            Channels = channels;
            DataSet = dataSet;
        }

        public int XmlOffset { get; }
        public IReadOnlyList<Channel> Channels { get; }
        public ITraceDataSet? DataSet { get; }
    }

    private sealed class Extent
    {
        public Extent(uint handle, int offset, int length)
        {
            Handle = handle;
            Offset = offset;
            Length = length;
        }

        public uint Handle { get; }
        public int Offset { get; }
        public int Length { get; }
    }

    private sealed class Channel
    {
        public Channel(Extent extent, string name, string typeName, ChannelKind kind, ulong sampleTime)
        {
            Extent = extent;
            Name = name;
            TypeName = typeName;
            Kind = kind;
            SampleTime = checked((int)sampleTime);
            ValueSize = ValueSizeFor(kind);
            IndexRecordSize = FileTimeSize + ValueSize * 2;
        }

        public Extent Extent { get; }
        public string Name { get; }
        public string TypeName { get; }
        public ChannelKind Kind { get; }
        public int SampleTime { get; }
        public int ValueSize { get; }
        public int IndexRecordSize { get; }

        private static int ValueSizeFor(ChannelKind kind) => kind switch
        {
            ChannelKind.Real64 => 8,
            ChannelKind.Int16 => 2,
            ChannelKind.Bit => 1,
            _ => throw new InvalidDataException("Unsupported SVDX data type.")
        };
    }

    private sealed class SummaryRecord
    {
        public SummaryRecord(long timestamp, double firstValue, double secondValue)
        {
            Timestamp = timestamp;
            FirstValue = firstValue;
            SecondValue = secondValue;
        }

        public long Timestamp { get; }
        public double FirstValue { get; }
        public double SecondValue { get; }
    }

    private enum ChannelKind
    {
        Real64,
        Int16,
        Bit
    }
}
