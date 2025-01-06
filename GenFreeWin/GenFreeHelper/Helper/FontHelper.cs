using System;
using System.Drawing;

namespace GenFree.Helper
{
    public static class FontHelper
    {

        public static Font ChangeFUnderline(this Font font, bool Underline)
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsWindows())
#endif
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                return new Font(font.Name, font.Size, font.Style & ~FontStyle.Underline | (Underline ? FontStyle.Underline : FontStyle.Regular));
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#if NET6_0_OR_GREATER
            else
                throw new PlatformNotSupportedException("FontHelper.ChangeFUnderline");
#endif
        }

        public static Font ChangeFBold(this Font font, bool Bold)
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsWindows())
#endif
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                return new Font(font, font.Style & ~FontStyle.Bold | (Bold ? FontStyle.Bold : FontStyle.Regular));
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#if NET6_0_OR_GREATER
            else
                throw new PlatformNotSupportedException("FontHelper.ChangeFBold");
#endif
        }

        public static Font ChangeFSize(this Font font, float fSize)
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsWindows())
#endif
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                return new Font(font.FontFamily, fSize, font.Style);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#if NET6_0_OR_GREATER
            else
                throw new PlatformNotSupportedException("FontHelper.ChangeFSize");
#endif
        }

        public static Font ChangeFName(this Font font, string sFontname)
        {
#if NET6_0_OR_GREATER
            if (OperatingSystem.IsWindows())
#endif
#pragma warning disable CA1416 // Plattformkompatibilität überprüfen
                return new Font(sFontname, font.Size, font.Style);
#pragma warning restore CA1416 // Plattformkompatibilität überprüfen
#if NET6_0_OR_GREATER
            else
                throw new PlatformNotSupportedException("FontHelper.ChangeFName");
#endif
        }
    }
}
