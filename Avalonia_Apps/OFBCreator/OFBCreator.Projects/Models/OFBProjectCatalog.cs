using System.Collections.Generic;

namespace OFBCreator.Projects.Models;

/// <summary>
/// Recent portable project-file paths for this user.
/// </summary>
public sealed class OFBProjectCatalog
{
    public int SchemaVersion { get; set; } = 1;

    public List<string> RecentProjects { get; set; } = [];
}
