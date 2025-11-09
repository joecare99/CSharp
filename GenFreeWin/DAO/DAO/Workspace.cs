using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [Guid("00000039-0000-0010-8000-00AA006D2EA4")]
    [DefaultMember("Databases")]
    [TypeLibType(4288)]
    public interface Workspace : _DAO
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
        string UserName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809347)]
        string _30_UserName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(1)]
            [DispId(1610809347)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809348)]
        string _30_Password
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(1)]
            [DispId(1610809348)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809349)]
        short IsolateODBCTrans
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            [param: In]
            set;
        }

        [DispId(0)]
        Databases Databases
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809352)]
        Users Users
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809353)]
        Groups Groups
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809363)]
        int LoginTimeout
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809363)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809363)]
            [param: In]
            set;
        }

        [DispId(1610809365)]
        int DefaultCursorDriver
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809365)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809365)]
            [param: In]
            set;
        }

        [DispId(1610809367)]
        int hEnv
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(1610809367)]
            get;
        }

        [DispId(1610809368)]
        int Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809368)]
            get;
        }

        [DispId(1610809369)]
        Connections Connections
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809369)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809354)]
        void BeginTrans();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809355)]
        void CommitTrans([In] int Options = 0);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809356)]
        void Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809357)]
        void Rollback();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809358)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Database OpenDatabase([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object ReadOnly, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Connect);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809359)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Database CreateDatabase([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string Connect, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Option);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809360)]
        [return: MarshalAs(UnmanagedType.Interface)]
        User CreateUser([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Password);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Group CreateGroup([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Connection OpenConnection([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object ReadOnly, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Connect);
    }
}
