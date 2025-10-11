using System.Xml;
using TranspilerLib.Interfaces;

namespace TranspilerLib.Models
{
    /// <summary>
    /// Wrapper um <see cref="XmlReader"/> zur Vereinheitlichung des Zugriffs in der Transpiler-Pipeline.
    /// Dient als Abstraktion, damit Parser / Scanner unabhängig vom konkreten Reader getestet werden können.
    /// </summary>
    public class IECReader : IReader
    {
        /// <summary>
        /// Der zugrunde liegende <see cref="XmlReader"/>.
        /// </summary>
        private readonly XmlReader _xr;

        /// <summary>
        /// Erstellt eine neue Instanz von <see cref="IECReader"/>.
        /// </summary>
        /// <param name="xr">Bereits geöffneter <see cref="XmlReader"/> (Besitz verbleibt beim Aufrufer).</param>
        public IECReader(XmlReader xr)
        {
            _xr = xr;
        }

        /// <summary>
        /// Liest den nächsten XML-Knoten.
        /// </summary>
        /// <returns><c>true</c>, falls ein weiterer Knoten gelesen werden konnte; andernfalls <c>false</c>.</returns>
        public bool Read() => _xr.Read();

        /// <summary>
        /// Prüft ob das Dateiende erreicht wurde.
        /// </summary>
        public bool EOF() => _xr.EOF;

        /// <summary>
        /// Prüft ob sich der Reader aktuell auf einem Start-Element befindet.
        /// </summary>
        public bool IsStartElement() => _xr.IsStartElement();

        /// <summary>
        /// Prüft ob sich der Reader aktuell auf einem End-Element befindet.
        /// </summary>
        public bool IsEndElement() => _xr.NodeType == XmlNodeType.EndElement;

        /// <summary>
        /// Liefert die Anzahl der Attribute des aktuellen Elements.
        /// </summary>
        public int GetAttributeCount() => _xr.AttributeCount;

        /// <summary>
        /// Liefert den Attributwert an Position <paramref name="i"/>.
        /// </summary>
        /// <param name="i">Nullbasierter Attributindex.</param>
        /// <returns>Wert des Attributs oder <c>null</c>.</returns>
        public object? GetAttributeValue(int i) => _xr.GetAttribute(i);

        /// <summary>
        /// Liefert den Attributnamen an Position <paramref name="i"/>.
        /// </summary>
        /// <param name="i">Nullbasierter Attributindex.</param>
        public string GetAttributeName(int i)
        {
            _xr.MoveToAttribute(i);
            return _xr.Name;
        }

        /// <summary>
        /// Liefert den lokalen Namen des aktuellen Knotens.
        /// </summary>
        public string GetLocalName() => _xr.LocalName;

        /// <summary>
        /// Liefert den aktuellen Text/Attributwert.
        /// </summary>
        public object getValue() => _xr.Value;

        /// <inheritdoc/>
        public bool HasValue => _xr.HasValue;

        /// <inheritdoc/>
        public bool IsEmptyElement => _xr.IsEmptyElement;
    }
}
