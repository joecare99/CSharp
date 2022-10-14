using System;
using System.Runtime.CompilerServices;

namespace JCAMS.Core.DataOperations
{
    /// <summary>A (base-)class that implements the basic INotifyPropertyChangedEx-interface</summary>
    public class CPropNotificationClass : INotifyPropertyChangedEx
    {

        /// <summary>Occurs when a property was changed (advanved).</summary>
        public event PropertyChangedExEventHandler OnPropertyChangedEx;

        /// <summary>Raises the property changed (adv.) event.</summary>
        /// <param name="oldVal">The old value.</param>
        /// <param name="newVal">The new value.</param>
        /// <param name="PropertyName">Name of the property.</param>
        public void RaisePropertyChangedEx(object oldVal,object newVal, [CallerMemberName] string PropertyName = "")
        {
             OnPropertyChangedEx?.Invoke(this, new PropertyChangedExEventArgs(PropertyName,oldVal,newVal)); 
        }
        /// <summary>Sets the value (incl. firing the OnChanged-event when a different value is assigned).</summary>
        /// <typeparam name="T">The (generic-) type of the values</typeparam>
        /// <param name="value">The new value.</param>
        /// <param name="store">The stored value.</param>
        /// <param name="action">The action to be called when a change is detected.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>[true] if a change occured (for external handling) otherwise [false]</returns>
        /// <remarks>Uses <see cref="SPropertyHandling.SetValue" /> to actually change the stored value</remarks>
        /// <example>Easy:<code>public string SomeValue { get =&gt; _val; set =&gt; SetValue(value, ref _val ); }
        /// </code>Advanced:<code>public string SomeValue {
        ///    get =&gt; _val; 
        ///    set =&gt; SetValue(value, ref _val, (o,n,s) =&gt; Debug.Print( "Property '{0}' changed from '{1}' to '{2}'", s, o, n));
        /// }
        /// </code></example>
        /// <seealso cref="SPropertyHandling.SetValue">SetValue</seealso>
        public bool SetValue<T>(T value, ref T store, Action<T, T, string> action = null, [CallerMemberName] string name = "")
        {
            T oldval = store;
            if (value.SetProperty(ref store, action, name))
            {
                RaisePropertyChangedEx(oldval, value,name);
                return true;
            }
            else return false;
        }
    }
}
