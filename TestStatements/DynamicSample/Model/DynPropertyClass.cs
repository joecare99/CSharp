// ***********************************************************************
// Assembly         : DynamicSample
// Author           : Mir
// Created          : 07-08-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="DynPropertyClass.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Dynamic;

/// <summary>
/// The Model namespace.
/// </summary>
namespace DynamicSample.Model
{
    /// <summary>
    /// Enum pFields
    /// </summary>
    public enum pFields
    {
        /// <summary>
        /// The name
        /// </summary>
        Name,
        /// <summary>
        /// The street
        /// </summary>
        Street,
        /// <summary>
        /// The city
        /// </summary>
        City,
        /// <summary>
        /// The PLZ
        /// </summary>
        PLZ,
        /// <summary>
        /// The country
        /// </summary>
        Country
    }

    /// <summary>
    /// Class DynPropertyClass.
    /// Implements the <see cref="DynamicObject" />
    /// </summary>
    /// <seealso cref="DynamicObject" />
    public class DynPropertyClass : DynamicObject
    {
        /// <summary>
        /// The values
        /// </summary>
        /// 
        private Dictionary<pFields, string> _values = new Dictionary<pFields, string>();
        /// <summary>
        /// The index
        /// </summary>
        private static Dictionary<string, pFields> _index = new Dictionary<string, pFields>();

        /// <summary>
        /// Stellt die Implementierung für Vorgänge, die Memberwerte abrufen.
        /// Abgeleitete Klassen die <see cref="T:System.Dynamic.DynamicObject" /> Klasse kann überschreiben diese Methode, um dynamisches Verhalten für Vorgänge wie das Abrufen eines Werts für eine Eigenschaft anzugeben.
        /// </summary>
        /// <param name="binder">Enthält Informationen über das Objekt, das den dynamischen Vorgang aufgerufen hat.
        /// Die binder.Name -Eigenschaft gibt den Namen des Elements für den dynamische Vorgang ausgeführt wird.
        /// Z. B. für die Console.WriteLine(sampleObject.SampleProperty) -Anweisung, in denen sampleObject ist eine Instanz der abgeleiteten Klasse von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse, binder.Name "SampleProperty" zurückgegeben.
        /// Die binder.IgnoreCase -Eigenschaft gibt an, ob der Membername Groß-/Kleinschreibung beachtet wird.</param>
        /// <param name="result">Das Ergebnis des Get-Vorgangs.
        /// Z. B. wenn die Methode für eine Eigenschaft aufgerufen wird, können Sie zuweisen den Eigenschaftswert <paramref name="result" />.</param>
        /// <returns><see langword="true" />, wenn der Vorgang erfolgreich ist, andernfalls <see langword="false" />.
        /// Wenn diese Methode gibt <see langword="false" />, die Laufzeitbinder der Sprache bestimmt das Verhalten.
        /// (In den meisten Fällen wird eine Laufzeitausnahme ausgelöst.)</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.StartsWith("str") && _index.TryGetValue(binder.Name.Substring(3),out var _Idx))
                result = _values[_Idx];
            else result = null;
            return result != null;
        }

        /// <summary>
        /// Gibt die Enumeration aller dynamischen Membernamen zurück.
        /// </summary>
        /// <returns>Eine Sequenz, die dynamische Membernamen enthält.</returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var name in _index.Keys)
                yield return $"str{name}";
            yield break;
        }

        /// <summary>
        /// Stellt die Implementierung für Vorgänge, die Memberwerte festlegen.
        /// Abgeleitete Klassen die <see cref="T:System.Dynamic.DynamicObject" /> Klasse kann überschreiben diese Methode, um dynamisches Verhalten für Vorgänge wie das Festlegen eines Werts für eine Eigenschaft anzugeben.
        /// </summary>
        /// <param name="binder">Enthält Informationen über das Objekt, das den dynamischen Vorgang aufgerufen hat.
        /// Die binder.Name -Eigenschaft gibt den Namen des Members, der der Wert zugewiesen wird.
        /// Beispielsweise für die Anweisung sampleObject.SampleProperty = "Test", wobei sampleObject ist eine Instanz der abgeleiteten Klasse von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse, binder.Name "SampleProperty" zurückgegeben.
        /// Die binder.IgnoreCase -Eigenschaft gibt an, ob der Membername Groß-/Kleinschreibung beachtet wird.</param>
        /// <param name="value">Der Wert für das Element fest.
        /// Z. B. für sampleObject.SampleProperty = "Test", wobei sampleObject ist eine Instanz der abgeleiteten Klasse von der <see cref="T:System.Dynamic.DynamicObject" /> -Klasse, die <paramref name="value" /> "Test" ist.</param>
        /// <returns><see langword="true" />, wenn der Vorgang erfolgreich ist, andernfalls <see langword="false" />.
        /// Wenn diese Methode gibt <see langword="false" />, die Laufzeitbinder der Sprache bestimmt das Verhalten.
        /// (In den meisten Fällen wird eine sprachspezifische Laufzeitausnahme ausgelöst.)</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            bool result=false;
            if (binder.Name.StartsWith("str") && _index.TryGetValue(binder.Name.Substring(3), out var _Idx))
            {
                _values[_Idx] = value as string;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String" /> with the specified field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>System.String.</returns>
        public string this[pFields field] { get => _values[field]; set => _values[field] = value; }
        /// <summary>
        /// Gets the full address.
        /// </summary>
        /// <value>The full address.</value>
        public string FullAddress => $"{this[pFields.Name]}\r\n{this[pFields.Street]}\r\n{this[pFields.PLZ]} {this[pFields.City]}\r\n{this[pFields.Country]}";

        /// <summary>
        /// Initializes a new instance of the <see cref="DynPropertyClass" /> class.
        /// </summary>
        public DynPropertyClass()
        {
            for(var i = (pFields)0;i<=pFields.Country; i++ )
            {
                _index[typeof(pFields).GetEnumName(i)] = i;
            }
        }
    }
}
