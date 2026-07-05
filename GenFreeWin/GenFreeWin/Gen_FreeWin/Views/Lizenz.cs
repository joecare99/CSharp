using BaseLib.Helper;
using Gen_FreeWin.Services;
using Gen_FreeWin.ViewModels;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.Sys;
using GenFree.Interfaces.VB;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Gen_FreeWin;

public partial class Lizenz : Form
{
    private static List<WeakReference> __ENCList = new List<WeakReference>();
    IModul1 Modul1 => _Modul1.Instance;
    IInteraction Interaction;
    IProjectData ProjectData;
    IStrings Strings;

    private LicenseViewModel _viewModel;
    private ILicensePersistenceService _persistenceService;

#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [AccessedThroughProperty(nameof(ACommand1))]
    private ControlArray<Button> _Command1;

    [AccessedThroughProperty(nameof(ALabel2))]
    private ControlArray<Label> _Label2;

    [AccessedThroughProperty(nameof(AText1))]
    private ControlArray<TextBox> _Text1;
#pragma warning restore CS0618 // Typ oder Element ist veraltet


#pragma warning disable CS0618 // Typ oder Element ist veraltet

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#pragma warning restore CS0618 // Typ oder Element ist veraltet


#pragma warning disable CS0618 // Typ oder Element ist veraltet

    public virtual ControlArray<Button> ACommand1
    {
        [DebuggerNonUserCode]
        get => _Command1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler obj = _Command1_Click;
            if (_Command1 != null)
            {
                _Command1.RemoveClick(obj);
            }
            _Command1 = value;
            if (_Command1 != null)
            {
                _Command1.AddClick(obj);
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ControlArray<Label> ALabel2
    {
        [DebuggerNonUserCode]
        get => _Label2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set => _Label2 = value;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual ControlArray<TextBox> AText1
    {
        [DebuggerNonUserCode]
        get => _Text1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            KeyEventHandler obj = _Text1_KeyUp;
            KeyPressEventHandler obj2 = _Text1_KeyPress;
            if (_Text1 != null)
            {
                _Text1.RemoveKeyUp(obj);
                _Text1.RemoveKeyPress(obj2);
            }
            _Text1 = value;
            if (_Text1 != null)
            {
                _Text1.AddKeyUp(obj);
                _Text1.AddKeyPress(obj2);
            }
        }
    }
#pragma warning restore CS0618 // Typ oder Element ist veraltet


    public Lizenz()
    {
        Load += _Lizenz_Load;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
#pragma warning disable CS0618 // Typ oder Element ist veraltet
        ACommand1 = new ControlArray<System.Windows.Forms.Button>();
        ALabel2 = new ControlArray<System.Windows.Forms.Label>();
        AText1 = new ControlArray<System.Windows.Forms.TextBox>();
#pragma warning restore CS0618 // Typ oder Element ist veraltet
        ((System.ComponentModel.ISupportInitialize)ACommand1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)ALabel2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)AText1).BeginInit();

        InitializeComponent();

        ACommand1.SetIndex(_Command1_1, 1);
        ACommand1.SetIndex(_Command1_0, 0);
        AText1.SetIndex(_Text1_3, 3);
        AText1.SetIndex(_Text1_1, 1);
        AText1.SetIndex(_Text1_0, 0);
        ALabel2.SetIndex(_Label2_1, 1);
        ALabel2.SetIndex(_Label2_0, 0);

        ((System.ComponentModel.ISupportInitialize)ACommand1).EndInit();
        ((System.ComponentModel.ISupportInitialize)ALabel2).EndInit();
        ((System.ComponentModel.ISupportInitialize)AText1).EndInit();

        // Initialize ViewModel and Persistence Service
        _persistenceService = new LicensePersistenceService(Modul1);
        _viewModel = new LicenseViewModel(Modul1, Interaction, ProjectData, Strings);
    }


    private void _Command1_Click(object eventSender, EventArgs eventArgs)
    {
        short index = (short)ACommand1.GetIndex((Button)eventSender);

        switch (index)
        {
            case 0: // Verify button clicked
                // Update ViewModel with UI values
                _viewModel.LicText1 = AText1[0].Text;
                _viewModel.LicText2 = AText1[1].Text;
                _viewModel.LicText3 = AText1[3].Text;

                // Execute verification
                if (_viewModel.VerifyLicense())
                {
                    Close();
                }
                break;

            case 1: // Cancel button clicked
                Close();
                break;
        }
    }

    private void _Lizenz_Load(object eventSender, EventArgs eventArgs)
    {
        // Initialize UI labels from localization
        Label1.Text = Modul1.IText[EUserText.t112];
        ACommand1[0].Text = Modul1.IText[EUserText.t113];
        ACommand1[1].Text = Modul1.IText[EUserText.tNMCancel];

        // Set tab order for better UX
        AText1[0].TabIndex = 0;
        AText1[1].TabIndex = 1;
        AText1[3].TabIndex = 2;
        ACommand1[0].TabIndex = 3;
        ACommand1[1].TabIndex = 4;

        // Initialization complete - Form is ready for user input
    }

    private void _Text1_KeyPress(object eventSender, KeyPressEventArgs eventArgs)
    {
        short num = checked((short)Strings.Asc(eventArgs.KeyChar));
        switch (AText1.GetIndex((TextBox)eventSender))
        {
            case 0:
                if (num == 189)
                {
                    num = 0;
                }
                if (num is not 8 and < 48)
                {
                    num = 0;
                }
                break;
            case 1:
            case 3:
                if (num > 57)
                {
                    num = 0;
                }
                if (num is not 8 and < 48)
                {
                    num = 0;
                }
                break;
        }
        eventArgs.KeyChar = Strings.Chr(num);
        if (num == 0)
        {
            eventArgs.Handled = true;
        }
    }

    private void _Text1_KeyUp(object eventSender, KeyEventArgs eventArgs)
    {
        short num = (short)eventArgs.KeyCode;
        short num2 = (short)unchecked((int)eventArgs.KeyData / 65536);
        switch (AText1.GetIndex((TextBox)eventSender))
        {
            case 0:
                if (AText1[0].Text.Length == 10)
                {
                    _ = AText1[1].Focus();
                }
                break;
            case 1:
                if (AText1[1].Text.Length == 10)
                {
                    _ = AText1[3].Focus();
                }
                break;
            case 2:
                if (AText1[2].Text.Length == 2)
                {
                    _ = AText1[1].Focus();
                }
                break;
            case 3:
                if (AText1[3].Text.Length >= 5)
                {
                    ACommand1[0].Enabled = true;
                    _ = ACommand1[0].Focus();
                }
                break;
        }
    }

    private void _Button1_Click(object sender, EventArgs e)
    {
        Label3.Visible = true;
    }

    private void _Command1_0_Click(object sender, EventArgs e)
    {
    }
}
