using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.CSharp.StatEqualCheck;

internal static class CodeBlockSourceRenderer
{
    public static string Render(ICodeBlock root, IEnumerable<ICodeBlock>? terminalBlocks, string parameterName)
    {
        var terminals = new HashSet<ICodeBlock>(ReferenceEqualityComparer.Instance);
        if (terminalBlocks is not null)
        {
            foreach (var block in terminalBlocks)
            {
                if (block is null)
                    throw new ArgumentException("Terminal block collections cannot contain null entries.", parameterName);
                terminals.Add(block);
            }
        }

        var visited = new HashSet<ICodeBlock>(ReferenceEqualityComparer.Instance);
        var foundTerminals = new HashSet<ICodeBlock>(ReferenceEqualityComparer.Instance);
        var clone = Clone(root, terminals, visited, foundTerminals);
        if (!foundTerminals.SetEquals(terminals))
            throw new ArgumentException("Every terminal block must be a block instance contained in its corresponding tree.", parameterName);
        return clone.ToCode();
    }

    private static CodeBlock Clone(ICodeBlock source, HashSet<ICodeBlock> terminals,
        HashSet<ICodeBlock> visited, HashSet<ICodeBlock> foundTerminals)
    {
        if (!visited.Add(source))
            throw new ArgumentException("The input code-block structure must be a tree without repeated or cyclic nodes.");

        var isTerminal = terminals.Contains(source);
        if (isTerminal)
            foundTerminals.Add(source);

        var clone = new CodeBlock
        {
            Type = isTerminal ? CodeBlockType.Operation : source.Type,
            Name = isTerminal ? nameof(CodeBlockType.Operation) : source.Name,
            Code = isTerminal ? "return;" : source.Code,
            SourcePos = source.SourcePos
        };
        if (!isTerminal)
        {
            foreach (var child in source.SubBlocks)
            {
                var childClone = Clone(child, terminals, visited, foundTerminals);
                childClone.Parent = clone;
            }
        }
        return clone;
    }
}
