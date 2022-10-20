// ***********************************************************************
// Assembly         : SyncAsyncParallel
// Author           : Mir
// Created          : 12-26-2021
//
// Last Modified By : Mir
// Last Modified On : 07-01-2022
// ***********************************************************************
// <copyright file="Settings.Designer.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2021
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace SyncAsyncParallel.Properties {


    /// <summary>
    /// Class Settings. This class cannot be inherited.
    /// Implements the <see cref="System.Configuration.ApplicationSettingsBase" />
    /// </summary>
    /// <seealso cref="System.Configuration.ApplicationSettingsBase" />
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.2.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {

        /// <summary>
        /// The default instance
        /// </summary>
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
    }
}
