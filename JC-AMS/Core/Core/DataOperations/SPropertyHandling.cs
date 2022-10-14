using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace JCAMS.Core.DataOperations
{
    /// <summary>Static class for handling class-properties.</summary>
    public static class SPropertyHandling
    {
        /// <summary>Sets the property and fires an action if a change happend.</summary>
        /// <typeparam name="T">The (generic-) type of the property</typeparam>
        /// <param name="value">The new value of the property.</param>
        /// <param name="store">The stored value (by ref).</param>
        /// <param name="action">The action that is fired if a change happend.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>[<see cref="true" />] if a change happend; [<see cref="false" />] otherwise</returns>
        /// <example>Easy:<code>public string SomeValue {get =&gt; _val; set =&gt; value.SetProperty(ref _val); }
        /// </code>Advanced:<code>public string SomeValue {
        ///    get =&gt; _val; 
        ///    set =&gt; value.SetProperty(ref _val;(o,n,s) =&gt; Debug.Print( "Property '{0}' changed from '{1}' to '{2}'", s, o, n)); 
        /// }
        /// </code></example>
        /// <remarks>Works as an extension</remarks>
        public static bool SetProperty<T>(this T value,ref T store,Action<T,T,string> action = null, [CallerMemberName] string name = "")         
        {
            if (EqualityComparer<T>.Default.Equals(store, value)) return false;  
            T oldval = store;
            lock (store ?? value)
               store = value; 
            action?.Invoke(oldval,store,name);
            return true;
        }
    }
}