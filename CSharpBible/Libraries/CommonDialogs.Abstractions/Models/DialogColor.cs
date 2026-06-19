namespace CommonDialogs.Models;

public struct DialogColor
{
    public DialogColor(uint argb, string? name = null)
    {
        Argb = argb;
        Name = name;
    }

    public uint Argb { get; set; }

    public string? Name { get; set; }

    public byte A => (byte)(Argb >> 24);

    public byte R => (byte)(Argb >> 16);

    public byte G => (byte)(Argb >> 8);

    public byte B => (byte)Argb;

    public static DialogColor FromArgb(byte alpha, byte red, byte green, byte blue, string? name = null)
        => new(((uint)alpha << 24) | ((uint)red << 16) | ((uint)green << 8) | blue, name);

    public override string ToString()
        => !string.IsNullOrWhiteSpace(Name)
            ? $"{Name} (#{Argb:X8})"
            : $"#{Argb:X8}";
}
