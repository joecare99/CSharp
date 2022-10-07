// ***********************************************************************
// Assembly         : Calc64WF
// Author           : Mir
// Created          : 09-01-2022
//
// Last Modified By : Mir
// Last Modified On : 09-01-2022
// ***********************************************************************
// <copyright file="Resources.Designer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Calc64WF.Properties {
    using System;


    /// <summary>
    /// Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {

        /// <summary>
        /// The resource man
        /// </summary>
        private static global::System.Resources.ResourceManager resourceMan;

        /// <summary>
        /// The resource culture
        /// </summary>
        private static global::System.Globalization.CultureInfo resourceCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resources"/> class.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }

        /// <summary>
        /// Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        /// <value>The resource manager.</value>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Calc64WF.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        /// Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        /// Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        /// <value>The culture.</value>
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
        /// Sucht eine lokalisierte Ressource vom Typ System.Drawing.Icon ähnlich wie (Symbol).
        /// </summary>
        /// <value>The calculate 32.</value>
        public static System.Drawing.Icon Calc_32 {
            get {
                object obj = ResourceManager.GetObject("Calc_32", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        /// <value>The calculator 64.</value>
        public static System.Drawing.Bitmap calculator_64 {
            get {
                object obj = ResourceManager.GetObject("calculator_64", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        /// <value>The exit.</value>
        public static System.Drawing.Bitmap Exit {
            get {
                object obj = ResourceManager.GetObject("Exit", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Ressource vom Typ System.Drawing.Bitmap.
        /// </summary>
        /// <value>The glow white.</value>
        public static System.Drawing.Bitmap Glow_White {
            get {
                object obj = ResourceManager.GetObject("Glow_White", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die /\ ähnelt.
        /// </summary>
        /// <value>The op mode binary and.</value>
        public static string OPMode_BinaryAnd {
            get {
                return ResourceManager.GetString("OPMode_BinaryAnd", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die == ähnelt.
        /// </summary>
        /// <value>The op mode binary equals.</value>
        public static string OPMode_BinaryEquals {
            get {
                return ResourceManager.GetString("OPMode_BinaryEquals", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die ! ähnelt.
        /// </summary>
        /// <value>The op mode binary not.</value>
        public static string OPMode_BinaryNot {
            get {
                return ResourceManager.GetString("OPMode_BinaryNot", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die \/ ähnelt.
        /// </summary>
        /// <value>The op mode binary or.</value>
        public static string OPMode_BinaryOr {
            get {
                return ResourceManager.GetString("OPMode_BinaryOr", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die X ähnelt.
        /// </summary>
        /// <value>The op mode binary xor.</value>
        public static string OPMode_BinaryXor {
            get {
                return ResourceManager.GetString("OPMode_BinaryXor", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die = ähnelt.
        /// </summary>
        /// <value>The op mode calculate result.</value>
        public static string OPMode_CalcResult {
            get {
                return ResourceManager.GetString("OPMode_CalcResult", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die / ähnelt.
        /// </summary>
        /// <value>The op mode divide.</value>
        public static string OPMode_Divide {
            get {
                return ResourceManager.GetString("OPMode_Divide", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die M+ ähnelt.
        /// </summary>
        /// <value>The op mode memory add.</value>
        public static string OPMode_MemAdd {
            get {
                return ResourceManager.GetString("OPMode_MemAdd", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die MC ähnelt.
        /// </summary>
        /// <value>The op mode memory clear.</value>
        public static string OPMode_MemClear {
            get {
                return ResourceManager.GetString("OPMode_MemClear", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die MR ähnelt.
        /// </summary>
        /// <value>The op mode memory retreive.</value>
        public static string OPMode_MemRetreive {
            get {
                return ResourceManager.GetString("OPMode_MemRetreive", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die MS ähnelt.
        /// </summary>
        /// <value>The op mode memory store.</value>
        public static string OPMode_MemStore {
            get {
                return ResourceManager.GetString("OPMode_MemStore", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die M- ähnelt.
        /// </summary>
        /// <value>The op mode memory subtract.</value>
        public static string OPMode_MemSubtract {
            get {
                return ResourceManager.GetString("OPMode_MemSubtract", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die - ähnelt.
        /// </summary>
        /// <value>The op mode minus.</value>
        public static string OPMode_Minus {
            get {
                return ResourceManager.GetString("OPMode_Minus", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die Mod ähnelt.
        /// </summary>
        /// <value>The op mode modulo.</value>
        public static string OPMode_Modulo {
            get {
                return ResourceManager.GetString("OPMode_Modulo", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die * ähnelt.
        /// </summary>
        /// <value>The op mode multiply.</value>
        public static string OPMode_Multiply {
            get {
                return ResourceManager.GetString("OPMode_Multiply", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die !/\ ähnelt.
        /// </summary>
        /// <value>The op mode nand.</value>
        public static string OPMode_Nand {
            get {
                return ResourceManager.GetString("OPMode_Nand", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die +/- ähnelt.
        /// </summary>
        /// <value>The op mode negate.</value>
        public static string OPMode_Negate {
            get {
                return ResourceManager.GetString("OPMode_Negate", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die  ähnelt.
        /// </summary>
        /// <value>The op mode no mode.</value>
        public static string OPMode_NoMode {
            get {
                return ResourceManager.GetString("OPMode_NoMode", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die !/\ ähnelt.
        /// </summary>
        /// <value>The op mode nor.</value>
        public static string OPMode_Nor {
            get {
                return ResourceManager.GetString("OPMode_Nor", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die + ähnelt.
        /// </summary>
        /// <value>The op mode plus.</value>
        public static string OPMode_Plus {
            get {
                return ResourceManager.GetString("OPMode_Plus", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die ^ ähnelt.
        /// </summary>
        /// <value>The op mode power.</value>
        public static string OPMode_Power {
            get {
                return ResourceManager.GetString("OPMode_Power", resourceCulture);
            }
        }

        /// <summary>
        /// Sucht eine lokalisierte Zeichenfolge, die !X ähnelt.
        /// </summary>
        /// <value>The op mode x nor.</value>
        public static string OPMode_XNor {
            get {
                return ResourceManager.GetString("OPMode_XNor", resourceCulture);
            }
        }
    }
}
