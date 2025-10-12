using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [DefaultMember("Groups")]
    [Guid("00000107-0000-0010-8000-00AA006D2EA4")]
    [TypeLibType(6)]
    [ClassInterface((short)0)]
    public class UserClass : _User, User
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
        public extern virtual string PID
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809346)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(1610809347)]
        public extern virtual string Password
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1610809347)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(0)]
        public extern virtual Groups Groups
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern UserClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809349)]
        public extern virtual void NewPassword([In][MarshalAs(UnmanagedType.BStr)] string bstrOld, [In][MarshalAs(UnmanagedType.BStr)] string bstrNew);

        void _User.NewPassword([In][MarshalAs(UnmanagedType.BStr)] string bstrOld, [In][MarshalAs(UnmanagedType.BStr)] string bstrNew)
        {
            //ILSpy generated this explicit interface implementation from .override directive in NewPassword
            this.NewPassword(bstrOld, bstrNew);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809350)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual Group CreateGroup([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID);

        Group _User.CreateGroup([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateGroup
            return this.CreateGroup(Name, PID);
        }
    }
}
