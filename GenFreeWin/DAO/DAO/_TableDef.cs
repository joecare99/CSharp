using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [DefaultMember("Fields")]
    [TypeLibType(4288)]
    [Guid("00000049-0000-0010-8000-00AA006D2EA4")]
    public interface _TableDef : _DAO
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
        int Attributes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [param: In]
            set;
        }

        [DispId(1610809346)]
        string Connect
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809348)]
        object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809349)]
        object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809350)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809352)]
        string SourceTableName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809354)]
        bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            get;
        }

        [DispId(1610809355)]
        string ValidationText
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809355)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809355)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809357)]
        string ValidationRule
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809359)]
        int RecordCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            get;
        }

        [DispId(0)]
        Fields Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809361)]
        Indexes Indexes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809361)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809367)]
        string ConflictTable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809368)]
        object ReplicaFilter
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809368)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809368)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809363)]
        void RefreshLink();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809364)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Field CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Index CreateIndex([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809366)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);
    }
}
