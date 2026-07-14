using BaseLib.Helper;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GenFreeWin.Main;

[EditorBrowsable(EditorBrowsableState.Advanced)]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
[CompilerGenerated]
internal sealed class MainSettings : ApplicationSettingsBase
{
    private static MainSettings defaultInstance = (MainSettings)SettingsBase.Synchronized(new MainSettings());

    private static bool addedHandler;

    private static object addedHandlerLockObject = (new object());

    public static MainSettings Default
    {
        get
        {
            if (!addedHandler)
            {
                object obj = addedHandlerLockObject;
                ObjectFlowControl.CheckForSyncLockOnValueType(obj);
                Monitor.Enter(obj);
                try
                {
                    if (!addedHandler)
                    {
                        MainProject.Application.Shutdown += delegate
                        {
                            if (MainProject.Application.SaveMySettingsOnExit)
                            {
                                MainSettingsProperty.Settings.Save();
                            }
                        };
                        addedHandler = true;
                    }
                }
                finally
                {
                    Monitor.Exit(obj);
                }
            }
            return defaultInstance;
        }
    }

    [SpecialSetting(SpecialSetting.ConnectionString)]
    [DefaultSettingValue("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=d:\\Gen_FreeWin\\JC-Soft\\Gen_Plusdaten.mdb")]
    [DebuggerNonUserCode]
    [ApplicationScopedSetting]
    public string Gen_PlusdatenConnectionString => this["Gen_PlusdatenConnectionString"].AsString();

    [DebuggerNonUserCode]
    public MainSettings()
    {
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    private static void AutoSaveSettings(object sender, EventArgs e)
    {
        if (MainProject.Application.SaveMySettingsOnExit)
        {
            MainSettingsProperty.Settings.Save();
        }
    }
}
