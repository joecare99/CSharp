using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [DefaultMember("TableDefs")]
    [TypeLibType(4288)]
    [Guid("00000071-0000-0010-8000-00AA006D2EA4")]
    public interface Database : _DAO
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
        int CollatingOrder
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            get;
        }

        [DispId(1610809345)]
        string Connect
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

        [DispId(1610809346)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809347)]
        short QueryTimeout
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
        bool Transactions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            get;
        }

        [DispId(1610809350)]
        bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            get;
        }

        [DispId(1610809351)]
        string Version
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809352)]
        int RecordsAffected
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            get;
        }

        [DispId(0)]
        TableDefs TableDefs
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809354)]
        QueryDefs QueryDefs
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809355)]
        Relations Relations
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809355)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809356)]
        Containers Containers
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809356)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809357)]
        Recordsets Recordsets
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809376)]
        string ReplicaID
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809376)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809377)]
        string DesignMasterID
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809377)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809377)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809384)]
        Connection Connection
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809384)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809358)]
        void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809359)]
        void Execute([In][MarshalAs(UnmanagedType.BStr)] string Query, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809360)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset _30_OpenRecordset([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Relation CreateRelation([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Table, [Optional][In][MarshalAs(UnmanagedType.Struct)] object ForeignTable, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Attributes);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809363)]
        [return: MarshalAs(UnmanagedType.Interface)]
        TableDef CreateTableDef([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Attributes, [Optional][In][MarshalAs(UnmanagedType.Struct)] object SourceTableName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Connect);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809364)]
        [TypeLibFunc(1)]
        void BeginTrans();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        [TypeLibFunc(1)]
        void CommitTrans([In] int Options = 0);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809366)]
        void Rollback();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809367)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateDynaset([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Inconsistent);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809368)]
        [return: MarshalAs(UnmanagedType.Interface)]
        QueryDef CreateQueryDef([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object SQLText);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809369)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateSnapshot([In][MarshalAs(UnmanagedType.BStr)] string Source, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809370)]
        void DeleteQueryDef([In][MarshalAs(UnmanagedType.BStr)] string Name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809371)]
        [TypeLibFunc(1)]
        int ExecuteSQL([In][MarshalAs(UnmanagedType.BStr)] string SQL);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809372)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset ListFields([In][MarshalAs(UnmanagedType.BStr)] string Name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809373)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset ListTables();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809374)]
        [return: MarshalAs(UnmanagedType.Interface)]
        QueryDef OpenQueryDef([In][MarshalAs(UnmanagedType.BStr)] string Name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809375)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenTable([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809379)]
        void Synchronize([In][MarshalAs(UnmanagedType.BStr)] string DbPathName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object ExchangeType);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809380)]
        void MakeReplica([In][MarshalAs(UnmanagedType.BStr)] string PathName, [In][MarshalAs(UnmanagedType.BStr)] string Description, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809382)]
        void NewPassword([In][MarshalAs(UnmanagedType.BStr)] string bstrOld, [In][MarshalAs(UnmanagedType.BStr)] string bstrNew);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809383)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenRecordset([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809385)]
        void PopulatePartial([In][MarshalAs(UnmanagedType.BStr)] string DbPathName);
    }
}
