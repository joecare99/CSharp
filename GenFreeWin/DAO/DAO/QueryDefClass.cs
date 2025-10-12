using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [DefaultMember("Parameters")]
    [Guid("00000108-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(6)]
    [ClassInterface((short)0)]
    public class QueryDefClass : _QueryDef, QueryDef
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
        public extern virtual object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809345)]
        public extern virtual object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809346)]
        public extern virtual string Name
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
        public extern virtual short ODBCTimeout
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
        public extern virtual short Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            get;
        }

        [DispId(1610809351)]
        public extern virtual string SQL
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
        public extern virtual bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809353)]
            get;
        }

        [DispId(1610809354)]
        public extern virtual string Connect
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
        public extern virtual bool ReturnsRecords
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
        public extern virtual int RecordsAffected
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809358)]
            get;
        }

        [DispId(1610809359)]
        public extern virtual Fields Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(0)]
        public extern virtual Parameters Parameters
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809374)]
        public extern virtual int hStmt
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(1610809374)]
            get;
        }

        [DispId(1610809375)]
        public extern virtual int MaxRecords
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
        public extern virtual bool StillExecuting
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809377)]
            get;
        }

        [DispId(1610809378)]
        public extern virtual int CacheSize
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
        public extern virtual object Prepare
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
        public extern QueryDefClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        public extern virtual void Close();

        void _QueryDef.Close()
        {
            //ILSpy generated this explicit interface implementation from .override directive in Close
            this.Close();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset _30_OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        Recordset _QueryDef._30_OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options)
        {
            //ILSpy generated this explicit interface implementation from .override directive in _30_OpenRecordset
            return this._30_OpenRecordset(Type, Options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809363)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset _30__OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        Recordset _QueryDef._30__OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options)
        {
            //ILSpy generated this explicit interface implementation from .override directive in _30__OpenRecordset
            return this._30__OpenRecordset(Type, Options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809364)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual QueryDef _Copy();

        QueryDef _QueryDef._Copy()
        {
            //ILSpy generated this explicit interface implementation from .override directive in _Copy
            return this._Copy();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        public extern virtual void Execute([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        void _QueryDef.Execute([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options)
        {
            //ILSpy generated this explicit interface implementation from .override directive in Execute
            this.Execute(Options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809366)]
        [TypeLibFunc(1)]
        public extern virtual void Compare([In][MarshalAs(UnmanagedType.Interface)] QueryDef pQdef, [In] ref short lps);

        void _QueryDef.Compare([In][MarshalAs(UnmanagedType.Interface)] QueryDef pQdef, [In] ref short lps)
        {
            //ILSpy generated this explicit interface implementation from .override directive in Compare
            this.Compare(pQdef, ref lps);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809367)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset CreateDynaset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Inconsistent);

        Recordset _QueryDef.CreateDynaset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Inconsistent)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateDynaset
            return this.CreateDynaset(Options, Inconsistent);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809368)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset CreateSnapshot([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        Recordset _QueryDef.CreateSnapshot([Optional][In][MarshalAs(UnmanagedType.Struct)] object Options)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateSnapshot
            return this.CreateSnapshot(Options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809369)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset ListParameters();

        Recordset _QueryDef.ListParameters()
        {
            //ILSpy generated this explicit interface implementation from .override directive in ListParameters
            return this.ListParameters();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809370)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        Property _QueryDef.CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateProperty
            return this.CreateProperty(Name, Type, Value, DDL);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809371)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);

        Recordset _QueryDef.OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit)
        {
            //ILSpy generated this explicit interface implementation from .override directive in OpenRecordset
            return this.OpenRecordset(Type, Options, LockEdit);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809372)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset _OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit);

        Recordset _QueryDef._OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options, [Optional][In][MarshalAs(UnmanagedType.Struct)] object LockEdit)
        {
            //ILSpy generated this explicit interface implementation from .override directive in _OpenRecordset
            return this._OpenRecordset(Type, Options, LockEdit);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809373)]
        public extern virtual void Cancel();

        void _QueryDef.Cancel()
        {
            //ILSpy generated this explicit interface implementation from .override directive in Cancel
            this.Cancel();
        }
    }
}
