using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using TraceAnalysis.Base.Filters;
using TraceAnalysis.Base.Models.Interfaces;

namespace TraceAnalysis.Filter.Excel.Filters;

/// <summary>
/// Output filter that serializes canonical <see cref="ITraceDataSet"/> records
/// to an Excel workbook package.
/// </summary>
public class ExcelOutputFilter : IOutputFilter
{
    private const string DataWorksheetName = "Data";
    private readonly bool _includeMetadataWorksheetWhenAvailable;
    private readonly bool _metadataWorksheetRequired;
    private readonly string _metadataWorksheetName;

    /// <summary>
    /// Initializes a new instance of <see cref="ExcelOutputFilter"/>.
    /// </summary>
    /// <param name="_includeMetadataWorksheetWhenAvailable">
    /// When <c>true</c>, a metadata worksheet is created when metadata is available.
    /// </param>
    /// <param name="_metadataWorksheetRequired">
    /// When <c>true</c>, a metadata worksheet is always created (for example when the
    /// source CSV had no header row).
    /// </param>
    /// <param name="_metadataWorksheetName">Worksheet name used for metadata output.</param>
    public ExcelOutputFilter(
        bool _includeMetadataWorksheetWhenAvailable = true,
        bool _metadataWorksheetRequired = false,
        string _metadataWorksheetName = "Metadata")
    {
        this._includeMetadataWorksheetWhenAvailable = _includeMetadataWorksheetWhenAvailable;
        this._metadataWorksheetRequired = _metadataWorksheetRequired;
        this._metadataWorksheetName = string.IsNullOrWhiteSpace(_metadataWorksheetName)
            ? "Metadata"
            : _metadataWorksheetName;
    }

    /// <inheritdoc/>
    public void Write(ITraceDataSet _dataSet, Stream _stream)
    {
        if (_dataSet == null)
            throw new ArgumentNullException(nameof(_dataSet));

        if (_stream == null)
            throw new ArgumentNullException(nameof(_stream));

        if (!_stream.CanWrite)
            throw new ArgumentException("Target stream must be writable.", nameof(_stream));

        var orderedDataColumns = BuildOrderedDataColumns(_dataSet);
        var hasAvailableMetadata = orderedDataColumns.Count > 0;
        var writeMetadataWorksheet = _metadataWorksheetRequired
                                     || (_includeMetadataWorksheetWhenAvailable && hasAvailableMetadata);

        using var archive = new ZipArchive(_stream, ZipArchiveMode.Create, leaveOpen: true);

        CreateEntry(archive, "[Content_Types].xml", writer => WriteContentTypes(writer, writeMetadataWorksheet));
        CreateEntry(archive, "_rels/.rels", WriteRootRelationships);
        CreateEntry(archive, "xl/workbook.xml", writer => WriteWorkbook(writer, writeMetadataWorksheet));
        CreateEntry(archive, "xl/_rels/workbook.xml.rels", writer => WriteWorkbookRelationships(writer, writeMetadataWorksheet));
        CreateEntry(archive, "xl/worksheets/sheet1.xml", writer => WriteDataWorksheet(writer, _dataSet, orderedDataColumns));

        if (writeMetadataWorksheet)
            CreateEntry(archive, "xl/worksheets/sheet2.xml", writer => WriteMetadataWorksheet(writer, _dataSet, orderedDataColumns));
    }

    private static List<string> BuildOrderedDataColumns(ITraceDataSet _dataSet)
    {
        var columnSet = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var field in _dataSet.Metadata.Fields)
        {
            if (!string.Equals(field.sName, "timestamp", StringComparison.OrdinalIgnoreCase))
                columnSet.Add(field.sName);
        }

        foreach (var record in _dataSet.Records)
        {
            foreach (var key in record.Values.Keys)
            {
                if (!string.Equals(key, "timestamp", StringComparison.OrdinalIgnoreCase))
                    columnSet.Add(key);
            }
        }

        return columnSet.ToList();
    }

    private static void CreateEntry(ZipArchive _archive, string _entryPath, Action<XmlWriter> _writeAction)
    {
        var entry = _archive.CreateEntry(_entryPath, CompressionLevel.Optimal);
        using var stream = entry.Open();
        using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Encoding = new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = false,
            CloseOutput = false
        });

        _writeAction(writer);
        writer.Flush();
    }

    private static void WriteContentTypes(XmlWriter _writer, bool _hasMetadataWorksheet)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");

        _writer.WriteStartElement("Default");
        _writer.WriteAttributeString("Extension", "rels");
        _writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml");
        _writer.WriteEndElement();

        _writer.WriteStartElement("Default");
        _writer.WriteAttributeString("Extension", "xml");
        _writer.WriteAttributeString("ContentType", "application/xml");
        _writer.WriteEndElement();

        _writer.WriteStartElement("Override");
        _writer.WriteAttributeString("PartName", "/xl/workbook.xml");
        _writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml");
        _writer.WriteEndElement();

        _writer.WriteStartElement("Override");
        _writer.WriteAttributeString("PartName", "/xl/worksheets/sheet1.xml");
        _writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
        _writer.WriteEndElement();

        if (_hasMetadataWorksheet)
        {
            _writer.WriteStartElement("Override");
            _writer.WriteAttributeString("PartName", "/xl/worksheets/sheet2.xml");
            _writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml");
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private static void WriteRootRelationships(XmlWriter _writer)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");

        _writer.WriteStartElement("Relationship");
        _writer.WriteAttributeString("Id", "rId1");
        _writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
        _writer.WriteAttributeString("Target", "xl/workbook.xml");
        _writer.WriteEndElement();

        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private void WriteWorkbook(XmlWriter _writer, bool _hasMetadataWorksheet)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("workbook", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        _writer.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

        _writer.WriteStartElement("sheets");
        _writer.WriteStartElement("sheet");
        _writer.WriteAttributeString("name", DataWorksheetName);
        _writer.WriteAttributeString("sheetId", "1");
        _writer.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1");
        _writer.WriteEndElement();

        if (_hasMetadataWorksheet)
        {
            _writer.WriteStartElement("sheet");
            _writer.WriteAttributeString("name", _metadataWorksheetName);
            _writer.WriteAttributeString("sheetId", "2");
            _writer.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId2");
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private static void WriteWorkbookRelationships(XmlWriter _writer, bool _hasMetadataWorksheet)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");

        _writer.WriteStartElement("Relationship");
        _writer.WriteAttributeString("Id", "rId1");
        _writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
        _writer.WriteAttributeString("Target", "worksheets/sheet1.xml");
        _writer.WriteEndElement();

        if (_hasMetadataWorksheet)
        {
            _writer.WriteStartElement("Relationship");
            _writer.WriteAttributeString("Id", "rId2");
            _writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
            _writer.WriteAttributeString("Target", "worksheets/sheet2.xml");
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private static void WriteDataWorksheet(XmlWriter _writer, ITraceDataSet _dataSet, IReadOnlyList<string> _orderedDataColumns)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("worksheet", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        _writer.WriteStartElement("sheetData");

        var headerRow = new List<string> { "timestamp" };
        headerRow.AddRange(_orderedDataColumns);
        WriteRow(_writer, 1, headerRow);

        for (var rowIndex = 0; rowIndex < _dataSet.Records.Count; rowIndex++)
        {
            var record = _dataSet.Records[rowIndex];
            var rowValues = new List<string> { FormatCellValue(record.Timestamp) };

            foreach (var column in _orderedDataColumns)
            {
                record.Values.TryGetValue(column, out var value);
                rowValues.Add(FormatCellValue(value));
            }

            WriteRow(_writer, rowIndex + 2, rowValues);
        }

        _writer.WriteEndElement();
        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private static void WriteMetadataWorksheet(XmlWriter _writer, ITraceDataSet _dataSet, IReadOnlyList<string> _orderedDataColumns)
    {
        _writer.WriteStartDocument();
        _writer.WriteStartElement("worksheet", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
        _writer.WriteStartElement("sheetData");

        var allColumns = new List<string> { "timestamp" };
        allColumns.AddRange(_orderedDataColumns);
        WriteRow(_writer, 1, allColumns);

        var fieldMetadataMap = new Dictionary<string, ITraceFieldMetadata>(StringComparer.Ordinal);
        foreach (var field in _dataSet.Metadata.Fields)
        {
            if (!fieldMetadataMap.ContainsKey(field.sName))
                fieldMetadataMap[field.sName] = field;
        }

        var formatOrTypeRow = new List<string>();
        var groupRow = new List<string>();

        for (var i = 0; i < allColumns.Count; i++)
        {
            var columnName = allColumns[i];
            if (!fieldMetadataMap.TryGetValue(columnName, out var fieldMetadata))
            {
                formatOrTypeRow.Add(string.Empty);
                groupRow.Add(string.Empty);
                continue;
            }

            var formatOrTypeValue = !string.IsNullOrWhiteSpace(fieldMetadata.sFormat)
                ? fieldMetadata.sFormat!
                : fieldMetadata.FieldType?.FullName ?? string.Empty;

            formatOrTypeRow.Add(formatOrTypeValue);
            groupRow.Add(fieldMetadata.sGroup ?? string.Empty);
        }

        WriteRow(_writer, 2, formatOrTypeRow);
        WriteRow(_writer, 3, groupRow);

        _writer.WriteEndElement();
        _writer.WriteEndElement();
        _writer.WriteEndDocument();
    }

    private static string FormatCellValue(object? _value)
    {
        if (_value == null)
            return string.Empty;

        if (_value is DateTime dateTime)
            return dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

        if (_value is DateTimeOffset dateTimeOffset)
            return dateTimeOffset.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);

        if (_value is IFormattable formattable)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        return _value.ToString() ?? string.Empty;
    }

    private static void WriteRow(XmlWriter _writer, int _rowIndex, IReadOnlyList<string> _values)
    {
        _writer.WriteStartElement("row");
        _writer.WriteAttributeString("r", _rowIndex.ToString(CultureInfo.InvariantCulture));

        for (var i = 0; i < _values.Count; i++)
        {
            var cellReference = GetCellReference(i + 1, _rowIndex);
            WriteInlineStringCell(_writer, cellReference, _values[i]);
        }

        _writer.WriteEndElement();
    }

    private static void WriteInlineStringCell(XmlWriter _writer, string _cellReference, string _value)
    {
        _writer.WriteStartElement("c");
        _writer.WriteAttributeString("r", _cellReference);
        _writer.WriteAttributeString("t", "inlineStr");
        _writer.WriteStartElement("is");
        _writer.WriteElementString("t", _value);
        _writer.WriteEndElement();
        _writer.WriteEndElement();
    }

    private static string GetCellReference(int _columnIndex, int _rowIndex)
    {
        var column = GetColumnName(_columnIndex);
        return string.Concat(column, _rowIndex.ToString(CultureInfo.InvariantCulture));
    }

    private static string GetColumnName(int _columnIndex)
    {
        var index = _columnIndex;
        var columnName = string.Empty;

        while (index > 0)
        {
            var modulo = (index - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            index = (index - modulo) / 26;
        }

        return columnName;
    }
}
