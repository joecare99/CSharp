﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVVM_06_Converters_4.Properties; 


[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.5.0.0")]
internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
    
    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
    
    public static Settings Default {
        get {
            return defaultInstance;
        }
    }
    
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("1200")]
    public double Vehicle_Width {
        get {
            return ((double)(this["Vehicle_Width"]));
        }
        set {
            this["Vehicle_Width"] = value;
        }
    }
    
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("2000")]
    public double Vehicle_Length {
        get {
            return ((double)(this["Vehicle_Length"]));
        }
        set {
            this["Vehicle_Length"] = value;
        }
    }
    
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("800")]
    public double SwivelKoor_X {
        get {
            return ((double)(this["SwivelKoor_X"]));
        }
        set {
            this["SwivelKoor_X"] = value;
        }
    }
    
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("-200")]
    public double SwivelKoor_Y {
        get {
            return ((double)(this["SwivelKoor_Y"]));
        }
        set {
            this["SwivelKoor_Y"] = value;
        }
    }
    
    [global::System.Configuration.UserScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.DefaultSettingValueAttribute("400")]
    public double AxisOffset {
        get {
            return ((double)(this["AxisOffset"]));
        }
        set {
            this["AxisOffset"] = value;
        }
    }
}
