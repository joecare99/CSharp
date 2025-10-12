using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DAO
{
    //[ComImport]
    [Guid("00000106-0000-0010-8000-00AA006D2EA4")]
    [DefaultMember("Users")]
    [TypeLibType(6)]
    [ClassInterface((short)0)]
    public class GroupClass : _Group, Group
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

        [DispId(0)]
        public extern virtual Users Users
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(0)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        //[MethodImpl(MethodImplOptions.InternalCall)]
        //public extern GroupClass();

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(1610809348)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public extern virtual User CreateUser([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Password);

        User _Group.CreateUser([Optional][In][MarshalAs(UnmanagedType.Struct)] object Name, [Optional][In][MarshalAs(UnmanagedType.Struct)] object PID, [Optional][In][MarshalAs(UnmanagedType.Struct)] object Password)
        {
            //ILSpy generated this explicit interface implementation from .override directive in CreateUser
            return this.CreateUser(Name, PID, Password);
        }
    }
}
