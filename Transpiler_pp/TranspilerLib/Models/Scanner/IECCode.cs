using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner
{
    public partial class IECCode : CodeBase, IIECCode
    {

        private static readonly string[] reservedWords = new[] {
            "catch", "do", "else", "IF", "try","THEN",
            "while", "switch", "for", "foreach", "return",
            "throw", "using", "lock", "get", "set", "break",
        };
        public static string[] ReservedWords2 => Enum.GetNames(typeof(IECResWords)).Select(s => s.Substring(3)).ToArray();

        public static Dictionary<IECResWords, IECResWords> IECBlocksWords = new()
        {
            {IECResWords.rw_CONFIGURATION,IECResWords.rw_END_CONFIGURATION},
            {IECResWords.rw_FOR,IECResWords.rw_END_FOR},
            {IECResWords.rw_FUNCTION,IECResWords.rw_END_FUNCTION},
            {IECResWords.rw_FUNCTION_BLOCK,IECResWords.rw_END_FUNCTION_BLOCK},
            {IECResWords.rw_IF,IECResWords.rw_END_IF},
            {IECResWords.rw_NAMESPACE,IECResWords.rw_END_NAMESPACE},
            {IECResWords.rw_PROGRAM,IECResWords.rw_END_PROGRAM},
            {IECResWords.rw_REPEAT,IECResWords.rw_END_REPEAT},
            {IECResWords.rw_RESOURCE,IECResWords.rw_END_RESOURCE},
            {IECResWords.rw_STEP,IECResWords.rw_END_STEP},
            {IECResWords.rw_STRUCT,IECResWords.rw_END_STRUCT},
            {IECResWords.rw_TRANSITION,IECResWords.rw_END_TRANSITION},
            {IECResWords.rw_TYPE,IECResWords.rw_END_TYPE},
            {IECResWords.rw_VAR,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_EXTERNAL,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_GLOBAL,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_INPUT,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_INST,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_IN_OUT,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_OUTPUT,IECResWords.rw_END_VAR},
            {IECResWords.rw_VAR_TEMP,IECResWords.rw_END_VAR},
            {IECResWords.rw_UNION,IECResWords.rw_END_UNION},
            {IECResWords.rw_WHILE,IECResWords.rw_END_WHILE},
        };

        public static Dictionary<IECResWords, TypeCode> IECSysTypes = new()
        {
            {IECResWords.rw_BOOL, TypeCode.Boolean},
            {IECResWords.rw_BYTE, TypeCode.Byte},
            {IECResWords.rw_CHAR, TypeCode.Char},
            {IECResWords.rw_DATE, TypeCode.DateTime},
            {IECResWords.rw_DATE_AND_TIME, TypeCode.DateTime},
            {IECResWords.rw_DINT, TypeCode.Int32},
            {IECResWords.rw_DT, TypeCode.DateTime},
            {IECResWords.rw_DWORD, TypeCode.UInt32},
            {IECResWords.rw_INT, TypeCode.Int32},
            {IECResWords.rw_LDATE, TypeCode.DateTime},
            {IECResWords.rw_LDATE_AND_TIME, TypeCode.DateTime},
            {IECResWords.rw_LINT, TypeCode.Int64},
            {IECResWords.rw_LREAL, TypeCode.Double},
            {IECResWords.rw_LTIME, TypeCode.DateTime},
            {IECResWords.rw_LTIME_OF_DAY, TypeCode.DateTime},
            {IECResWords.rw_LTOD, TypeCode.DateTime},
            {IECResWords.rw_LWORD, TypeCode.UInt64},
            {IECResWords.rw_REAL, TypeCode.Single},
            {IECResWords.rw_SINGLE, TypeCode.Single},
            {IECResWords.rw_SINT, TypeCode.SByte},
            {IECResWords.rw_STRING, TypeCode.String},
            {IECResWords.rw_TIME, TypeCode.DateTime},
            {IECResWords.rw_TIME_OF_DAY, TypeCode.DateTime},
            {IECResWords.rw_TOD, TypeCode.DateTime},
            {IECResWords.rw_UDINT, TypeCode.UInt32},
            {IECResWords.rw_UINT, TypeCode.UInt32},
            {IECResWords.rw_ULINT, TypeCode.UInt64},
            {IECResWords.rw_USINT, TypeCode.Byte},
            {IECResWords.rw_WORD, TypeCode.UInt16},
            {IECResWords.rw_WSTRING, TypeCode.String},
        };

        public bool DoWhile { get => !codeOptimizer._noWhile; set => codeOptimizer._noWhile = !value; }

        private ITokenHandler tokenHandler = new IECTokenHandler() { reservedWords = ReservedWords2, blockWords = IECBlocksWords, sysTypes = IECSysTypes };
        private ICodeBuilder codeBuilder = new IECCodeBuilder() { };
        private ICodeOptimizer codeOptimizer = new CodeOptimizer();
        //   public string Code { get; set; }

        private static string GetDebug(TokenizeData data, string code) => $"{code.Substring(Math.Max(0, data.Pos - 20), data.Pos - Math.Max(0, data.Pos - 20))}" +
                            $">{code[data.Pos]}<" +
                            $"{code.Substring(Math.Min(data.Pos + 1, code.Length - 1), Math.Min(40, code.Length - data.Pos - 1))}";

        public override IEnumerable<TokenData> Tokenize()
        {
            TokenizeData data = new();
            List<TokenData> fifo = new();
            Tokenize(t => fifo.Add(t));
            foreach (var itm in fifo)
                yield return itm;
            fifo.Clear();
        }

        public override void Tokenize(TokenDelegate? doToken)
        {
            TokenizeData data = new();
            while (data.Pos < OriginalCode.Length 
                && tokenHandler.TryGetValue(data.State, out var Handler))
            {
                //Debug:
                string debug = GetDebug(data, OriginalCode);
                Handler(doToken, OriginalCode, data);
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


    }
}
