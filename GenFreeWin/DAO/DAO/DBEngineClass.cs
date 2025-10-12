using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [DefaultMember("Workspaces")]
    [TypeLibType(15)]
    [ClassInterface((short)0)]
    [ComImport]
    [Guid("00000100-0000-0010-8000-00AA006D2EA4")]
    public class DBEngineClass : _DBEngine, DBEngine
    {
        [DispId(10)]
        public extern virtual Properties Properties
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809344)]
        public extern virtual string Version
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809345)]
        public extern virtual string IniPath
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
        public extern virtual string DefaultUser
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809348)]
        public extern virtual string DefaultPassword
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809349)]
        public extern virtual short LoginTimeout
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
        public extern virtual Workspaces Workspaces
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809352)]
        public extern virtual Errors Errors
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809367)]
        public extern virtual string SystemDB
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809371)]
        public extern virtual int DefaultType
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809371)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809371)]
            [param: In]
            set;
        }

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern DBEngineClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809353)]
        public extern virtual void Idle([Optional][In][MarshalAs(UnmanagedType.Struct)] object Action);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809354)]
        public extern virtual void CompactDatabase([In][MarshalAs(UnmanagedType.BStr)] string SrcName, [In][MarshalAs(UnmanagedType.BStr)] string DstName, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DstLocale, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object SrcLocale);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809355)]
        [TypeLibFunc(64)]
        public extern virtual void RepairDatabase([In][MarshalAs(UnmanagedType.BStr)] string Name);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809356)]
        public extern virtual void RegisterDatabase([In][MarshalAs(UnmanagedType.BStr)] string Dsn, [In][MarshalAs(UnmanagedType.BStr)] string Driver, [In] bool Silent, [In][MarshalAs(UnmanagedType.BStr)] string Attributes);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809357)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Workspace _30_CreateWorkspace([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string UserName, [In][MarshalAs(UnmanagedType.BStr)] string Password);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809358)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Database OpenDatabase(
            [In][MarshalAs(UnmanagedType.BStr)] string Name, 
            [In][MarshalAs(UnmanagedType.Struct)] object Options=null, 
            [In][MarshalAs(UnmanagedType.Struct)] object ReadOnly=null, 
            [In][MarshalAs(UnmanagedType.Struct)] object Connect=null);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809359)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Database CreateDatabase(
            [In][MarshalAs(UnmanagedType.BStr)] string Name, 
            [In][MarshalAs(UnmanagedType.BStr)] string Locale, 
            [Optional][In][MarshalAs(UnmanagedType.Struct)] object Option);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809360)]
        public extern virtual void FreeLocks();


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        public extern virtual void BeginTrans();


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809362)]
        public extern virtual void CommitTrans([In] int Option = 0);


        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809363)]
        public extern virtual void Rollback();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809364)]
        public extern virtual void SetDefaultWorkspace([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string Password);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        [TypeLibFunc(1)]
        public extern virtual void SetDataAccessOption([In] short Option, [In][MarshalAs(UnmanagedType.Struct)] object Value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809366)]
        [TypeLibFunc(64)]
        public extern virtual int ISAMStats([In] int StatNum, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Reset);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809369)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Workspace CreateWorkspace([In][MarshalAs(UnmanagedType.BStr)] string Name, [In][MarshalAs(UnmanagedType.BStr)] string UserName, [In][MarshalAs(UnmanagedType.BStr)] string Password, [Optional][In][MarshalAs(UnmanagedType.Struct)] object UseType);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809370)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Connection OpenConnection([In][MarshalAs(UnmanagedType.BStr)] string Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object ReadOnly, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Connect);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809373)]
        public extern virtual void SetOption([In] int Option, [In][MarshalAs(UnmanagedType.Struct)] object Value);

    }
}
