using System.Collections.Generic;

namespace NebelEbook;

/// <summary>
/// Repräsentiert einen Knoten einer interaktiven Geschichte mit Titel, Absätzen und auswählbaren Optionen.
/// </summary>
/// <remarks>
/// Die übergebenen <see cref="string"/>-Arrays und <see cref="List{T}"/>-Instanzen werden ohne Kopie übernommen.
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
    /// Sammlung der Textabsätze in der vorgesehenen Anzeige-Reihenfolge.
    /// </summary>
    public string[] Paragraphs { get; }

    /// <summary>
    /// Liste der möglichen Entscheidungen, die von diesem Knoten aus getroffen werden können.
    /// </summary>
    /// <seealso cref="Choice"/>
    public List<Choice> Choices { get; }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="StoryNode"/>-Klasse.
    /// </summary>
    /// <param name="id">Die eindeutige Id des Knotens.</param>
    /// <param name="title">Der Titel des Knotens.</param>
    /// <param name="paragraphs">Die Absätze, die zu diesem Knoten gehören.</param>
    /// <param name="choices">Die verfügbaren Entscheidungen ab diesem Knoten.</param>
    public StoryNode(string id, string title, string[] paragraphs, List<Choice> choices)
    {
        Id = id;
        Title = title;
        Paragraphs = paragraphs;
        Choices = choices;
    }
}
