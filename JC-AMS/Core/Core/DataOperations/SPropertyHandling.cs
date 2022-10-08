using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JCAMS.Core.DataOperations
{
    public static class SPropertyHandling
    {
        public static bool SetProperty<T>(this T value,ref T store,Action<T,T,string> action = null, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(store, value)) return false;  
            T oldval = store;
            store = value;
            action?.Invoke(oldval,store,name);
            return true;
        } 
    }
}
