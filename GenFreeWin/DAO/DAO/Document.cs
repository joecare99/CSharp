using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [TypeLibType(4288)]
    [Guid("00000099-0000-0010-8000-00AA006D2EA4")]
    public interface Document : _DAO
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
        }

        [DispId(1610809345)]
        string Owner
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809347)]
        string Container
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809348)]
        string UserName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809350)]
        int Permissions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            [param: In]
            set;
        }

        [DispId(1610809352)]
        object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809353)]
        object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809354)]
        int AllPermissions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809355)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);
    }
}
