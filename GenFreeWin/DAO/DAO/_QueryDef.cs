using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [DefaultMember("Parameters")]
    [Guid("00000079-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(4288)]
    public interface _QueryDef : _DAO
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
        object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809345)]
        object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809346)]
        string Name
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
        short ODBCTimeout
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [param: In]
            set;
        }

        [DispId(1610809350)]
        short Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            get;
        }

        [DispId(1610809351)]
        string SQL
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809353)]
        bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            get;
        }

        [DispId(1610809354)]
        string Connect
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809356)]
        bool ReturnsRecords
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809356)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809356)]
            [param: In]
            set;
        }

        [DispId(1610809358)]
        int RecordsAffected
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809358)]
            get;
        }

        [DispId(1610809359)]
        Fields Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(0)]
        Parameters Parameters
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809374)]
        int hStmt
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809374)]
            [TypeLibFunc(64)]
            get;
        }

        [DispId(1610809375)]
        int MaxRecords
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809375)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809375)]
            [param: In]
            set;
        }

        [DispId(1610809377)]
        bool StillExecuting
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809377)]
            get;
        }

        [DispId(1610809378)]
        int CacheSize
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809378)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809378)]
            [param: In]
            set;
        }

        [DispId(1610809380)]
        object Prepare
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809380)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809380)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset _30_OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809363)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset _30__OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809364)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        QueryDef _Copy();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        void Execute([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809366)]
        [TypeLibFunc(1)]
        void Compare([In][MarshalAs(UnmanagedType.Interface)] QueryDef pQdef, [In] ref short lps);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809367)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateDynaset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Inconsistent);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809368)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateSnapshot([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809369)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset ListParameters();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809370)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809371)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809372)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset _OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809373)]
        void Cancel();
    }
}
