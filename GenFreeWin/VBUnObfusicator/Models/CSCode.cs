using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static VBUnObfusicator.Models.ICSCode;

namespace VBUnObfusicator.Models
{
    public partial class CSCode : ICSCode
    {

        private static readonly string[] reservedWords = new[] {
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
        private static readonly char[] operatorSet = new[] { '(', ')', '{', '}', '[', ']', ';', ',', '.', '+', '-', '*', '/', '%', '&', '|', '^', '!', '~', '=', '<', '>', '?', ':', '"', '\'', '\\', '#' };
        private static readonly char[] whitespace = new[] { ' ', '\t', '\r', '\n', '\u0000' };
        private static readonly char[] numbers = '0'.To('9');
        private static readonly char[] hexNumbers = numbers.Concat('A'.To('F')).Concat('a'.To('f')).ToArray();
        private static readonly char[] letters = 'A'.To('Z').Concat('a'.To('z')).ToArray();
        private static readonly char[] lettersAndNumbers = letters.Concat(numbers).ToArray();
        private static readonly char[] stringEndChars = { '"', '\\', '{', '\r' };

        public string OriginalCode { get; set; } = string.Empty;
        public bool DoWhile { get => !CodeOptimizer._noWhile; set => CodeOptimizer._noWhile = !value; }

        //   public string Code { get; set; }

        private class TokenizeData
        {
            public int Pos = 0;
            public int Pos2 = 0;
            public int Stack = 0;
            public int State = 0;
            public bool flag = false;
            public TokenizeData() { }
        }

        private static readonly Dictionary<int, Action<TokenDelegate?, string, TokenizeData>> _tokenStateHandler = new()
        {
            { 0, TokenHandler.HandleDefault },
            { 1, TokenHandler.HandleStrings },
            { 2, TokenHandler.HandleLineComments },
            { 3, TokenHandler.HandleBlockComments },
            { 4, TokenHandler.HandleStrings },
            { 5, TokenHandler.HandleStrings },
            { 6, (_, s, d) => d.State = s[d.Pos] == '}' ? 4 : d.State},
        };
        private static string GetDebug(TokenizeData data, string code) => $"{code.Substring(Math.Max(0, data.Pos - 20), data.Pos - Math.Max(0, data.Pos - 20))}" +
                            $">{code[data.Pos]}<" +
                            $"{code.Substring(Math.Min(data.Pos + 1, code.Length - 1), Math.Min(40, code.Length - data.Pos - 1))}";

        public void Tokenize(TokenDelegate? token)
        {
            TokenizeData data = new();
            while (data.Pos < OriginalCode.Length && _tokenStateHandler.TryGetValue(data.State, out var Handler))
            {
                //Debug:
                string debug = GetDebug(data, OriginalCode);
                Handler(token, OriginalCode, data);
                data.Pos++;
            }
        }

        public IEnumerable<TokenData> Tokenize()
        {
            TokenizeData data = new();
            Stack<TokenData> stack = new();
            while (data.Pos < OriginalCode.Length && _tokenStateHandler.TryGetValue(data.State, out var Handler))
            {
                string debug = GetDebug(data, OriginalCode);

                Handler(t => stack.Push(t), OriginalCode, data);
                stack.Reverse();
                while (stack.Count > 0)
                    yield return stack.Pop();
                data.Pos++;
            }
        }

        public ICodeBlock Parse(IEnumerable<TokenData>? values = null)
        {
            string BlockCode = string.Empty;
            ICodeBlock codeBlock = new CodeBlock() { Name = "Declaration", Code = "", Parent = null };
            var data = new CodeBuilder.CodeBuilderData(codeBlock);

            if (values == null)
                Tokenize((tokenData) => CodeBuilder.OnToken(tokenData, data));
            else
                foreach (var item in values)
                {
                    CodeBuilder.OnToken(item, data);
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

        public string ToCode(ICodeBlock codeBlock, int indent = 4)
        {
            return codeBlock.ToCode(indent);
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
                CodeOptimizer.TestItem(item);
            }
            foreach (var item in codeBlock.SubBlocks.ToArray())
                if (item.SubBlocks.Count > 0)
                    RemoveSingleSourceLabels1(item);
        }

    }
}
