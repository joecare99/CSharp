using System;

namespace AhnWin52Backup.Core.Hej;

public sealed class HejFormatException : FormatException
{
    public HejFormatException(string message, int lineNumber, HejSection? section = null)
        : base($"Line {lineNumber}{(section is null ? string.Empty : $" ({section})")}: {message}")
    {
        LineNumber = lineNumber;
        Section = section;
    }

    public int LineNumber { get; }

    public HejSection? Section { get; }
}
