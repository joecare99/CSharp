using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [DefaultMember("Value")]
    [Guid("00000051-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(4288)]
    public interface _Field : _DAO
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
        short Type
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
        string Name
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
        int Size
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
        string SourceField
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809351)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(1610809352)]
        string SourceTable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809352)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
        }

        [DispId(0)]
        object Value
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
        int Attributes
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
        short OrdinalPosition
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
        string ValidationText
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
        bool ValidateOnSet
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
        string ValidationRule
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
        object DefaultValue
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
        bool Required
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
        bool AllowZeroLength
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
        bool DataUpdatable
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809371)]
            get;
        }

        [DispId(1610809372)]
        string ForeignName
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
        short CollectionIndex
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809378)]
            [TypeLibFunc(64)]
            get;
        }

        [DispId(1610809379)]
        object OriginalValue
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809379)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809380)]
        object VisibleValue
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809380)]
            [return: MarshalAs(UnmanagedType.Struct)]
            get;
        }

        [DispId(1610809381)]
        int FieldSize
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809381)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809374)]
        void AppendChunk([In][MarshalAs(UnmanagedType.Struct)] object Val);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809375)]
        [return: MarshalAs(UnmanagedType.Struct)]
        object GetChunk([In] int Offset, [In] int Bytes);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [TypeLibFunc(1)]
        [DispId(1610809376)]
        int _30_FieldSize();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809377)]
        [return: MarshalAs(UnmanagedType.Interface)]
        Property CreateProperty([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Type, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Value, [Optional][In][MarshalAs(UnmanagedType.Struct)] object DDL);
    }
}
