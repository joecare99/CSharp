namespace Property.Editor;

/// <summary>Identifies the ownership scope of a property value.</summary>
public enum PropertyScope
{
    /// <summary>No source scope has been declared.</summary>
    Unspecified,
    /// <summary>Application-wide packaged or built-in value.</summary>
    Application,
    /// <summary>Value belonging to the current user.</summary>
    User,
    /// <summary>Value supplied by the user's configured group.</summary>
    Group,
    /// <summary>Machine-wide value or policy.</summary>
    System,
    /// <summary>Value persisted in an individual project.</summary>
    Project,
    /// <summary>Value persisted in a solution or workspace.</summary>
    Solution,
    /// <summary>Value that exists only for the current process or editing session.</summary>
    Session,
}
