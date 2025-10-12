using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    [ComImport]
    [TypeLibType(4288)]
    [Guid("00000063-0000-0010-8000-00AA006D2EA4")]
    public interface Groups : _DynaCollection
    {
        [DispId(1610743808)]
        new short Count
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610743808)]
            get;
        }

        [DispId(0)]
        Group this[[In][MarshalAs(UnmanagedType.Struct)] object Item]
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [TypeLibFunc(64)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(-4)]
        [TypeLibFunc(1)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        new IEnumerator GetEnumerator();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610743810)]
        new void Refresh();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809344)]
        new void Append([In][MarshalAs(UnmanagedType.IDispatch)] object Object);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809345)]
        new void Delete([In][MarshalAs(UnmanagedType.BStr)] string Name);
    }
}
