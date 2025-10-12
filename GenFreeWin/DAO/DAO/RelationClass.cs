using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [TypeLibType(6)]
    [DefaultMember("Fields")]
    [Guid("00000109-0000-0010-8000-00AA006D2EA4")]
    [ClassInterface((short)0)]
    public class RelationClass : _Relation, Relation
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
        public extern virtual string Table
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
        public extern virtual string ForeignTable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809348)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809350)]
        public extern virtual int Attributes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809350)]
            [param: In]
            set;
        }

        [DispId(0)]
        public extern virtual Fields Fields
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [DispId(1610809354)]
        public extern virtual bool PartialReplica
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809354)]
            [param: In]
            set;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern RelationClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809353)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Field CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size);

        Field _Relation.CreateField([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Size)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateField
            return this.CreateField(Name, Type, Size);
        }
    }
}
