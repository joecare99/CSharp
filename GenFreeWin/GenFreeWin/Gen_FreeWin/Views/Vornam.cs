using BaseLib.Helper;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Views;

namespace GenFreeWin.Views;

/// <summary>
/// Vornam (given name) WinForms View.
/// Uses MVVM pattern with CommandBindingAttribute for declarative command binding.
/// Observable properties are bound via custom binding attributes.
/// </summary>
public partial class Vornam : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();

    public ToolTip ToolTip1;

    public ControlArray<TextBox> Text_Renamed;

#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#pragma warning restore CS0618 // Typ oder Element ist veraltet

#pragma warning disable CS0618 // Typ oder Element ist veraltet

    private IVornamViewModel _viewModel;
#pragma warning restore CS0618 // Typ oder Element ist veraltet

    [DebuggerNonUserCode]
    public Vornam(IVornamViewModel viewModel)
    {
        _viewModel = viewModel;
        Load += _Vornam_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        Text_Renamed = new ControlArray<TextBox>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        ((ISupportInitialize)Text_Renamed).BeginInit();

        InitializeComponent();

        Text_Renamed.SetIndex(_Text_1, 1);
        Text_Renamed.SetIndex(_Text_2, 2);
        Text_Renamed.SetIndex(_Text_3, 3);
        Text_Renamed.SetIndex(_Text_4, 4);
        Text_Renamed.SetIndex(_Text_5, 5);
        Text_Renamed.SetIndex(_Text_6, 6);
        Text_Renamed.SetIndex(_Text_7, 7);
        Text_Renamed.SetIndex(_Text_8, 8);
        Text_Renamed.SetIndex(_Text_9, 9);
        Text_Renamed.SetIndex(_Text_10, 10);
        Text_Renamed.SetIndex(_Text_11, 11);
        Text_Renamed.SetIndex(_Text_12, 12);
        Text_Renamed.SetIndex(_Text_13, 13);
        Text_Renamed.SetIndex(_Text_14, 14);
        Text_Renamed.SetIndex(_Text_15, 15);
        Text_Renamed.SetIndex(_Text_16, 16);

        Text_Renamed.SetIndex(_Text_51, 51);
        Text_Renamed.SetIndex(_Text_52, 52);
        Text_Renamed.SetIndex(_Text_53, 53);
        Text_Renamed.SetIndex(_Text_54, 54);
        Text_Renamed.SetIndex(_Text_55, 55);
        Text_Renamed.SetIndex(_Text_56, 56);
        Text_Renamed.SetIndex(_Text_57, 57);
        Text_Renamed.SetIndex(_Text_58, 58);
        Text_Renamed.SetIndex(_Text_59, 59);
        Text_Renamed.SetIndex(_Text_60, 60);
        Text_Renamed.SetIndex(_Text_61, 61);
        Text_Renamed.SetIndex(_Text_62, 62);
        Text_Renamed.SetIndex(_Text_63, 63);
        Text_Renamed.SetIndex(_Text_64, 64);
        Text_Renamed.SetIndex(_Text_65, 65);

        ((ISupportInitialize)Text_Renamed).EndInit();

        // Apply MVVM binding attributes for command and property binding
        CommandBindingAttribute.Commit(this, _viewModel);
        TextBindingAttribute.Commit(this, _viewModel);
    }

    /// <summary>
    /// Form load event handler: executes LoadNamesCommand from ViewModel.
    /// </summary>
    private void _Vornam_Load(object sender, EventArgs e)
    {
        try
        {
            // Relay to the ViewModel's LoadNamesCommand (if bound via CommandBindingAttribute)
            _viewModel?.Form_Load(sender, e);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Form load error: {ex.Message}");
        }
    }

}

