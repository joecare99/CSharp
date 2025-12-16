using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerLib.CSharp.Data;

public static class CSReservedWords
{
    /// <summary>
    /// Reserved C# keywords used by the tokenizer to distinguish identifiers from language constructs.
    /// </summary>
    /// <remarks>
    /// This set is consumed by <see cref="CSTokenHandler"/> to drive lexical decisions while scanning control-flow
    /// and other language elements. Extend with care to avoid misclassification of identifiers.
    /// </remarks>
    public static readonly string[] ReservedWords = Enum.GetNames(typeof(ECSharpResWords1));

    public static readonly string[] ReservedWords2 = [
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
    ];

}
