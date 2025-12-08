using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [ClassInterface((short)0)]
    [DefaultMember("Value")]
    [Guid("00000104-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(6)]
    public class FieldClass : _Field, Field
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
        public extern virtual int CollatingOrder
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809344)]
            get;
        }

        [DispId(1610809345)]
        public extern virtual short Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809345)]
            [param: In]
            set;
        }

        [DispId(1610809347)]
        public extern virtual string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809349)]
        public extern virtual int Size
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
        public extern virtual string SourceField
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809352)]
        public extern virtual string SourceTable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(0)]
        public extern virtual object Value
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [DispId(1610809355)]
        public extern virtual int Attributes
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
        public extern virtual short OrdinalPosition
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809357)]
            [param: In]
            set;
        }

        [DispId(1610809359)]
        public extern virtual string ValidationText
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809359)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809361)]
        public extern virtual bool ValidateOnSet
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809361)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809361)]
            [param: In]
            set;
        }

        [DispId(1610809363)]
        public extern virtual string ValidationRule
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809363)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809363)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809365)]
        public extern virtual object DefaultValue
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809365)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809365)]
            [param: In]
            [param: MarshalAs(UnmanagedType.Struct)]
            set;
        }

        [DispId(1610809367)]
        public extern virtual bool Required
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809367)]
            [param: In]
            set;
        }

        [DispId(1610809369)]
        public extern virtual bool AllowZeroLength
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809369)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809369)]
            [param: In]
            set;
        }

        [DispId(1610809371)]
        public extern virtual bool DataUpdatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809371)]
            get;
        }

        [DispId(1610809372)]
        public extern virtual string ForeignName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809372)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809372)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809378)]
        public extern virtual short CollectionIndex
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(1610809378)]
            get;
        }

        [DispId(1610809379)]
        public extern virtual object OriginalValue
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809379)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809380)]
        public extern virtual object VisibleValue
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809380)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809381)]
        public extern virtual int FieldSize
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809381)]
            get;
        }

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern FieldClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809374)]
        public extern virtual void AppendChunk([In][MarshalAs(UnmanagedType.Struct)] object Val);

        void _Field.AppendChunk([In][MarshalAs(UnmanagedType.Struct)] object Val)
        {
            //ILSpy generated this explicit interface implementation from .override directive in AppendChunk
            this.AppendChunk(Val);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809375)]
        [return: MarshalAs(UnmanagedType.Struct)]
        public extern virtual object GetChunk([In] int Offset, [In] int Bytes);

        object _Field.GetChunk([In] int Offset, [In] int Bytes)
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetChunk
            return this.GetChunk(Offset, Bytes);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809376)]
        public extern virtual int _30_FieldSize();

        int _Field._30_FieldSize()
        {
            //ILSpy generated this explicit interface implementation from .override directive in _30_FieldSize
            return this._30_FieldSize();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809377)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);

        Property _Field.CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateProperty
            return this.CreateProperty(Name, Type, Value, DDL);
        }
    }
}
