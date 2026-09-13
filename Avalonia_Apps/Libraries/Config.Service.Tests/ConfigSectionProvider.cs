using System;

namespace Config.Service.Tests;

public class ConfigSectionProvider : IConfigSectionProvider
{
    public string Name { get; }
    public string DisplayName { get; }
    public string? Description { get; }
    public int Order { get; }
    public Type ModelType { get; }
    public object CreateModel() => new();

    public ConfigSectionProvider(string name, string displayName, int order)
    {
        Name = name;
        DisplayName = displayName;
        Order = order;
    }
}
