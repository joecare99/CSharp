namespace TranspilerLib.Interfaces
{
    /// <summary>
    /// Definiert ein Objekt, das einen Verweis auf sein übergeordnetes Objekt (Parent) besitzt.
    /// </summary>
    /// <typeparam name="T">Der Typ des übergeordneten Objekts. Muss eine Referenztyp-Klasse sein.</typeparam>
    /// <remarks>
    /// Diese Schnittstelle unterstützt hierarchische Strukturen (z. B. AST-Knoten),
    /// indem jedes Element optional auf sein Parent-Element verweist. Der Wert kann
    /// <c>null</c> sein, wenn das Objekt an der Wurzel der Hierarchie steht.
    /// </remarks>
    public interface IHasParents<T> where T : class
    {
        /// <summary>
        /// Ruft das übergeordnete Objekt ab oder legt es fest.
        /// </summary>
        /// <value>
        /// Das Parent-Objekt vom Typ <typeparamref name="T"/> oder <c>null</c>, wenn kein Parent vorhanden ist.
        /// </value>
        T? Parent { get; set; }
    }
}