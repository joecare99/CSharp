namespace TranspilerLib.Interfaces
{
    /// <summary>
    /// Definiert einen vorwärtsgerichteten, zustandsbehafteten Reader über eine
    /// strukturierte Eingabe (z. B. Token-, Knoten- oder Elementsequenz).
    /// Der Reader erlaubt sequenzielles Lesen von Start-/End-Elementen, Attributen und Werten.
    /// </summary>
    /// <remarks>
    /// Das Interface abstrahiert typische Funktionen ähnlich eines XML- oder AST-Readers:
    /// - Navigation erfolgt ausschließlich vorwärts mittels <see cref="Read"/>.
    /// - Attribute eines aktuellen Start-Elements können über Index abgefragt werden.
    /// - Werte sind nur verfügbar, wenn <see cref="HasValue"/> wahr ist.
    /// - Elementgrenzen werden über <see cref="IsStartElement"/> und <see cref="IsEndElement"/> kenntlich.
    /// Implementierungen sollten nach Erreichen des Endes <see cref="EOF"/> dauerhaft true zurückgeben.
    /// </remarks>
    public interface IReader
    {
        /// <summary>
        /// Gibt an, ob das aktuelle Start-Element leer ist (d. h. kein separates End-Element folgt).
        /// </summary>
        /// <remarks>Typisch für Konstrukte wie &lt;element /&gt; in XML-ähnlichen Strukturen.</remarks>
        bool IsEmptyElement { get; }

        /// <summary>
        /// Gibt an, ob am aktuellen Readerpunkt ein Wert verfügbar ist.
        /// </summary>
        /// <remarks>
        /// Ist <c>true</c>, kann <see cref="getValue"/> aufgerufen werden, um den Inhalt zu lesen.
        /// Ein Wert kann z. B. Text- oder Literalinhalt sein.
        /// </remarks>
        bool HasValue { get; }

        /// <summary>
        /// Bestimmt, ob das Ende des Datenstroms erreicht wurde.
        /// </summary>
        /// <returns><c>true</c>, wenn keine weiteren Elemente gelesen werden können; andernfalls <c>false</c>.</returns>
        bool EOF();

        /// <summary>
        /// Liefert die Anzahl der Attribute des aktuellen Start-Elements.
        /// </summary>
        /// <returns>Anzahl der verfügbaren Attribute (0, wenn keine vorhanden).</returns>
        int GetAttributeCount();

        /// <summary>
        /// Liefert den lokalen Namen eines Attributes anhand seines Index.
        /// </summary>
        /// <param name="i">Der nullbasierte Attributindex.</param>
        /// <returns>Der Name des Attributs.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Wenn der Index außerhalb des gültigen Bereichs liegt.</exception>
        string GetAttributeName(int i);

        /// <summary>
        /// Liefert den Wert eines Attributes anhand seines Index.
        /// </summary>
        /// <param name="i">Der nullbasierte Attributindex.</param>
        /// <returns>Der Attributwert (typischerweise <see cref="string"/>), oder ein typ-spezifisches Objekt.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Wenn der Index außerhalb des gültigen Bereichs liegt.</exception>
        object GetAttributeValue(int i);

        /// <summary>
        /// Liefert den lokalen Namen des aktuellen Elements oder Knotens.
        /// </summary>
        /// <returns>Der lokale Elementname, oder <c>null</c>, falls nicht anwendbar.</returns>
        string GetLocalName();

        /// <summary>
        /// Liefert den Wert am aktuellen Positionierungspunkt.
        /// </summary>
        /// <remarks>
        /// Nur aufrufen, wenn <see cref="HasValue"/> wahr ist. Rückgabewert kann je nach Implementierung
        /// typisiert sein (z. B. <see cref="string"/>, numerische Literaltypen oder komplexe Objekte).
        /// </remarks>
        /// <returns>Der aktuelle Wert oder <c>null</c>, falls keiner vorhanden.</returns>
        object getValue();

        /// <summary>
        /// Gibt an, ob sich der Reader aktuell auf einem End-Element befindet.
        /// </summary>
        /// <returns><c>true</c>, wenn ein End-Element erkannt wurde; andernfalls <c>false</c>.</returns>
        bool IsEndElement();

        /// <summary>
        /// Gibt an, ob sich der Reader aktuell auf einem Start-Element befindet.
        /// </summary>
        /// <returns><c>true</c>, wenn ein Start-Element erkannt wurde; andernfalls <c>false</c>.</returns>
        bool IsStartElement();

        /// <summary>
        /// Liest den nächsten Knoten / das nächste Element aus der Eingabesequenz.
        /// </summary>
        /// <remarks>
        /// Bei erfolgreichem Lesen wird der interne Cursor weitergeschoben und die Eigenschaften
        /// spiegeln den neuen Zustand wider. Bei Ende des Stroms liefert der Aufruf <c>false</c>.
        /// </remarks>
        /// <returns><c>true</c>, wenn ein weiterer Eintrag gelesen wurde; <c>false</c> bei Ende.</returns>
        bool Read();
    }
}