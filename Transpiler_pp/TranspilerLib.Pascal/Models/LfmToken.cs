namespace TranspilerLib.Pascal.Models;

public class LfmToken
{
    public LfmTokenType Type { get; }
    public string Value { get; }
    public int Line { get; }
    public int Column { get; }

    public LfmToken(LfmTokenType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return $"{Type}: '{Value}' at {Line}:{Column}";
    }
}
