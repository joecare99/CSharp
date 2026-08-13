using System;
using System.Collections.Generic;
using System.Text.Json;
using Ollama.Tools;

namespace Ollama.CodingAgent;

/// <summary>
/// Parses tool-call JSON from model output with basic markdown tolerance.
/// </summary>
public static class ToolCallParser
{
    /// <summary>
    /// Tries to parse an <see cref="OllamaToolCall"/> from raw model output.
    /// </summary>
    /// <param name="rawContent">The raw model output.</param>
    /// <returns>The parsed tool call.</returns>
    public static OllamaToolCall Parse(string rawContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawContent);

        string candidate = ExtractJsonObject(rawContent);
        using JsonDocument document = JsonDocument.Parse(candidate);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException("Model output did not contain a valid tool call.");
        }

        Dictionary<string, JsonElement> values = new(StringComparer.OrdinalIgnoreCase);
        foreach (JsonProperty property in root.EnumerateObject())
        {
            values[property.Name] = property.Value;
        }

        string toolName = ReadString(values, "toolName")
            ?? ReadString(values, "tool_name")
            ?? ReadString(values, "tool")
            ?? ReadString(values, "name")
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(toolName))
        {
            throw new InvalidOperationException("Model output did not contain a tool name.");
        }

        string input = ReadInput(values);
        return new OllamaToolCall
        {
            ToolName = toolName,
            Input = input,
        };
    }

    private static string ExtractJsonObject(string rawContent)
    {
        string trimmed = rawContent.Trim();
        trimmed = trimmed.Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("```", string.Empty, StringComparison.Ordinal);

        int start = trimmed.IndexOf('{');
        if (start < 0)
        {
            throw new InvalidOperationException("No JSON object was found in model output.");
        }

        int depth = 0;
        bool inString = false;
        bool escape = false;
        for (int i = start; i < trimmed.Length; i++)
        {
            char current = trimmed[i];
            if (inString)
            {
                if (escape)
                {
                    escape = false;
                    continue;
                }

                if (current == '\\')
                {
                    escape = true;
                    continue;
                }

                if (current == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (current == '"')
            {
                inString = true;
                continue;
            }

            if (current == '{')
            {
                depth++;
                continue;
            }

            if (current == '}')
            {
                depth--;
                if (depth == 0)
                {
                    return trimmed.Substring(start, i - start + 1);
                }
            }
        }

        throw new InvalidOperationException("No complete JSON object was found in model output.");
    }

    private static string? ReadString(IReadOnlyDictionary<string, JsonElement> values, string key)
    {
        if (!values.TryGetValue(key, out JsonElement value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
    }

    private static string ReadInput(IReadOnlyDictionary<string, JsonElement> values)
    {
        if (values.TryGetValue("input", out JsonElement inputValue))
        {
            return NormalizeInput(inputValue);
        }

        if (values.TryGetValue("arguments", out JsonElement argumentsValue))
        {
            return NormalizeInput(argumentsValue);
        }

        if (values.TryGetValue("params", out JsonElement paramsValue))
        {
            return NormalizeInput(paramsValue);
        }

        return "{}";
    }

    private static string NormalizeInput(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Object or JsonValueKind.Array => value.GetRawText(),
            JsonValueKind.Null => string.Empty,
            _ => value.ToString(),
        };
    }
}
