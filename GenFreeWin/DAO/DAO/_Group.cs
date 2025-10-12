using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000061-0000-0010-8000-00AA006D2EA4")]
    [DefaultMember("Users")]
    [TypeLibType(4288)]
    public interface _Group : _DAO
    {
        [DispId(10)]
        new Properties Properties
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809344)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809346)]
        string PID
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(0)]
        Users Users
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809348)]
        [return: MarshalAs(UnmanagedType.Interface)]
        User CreateUser([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Password);
    }
}
