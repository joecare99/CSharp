﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVVM_04_DelegateCommand.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MVVM_04_DelegateCommand.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Clear ähnelt.
        /// </summary>
        public static string btnClear {
            get {
                return ResourceManager.GetString("btnClear", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die &lt;Page x:Class=&quot;MVVM_04_DelegateCommand.Views.DelegateCommandView&quot;
        ///      xmlns=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;
        ///      xmlns:x=&quot;http://schemas.microsoft.com/winfx/2006/xaml&quot;
        ///      xmlns:mc=&quot;http://schemas.openxmlformats.org/markup-compatibility/2006&quot; 
        ///      xmlns:d=&quot;http://schemas.microsoft.com/expression/blend/2008&quot; 
        ///      xmlns:local=&quot;clr-namespace:MVVM_04_DelegateCommand.Views&quot;
        ///      xmlns:p=&quot;clr-namespace:MVVM_04_DelegateCommand.Properties&quot;
        ///      xmlns:mvvm=&quot;clr-namespace [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        public static string DelegateCommandView {
            get {
                return ResourceManager.GetString("DelegateCommandView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die using CommunityToolkit.Mvvm.Input;
        ///using MVVM.ViewModel;
        ///
        ///namespace MVVM_04_DelegateCommand.ViewModels
        ///{
        ///    public class DelegateCommandViewModel : BaseViewModel
        ///    {
        ///        #region Properties
        ///        private string _firstname;
        ///        private string _lastname;
        ///
        ///        public RelayCommand&lt;object&gt; ClearCommand { get; set; }
        ///        public string Firstname { get =&gt; _firstname; set =&gt; SetProperty(ref _firstname, value); }
        ///        public string Lastname { get =&gt; _lastname; set =&gt; SetProperty(re [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        public static string DelegateCommandViewModel {
            get {
                return ResourceManager.GetString("DelegateCommandViewModel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Firstname: ähnelt.
        /// </summary>
        public static string FirstName {
            get {
                return ResourceManager.GetString("FirstName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Fullname: ähnelt.
        /// </summary>
        public static string FullName {
            get {
                return ResourceManager.GetString("FullName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Lastname: ähnelt.
        /// </summary>
        public static string LastName {
            get {
                return ResourceManager.GetString("LastName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die MVVM #4 Delegate Command ähnelt.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
    }
}
