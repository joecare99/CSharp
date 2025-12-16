using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.CSharp.Data;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Provides a concrete implementation for tokenizing, parsing, and optimizing C# source code.
/// </summary>
/// <remarks>
/// The class orchestrates three main collaborators:
/// <list type="bullet">
/// <item><description><see cref="ITokenHandler"/> to scan the raw <see cref="CodeBase.OriginalCode"/> and produce tokens.</description></item>
/// <item><description><see cref="ICodeBuilder"/> to build an <see cref="ICodeBlock"/> tree from the token stream.</description></item>
/// <item><description><see cref="ICodeOptimizer"/> to perform structural optimizations on the produced block tree.</description></item>
/// </list>
/// It also exposes utilities to reorder labels and to simplify label usage, which can improve readability of the re-emitted code.
/// </remarks>
public partial class CSCode : CodeBase, ICSCode
{

    /// <summary>
    /// Reserved C# keywords used by the tokenizer to distinguish identifiers from language constructs.
    /// </summary>
    /// <remarks>
    /// This set is consumed by <see cref="CSTokenHandler"/> to drive lexical decisions while scanning control-flow
    /// and other language elements. Extend with care to avoid misclassification of identifiers.
    /// </remarks>
    public static readonly string[] ReservedWords = CSReservedWords.ReservedWords;

    private static readonly string[] reservedWords2 = CSReservedWords.ReservedWords2;

    /// <summary>
    /// Characters that signal the end (or state transition) of a string/interpolated string during tokenization.
    /// </summary>
    /// <remarks>
    /// The handler uses these characters to split string literals and handle escapes/interpolation correctly: a quote ('"'),
    /// a backslash ('\\'), an opening brace ('{') for interpolated strings, and carriage return ('\r').
    /// </remarks>
    public static readonly char[] stringEndChars = ['"', '\\', '{', '\r'];

    /// <summary>
    /// Enables or disables <c>do..while</c> loop reconstruction during optimization.
    /// </summary>
    /// <value>
    /// <c>true</c> to allow the optimizer to synthesize <c>do..while</c> constructs where applicable; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// Internally mapped to <see cref="ICodeOptimizer"/> via an inverted flag (<c>_noWhile</c>). Setting this property to
    /// <c>true</c> clears the internal "no while" restriction; setting it to <c>false</c> disables such rewrites.
    /// </remarks>
    public bool DoWhile { get => !codeOptimizer._noWhile; set => codeOptimizer._noWhile = !value; }

    private ITokenHandler tokenHandler;
    private ICodeBuilder codeBuilder;
    private ICodeOptimizer codeOptimizer;
    //   public string Code { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CSCode"/> class with default tokenizer, builder, and optimizer.
    /// </summary>
    /// <remarks>
    /// The constructor wires <see cref="CSTokenHandler"/> with <see cref="stringEndChars"/> and <see cref="ReservedWords"/>, 
    /// uses a <see cref="CSCodeBuilder"/> to construct block trees, and applies a default <c>CodeOptimizer</c> for structural clean-up.
    /// </remarks>
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

    /// <summary>
    /// Produces a complete token stream for the current <see cref="CodeBase.OriginalCode"/>.
    /// </summary>
    /// <returns>
    /// An ordered sequence of <see cref="TokenData"/> items representing the lexed input.
    /// </returns>
    /// <remarks>
    /// This method iteratively invokes the configured <see cref="ITokenHandler"/> states until the end of the input is reached.
    /// Internally, tokens collected on a stack are reversed to preserve their semantic order before being yielded.
    /// </remarks>
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

    /// <summary>
    /// Tokenizes the input and optionally streams each token to a callback.
    /// </summary>
    /// <param name="token">Optional callback to receive tokens as they are produced; may be <c>null</c> to skip streaming.</param>
    /// <remarks>
    /// Compared to <see cref="Tokenize()"/>, this variant is designed for on-the-fly processing. It advances through the input
    /// by repeatedly invoking the active <see cref="ITokenHandler"/> state and calling the provided delegate for each emitted token.
    /// </remarks>
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

    /// <summary>
    /// Parses either a provided token sequence or the tokens produced from the current <see cref="CodeBase.OriginalCode"/>.
    /// </summary>
    /// <param name="values">Optional externally supplied tokens; when <c>null</c>, the method tokenizes the current input first.</param>
    /// <returns>
    /// The root <see cref="ICodeBlock"/> representing the parsed structure (e.g., declarations and nested blocks).
    /// </returns>
    /// <remarks>
    /// The method constructs a fresh <see cref="ICodeBuilderData"/> context, feeds all tokens into the <see cref="ICodeBuilder"/>,
    /// and finally resolves <c>goto</c> destinations to their corresponding label blocks via weak references.
    /// </remarks>
    public override ICodeBlock Parse(IEnumerable<TokenData>? values = null)
    {
        string BlockCode = string.Empty;
        ICodeBlock codeBlock = new CodeBlock() { Name = "Declaration",Type=CodeBlockType.MainBlock, Code = "", Parent = null };
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

    /// <summary>
    /// Reorders label blocks in a subtree to restore natural control-flow order.
    /// </summary>
    /// <param name="codeBlock">The root whose immediate and nested labels will be considered for reordering.</param>
    /// <remarks>
    /// Only labels that are not <c>case</c> or <c>default</c> labels are considered. Labels are sorted by their textual code,
    /// with special handling for MSIL-style prefixes (e.g., <c>IL_</c>) and "Start" markers to maintain numeric ordering.
    /// When necessary, contiguous chunks are moved via <see cref="ICodeBlock.MoveSubBlocks(int, ICodeBlock, int)"/>.
    /// </remarks>
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
    /// Simplifies labels that are referenced only from a single source (or at most two), recursively across the tree.
    /// </summary>
    /// <param name="codeBlock">The root block to inspect and simplify.</param>
    /// <remarks>
    /// This method collects candidate labels that are not switch <c>case</c>/<c>default</c> labels and delegates simplification
    /// to the configured <see cref="ICodeOptimizer"/>. It then recurses into child blocks to continue the clean-up.
    /// </remarks>
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
