// ***********************************************************************
// Assembly         : MVVM_21_Buttons
// Author           : Mir
// Created          : 08-14-2022
//
// Last Modified By : Mir
// Last Modified On : 08-14-2022
// ***********************************************************************
// <copyright file="Settings.Designer.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace MVVM_22_WpfCap.Properties;



/// <summary>
/// Class Settings. This class cannot be inherited.
/// Implements the <see cref="System.Configuration.ApplicationSettingsBase" />
/// </summary>
/// <seealso cref="System.Configuration.ApplicationSettingsBase" />
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
{

    /// <summary>
    /// The default instance
    /// </summary>
    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    /// <summary>
    /// Gets the default.
    /// </summary>
    /// <value>The default.</value>
    public static Settings Default
    {
        get
        {
            return defaultInstance;
        }
    }
}
