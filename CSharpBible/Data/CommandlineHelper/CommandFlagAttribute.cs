namespace CommandlineHelper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandFlagAttribute : Attribute
{
    public CommandFlagAttribute(string longName)
    {
        if (string.IsNullOrWhiteSpace(longName))
            throw new ArgumentException("Flag name must not be empty.", nameof(longName));

        LongName = longName;
    }

    public string LongName { get; }

    public string? ShortName { get; set; }

    public Type? ResourceType { get; set; }

    public string? DescriptionResourceName { get; set; }

    public bool? DefaultValue { get; set; }
}
