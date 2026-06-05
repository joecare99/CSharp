namespace CommandlineHelper;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CommandDescriptorAttribute : Attribute
{
    public CommandDescriptorAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Command name must not be empty.", nameof(name));

        Name = name;
    }

    public string Name { get; }

    public Type? ResourceType { get; set; }

    public string? DescriptionResourceName { get; set; }

    public string? HelpTextResourceName { get; set; }
}
