namespace TranspilerLib.Models
{
    /// <summary>
    /// Kapselt den veränderlichen Zustand während des Tokenisierungs-/Scan-Vorgangs.
    /// </summary>
    /// <remarks>
    /// Diese Struktur dient als einfacher Container für Positions- und Zustandsinformationen,
    /// die typischerweise von einem Scanner oder Lexer inkrementell aktualisiert werden.
    /// Alle Felder sind absichtlich öffentlich und als einfache Felder (keine Properties) gehalten,
    /// um Overhead bei sehr häufigen Aktualisierungen zu vermeiden.
    /// </remarks>
    public class TokenizeData
    {
        /// <summary>
        /// Primärer Positionszeiger im Quellpuffer (aktuelles Zeichen / aktueller Index).
        /// </summary>
        public int Pos = 0;

        /// <summary>
        /// Sekundärer Positionszeiger (z. B. Start eines Tokens oder Lookahead-Beginn).
        /// </summary>
        public int Pos2 = 0;

        /// <summary>
        /// Allgemeiner Zähler / Stack-Level (z. B. für Verschachtelungen wie Klammern).
        /// </summary>
        public int Stack = 0;

        /// <summary>
        /// Aktueller Zustandswert der zustandsbasierten Tokenisierungs- / FSM-Logik.
        /// </summary>
        public int State = 0;

        /// <summary>
        /// Allgemeines Flag für temporäre Markierungen oder bedingte Pfade im Scan-Prozess.
        /// </summary>
        public bool flag = false;

        /// <summary>
        /// Initialisiert eine neue Instanz mit Standardwerten (alle numerischen Felder = 0, Flag = false).
        /// </summary>
        public TokenizeData() { }
    }
}
