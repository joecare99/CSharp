using System;
using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Abstrakte Basisklasse zur Erstellung eines hierarchischen <see cref="ICodeBlock"/>-Baumes
/// aus einer Sequenz von <see cref="TokenData"/>. Konkrete Ableitungen übersteuern
/// üblicherweise <see cref="OnToken(TokenData, ICodeBuilderData)"/>, um sprachspezifische
/// Strukturierungsregeln (z.B. Blockgrenzen) zu implementieren.
/// </summary>
public abstract class CodeBuilder : ICodeBuilder
{
    /// <summary>
    /// Fabrik-Delegate zum Anlegen eines neuen <see cref="ICodeBlock"/>. Kann von außen ersetzt werden
    /// (z.B. für spezielle Block-Typen oder Instrumentierung / Tests).
    /// </summary>
    /// <remarks>
    /// Parameter: (name, type, code, parent, pos)
    /// </remarks>
    public Func<string, CodeBlockType, string, ICodeBlock, int, ICodeBlock> NewCodeBlock { get; set; } =
        (name, type, code, parent, pos) => new CodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };

    /// <summary>
    /// Interne Standard-Implementierung der zur Laufzeit gehaltenen Build-Daten.
    /// </summary>
    private class CodeBuilderData : ICodeBuilderData
    {
        /// <inheritdoc />
        public Dictionary<string, ICodeBlock> labels { get; set; } = new();
        /// <inheritdoc />
        public List<ICodeBlock> gotos { get; set; } = new();
        /// <inheritdoc />
        public ICodeBlock actualBlock { get; set; }
        /// <inheritdoc />
        public bool xBreak { get; set; } = false;
        /// <inheritdoc />
        public CodeBlockType cbtLast { get; set; } = CodeBlockType.Unknown;

        /// <summary>
        /// Erstellt eine neue Dateninstanz für den angegebenen Startblock.
        /// </summary>
        /// <param name="codeBlock">Aktueller (Start-)Block.</param>
        public CodeBuilderData(ICodeBlock codeBlock)
        {
            actualBlock = codeBlock;
        }
    }

    /// <summary>
    /// Erzeugt eine neue Build-Datenstruktur für einen gegebenen Wurzelblock.
    /// Kann in Ableitungen übersteuert werden um angepasste Implementierungen bereitzustellen.
    /// </summary>
    /// <param name="block">Wurzel / Startblock.</param>
    /// <returns>Datencontainer zur Fortschrittsverwaltung.</returns>
    public virtual ICodeBuilderData NewData(ICodeBlock block)
        => new CodeBuilderData(block);

    /// <summary>
    /// Verarbeitet ein einzelnes Token und erzeugt (bzw. verschiebt) den aktuellen Block-Kontext.
    /// Die Default-Implementierung leitet anhand der Level-Differenz die Elternebene ab.
    /// </summary>
    /// <param name="tokenData">Aktuelles Token.</param>
    /// <param name="data">Mutable Builder-Daten (enthält u.a. den aktuellen Block).</param>
    /// <exception cref="NotImplementedException">Geworfen wenn eine unvorhergesehene Level-Differenz auftritt.</exception>
    public virtual void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        ICodeBlock NewBlock = (data.actualBlock.Level) switch
        {
            // Ein Level höher als das Token: wir hängen als Geschwister an den Parent
            int i when i == tokenData.Level + 1
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent, tokenData.Pos),
            // Gleiches Level: Kind des aktuellen Blocks
            int i when i == tokenData.Level
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock, tokenData.Pos),
            // Zwei Levels höher: wir steigen zwei Ebenen auf
            int i when i == tokenData.Level + 2
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent.Parent, tokenData.Pos),
            // Drei Levels höher (Sonderfall) – vermutlich fehlerhafte Struktur aber aktuell zugelassen
            int i when i == tokenData.Level + 3
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent.Parent.Parent, tokenData.Pos),
            _ => throw new NotImplementedException()
        };

        data.actualBlock = NewBlock;
    }
}
