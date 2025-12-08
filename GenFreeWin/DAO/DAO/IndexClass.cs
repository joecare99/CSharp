using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [Guid("00000105-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(6)]
    [ClassInterface((short)0)]
    public class IndexClass : _Index, Index
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
        public extern virtual string Name
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
        public extern virtual bool Foreign
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            get;
        }

        [DispId(1610809347)]
        public extern virtual bool Unique
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
        public extern virtual bool Clustered
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
        public extern virtual bool Required
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
        public extern virtual bool IgnoreNulls
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
        public extern virtual bool Primary
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
        public extern virtual int DistinctCount
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            get;
        }

        [DispId(1610809358)]
        public extern virtual object Fields
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

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern IndexClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809360)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Field CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size);

        Field _Index.CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateField
            return this.CreateField(Name, Type, Size);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809361)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        Property _Index.CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateProperty
            return this.CreateProperty(Name, Type, Value, DDL);
        }
    }
}
