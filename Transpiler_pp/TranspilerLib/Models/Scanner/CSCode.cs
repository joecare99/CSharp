﻿using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

public partial class CSCode : CodeBase, ICSCode
{

    public static readonly string[] ReservedWords = new[] {
        "catch", "do", "else", "if", "try",
        "while", "switch", "for", "foreach", "return",
        "throw", "using", "lock", "get", "set", "break",
    };

    private static readonly string[] reservedWords2 = new[] {
        "case", "default", "goto", "break", "continue",
        "new", "var", "const", "static", "public",
        "private", "protected", "internal", "class", "struct",
        "interface", "enum", "delegate", "event", "namespace",
        "abstract", "sealed", "override", "virtual",            "extern",
        "readonly", "volatile", "unsafe", "ref", "out",
        "in", "is", "as", "sizeof", "typeof",
        "checked", "unchecked", "fixed", "operator", "implicit",
        "explicit", "this", "base", "params",            "true",
        "false", "null", "void", "object", "string",
        "bool", "byte", "sbyte", "short", "ushort",
        "int", "uint", "long", "ulong", "float",
        "double", "decimal"
    };
    public static readonly char[] stringEndChars = ['"', '\\', '{', '\r'];

    public bool DoWhile { get => !codeOptimizer._noWhile; set => codeOptimizer._noWhile = !value; }

    private ITokenHandler tokenHandler;
    private ICodeBuilder codeBuilder;
    private ICodeOptimizer codeOptimizer;
    //   public string Code { get; set; }

    public CSCode() : base() {
        tokenHandler = new CSTokenHandler() { 
            stringEndChars = stringEndChars, 
            reservedWords = ReservedWords };
        codeBuilder = new CSCodeBuilder() {  };
        codeOptimizer = new CodeOptimizer();
    }
    private static string GetDebug(TokenizeData data, string code) => $"{code.Substring(Math.Max(0, data.Pos - 20), data.Pos - Math.Max(0, data.Pos - 20))}" +
                        $">{code[data.Pos]}<" +
                        $"{code.Substring(Math.Min(data.Pos + 1, code.Length - 1), Math.Min(40, code.Length - data.Pos - 1))}";

    public override IEnumerable<TokenData> Tokenize()
    {
        TokenizeData data = new();
        Stack<TokenData> stack = new();
        while (
                data.Pos < OriginalCode.Length 
                && tokenHandler.TryGetValue(data.State, out var Handler))
        {
            string debug = GetDebug(data, OriginalCode);

            Handler(t => stack.Push(t), OriginalCode, data);
            stack.Reverse();
            while (stack.Count > 0)
                yield return stack.Pop();
            data.Pos++;
        }
    }

    public override void Tokenize(TokenDelegate? token)
    {
        TokenizeData data = new();
        while (data.Pos < OriginalCode.Length && tokenHandler.TryGetValue(data.State, out var Handler))
        {
            //Debug:
            string debug = GetDebug(data, OriginalCode);
            Handler(token, OriginalCode, data);
            data.Pos++;
        }
    }

    public override ICodeBlock Parse(IEnumerable<TokenData>? values = null)
    {
        string BlockCode = string.Empty;
        ICodeBlock codeBlock = new CodeBlock() { Name = "Declaration", Code = "", Parent = null };
        var data = codeBuilder.NewData(codeBlock);

        if (values == null)
            Tokenize((tokenData) => codeBuilder.OnToken(tokenData, data));
        else
            foreach (var item in values)
            {
                codeBuilder.OnToken(item, data);
            }

        foreach (var item in data.gotos)
        {
            if (data.labels.TryGetValue(item.Code.Remove(0, 5).Replace(";", ":"), out var label))
            {
                item.Destination = new(label);
                label.Sources.Add(new(item));
            }
        }

        return codeBlock;
    }

    public void ReorderLabels(ICodeBlock codeBlock)
    {
        List<ICodeBlock> labels = new();
        foreach (var item in codeBlock.SubBlocks)
            if (item.Type is CodeBlockType.Label
                && !item.Code.StartsWith("case ")
                && !item.Code.StartsWith("default:"))
            {
                labels.Add(item);
            }
        if (labels.Count > 1)
        {
            labels.Sort((a, b) => Code2(a.Code).CompareTo(Code2(b.Code)));
            //  System.Diagnostics.Debug.WriteLine($"List: ({string.Join(", ", labels.Cast<CodeBlock>().Select((cb) => cb.Index))})");
            for (var i = labels.Count - 1; i > 0; i--)
                if (labels[i - 1].Index > labels[i].Index)
                {
                    var l = CodeOptimizer.CalcChunksize(labels[i - 1]);
                    //   System.Diagnostics.Debug.WriteLine($"Move[{i - 1}] ({labels[i - 1].Index},{labels[i].Index},{l})");
                    _ = codeBlock.MoveSubBlocks(labels[i - 1].Index, labels[i], l);
                    // System.Diagnostics.Debug.WriteLine($"List: ({string.Join(", ", labels.Cast<CodeBlock>().Select((cb) => cb.Index))})");
                }
            labels.Clear();
        }
        foreach (var item in codeBlock.SubBlocks)
            if (item.SubBlocks.Count > 0)
                ReorderLabels(item);

        static string Code2(string code)
        {
            return code.StartsWith("IL_")
                ? "IL_" + code.Substring(3).PadLeft(7, '0')
                : code.Replace("Start", "0000");
        }
    }

    /// <summary>
    /// Removes the single source labels1.
    /// </summary>
    /// <param name="codeBlock">The code block.</param>
    public void RemoveSingleSourceLabels1(ICodeBlock codeBlock)
    {
        List<ICodeBlock> labels = new();
        foreach (var item in codeBlock.SubBlocks)
            if (item.Type is CodeBlockType.Label
                && item.Sources.Count <= 2
                && item.Sources.Count > 0
                && !item.Code.StartsWith("case ")
                && !item.Code.StartsWith("default:"))
                labels.Add(item);

        foreach (var item in labels)
        {
            codeOptimizer.TestItem(item);
        }
        foreach (var item in codeBlock.SubBlocks.ToArray())
            if (item.SubBlocks.Count > 0)
                RemoveSingleSourceLabels1(item);
    }

}
