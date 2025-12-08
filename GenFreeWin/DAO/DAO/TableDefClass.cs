using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [TypeLibType(6)]
    [ClassInterface((short)0)]
    [DefaultMember("Fields")]
    [Guid("00000103-0000-0010-8000-00AA006D2EA4")]
    public class TableDefClass : _TableDef, TableDef
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
        public extern virtual int Attributes
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
        public extern virtual string Connect
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
        public extern virtual object DateCreated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809349)]
        public extern virtual object LastUpdated
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809349)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809350)]
        public extern virtual string Name
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
        public extern virtual string SourceTableName
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
        public extern virtual bool Updatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            get;
        }

        [DispId(1610809355)]
        public extern virtual string ValidationText
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
        public extern virtual string ValidationRule
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
        public extern virtual int RecordCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            get;
        }

        [DispId(0)]
        public extern virtual Fields Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809361)]
        public extern virtual Indexes Indexes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809361)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809367)]
        public extern virtual string ConflictTable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809368)]
        public extern virtual object ReplicaFilter
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
        public extern TableDefClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809362)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Recordset OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options);

        Recordset _TableDef.OpenRecordset([Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Options)
        {
            //ILSpy generated this explicit interface implementation from .override directive in OpenRecordset
            return this.OpenRecordset(Type, Options);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809363)]
        public extern virtual void RefreshLink();

        void _TableDef.RefreshLink()
        {
            //ILSpy generated this explicit interface implementation from .override directive in RefreshLink
            this.RefreshLink();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809364)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Field CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size);

        Field _TableDef.CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateField
            return this.CreateField(Name, Type, Size);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809365)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Index CreateIndex([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name);

        Index _TableDef.CreateIndex([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateIndex
            return this.CreateIndex(Name);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809366)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        Property _TableDef.CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateProperty
            return this.CreateProperty(Name, Type, Value, DDL);
        }
    }
}
