using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Abstrakte Basisklasse für IEC / C / andere Quellcode-Repräsentationen.
/// Bietet den gemeinsamen Lebenszyklus: <see cref="OriginalCode"/> setzen, <see cref="Tokenize()"/> (oder Callback-Variante),
/// anschließend <see cref="Parse(System.Collections.Generic.IEnumerable{TranspilerLib.Data.TokenData}?)"/> zum Aufbau eines <see cref="ICodeBlock"/>-Baums
/// und abschließend <see cref="ToCode(ICodeBlock, int)"/> zur Re-Emission / Transformation.
/// </summary>
public abstract class CodeBase : ICodeBase
{
    /// <summary>
    /// Roh-Quelltext der Einheit (z.B. eine Datei, Funktionsblock, Fragment).
    /// Kann von konkreten Implementierungen beim Tokenisieren ausgewertet werden.
    /// </summary>
    public string OriginalCode { get; set; } = string.Empty;

    /// <summary>
    /// Vollständige Tokenisierung des aktuell gesetzten <see cref="OriginalCode"/>.
    /// </summary>
    /// <returns>Enumerable aller erzeugten <see cref="TokenData"/> in lexikalischer Reihenfolge.</returns>
    public abstract IEnumerable<TokenData> Tokenize();

    /// <summary>
    /// Tokenisiert den Quelltext und ruft für jedes Token den optionalen Callback auf.
    /// </summary>
    /// <param name="token">Optionaler Delegat zur Streaming-Verarbeitung einzelner Tokens (kann <c>null</c> sein).</param>
    public abstract void Tokenize(ICodeBase.TokenDelegate? token);

    /// <summary>
    /// Parst entweder die übergebenen Tokens oder – falls <paramref name="values"/> <c>null</c> ist – das Ergebnis von <see cref="Tokenize()"/>.
    /// </summary>
    /// <param name="values">Vorgegebene Token-Sequenz (optional).</param>
    /// <returns>Wurzel-<see cref="ICodeBlock"/> des generierten Syntax-/Strukturbaumes.</returns>
    public abstract ICodeBlock Parse(IEnumerable<TokenData>? values = null);

    /// <summary>
    /// Konvertiert einen vorhandenen Block-Baum in formatierten Code.
    /// </summary>
    /// <param name="codeBlock">Wurzel des auszugebenden Codeblock-Baumes.</param>
    /// <param name="indent">Basis-Einrückung (Default 4).</param>
    /// <returns>Formatierter Code als String.</returns>
    public string ToCode(ICodeBlock codeBlock, int indent = 4)
    {
        return codeBlock.ToCode(indent);
    }
}