using System;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner
{
    /// <summary>
    /// Repräsentiert einen IEC Code-Block mit auf IEC zugeschnittener Formatierungslogik für die Code-Generierung.
    /// Ableitung von <see cref="CodeBlock"/> zur Spezialisierung der Ausgabe (<see cref="ToCode"/>).
    /// </summary>
    public class IECCodeBlock : CodeBlock, ICodeBlock
    {
        /// <summary>
        /// Erzeugt den (formatierten) Quellcode für diesen Block und seine Unter-Blöcke.
        /// </summary>
        /// <param name="indent">Einrückung (Anzahl Leerzeichen) für den aktuellen Block. Default 2 für kompaktere IEC Darstellung.</param>
        /// <returns>Formatierter Code-String inklusive aller Kind-Blöcke.</returns>
        /// <remarks>
        /// Spezielle Regeln:
        /// <list type="bullet">
        /// <item><description>Labels mit mehr als 2 eingehenden Sprüngen erhalten einen Kommentar zur Visualisierung.</description></item>
        /// <item><description>Operations-/Assignments-Blöcke mit Unterblöcken werden als verkettete Operationkette ausgegeben.</description></item>
        /// <item><description>Kommentarblöcke (Line-/Full-Line/Inline) werden mit Einrückung und Zeilenumbruch ausgegeben.</description></item>
        /// <item><description>Trennzeichen wie ':' oder ';' beenden eine Zeile, außer nach ':' folgt unmittelbar ein Kommentar.</description></item>
        /// <item><description>Variablenblöcke erhalten ein führendes Leerzeichen (für Deklarationslisten).</description></item>
        /// </list>
        /// </remarks>
        public 
            override 
            string ToCode(int indent = 2)
        {
            string codeComment = string.Empty;
            string subCode = string.Empty;

            // Label-Hinweis (Anzahl eingehender Kanten)
            if (Type is CodeBlockType.Label && Sources.Count > 2)
                codeComment = $" // <========== {Sources.Count}";

            // Unterblöcke für Operation / Assignment inline verkettet mit dem Operator-Code
            if (SubBlocks?.Count > 0 && Type is CodeBlockType.Operation or CodeBlockType.Assignment)
                subCode = string.Join(' ' + Code, SubBlocks.Select(c => c.ToCode(indent + 2)));
            else if (SubBlocks?.Count > 1)
                subCode = string.Join(' ', SubBlocks.Select(c => c.ToCode(indent + 2)));

            // Kommentarzeilen (verschiedene Kommentararten) – eigene Zeile
            if (new[] { CodeBlockType.LComment, CodeBlockType.FLComment, CodeBlockType.Comment }.Contains(Type))
                return $"{new string(' ', indent)}{Code}{Environment.NewLine}";
            // Separatoren ':' ';' behandeln – ggf. Zeilenumbruch wenn nicht unmittelbar ein Kommentar folgt
            else if (Type == CodeBlockType.Operation && new[] { ":", ";" }.Contains(Code)
                && Next?.Type != CodeBlockType.LComment
                && (Next?.Type == CodeBlockType.Block || Code != ":"))
                return $"{Code}{codeComment}{Environment.NewLine}";
            // Inline verkettete Operation/Ausdrucksketten
            else if (Type is CodeBlockType.Operation or CodeBlockType.Assignment && SubBlocks?.Count > 0)
                return $"{subCode}";
            // Variablen erhalten einen führenden Space (z.B. nach VAR)
            else if (Type == CodeBlockType.Variable)
                return $" {Code}";
            // Standardformatierung (Einrückung abhängig vom Blocktyp)
            else
                return $"{new string(' ', Type is CodeBlockType.Block or CodeBlockType.Label ? indent - 4 : 1)}{Code}{codeComment}{(SubBlocks.Count > 0 ? "\r\n" : string.Empty)}{subCode}";
        }

        /// <summary>
        /// Debug-/Diagnoseausgabe des Blockes inklusive Meta-Informationen (Name, Typ, Position, Quellverweise).
        /// </summary>
        public override string ToString()
        {
            return $"///{Name} {Type} {Level},{SourcePos},{Index}{(Destination != null ? " Dest:OK" : "")}{(Sources.Count > 0 ? $" {Sources.Count}" : "")}\r\n{Code}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks)}";
        }
    }
}