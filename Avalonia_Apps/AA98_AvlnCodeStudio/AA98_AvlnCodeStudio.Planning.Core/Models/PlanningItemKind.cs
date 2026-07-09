namespace AA98_AvlnCodeStudio.Planning.Core.Models;

/// <summary>
/// Defines the normalized kinds of local planning items.
/// </summary>
public enum PlanningItemKind
{
    /// <summary>
    /// The planning item kind is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The planning item is a vision.
    /// </summary>
    Vision = 1,

    /// <summary>
    /// The planning item is a roadmap.
    /// </summary>
    Roadmap = 2,
    
    /// <summary>
    /// The planning item is an epic.
    /// </summary>
    Epic = 3,

    /// <summary>
    /// The planning item is a feature.
    /// </summary>
    Feature = 4,

    /// <summary>
    /// The planning item is a backlog item.
    /// </summary>
    BacklogItem = 5,

    /// <summary>
    /// The planning item is a task.
    /// </summary>
    Task = 6,

    /// <summary>
    /// The planning item is a document.
    /// </summary>
    Document = 7,

    /// <summary>
    /// The planning item is a bug.
    /// </summary>
    Bug = 8,

    /// <summary>
    /// The planning item is a test case.
    /// </summary>
    TestCase = 9,

    /// <summary>
    /// The planning item is an impediment.
    /// </summary>
    Impediment = 10,
}
