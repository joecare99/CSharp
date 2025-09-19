using System.Collections.Generic;

namespace NebelEbook;

/// <summary>
/// Repr�sentiert einen Knoten einer interaktiven Geschichte mit Titel, Abs�tzen und ausw�hlbaren Optionen.
/// </summary>
/// <remarks>
/// Die �bergebenen <see cref="string"/>-Arrays und <see cref="List{T}"/>-Instanzen werden ohne Kopie �bernommen.
/// </remarks>
public class StoryNode
{
    /// <summary>
    /// Eindeutige Kennung des Knotens.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Titel des Story-Knotens.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Sammlung der Textabs�tze in der vorgesehenen Anzeige-Reihenfolge.
    /// </summary>
    public string[] Paragraphs { get; }

    /// <summary>
    /// Liste der m�glichen Entscheidungen, die von diesem Knoten aus getroffen werden k�nnen.
    /// </summary>
    /// <seealso cref="Choice"/>
    public List<Choice> Choices { get; }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StoryNode"/>-Klasse.
    /// </summary>
    /// <param name="id">Die eindeutige Id des Knotens.</param>
    /// <param name="title">Der Titel des Knotens.</param>
    /// <param name="paragraphs">Die Abs�tze, die zu diesem Knoten geh�ren.</param>
    /// <param name="choices">Die verf�gbaren Entscheidungen ab diesem Knoten.</param>
    public StoryNode(string id, string title, string[] paragraphs, List<Choice> choices)
    {
        Id = id;
        Title = title;
        Paragraphs = paragraphs;
        Choices = choices;
    }
}
