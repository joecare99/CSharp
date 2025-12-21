using System;
using System.Collections.Generic;
using BaseLib.Helper;
using TranspilerLib.DriveBASIC.Data;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveCompiler
{
    public IReadOnlyList<KeyValuePair<string, object?>>? ParseLine(string placeholder, string line, out int errp)=>
        new Compiler(this).ParseLine(placeholder,line, out errp);

    public bool TestPlaceHolderCharset(string placeholder, string text) =>
        new Compiler(this).TestPlaceHolderCharset(placeholder, text);

    public partial class Compiler
    {

        public bool TestPlaceHolderCharset(string placeholder, string text)
        {
            foreach (var sysPlaceHolder in ParseDefinitions.SysPHCharset)
            {
                var result = true;
                if (placeholder.StartsWith(sysPlaceHolder.Placeholder, StringComparison.OrdinalIgnoreCase))
                {
                    var separatorPos = 0;
                    if (sysPlaceHolder.HasPointAsSep)
                        separatorPos = text.IndexOf('.');

                    for (var index = 0; index < text.Length; index++)
                    {
                        if (index == 0)
                            result &= sysPlaceHolder.first.Contains(text[index]);
                        else if (index == text.Length - 1 && sysPlaceHolder.last != null)
                            result &= sysPlaceHolder.last.Contains(text[index]);
                        else if (sysPlaceHolder.inner != null && index != separatorPos)
                            result &= sysPlaceHolder.inner.Contains(text[index]);
                        else
                            result &= index == separatorPos;
                    }

                    if (result)
                        return true;
                }
            }

            return false;
        }

        public IReadOnlyList<KeyValuePair<string, object?>>? ParseLine(string placeholder, string line, out int errp)
        {
            errp = 1;
            placeholder ??= string.Empty;
            line ??= string.Empty;

            var isTokenPlaceholder = placeholder.Equals(_parent.CToken, StringComparison.OrdinalIgnoreCase);
            var isExpressionPlaceholder = placeholder.Equals(CExpression, StringComparison.OrdinalIgnoreCase);

            int iterationCount = isTokenPlaceholder
                ? _parent.parseDefs.Length
                : isExpressionPlaceholder
                    ? _parent.expressionNormals.Count
                    : _parent.PlaceHolderSubst.Length;

            var trimmedLine = line.Trim();

            for (int index = 0; index < iterationCount; index++)
            {
                string testedPlaceholder = isTokenPlaceholder
                    ? _parent.CToken
                    : isExpressionPlaceholder
                        ? CExpression
                        : _parent.PlaceHolderSubst[index].PlaceHolder;

                if (!placeholder.Equals(testedPlaceholder, StringComparison.OrdinalIgnoreCase))
                    continue;

                string matchingText = (isTokenPlaceholder
                    ? _parent.parseDefs[index].text
                    : isExpressionPlaceholder
                        ? _parent.expressionNormals[index].Item3
                        : _parent.PlaceHolderSubst[index].text) ?? string.Empty;

                matchingText = matchingText.MTSpaceTrim();

                var wildcardMatches = new List<KeyValuePair<string, string>>();
                if (!StringUtils.TryPlaceHolderMatching(trimmedLine, matchingText, wildcardMatches))
                    continue;

                var resultMatches = new List<KeyValuePair<string, object?>>(wildcardMatches.Count + 1)
            {
                new(testedPlaceholder, index)
            };

                bool success = true;
                int localError = 0;

                for (int matchIndex = 0; matchIndex < wildcardMatches.Count; matchIndex++)
                {
                    var currentMatch = wildcardMatches[matchIndex];
                    var matchedPlaceholder = currentMatch.Key;
                    var matchedText = currentMatch.Value?.Trim() ?? string.Empty;

                    bool isCToken = matchedPlaceholder.Equals(ParseDefinitions.CTToken, StringComparison.OrdinalIgnoreCase);
                    bool isSystemPlaceholder = _parent.IsSystemPlaceholder(matchedPlaceholder, out _);

                    if (isSystemPlaceholder && !isCToken)
                    {
                        if (!TestPlaceHolderCharset(matchedPlaceholder, matchedText))
                        {
                            success = false;
                            localError = 1 + matchIndex;
                            break;
                        }
                        resultMatches.Add(new KeyValuePair<string, object?>(matchedPlaceholder, matchedText));
                        continue;
                    }

                    var nestedPlaceholder = isCToken ? _parent.CToken : matchedPlaceholder;
                    var nested = ParseLine(nestedPlaceholder, matchedText, out var nestedError);
                    if (nestedError != 0)
                    {
                        success = false;
                        localError = nestedError + (matchIndex + 1) * 2;
                        break;
                    }

                    object? nestedValue = (object?)nested ?? matchedText;
                    resultMatches.Add(new KeyValuePair<string, object?>(matchedPlaceholder, nestedValue));
                }

                if (success)
                {
                    errp = 0;
                    return resultMatches;
                }

                if (localError != 0)
                    errp = localError;
            }

            return null;
        }

        bool CheckPlaceholderCharset(string placeholderToken, string trimmed)
            => !_parent.IsSystemPlaceholder(placeholderToken, out _) || TestPlaceHolderCharset(placeholderToken, trimmed);
    }
}
