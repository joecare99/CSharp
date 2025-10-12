using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000059-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(4288)]
    public interface _Index : _DAO
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
        bool Foreign
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            get;
        }

        [DispId(1610809347)]
        bool Unique
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [param: In]
            set;
        }

        [DispId(1610809349)]
        bool Clustered
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            [param: In]
            set;
        }

        [DispId(1610809351)]
        bool Required
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [param: In]
            set;
        }

        [DispId(1610809353)]
        bool IgnoreNulls
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            [param: In]
            set;
        }

        [DispId(1610809355)]
        bool Primary
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809355)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809355)]
            [param: In]
            set;
        }

        [DispId(1610809357)]
        int DistinctCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            get;
        }

        [DispId(1610809358)]
        object Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809358)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809358)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809360)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Field CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);
    }
}
