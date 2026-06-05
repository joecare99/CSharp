namespace CommandlineHelper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandArgumentAttribute : Attribute
{
    public CommandArgumentAttribute(int position)
    {
        Position = position;
    }

    public int Position { get; }

    public Type? ResourceType { get; set; }

    public string? DescriptionResourceName { get; set; }

    public bool? Required { get; set; }

    public object? DefaultValue { get; set; }
}
