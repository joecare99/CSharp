using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [TypeLibType(4288)]
    [DefaultMember("QueryDefs")]
    [Guid("00000041-0000-0010-8000-00AA006D2EA4")]
    public interface Connection
    {
        [DispId(1610743808)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743808)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610743809)]
        string Connect
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743809)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610743810)]
        Database Database
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743810)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610743811)]
        int hDbc
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743811)]
            [TypeLibFunc(64)]
            get;
        }

        [DispId(1610743812)]
        short QueryTimeout
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743812)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743812)]
            [param: In]
            set;
        }

        [DispId(1610743814)]
        bool Transactions
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743814)]
            get;
        }

        [DispId(1610743815)]
        int RecordsAffected
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743815)]
            get;
        }

        [DispId(1610743816)]
        bool StillExecuting
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743816)]
            get;
        }

        [DispId(1610743817)]
        bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743817)]
            get;
        }

        [DispId(0)]
        QueryDefs QueryDefs
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610743819)]
        Recordsets Recordsets
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743819)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743820)]
        void Cancel();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743821)]
        void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743822)]
        [return: MarshalAs(UnmanagedType.Interface)]
        QueryDef CreateQueryDef([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object SQLText);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743823)]
        void Execute([In][MarshalAs(UnmanagedType.BStr)] string Query, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743824)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Recordset OpenRecordset([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);
    }
}
