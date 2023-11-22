using System;
using System.Runtime.InteropServices;

namespace Helper
{
    public static class Externals
    {

        [DllImport("gdi32", CharSet = CharSet.Ansi, EntryPoint = "AddFontResourceA", ExactSpelling = true, SetLastError = true)]
        public static extern int AddFontresource([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpfilename);

        [Obsolete("use new DriveInfo")]
        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetDriveTypeA", ExactSpelling = true, SetLastError = true)]
        public static extern int GetDriveType([MarshalAs(UnmanagedType.VBByRefStr)] ref string nDrive);

        [DllImport("kernel32", CharSet = CharSet.Ansi, EntryPoint = "GetModuleFileNameA", ExactSpelling = true, SetLastError = true)]
        public static extern int GetModuleFileName(int hModule, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpfilename, int nSize);

        [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern short GetWindowWord(int hwnd, int nIndex);
    }
}