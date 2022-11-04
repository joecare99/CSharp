using System;
using System.ComponentModel;
using System.Security.Permissions;

namespace BaseLib.Interfaces
{
#if !NET6_0_OR_GREATER
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
#endif
    public class PropertyChangedAdvEventArgs : EventArgs
    {
        private readonly string _propertyName;
#if NET6_0_OR_GREATER
        private readonly object? _oldVal;
        private readonly object? _newVal;
#else
        private readonly object _oldVal;
        private readonly object _newVal;
#endif

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
        public virtual string PropertyName => _propertyName;

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
#if NET6_0_OR_GREATER
        public virtual object? OldVal => _oldVal;
#else
        public virtual object OldVal => _oldVal;
#endif

        //
        // Zusammenfassung:
        //     Ruft den Namen der geänderten Eigenschaft ab.
        //
        // Rückgabewerte:
        //     Der Name der geänderten Eigenschaft.
#if NET6_0_OR_GREATER
        public virtual object? NewVal => _newVal;
#else
        public virtual object NewVal => _newVal;
#endif

        //
        // Zusammenfassung:
        //     Initialisiert eine neue Instanz der System.ComponentModel.PropertyChangedEventArgs-Klasse.
        //
        // Parameter:
        //   propertyName:
        //     Der Name der geänderten Eigenschaft.
#if NET6_0_OR_GREATER
        public PropertyChangedAdvEventArgs(string propertyName,object? oldVal,object? newVal)
#else
        public PropertyChangedAdvEventArgs(string propertyName,object oldVal,object newVal)
#endif
        {
            _propertyName = propertyName;
            _oldVal = oldVal;
            _newVal = newVal;
        }
    }

#if !NET6_0_OR_GREATER
    [HostProtection(SecurityAction.LinkDemand, SharedState = true)]
#endif
    public delegate void PropertyChangedAdvEventHandler(object sender, PropertyChangedAdvEventArgs e);

    //
    // Zusammenfassung:
    //     Benachrichtigt Clients, dass sich ein Eigenschaftswert geändert hat.
    public interface INotifyPropertyChangedAdv
    {
        //
        // Zusammenfassung:
        //     Tritt ein, wenn sich ein Eigenschaftswert ändert.
#if NET6_0_OR_GREATER
        event PropertyChangedAdvEventHandler? PropertyChangedAdv;
#else
        event PropertyChangedAdvEventHandler PropertyChangedAdv;
#endif
    }
    
}
