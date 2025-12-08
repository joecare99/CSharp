using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NebelEbook;

public static class TextTemplate
{
    private static readonly Regex Token = new(@"\{\{(?<k>[^}]+)\}\}", RegexOptions.Compiled);

    public static string Render(string template, IDictionary<string, string> vars)
    {
        if (string.IsNullOrEmpty(template) || vars.Count == 0) return template;
        return Token.Replace(template, m =>
        {
            var k = m.Groups["k"].Value.Trim();
            return vars.TryGetValue(k, out var v) ? v ?? string.Empty : m.Value;
        });
    }
}