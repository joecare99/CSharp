namespace NebelEbook;

/// <summary>
/// Repräsentiert eine auswählbare Option innerhalb des E-Books,
/// die zu einem Zielabschnitt mit der angegebenen Kennung verweist.
/// </summary>
public class Choice
{
    /// <summary>
    /// Der anzuzeigende Text der Auswahl (z. B. Button- oder Link-Beschriftung).
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Die Zielkennung (ID), auf die diese Auswahl verweist
    /// (z. B. Kapitel-/Abschnitts- oder Knoten-ID).
    /// </summary>
    public string TargetId { get; }

    /// <summary>
    /// Erstellt eine neue Auswahl mit Anzeigetext und Zielkennung.
    /// </summary>
    /// <param name="label">Der anzuzeigende Text der Auswahl.</param>
    /// <param name="targetId">Die Kennung des Zielabschnitts, zu dem navigiert werden soll.</param>
    public Choice(string label, string targetId)
    {
        Label = label;
        TargetId = targetId;
    }
}

