using System;
using System.Collections;
using System.Collections.Generic;
using TranspilerLib.Interfaces;

namespace TranspilerLib.Models;

/// <summary>
/// Stellt eine Liste von Elementen bereit, die jeweils einen Verweis auf ihr übergeordnetes Objekt (Parent) besitzen.
/// Beim Einfügen oder Hinzufügen eines Elements wird dessen <see cref="IHasParents{T}.Parent"/> automatisch auf den Parent dieser Liste gesetzt,
/// sofern es noch keinen Parent hat oder ein anderer Parent hinterlegt ist.
/// </summary>
/// <typeparam name="T">
/// Der Elementtyp. Muss eine Klassen-Referenz sein, <see cref="IHasParents{T}"/> implementieren und vergleichbar über <see cref="IEquatable{T}"/> sein.
/// </typeparam>
/// <remarks>
/// Typisches Einsatzszenario sind hierarchische oder baumartige Strukturen (z. B. AST-Knoten),
/// bei denen ein konsistenter Parent-Verweis gepflegt werden soll.
/// </remarks>
public class ParentedItemsList<T> : IHasParents<T>, IList<T> where T : class, IHasParents<T>, IEquatable<T>
{
    /// <summary>
    /// Interne Speicherliste der Elemente.
    /// </summary>
    private readonly List<T> list = new();

    /// <summary>
    /// Der Parent, der allen eingefügten Elementen zugewiesen wird (sofern erforderlich).
    /// </summary>
    /// <remarks>
    /// Ist dieser Wert <c>null</c>, werden neue Elemente nicht verändert, selbst wenn sie keinen Parent besitzen.
    /// </remarks>
    public T Parent { get; set; }

    /// <summary>
    /// Gibt die Anzahl der enthaltenen Elemente zurück.
    /// </summary>
    public int Count => list.Count;

    /// <summary>
    /// Immer <c>false</c>, da diese Liste veränderbar ist.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Ruft das Element am angegebenen Index ab oder legt es fest.
    /// </summary>
    /// <param name="index">Der nullbasierte Index.</param>
    /// <returns>Das Element am angegebenen Index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Wenn <paramref name="index"/> ungültig ist.</exception>
    public T this[int index] { get => list[index]; set => list[index] = value; }

    /// <summary>
    /// Erstellt eine neue Instanz der <see cref="ParentedItemsList{T}"/> mit dem angegebenen Parent.
    /// </summary>
    /// <param name="_parent">Der Parent, der neuen Elementen zugewiesen wird (falls nötig).</param>
    public ParentedItemsList(T _parent)
    {
        Parent = _parent;
    }

    /// <summary>
    /// Ermittelt den Index eines bestimmten Elements.
    /// </summary>
    /// <param name="item">Das zu suchende Element.</param>
    /// <returns>Der Index oder -1, falls nicht gefunden.</returns>
    public int IndexOf(T item) => list.IndexOf(item);

    /// <summary>
    /// Fügt ein Element an einer bestimmten Position ein und setzt dessen Parent falls erforderlich.
    /// </summary>
    /// <param name="index">Der nullbasierte Insert-Index.</param>
    /// <param name="item">Das einzufügende Element.</param>
    /// <exception cref="ArgumentOutOfRangeException">Wenn <paramref name="index"/> ungültig ist.</exception>
    public void Insert(int index, T item)
    {
        if (!item.Parent?.Equals(Parent) ?? Parent != null)
            item.Parent = Parent;
        list.Insert(index, item);
    }

    /// <summary>
    /// Entfernt das Element am angegebenen Index.
    /// </summary>
    /// <param name="index">Der nullbasierte Index.</param>
    /// <exception cref="ArgumentOutOfRangeException">Wenn <paramref name="index"/> ungültig ist.</exception>
    public void RemoveAt(int index) => list.RemoveAt(index);

    /// <summary>
    /// Fügt ein Element am Ende ein und setzt dessen Parent falls erforderlich.
    /// </summary>
    /// <param name="item">Das hinzuzufügende Element.</param>
    public void Add(T item)
    {
        if (!item.Parent?.Equals(Parent) ?? Parent != null)
            item.Parent = Parent;
        list.Add(item);
    }

    /// <summary>
    /// Entfernt alle Elemente aus der Liste.
    /// </summary>
    public void Clear() => list.Clear();

    /// <summary>
    /// Prüft, ob ein Element enthalten ist.
    /// </summary>
    /// <param name="item">Das zu prüfende Element.</param>
    /// <returns><c>true</c>, wenn enthalten; andernfalls <c>false</c>.</returns>
    public bool Contains(T item) => list.Contains(item);

    /// <summary>
    /// Kopiert die Elemente in ein Zielarray ab einem Startindex.
    /// </summary>
    /// <param name="array">Das Zielarray.</param>
    /// <param name="arrayIndex">Startindex im Zielarray.</param>
    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    /// <summary>
    /// Entfernt ein bestimmtes Element, sofern vorhanden.
    /// </summary>
    /// <param name="item">Das zu entfernende Element.</param>
    /// <returns><c>true</c>, wenn entfernt; andernfalls <c>false</c>.</returns>
    public bool Remove(T item) => list.Remove(item);

    /// <summary>
    /// Gibt einen Enumerator über die Elemente zurück.
    /// </summary>
    /// <returns>Enumerator über die Liste.</returns>
    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    /// <summary>
    /// Nicht-generischer Enumerator.
    /// </summary>
    /// <returns>Enumerator über die Liste.</returns>
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
}


