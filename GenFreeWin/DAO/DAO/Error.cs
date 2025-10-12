using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000023-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(4288)]
    [DefaultMember("Description")]
    public interface Error
    {
        [DispId(1610743808)]
        int Number
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743808)]
            get;
        }

        [DispId(1610743809)]
        string Source
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743809)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(0)]
        string Description
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610743811)]
        string HelpFile
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(1610743811)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610743812)]
        int HelpContext
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(1610743812)]
            get;
        }
    }
}
