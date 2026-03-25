using System.IO;

namespace RemoteTerminal.Net;

internal static class TelnetNegotiation
{
    // Telnet command bytes
    private const byte IAC = 255;
    private const byte DONT = 254;
    private const byte DO = 253;
    private const byte WONT = 252;
    private const byte WILL = 251;

    // Telnet options
    private const byte ECHO = 1;
    private const byte SUPPRESS_GO_AHEAD = 3;
    private const byte STATUS = 5;
    private const byte TERMINAL_TYPE = 24;
    private const byte WINDOW_SIZE = 31;
    private const byte TERMINAL_SPEED = 32;
    private const byte LINEMODE = 34;

    public static void SendBasicServerNegotiation(Stream stream)
    {
        // Goal:
        // - suppress go-ahead (character mode)
        // - disable line mode
        // - disable local echo (client should not echo)
        // Note: Telnet negotiation is best-effort; clients may ignore.

        WriteCmd(stream, WILL, SUPPRESS_GO_AHEAD);
        WriteCmd(stream, DO, SUPPRESS_GO_AHEAD);

        WriteCmd(stream, WONT, ECHO);
        WriteCmd(stream, DONT, ECHO);

        WriteCmd(stream, WONT, LINEMODE);
        WriteCmd(stream, DONT, LINEMODE);

        // Optional asks (ignored if unsupported)
        WriteCmd(stream, DO, TERMINAL_TYPE);
        WriteCmd(stream, DO, WINDOW_SIZE);
        WriteCmd(stream, DO, TERMINAL_SPEED);
        WriteCmd(stream, DO, STATUS);

        stream.Flush();
    }

    private static void WriteCmd(Stream stream, byte cmd, byte option)
    {
        stream.WriteByte(IAC);
        stream.WriteByte(cmd);
        stream.WriteByte(option);
    }
}
