using System;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Basisklasse für spezifische Token-Handler innerhalb des Scannervorgangs.
/// Stellt Hilfsmethoden bereit, um Zeichen aus dem Originalcode zu lesen und aus dem Scan-Bereich Tokens zu emittieren.
/// </summary>
/// <remarks>
/// Die Klasse kapselt wiederkehrende Operationen wie:
/// 1. Ermitteln des nächsten / vorherigen Zeichens ohne Ausnahme auszulösen.
/// 2. Extraktion des aktuell gescannten Textsegments (Bereich zwischen <see cref="TokenizeData.Pos2"/> und <see cref="TokenizeData.Pos"/>).
/// 3. Erzeugung und Übergabe eines <see cref="TokenData"/> an einen bereitgestellten Delegaten (<see cref="TokenDelegate"/>).
/// </remarks>
public abstract class TokenHandlerBase
{
    /// <summary>
    /// Erzeugt aus dem aktuellen Scanbereich ein <see cref="TokenData"/> und übergibt es an den angegebenen Delegaten.
    /// </summary>
    /// <param name="token">Delegat, der das erzeugte Token verarbeitet (kann null sein).</param>
    /// <param name="data">Scan-Zustandsdaten (Positionsmarker, Stacktiefe etc.).</param>
    /// <param name="type">Der zuzuweisende <see cref="CodeBlockType"/> für das erzeugte Token.</param>
    /// <param name="originalCode">Der vollständige Quellcode, aus dem gescannt wird.</param>
    /// <param name="offs">
    /// Optionaler Off-Set, um den Endbereich zu erweitern (z. B. um abschließende Zeichen einzuschließen).
    /// Typischer Einsatz bei Abschlusszeichen oder nachlaufenden Trennern.
    /// </param>
    /// <remarks>
    /// Der extrahierte Text reicht von <c>data.Pos2</c> bis <c>data.Pos + offs</c> (inklusive) und wird vor Übergabe getrimmt.
    /// Wenn <paramref name="token"/> null ist, passiert nichts.
    /// </remarks>
    protected static void EmitToken(TokenDelegate? token, TokenizeData data, CodeBlockType type, string originalCode, int offs = 0)
        => token?.Invoke(new TokenData(
            originalCode.Substring(data.Pos2, data.Pos - data.Pos2 + offs).Trim(),
            type,
            data.Stack,
            data.Pos2));

    /// <summary>
    /// Liefert das nächste Zeichen relativ zur übergebenen Position.
    /// </summary>
    /// <param name="Pos">Aktuelle Indexposition im Quellcode.</param>
    /// <param name="OriginalCode">Der vollständige Quelltext.</param>
    /// <returns>
    /// Das Zeichen an Position <c>Pos + 1</c>, oder das Null-Zeichen <c>'\0'</c>, wenn das Ende erreicht ist.
    /// </returns>
    /// <remarks>
    /// Sicherer Zugriff ohne Ausnahme am Ende des Strings.
    /// </remarks>
    protected static char GetNxtChar(int Pos, string OriginalCode)
        => Pos + 1 < OriginalCode.Length ? OriginalCode[Pos + 1] : '\u0000';

    /// <summary>
    /// Liefert das vorherige Zeichen relativ zur übergebenen Position.
    /// </summary>
    /// <param name="Pos">Aktuelle Indexposition im Quellcode.</param>
    /// <param name="OriginalCode">Der vollständige Quelltext.</param>
    /// <returns>
    /// Das Zeichen an Position <c>Pos - 1</c>, oder das erste Zeichen, wenn <paramref name="Pos"/> gleich 0 ist.
    /// </returns>
    /// <remarks>
    /// Negative Indizes werden abgefangen, indem auf Index 0 zurückgegriffen wird.
    /// </remarks>
    protected static char GetPrvChar(int Pos, string OriginalCode)
        => OriginalCode[Math.Max(Pos - 1, 0)];

    /// <summary>
    /// Extrahiert den aktuell gescannten Text zwischen <see cref="TokenizeData.Pos2"/> und <see cref="TokenizeData.Pos"/>.
    /// </summary>
    /// <param name="data">Aktuelle Scan-Daten mit Start- und Endposition.</param>
    /// <param name="OriginalCode">Optional: Vollständiger Quellcode. Wenn leer, wird ein leerer String zurückgegeben.</param>
    /// <param name="offs">Optionaler Erweiterungs-Offset (positiv oder 0). Negative Werte werden als 0 behandelt.</param>
    /// <returns>
    /// Das nicht getrimmte Rohsegment des Quelltexts. Bei ungültigen Längen wird ein leerer String geliefert.
    /// </returns>
    /// <remarks>
    /// Diese Methode führt kein Trimming durch (im Gegensatz zu <see cref="EmitToken(TokenDelegate?, TokenizeData, CodeBlockType, string, int)"/>).
    /// </remarks>
    protected static string GetText(TokenizeData data, string OriginalCode = "", int offs = 0)
        => OriginalCode.Substring(data.Pos2, Math.Max(data.Pos - data.Pos2 + offs, 0));
}