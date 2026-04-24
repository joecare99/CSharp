using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Pascal.Data; // hinzugefügt für PasReservedWords

namespace TranspilerLib.Pascal.Interpreter;

public class PasInterpreter
{
    // Pseudocode Änderung:
    // - Feste Liste der Schlüsselwörter durch PasReservedWords.Words ersetzen.
    // - Zusätzliche nicht in PasReservedWords.Words enthaltene Pascal/Delphi Schlüsselwörter ergänzen.
    // - Als HashSet zwischenspeichern (case-insensitive).
    private static readonly HashSet<string> ReservedWordSet = new(
        PasReservedWords.Words
            .Concat(
            [
                // Zusätzliche Operatoren / Literale / Kontrollworte
                "AND","OR","NOT","DIV","MOD","TRUE","FALSE","BREAK","CONTINUE","EXIT","NIL","GOTO","RECORD"
            ]),
        StringComparer.OrdinalIgnoreCase
    );

    public bool Interpret(ICodeBlock codeBlock)
    {
        if (Prepare(codeBlock))
        {
            return Execute(codeBlock);
        }
        else
        {
            return false;
        }
    }

    public Dictionary<string, object> Variables { get; set; } = new();
    public Dictionary<string, Func<object, object>> Externals { get; set; } = new();

    private bool Execute(ICodeBlock codeBlock)
    {
        // Todo: Implement execution logic here
        return true;
    }

    public bool Prepare(ICodeBlock codeBlock)
    {
        if (codeBlock is null) return false;

        var declaredVars = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var declaredFuncs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var externalCallCandidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        Walk(codeBlock, declaredVars, declaredFuncs, externalCallCandidates);

        foreach (var name in externalCallCandidates)
        {
            if (declaredFuncs.Contains(name)) continue;
            if (!Externals.ContainsKey(name))
            {
                var localName = name;
                Externals[localName] = (obj) =>
                    throw new InvalidOperationException($"External '{localName}' is not linked.");
            }
        }

        return true;
    }

    private void Walk(ICodeBlock node, HashSet<string>? declaredVars, HashSet<string>? declaredFuncs, HashSet<string>? externalCallCandidates)
    {
        switch (node.Type)
        {
            case CodeBlockType.Variable:
            case CodeBlockType.Declaration:
            case CodeBlockType.Parameter:
                ParseVariableDecl(node, declaredVars);
                break;
            case CodeBlockType.Function:
                ParseFunctionDecl(node, declaredFuncs);
                break;
            default:
                break;
        }

        ExtractExternalCalls(node, declaredFuncs, externalCallCandidates);

        var subs = node.SubBlocks;
        if (subs is not null)
        {
            for (int i = 0; i < subs.Count; i++)
            {
                var child = subs[i];
                if (child is not null)
                    Walk(child, declaredVars, declaredFuncs, externalCallCandidates);
            }
        }
    }

    private static void ExtractExternalCalls(ICodeBlock node, HashSet<string>? declaredFuncs, HashSet<string>? externalCallCandidates)
    {
        if (externalCallCandidates is null) return;

        var code = node.Code ?? string.Empty;
        code = StripStrings(code);

        // Einzelnes Wort direkt als Schlüsselwort / Funktionsname prüfen
        if (ReservedWordSet.Contains(code)) return;
        if (declaredFuncs != null && declaredFuncs.Contains(code)) return;

        externalCallCandidates.Add(code);
    }

    private static string StripStrings(string s)
    {
        if (string.IsNullOrEmpty(s)) return s ?? string.Empty;
        var sb = new StringBuilder(s.Length);
        var inStr = false;

        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c == '\'')
            {
                if (inStr)
                {
                    if (i + 1 < s.Length && s[i + 1] == '\'')
                    {
                        i++; // skip escaped apostrophe
                        continue;
                    }
                    inStr = false;
                }
                else
                {
                    inStr = true;
                }
                continue;
            }
            if (!inStr) sb.Append(c);
        }
        return sb.ToString();
    }

    private static void ParseFunctionDecl(ICodeBlock node, HashSet<string>? declaredFuncs)
    {
        if (declaredFuncs is null) return;

        if (!string.IsNullOrWhiteSpace(node.Name))
        {
            declaredFuncs.Add(node.Name.Trim());
            return;
        }

        var code = node.Code ?? string.Empty;
        var m = System.Text.RegularExpressions.Regex.Match(
            code,
            @"\b(function|procedure)\s+([A-Za-z_]\w*)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
        if (m.Success)
            declaredFuncs.Add(m.Groups[2].Value);
    }

    private void ParseVariableDecl(ICodeBlock node, HashSet<string>? declaredVars)
    {
        if (declaredVars is null) return;

        if (!string.IsNullOrWhiteSpace(node.Name))
        {
            RegisterVariable(node.Name, declaredVars);
            return;
        }

        var code = node.Code ?? string.Empty;
        var beforeColon = code;
        var colonIdx = code.IndexOf(':');
        if (colonIdx >= 0) beforeColon = code[..colonIdx];

        foreach (var part in beforeColon.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            RegisterVariable(part, declaredVars);
    }

    private void RegisterVariable(string? name, HashSet<string>? declaredVars)
    {
        if (declaredVars is null) return;
        if (string.IsNullOrWhiteSpace(name)) return;
        var clean = name.Trim();
        if (!declaredVars.Contains(clean))
            declaredVars.Add(clean);
        if (!Variables.ContainsKey(clean))
            Variables[clean] = null!;
    }
}
