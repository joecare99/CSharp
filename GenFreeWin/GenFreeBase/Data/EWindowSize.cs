using System;

namespace GenFree.Data;

public enum EWindowSize : short
{
    ws800x600 = 0,
    ws900x670 = 1,
    ws900x710 = 2,
    ws1024x710 = 3,
    ws1024x768 = 4,
    ws1150x800 = 5,
    ws1150x835 = 6,
    ws1280x920 = 7,
    ws1400x1050 = 8,
    ws1600x1200 = 9
}

public static class WindowSizeHelper
{
    public static (int Width, int Height) GetWindowSize(this EWindowSize size)
    {
        return size switch
        {
            EWindowSize.ws800x600 => (800, 600),
            EWindowSize.ws900x670 => (900, 670),
            EWindowSize.ws900x710 => (900, 710),
            EWindowSize.ws1024x710 => (1024, 710),
            EWindowSize.ws1024x768 => (1024, 768),
            EWindowSize.ws1150x800 => (1150, 800),
            EWindowSize.ws1150x835 => (1150, 835),
            EWindowSize.ws1280x920 => (1280, 920),
            EWindowSize.ws1400x1050 => (1400, 1050),
            EWindowSize.ws1600x1200 => (1600, 1200),
            _ => throw new NotImplementedException(),
        };
    }
    public static EWindowSize GetWindowSize(this int size)
    {
        return size switch
        {
            < 670 => EWindowSize.ws800x600,
            < 700 => EWindowSize.ws900x670,
            < 710 => EWindowSize.ws900x710,
            < 768 => EWindowSize.ws1024x710,
            < 800 => EWindowSize.ws1024x768,
            < 835 => EWindowSize.ws1150x800,
            < 920 => EWindowSize.ws1150x835,
            < 1050 => EWindowSize.ws1280x920,
            < 1200 => EWindowSize.ws1400x1050,
            _ => EWindowSize.ws1600x1200,
        };
    }

}
