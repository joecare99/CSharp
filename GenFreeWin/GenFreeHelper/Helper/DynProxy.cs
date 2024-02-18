using System;
using System.Dynamic;
using System.Reflection;

namespace GenFree.Helper
{
    public class DynProxy : DynamicObject
    {
        private Type _t;
        private object? _o;

        public DynProxy(Type t)
        {
            _t = t;
            _o = Activator.CreateInstance(t);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            if (_t.GetProperty(binder.Name) is PropertyInfo p && p.CanRead)
            {
                result = p.GetValue(_o);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            if (_t.GetProperty(binder.Name) is PropertyInfo p && p.CanWrite)
            {
                p.SetValue(_o, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object? result)
        {
            if (_t.GetMethod(binder.Name) is MethodInfo m)
            {
                result = m.Invoke(_o, args);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
