using System;
using System.Collections.Generic;

namespace RnzTrauer.Import.Services;

/// <summary>
/// UI-agnostic orchestration service for the documented RNZ HTML import pipeline.
/// This is deliberately a narrow callback tokenizer, not a replacement HTML DOM.
/// </summary>
public sealed class HtmlSchemaImporter : IHtmlSchemaImporter
{
    private readonly IHtmlTextNormalizer _normalizer;
    private readonly IHtmlEncodingDecoder _encodingDecoder;
    private readonly IHtmlCallbackTokenizer _tokenizer;
    private readonly ISchemaFilter _filter;
    private readonly ISchemaImportAccumulator _accumulator;

    /// <summary>Creates an importer from the three independent pipeline seams.</summary>
    public HtmlSchemaImporter(
        IHtmlTextNormalizer normalizer,
        IHtmlCallbackTokenizer tokenizer,
        ISchemaFilter filter,
        ISchemaImportAccumulator accumulator,
        IHtmlEncodingDecoder encodingDecoder)
    {
        _normalizer = normalizer;
        _encodingDecoder = encodingDecoder;
        _tokenizer = tokenizer;
        _filter = filter;
        _accumulator = accumulator;
    }

    /// <inheritdoc />
    public HtmlSchemaImportResult Import(string html, IReadOnlyList<string> schema)
    {
        html ??= string.Empty;
        ArgumentNullException.ThrowIfNull(schema);
        _filter.SetSchema(schema);
        _tokenizer.Reset();
        _accumulator.Reset();

        ProcessCallbacks(_tokenizer.Feed(html));
        ProcessCallbacks(_tokenizer.Complete());

        return new HtmlSchemaImportResult(
            _accumulator.CompletedRows,
            _accumulator.CurrentRow,
            _accumulator.NewFiles);
    }

    /// <inheritdoc />
    public HtmlSchemaImportResult Import(ReadOnlyMemory<byte> bytes, IReadOnlyList<string> schema)
    {
        return Import(_encodingDecoder.Decode(bytes).Text, schema);
    }

    private void ProcessCallbacks(IReadOnlyList<HtmlCallbackEvent> callbacks)
    {
        foreach (var callback in callbacks)
        {
            switch (callback.Kind)
            {
                case HtmlCallbackKind.StandardText:
                    var text = _normalizer.Normalize(callback.Value);
                    if (!string.IsNullOrWhiteSpace(text))
                        ProcessCallback(3, text, "S: " + text);
                    break;
                case HtmlCallbackKind.StartTag:
                    ProcessCallback(2, callback.Raw, "TS: " + callback.Value.ToUpperInvariant());
                    break;
                case HtmlCallbackKind.TagModifier:
                    ProcessFilterOnly("TM: " + callback.TagName + "," + callback.Value);
                    break;
                case HtmlCallbackKind.EndTag:
                    ProcessCallback(4, callback.Raw, "TE: " + callback.Value.ToUpperInvariant());
                    break;
                case HtmlCallbackKind.Comment:
                    ProcessCallback(5, callback.Raw, "C: " + callback.Value);
                    break;
                case HtmlCallbackKind.Script:
                    ProcessCallback(6, callback.Raw, "Sc: " + callback.Value);
                    break;
            }
        }
    }

    private void ProcessCallback(byte callbackType, string payload, string filterToken)
    {
        ProcessFilterOnly(filterToken);

        if (_filter.FilterMode)
            _accumulator.Process(callbackType, payload);
    }

    private void ProcessFilterOnly(string filterToken)
    {
        foreach (var emission in _filter.Test(filterToken))
            _accumulator.Process(emission.Mode, emission.Text);
    }
}
