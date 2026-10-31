using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IDR.Core.Services;

public static class EventHandlerDeclarationParser
{
    private const int MaximumDeclarationLength = 4096;
    private const int MaximumParameterCount = 256;
    private const int MaximumParameterSizeBytes = 65536;

    public static bool TryParse(
        string declaration,
        out IReadOnlyList<EventHandlerParameterCandidate> parameters,
        out string? returnTypeName)
    {
        parameters = Array.Empty<EventHandlerParameterCandidate>();
        returnTypeName = null;
        if (string.IsNullOrWhiteSpace(declaration) || declaration.Length > MaximumDeclarationLength)
        {
            return false;
        }

        string text = declaration.Trim();
        bool isFunction;
        int keywordLength;
        if (StartsWithKeyword(text, "procedure"))
        {
            isFunction = false;
            keywordLength = "procedure".Length;
        }
        else if (StartsWithKeyword(text, "function"))
        {
            isFunction = true;
            keywordLength = "function".Length;
        }
        else
        {
            return false;
        }

        string body = text[keywordLength..].TrimStart();
        int openParenthesis = body.IndexOf('(');
        string parameterText;
        string suffix;
        if (openParenthesis < 0)
        {
            if (body.Contains(')'))
            {
                return false;
            }

            parameterText = string.Empty;
            suffix = body;
        }
        else
        {
            if (!TryFindClosingParenthesis(body, openParenthesis, out int closeParenthesis))
            {
                return false;
            }

            parameterText = body[(openParenthesis + 1)..closeParenthesis];
            suffix = body[(closeParenthesis + 1)..].Trim();
        }

        if (!TryParseReturnType(suffix, isFunction, out returnTypeName))
        {
            returnTypeName = null;
            return false;
        }

        if (parameterText.Length == 0)
        {
            return true;
        }

        if (!TrySplitTopLevel(parameterText, ';', out string[] groups))
        {
            returnTypeName = null;
            return false;
        }

        if (groups.Length > MaximumParameterCount)
        {
            returnTypeName = null;
            return false;
        }

        List<EventHandlerParameterCandidate> parsedParameters = [];
        int nextArgumentIndex = 1;
        int stackParameterOffset = 0;
        foreach (string group in groups)
        {
            if (!TryParseParameterGroup(
                    group,
                    parsedParameters,
                    ref nextArgumentIndex,
                    ref stackParameterOffset))
            {
                returnTypeName = null;
                return false;
            }
        }

        parameters = parsedParameters.ToArray();
        return true;
    }

    private static bool StartsWithKeyword(string text, string keyword) =>
        text.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
        && (text.Length == keyword.Length || char.IsWhiteSpace(text[keyword.Length]) || text[keyword.Length] == '(');

    private static bool TryFindClosingParenthesis(string text, int openIndex, out int closeIndex)
    {
        int depth = 0;
        for (int index = openIndex; index < text.Length; index++)
        {
            if (text[index] == '(')
            {
                depth++;
            }
            else if (text[index] == ')' && --depth == 0)
            {
                closeIndex = index;
                return true;
            }

            if (depth < 0)
            {
                break;
            }
        }

        closeIndex = -1;
        return false;
    }

    private static bool TryParseReturnType(string suffix, bool isFunction, out string? returnTypeName)
    {
        returnTypeName = null;
        if (!isFunction)
        {
            if (suffix.Length == 0 || suffix.StartsWith(';'))
            {
                return true;
            }

            if (suffix.StartsWith("of object", StringComparison.OrdinalIgnoreCase))
            {
                string directives = suffix["of object".Length..].TrimStart();
                return directives.Length == 0 || directives.StartsWith(';');
            }

            return false;
        }

        if (!suffix.StartsWith(':'))
        {
            return false;
        }

        string returnType = suffix[1..].Trim();
        int modifierIndex = returnType.IndexOf(" of object", StringComparison.OrdinalIgnoreCase);
        if (modifierIndex >= 0)
        {
            string directives = returnType[(modifierIndex + " of object".Length)..].TrimStart();
            if (directives.Length > 0 && !directives.StartsWith(';'))
            {
                return false;
            }

            returnType = returnType[..modifierIndex].TrimEnd();
        }

        int directiveIndex = returnType.IndexOf(';');
        if (directiveIndex >= 0)
        {
            returnType = returnType[..directiveIndex].TrimEnd();
        }

        if (returnType.Length == 0)
        {
            return false;
        }

        returnTypeName = NormalizeUnknownType(returnType);
        return true;
    }

    private static bool TryParseParameterGroup(
        string group,
        List<EventHandlerParameterCandidate> parameters,
        ref int nextArgumentIndex,
        ref int stackParameterOffset)
    {
        string text = group.Trim();
        if (text.Length == 0 || !TryFindTopLevel(text, ':', out int colonIndex))
        {
            return false;
        }

        string namesText = text[..colonIndex].Trim();
        string typeText = text[(colonIndex + 1)..].Trim();
        EventHandlerParameterPassingMode passingMode = EventHandlerParameterPassingMode.Value;
        foreach ((string modifier, EventHandlerParameterPassingMode mode) in new[]
        {
            ("var", EventHandlerParameterPassingMode.Var),
            ("const", EventHandlerParameterPassingMode.Const),
            ("val", EventHandlerParameterPassingMode.Value)
        })
        {
            if (StartsWithKeyword(namesText, modifier))
            {
                passingMode = mode;
                namesText = namesText[modifier.Length..].TrimStart();
                break;
            }
        }

        if (!TrySplitTopLevel(namesText, ',', out string[] names)
            || names.Length == 0
            || !TryParseType(typeText, out string? typeName, out int? declaredSize))
        {
            return false;
        }

        foreach (string rawName in names)
        {
            if (parameters.Count == MaximumParameterCount)
            {
                return false;
            }

            string name = rawName.Trim();
            if (name.Length == 0 || (name != "?" && !IsIdentifier(name)))
            {
                return false;
            }

            parameters.Add(new EventHandlerParameterCandidate(
                name == "?" ? null : name.Length == 0 ? null : name,
                typeName,
                passingMode,
                declaredSize,
                GetFastcallRegister(nextArgumentIndex, declaredSize),
                GetStackParameterOffset(nextArgumentIndex, declaredSize, ref stackParameterOffset)));
            nextArgumentIndex++;
        }

        return true;
    }

    private static string? GetFastcallRegister(int argumentIndex, int? declaredSize) =>
        argumentIndex switch
        {
            1 when (declaredSize ?? 4) == 4 => "EDX",
            2 when (declaredSize ?? 4) == 4 => "ECX",
            _ => null
        };

    private static int? GetStackParameterOffset(
        int argumentIndex,
        int? declaredSize,
        ref int stackParameterOffset)
    {
        if (GetFastcallRegister(argumentIndex, declaredSize) is not null)
        {
            return null;
        }

        int offset = stackParameterOffset;
        stackParameterOffset = checked(stackParameterOffset + (declaredSize ?? 4));
        return offset;
    }

    private static bool TryParseType(string text, out string? typeName, out int? declaredSize)
    {
        typeName = null;
        declaredSize = null;
        if (text.Length == 0)
        {
            return false;
        }

        int colonIndex = text.LastIndexOf(':');
        if (colonIndex > 0
            && int.TryParse(
                text.AsSpan(colonIndex + 1).Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out int size))
        {
            if (size <= 0 || size > MaximumParameterSizeBytes)
            {
                return false;
            }

            declaredSize = size;
            text = text[..colonIndex].TrimEnd();
        }

        if (text.Length == 0)
        {
            return false;
        }

        typeName = NormalizeUnknownType(text);
        return true;
    }

    private static bool TryFindTopLevel(string text, char delimiter, out int delimiterIndex)
    {
        int parentheses = 0;
        int brackets = 0;
        int generics = 0;
        for (int index = 0; index < text.Length; index++)
        {
            char current = text[index];
            if (current == delimiter && parentheses == 0 && brackets == 0 && generics == 0)
            {
                delimiterIndex = index;
                return true;
            }

            UpdateNesting(current, ref parentheses, ref brackets, ref generics);
            if (parentheses < 0 || brackets < 0 || generics < 0)
            {
                break;
            }
        }

        delimiterIndex = -1;
        return false;
    }

    private static bool TrySplitTopLevel(string text, char delimiter, out string[] parts)
    {
        List<string> result = [];
        int start = 0;
        int parentheses = 0;
        int brackets = 0;
        int generics = 0;
        for (int index = 0; index < text.Length; index++)
        {
            char current = text[index];
            if (current == delimiter && parentheses == 0 && brackets == 0 && generics == 0)
            {
                result.Add(text[start..index].Trim());
                start = index + 1;
                continue;
            }

            UpdateNesting(current, ref parentheses, ref brackets, ref generics);
            if (parentheses < 0 || brackets < 0 || generics < 0)
            {
                parts = [];
                return false;
            }
        }

        if (parentheses != 0 || brackets != 0 || generics != 0)
        {
            parts = [];
            return false;
        }

        result.Add(text[start..].Trim());
        parts = result.ToArray();
        return true;
    }

    private static void UpdateNesting(char current, ref int parentheses, ref int brackets, ref int generics)
    {
        switch (current)
        {
            case '(':
                parentheses++;
                break;
            case ')':
                parentheses--;
                break;
            case '[':
                brackets++;
                break;
            case ']':
                brackets--;
                break;
            case '<':
                generics++;
                break;
            case '>':
                generics--;
                break;
        }
    }

    private static bool IsIdentifier(string text) =>
        (char.IsLetter(text[0]) || text[0] == '_')
        && text.Skip(1).All(character => char.IsLetterOrDigit(character) || character == '_');

    private static string? NormalizeUnknownType(string text) =>
        string.Equals(text.Trim(), "?", StringComparison.Ordinal) ? null : text.Trim();
}
