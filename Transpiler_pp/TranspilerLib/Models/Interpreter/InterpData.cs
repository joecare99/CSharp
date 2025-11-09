using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Interpreter;

/// <summary>
/// Laufzeitdaten für den Interpreter (Program Counter und ggf. weitere Statusinformationen in Zukunft).
/// </summary>
public class InterpData
{
    /// <summary>
    /// Erstellt eine neue Instanz und setzt den Program Counter auf <paramref name="next"/>.
    /// </summary>
    /// <param name="next">Erster auszuführender Codeblock.</param>
    public InterpData(ICodeBlock? next)
    {
        pc = next;
    }

    /// <summary>
    /// Program Counter – zeigt auf den aktuell auszuführenden Block.
    /// </summary>
    public ICodeBlock? pc { get; set; }
}