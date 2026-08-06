using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RnzTrauer.Import.Services;

/// <summary>
/// Incremental, deliberately narrow tokenizer matching the legacy callback categories.
/// It buffers incomplete tags/comments/scripts instead of dropping split input.
/// </summary>
public sealed class HtmlCallbackTokenizer : IHtmlCallbackTokenizer
{
    private static readonly Regex TagPattern = new(
        @"^<(?<closing>/)?(?<name>!?[A-Za-z][A-Za-z0-9!:-]*)(?<attributes>[^>]*)>$",
        RegexOptions.Singleline | RegexOptions.CultureInvariant);
    private readonly StringBuilder _buffer = new();
    private readonly List<string> _tagStack = new();
    private string? _scriptTag;

    /// <inheritdoc />
    public IReadOnlyList<HtmlCallbackEvent> Feed(string chunk)
    {
        _buffer.Append(chunk ?? string.Empty);
        return Parse(false);
    }

    /// <inheritdoc />
    public IReadOnlyList<HtmlCallbackEvent> Complete() => Parse(true);

    /// <inheritdoc />
    public void Reset()
    {
        _buffer.Clear();
        _tagStack.Clear();
        _scriptTag = null;
    }

    private IReadOnlyList<HtmlCallbackEvent> Parse(bool flush)
    {
        var result = new List<HtmlCallbackEvent>();
        while (_buffer.Length > 0)
        {
            if (_scriptTag is not null)
            {
                var endScript = _buffer.ToString().IndexOf("</" + _scriptTag, StringComparison.OrdinalIgnoreCase);
                if (endScript < 0)
                {
                    if (flush)
                    {
                        result.Add(new HtmlCallbackEvent(HtmlCallbackKind.Script, _buffer.ToString(), _buffer.ToString()));
                        _buffer.Clear();
                    }
                    break;
                }

                if (endScript > 0)
                {
                    var script = _buffer.ToString(0, endScript);
                    result.Add(new HtmlCallbackEvent(HtmlCallbackKind.Script, script, script));
                    _buffer.Remove(0, endScript);
                }
                _scriptTag = null;
                continue;
            }

            if (_buffer[0] != '<')
            {
                var tagStart = _buffer.ToString().IndexOf('<');
                if (tagStart < 0)
                {
                    if (flush)
                    {
                        AddText(result, _buffer.ToString());
                        _buffer.Clear();
                    }
                    break;
                }

                AddText(result, _buffer.ToString(0, tagStart));
                _buffer.Remove(0, tagStart);
                continue;
            }

            if (_buffer.ToString().StartsWith("<!--", StringComparison.Ordinal))
            {
                var endComment = _buffer.ToString().IndexOf("-->", StringComparison.Ordinal);
                if (endComment < 0)
                    break;
                var raw = _buffer.ToString(0, endComment + 3);
                result.Add(new HtmlCallbackEvent(HtmlCallbackKind.Comment, raw[4..^3], raw));
                _buffer.Remove(0, endComment + 3);
                continue;
            }

            var endTag = FindTagEnd(_buffer);
            if (endTag < 0)
                break;

            var rawTag = _buffer.ToString(0, endTag + 1);
            var match = TagPattern.Match(rawTag);
            if (!match.Success)
            {
                AddText(result, rawTag);
                _buffer.Remove(0, endTag + 1);
                continue;
            }

            var name = match.Groups["name"].Value;
            var closing = match.Groups["closing"].Success;
            var attributes = match.Groups["attributes"].Value;
            if (closing)
            {
                var upperName = name.ToUpperInvariant();
                result.Add(new HtmlCallbackEvent(
                    HtmlCallbackKind.EndTag,
                    name,
                    rawTag,
                    upperName,
                    BuildTagPath()));
                CloseTag(upperName);
            }
            else
            {
                var upperName = name.ToUpperInvariant();
                result.Add(new HtmlCallbackEvent(
                    HtmlCallbackKind.StartTag,
                    name,
                    rawTag,
                    upperName,
                    BuildTagPath()));
                if (!IsSingleton(upperName))
                    _tagStack.Add(upperName);
                foreach (var modifier in SplitModifiers(attributes))
                    result.Add(new HtmlCallbackEvent(
                        HtmlCallbackKind.TagModifier,
                        modifier,
                        modifier,
                        upperName,
                        BuildTagPath()));
                if (name.Equals("script", StringComparison.OrdinalIgnoreCase))
                    _scriptTag = name;
                if (rawTag.TrimEnd().EndsWith("/>", StringComparison.Ordinal))
                    result.Add(new HtmlCallbackEvent(HtmlCallbackKind.EndTag, string.Empty, string.Empty));
            }
            _buffer.Remove(0, endTag + 1);
        }
        return result;
    }

    private static int FindTagEnd(StringBuilder buffer)
    {
        var quote = '\0';
        for (var index = 1; index < buffer.Length; index++)
        {
            var character = buffer[index];
            if (quote != '\0')
            {
                if (character == quote)
                    quote = '\0';
            }
            else if (character is '"' or '\'')
                quote = character;
            else if (character == '>')
                return index;
        }
        return -1;
    }

    private static IEnumerable<string> SplitModifiers(string attributes)
    {
        var start = -1;
        var quote = '\0';
        for (var index = 0; index < attributes.Length; index++)
        {
            var character = attributes[index];
            if (quote != '\0')
            {
                if (character == quote)
                    quote = '\0';
                continue;
            }

            if (character is '"' or '\'')
            {
                quote = character;
                if (start < 0)
                    start = index;
            }
            else if (char.IsWhiteSpace(character))
            {
                if (start >= 0)
                {
                    yield return attributes[start..index];
                    start = -1;
                }
            }
            else if (start < 0)
            {
                start = index;
            }
        }

        if (start >= 0)
            yield return attributes[start..];
    }

    private string BuildTagPath() => string.Join('\\', _tagStack);

    private void CloseTag(string tagName)
    {
        for (var index = _tagStack.Count - 1; index >= 0; index--)
        {
            if (_tagStack[index] != tagName)
                continue;
            _tagStack.RemoveRange(index, _tagStack.Count - index);
            return;
        }
    }

    private static bool IsSingleton(string tagName) =>
        tagName is "P" or "BR" or "META" or "IMG" or "!DOCTYPE";

    private static void AddText(List<HtmlCallbackEvent> result, string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
            result.Add(new HtmlCallbackEvent(HtmlCallbackKind.StandardText, text, text));
    }
}
