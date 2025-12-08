using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000031-0000-0010-8000-00AA006D2EA4")]
    [DefaultMember("Fields")]
    [TypeLibType(4288)]
    public interface Recordset : _DAO
    {
        [DispId(10)]
        new Properties Properties
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(101)]
        bool BOF
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(101)]
            get;
        }

        [DispId(102)]
        Array Bookmark
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(102)]
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(102)]
            [param: In]
            [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
            set;
        }

        [DispId(103)]
        bool Bookmarkable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(103)]
            get;
        }

        [DispId(104)]
        object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(104)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(105)]
        bool EOF
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(105)]
            get;
        }

        [DispId(106)]
        string Filter
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(106)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(106)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(107)]
        string Index
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(107)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(107)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(108)]
        Array LastModified
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(108)]
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
            get;
        }

        [DispId(109)]
        object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(109)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(110)]
        bool LockEdits
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(110)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(110)]
            [param: In]
            set;
        }

        [DispId(111)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(111)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(112)]
        bool NoMatch
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(112)]
            get;
        }

        [DispId(113)]
        string Sort
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(113)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(113)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(114)]
        bool Transactions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(114)]
            get;
        }

        [DispId(115)]
        short Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(115)]
            get;
        }

        [DispId(116)]
        int RecordCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(116)]
            get;
        }

        [DispId(117)]
        bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(117)]
            get;
        }

        [DispId(118)]
        bool Restartable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(118)]
            get;
        }

        [DispId(119)]
        string ValidationText
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(119)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(120)]
        string ValidationRule
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(120)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(121)]
        Array CacheStart
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(121)]
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(121)]
            [param: In]
            [param: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
            set;
        }

        [DispId(122)]
        int CacheSize
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(122)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(122)]
            [param: In]
            set;
        }

        [DispId(123)]
        float PercentPosition
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(123)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(123)]
            [param: In]
            set;
        }

        [DispId(124)]
        int AbsolutePosition
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(124)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(124)]
            [param: In]
            set;
        }

        [DispId(125)]
        short EditMode
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(125)]
            get;
        }

        [DispId(126)]
        int ODBCFetchCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(126)]
            [TypeLibFunc(64)]
            get;
        }

        [DispId(127)]
        int ODBCFetchDelay
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(127)]
            get;
        }

        [DispId(128)]
        Database Parent
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(128)]
            [return: MarshalAs(UnmanagedType.Interface)]
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

        [DispId(130)]
        Indexes Indexes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(130)]
            [TypeLibFunc(1)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(-8)]
        object Collect
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(-8)]
            [TypeLibFunc(64)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(-8)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [DispId(159)]
        int hStmt
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(159)]
            get;
        }

        [DispId(160)]
        bool StillExecuting
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(160)]
            get;
        }

        [DispId(161)]
        int BatchSize
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(161)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(161)]
            [param: In]
            set;
        }

        [DispId(162)]
        int BatchCollisionCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(162)]
            get;
        }

        [DispId(163)]
        object BatchCollisions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(163)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(164)]
        Connection Connection
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(164)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(164)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Interface)]
            set;
        }

        [DispId(165)]
        short RecordStatus
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(165)]
            get;
        }

        [DispId(166)]
        int UpdateOptions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(166)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(166)]
            [param: In]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(131)]
        void _30_CancelUpdate();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(132)]
        void AddNew();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(133)]
        void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(134)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(135)]
        void Delete();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(136)]
        void Edit();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(137)]
        void FindFirst([In][MarshalAs(UnmanagedType.BStr)] string Criteria);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(138)]
        void FindLast([In][MarshalAs(UnmanagedType.BStr)] string Criteria);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(139)]
        void FindNext([In][MarshalAs(UnmanagedType.BStr)] string Criteria);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(140)]
        void FindPrevious([In][MarshalAs(UnmanagedType.BStr)] string Criteria);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(141)]
        void MoveFirst();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(142)]
        void _30_MoveLast();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(143)]
        void MoveNext();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(144)]
        void MovePrevious();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(145)]
        void Seek([In][MarshalAs(UnmanagedType.BStr)] string Comparison, [In][MarshalAs(UnmanagedType.Struct)] object Key1, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key2, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key3, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key4, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key5, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key6, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key7, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key8, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key9, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key10, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key11, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key12, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Key13);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(146)]
        [TypeLibFunc(1)]
        void _30_Update();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(147)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset Clone();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(148)]
        void Requery([Optional][In][MarshalAs(UnmanagedType.Struct)] object NewQueryDef);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(149)]
        void Move([In] int Rows, [Optional][In][MarshalAs(UnmanagedType.Struct)] object StartBookmark);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(150)]
        void FillCache([Optional][In][MarshalAs(UnmanagedType.Struct)] object Rows, [Optional][In][MarshalAs(UnmanagedType.Struct)] object StartBookmark);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(151)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateDynaset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Inconsistent);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(152)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset CreateSnapshot([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(153)]
        [return: MarshalAs(UnmanagedType.Interface)]
        QueryDef CopyQueryDef();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(154)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset ListFields();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(155)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset ListIndexes();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(156)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetRows([Optional][In][MarshalAs(UnmanagedType.Struct)] object NumRows);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(157)]
        void Cancel();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(158)]
        bool NextRecordset();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(167)]
        void CancelUpdate([In] int UpdateType = 1);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(168)]
        void Update([In] int UpdateType = 1, [In] bool Force = false);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(169)]
        void MoveLast([In] int Options = 0);
    }
}
