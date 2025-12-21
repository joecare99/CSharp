using System;
using System.Collections.Generic;
using System.Globalization;
using BaseLib.Helper;
using TranspilerLib.DriveBASIC.Data;
using static TranspilerLib.DriveBASIC.Data.ParseDefinitions;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveCompiler
{
    public void BuildExpressionNormal(string line, Action<(int, bool, string)> actAdd, int subToken = 0, bool param2Used = false)
    {
        var startPos = line.GetNextPlaceHolder();
        string nonSysPlaceholder = string.Empty;
        while (startPos < line.Length && nonSysPlaceholder == string.Empty)
        {
            var endPos = line.IndexOf('>', startPos);
            string candidate = line.Substring(startPos, endPos - startPos + 1);
            if (IsSysPlaceholder(candidate, out bool setParam2Used))
            {
                param2Used |= setParam2Used;
            }
            else
            {
                nonSysPlaceholder = candidate;
            }
            startPos = line.GetNextPlaceHolder(endPos + 1);
        }

        if (nonSysPlaceholder == string.Empty)
        {
            actAdd?.Invoke((subToken, param2Used, line));
            return;
        }

        foreach (var substitution in PlaceHolderSubst)
        {
            if (substitution.PlaceHolder == nonSysPlaceholder)
            {
                BuildExpressionNormal(line.Replace(nonSysPlaceholder, substitution.text), actAdd, subToken + substitution.Number, param2Used);
            }
        }
    }

    private bool IsSystemPlaceholder(string placeholderToken)
        => IsSysPlaceholder(placeholderToken, out _);

    private bool IsSysPlaceholder(string candidate, out bool setParam2Used)
    {
        foreach (var charset in SysPHCharset)
        {
            if (candidate.StartsWith(charset.Placeholder, StringComparison.OrdinalIgnoreCase))
            {
                setParam2Used = candidate.EndsWith(":PARAM2>", StringComparison.OrdinalIgnoreCase);
                return true;
            }
        }

        setParam2Used = false;
        return false;
    }

    public bool TestPlaceHolderCharset(string placeholder, string text)
    {
        foreach (var charset in SysPHCharset)
        {
            var result = true;
            if (placeholder.StartsWith(charset.Placeholder, StringComparison.OrdinalIgnoreCase))
            {
                var separatorPos = 0;
                if (charset.HasPointAsSep)
                    separatorPos = text.IndexOf('.');

                for (var index = 0; index < text.Length; index++)
                {
                    if (index == 0)
                        result &= charset.first.Contains(text[index]);
                    else if (index == text.Length - 1 && charset.last != null)
                        result &= charset.last.Contains(text[index]);
                    else if (charset.inner != null && index != separatorPos)
                        result &= charset.inner.Contains(text[index]);
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

        var isTokenPlaceholder = placeholder.Equals(CToken, StringComparison.OrdinalIgnoreCase);
        var isExpressionPlaceholder = placeholder.Equals(CExpression, StringComparison.OrdinalIgnoreCase);

        int iterationCount = isTokenPlaceholder
            ? parseDefs.Length
            : isExpressionPlaceholder
                ? expressionNormals.Count
                : PlaceHolderSubst.Length;

        var trimmedLine = line.Trim();

        for (int index = 0; index < iterationCount; index++)
        {
            string testedPlaceholder = isTokenPlaceholder
                ? CToken
                : isExpressionPlaceholder
                    ? CExpression
                    : PlaceHolderSubst[index].PlaceHolder;

            if (!placeholder.Equals(testedPlaceholder, StringComparison.OrdinalIgnoreCase))
                continue;

            string matchingText = (isTokenPlaceholder
                ? parseDefs[index].text
                : isExpressionPlaceholder
                    ? expressionNormals[index].Item3
                    : PlaceHolderSubst[index].text) ?? string.Empty;

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

                bool isCToken = matchedPlaceholder.Equals(CTToken, StringComparison.OrdinalIgnoreCase);
                bool isSystemPlaceholder = IsSystemPlaceholder(matchedPlaceholder);

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

                var nestedPlaceholder = isCToken ? CToken : matchedPlaceholder;
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
        => !IsSysPlaceholder(placeholderToken, out _) || TestPlaceHolderCharset(placeholderToken, trimmed);
}
