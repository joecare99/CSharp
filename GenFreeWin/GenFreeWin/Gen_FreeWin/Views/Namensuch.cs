using BaseLib.Helper;
using GenFreeWin.ViewModels;
using GenFree;
using GenFree.Helper;
using GenFree.Interfaces.UI;
using GenFree.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace GenFreeWin.Views;

public partial class Namensuch : Form
{
    private static List<WeakReference> __ENCList = new();
    public static Namensuch Default => MainProject.Forms.Namensuch;

    INamenSuchViewModel _viewModel;
    public INamenSuchViewModel ViewModel => _viewModel;

    public int FamPerSchalt => _viewModel.FamPerschalt;
    public int SuchFam => _viewModel.FamNr;
    public int SuchPer => _viewModel.PersNr;

    IApplUserTexts strings;

    private IContainer components;

    [DebuggerNonUserCode]
    public Namensuch(INamenSuchViewModel viewModel)
    {
        _viewModel = viewModel;
        _viewModel.View = this;

        if (_viewModel is NamenSuchViewModel namenSuchViewModel)
        {
            namenSuchViewModel.AttachViewAdapter(new NamenSuchViewAdapter(this));
        }

        FormClosed += Namensuch_FormClosed;
        Load += Namensuch_Load;
        strings = _Modul1.Instance.IText;

        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        components = new Container();


        Label1 = new();
        Label5 = new();
        Label6 = new();
        Label7 = new();
        Label8 = new();
        Line1 = new();

        InitializeComponent();

        //Check2.AddCheckedChanged( new EventHandler(_viewModel.Check2_CheckStateChanged));

        //Check2.SetIndex(chbMale, 0);
        //Check2.SetIndex(chbFemales, 1);
        //Check2.SetIndex(chbFamOnly, 2);
        //Check2.SetIndex(chbSelection, 3);
        //Check2.SetIndex(chbFemale2, 5);
        //Check2.SetIndex(chbMale2, 4);

        //btnEnterNew.SetIndex(frmSrch_btnClose, 0);
        //btnEnterNew.SetIndex(btnPersonSheet, 1);
        //btnEnterNew.SetIndex(btnFamilySheet, 2);
        //btnEnterNew.SetIndex(btnStartSearch, 3);
        //btnEnterNew.SetIndex(btnReady, 4);
        //btnEnterNew.SetIndex(btnPrintList, 7);

        Label1.SetIndex(Label15, 15);
        Label1.SetIndex(Label16, 16);
        Label1.SetIndex(Label17, 17);
        Label1.SetIndex(Label18, 18);
        Label1.SetIndex(Label19, 19);
        Label1.SetIndex(Label20, 20);
        Label1.SetIndex(Label21, 21);
        Label1.SetIndex(Label22, 22);
        Label1.SetIndex(Label23, 23);
        Label1.SetIndex(Label24, 24);
        Label1.SetIndex(Label25, 25);
        Label1.SetIndex(Label26, 26);
        Label1.SetIndex(Label27, 27);

        Label5.SetIndex(_Label5_0, 0);
        Label5.SetIndex(_Label5_1, 1);
        Label5.SetIndex(_Label5_2, 2);
        Label5.SetIndex(_Label5_3, 3);
        Label5.SetIndex(_Label5_4, 4);
        Label5.SetIndex(_Label5_5, 5);
        Label5.SetIndex(_Label5_6, 6);
        Label5.SetIndex(_Label5_7, 7);
        Label5.SetIndex(_Label5_8, 8);
        Label5.SetIndex(_Label5_9, 9);
        Label5.SetIndex(_Label5_10, 10);
        Label5.SetIndex(_Label5_11, 11);
        Label5.SetIndex(_Label5_12, 12);
        Label5.SetIndex(_Label5_13, 13);
        Label5.SetIndex(_Label5_14, 14);
        Label5.SetIndex(_Label5_15, 15);

        Label6.SetIndex(lblMarr1, 0);
        Label6.SetIndex(lblMarr2, 1);
        Label6.SetIndex(_Label6_2, 2);
        Label6.SetIndex(_Label6_3, 3);
        Label6.SetIndex(_Label6_4, 4);
        Label6.SetIndex(_Label6_5, 5);
        Label6.SetIndex(lblMarr7, 6);
        Label6.SetIndex(lblMarr8, 7);

        Label7.SetIndex(_Label7_0, 0);
        Label7.SetIndex(_Label7_1, 1);
        Label7.SetIndex(_Label7_2, 2);
        Label7.SetIndex(_Label7_3, 3);
        Label7.SetIndex(_Label7_4, 4);
        Label7.SetIndex(_Label7_5, 5);
        Label7.SetIndex(_Label7_6, 6);
        Label7.SetIndex(_Label7_7, 7);
        Label7.SetIndex(_Label7_8, 8);
        Label7.SetIndex(_Label7_9, 9);
        Label7.SetIndex(_Label7_10, 10);
        Label7.SetIndex(_Label7_11, 11);
        Label7.SetIndex(_Label7_12, 12);
        Label7.SetIndex(_Label7_13, 13);
        Label7.SetIndex(_Label7_14, 14);
        Label7.SetIndex(_Label7_15, 15);

        Label8.SetIndex(_Label8_0, 0);
        Label8.SetIndex(_Label8_1, 1);
        Label8.SetIndex(_Label8_2, 2);
        Label8.SetIndex(_Label8_3, 3);
        Label8.SetIndex(_Label8_4, 4);
        Label8.SetIndex(_Label8_5, 5);
        Label8.SetIndex(_Label8_6, 6);
        Label8.SetIndex(_Label8_7, 7);

        Line1.SetIndex(_Line1_0, 0);
        Line1.SetIndex(_Line1_1, 1);
        Line1.SetIndex(_Line1_2, 2);
        Line1.SetIndex(_Line1_3, 3);
        Line1.SetIndex(_Line1_4, 4);
        Line1.SetIndex(_Line1_5, 5);
        Line1.SetIndex(_Line1_6, 6);
        Line1.SetIndex(_Line1_7, 7);
        Line1.SetIndex(_Line1_8, 8);
        Line1.SetIndex(_Line1_9, 9);
        Line1.SetIndex(_Line1_10, 10);
        Line1.SetIndex(_Line1_11, 11);
        Line1.SetIndex(_Line1_12, 12);
        Line1.SetIndex(_Line1_13, 13);
        Line1.SetIndex(_Line1_14, 14);
        Line1.SetIndex(_Line1_15, 15);
        Line1.SetIndex(_Line1_16, 16);
        Line1.SetIndex(_Line1_17, 17);
        Line1.SetIndex(_Line1_19, 19);
        Line1.SetIndex(_Line1_20, 20);
        Line1.SetIndex(_Line1_21, 21);
        Line1.SetIndex(_Line1_22, 22);
        Line1.SetIndex(_Line1_23, 23);
        Line1.SetIndex(_Line1_24, 24);
        Line1.SetIndex(_Line1_25, 25);
        Line1.SetIndex(_Line1_26, 26);
        Line1.SetIndex(_Line1_27, 27);
        Line1.SetIndex(_Line1_28, 28);

        foreach (var l in Label5.Values)
            l.DoubleClick += new EventHandler(Label5_DoubleClick);
        foreach (var l in Label6.Values)
            l.DoubleClick += new EventHandler(Label6_DoubleClick);
        _viewModel.Label1_Text.CollectionChanged += Label1_Text_CollectionChanged;
        _viewModel.Label5_Text.CollectionChanged += Label5_Text_CollectionChanged;
        _viewModel.Label7_Text.CollectionChanged += Label7_Text_CollectionChanged;
    }

    private void Label7_Text_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.NewItems)
        {
            Label7[_viewModel.Label7_Text.IndexOf(item)].Text = item.ToString();
        }
    }

    private void Label5_Text_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.NewItems)
        {
            Label5[_viewModel.Label5_Text.IndexOf(item)].Text = item.ToString();
        }
    }

    private void Label1_Text_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.NewItems)
        {
            Label1[_viewModel.Label1_Text.IndexOf(item)].Text = item.ToString();
        }
    }

    private void Label6_DoubleClick(object sender, EventArgs e)
    {
        var index = Label6.GetIndex(sender as Label);
        _viewModel.Label6_DoubleClickCommand.Execute(index);
    }

    private void Label5_DoubleClick(object sender, EventArgs e)
    {
        var index = Label5.GetIndex(sender as Label);
        _viewModel.Label5_DoubleClickCommand.Execute(index);
    }

    private void Namensuch_Load(object sender, EventArgs e)
    {
        _viewModel.FormLoadCommand.Execute(e);
    }

    private void Namensuch_FormClosed(object sender, FormClosedEventArgs e)
    {
        _viewModel.FormClosedCommand.Execute(e);
        lblPersNr.Visible = false;
        lblFamNr.Visible = false;

        chbMale.Text = strings[EUserText.t327];
        chbFemales.Text = strings[EUserText.t326];
        chbFamOnly.Text = strings[EUserText.t325];
        chbSelection.Text = strings[EUserText.t323];
        chbMale2.Text = strings[EUserText.t345];
        chbFemale2.Text = strings[EUserText.t344];
        chbOmitSpouse.Text = strings[EUserText.t324];
        Label10.Text = strings[EUserText.t322];
        lblPredicate.Text = strings[EUserText.t343];
        lblNickName.Text = strings[EUserText.t298];
        btnClose.Text = strings[EUserText.t409];
        btnPersonSheet.Text = strings[EUserText.t243];
        btnFamilySheet.Text = strings[EUserText.t277];
        btnStartSearch.Text = strings[EUserText.t410];
        btnPrintList.Text = strings[EUserText.t310];

        ComboBox2.Items.Clear();
        if (ComboBox2.Text == "")
        {
            ComboBox2.Text = strings[EUserText.t314];
        }
        ComboBox2.MaxDropDownItems = 13;
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t314], (int)EUserText.t314));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t315], (int)EUserText.t315));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t313], (int)EUserText.t313));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t311], (int)EUserText.t311));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t312], (int)EUserText.t312));
        if (_viewModel.xComboBox2AddT308)
        {
            _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t308], (int)EUserText.t308));
        }
        if (_viewModel.xComboBox2AddT309)
        {
            _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t309], (int)EUserText.t309));
        }
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t317], (int)EUserText.t317));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t316], (int)EUserText.t316));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t318], (int)EUserText.t318));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t319], (int)EUserText.t319));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t320], (int)EUserText.t320));
        _ = ComboBox2.Items.Add(new ListItem<int>(strings[EUserText.t321], (int)EUserText.t321));


        float fs;
        if ((fs = _Modul1.Instance.FontSize) > 0f)
        {
            Font = new Font("Arial", fs, FontStyle.Regular);
            List1.Font = new Font("Courier New", fs, FontStyle.Regular);
            Label3.Font = new Font("Courier New", fs, FontStyle.Regular);
            Label4.Font = new Font("Courier New", fs, FontStyle.Regular);
        }

        ComboBox1.Font = Font;
        Text1.Font = Font;
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool Disposing)
    {
        if (Disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(Disposing);
    }

    internal void SetPerson(int value, int an, short schalt)
    {
        _viewModel.SetPerson(value, an, schalt);
    }

    public DialogResult ShowDialog(int personNr, string title, int iFamNr)
    {
        _viewModel.ShowNamensuchDlg(title, personNr, iFamNr);
        Text = title;
        return ShowDialog();
    }

    public void ClearSelection()
    {
        chbMale.Checked = false;
        chbFemales.Checked = false;
        chbFamOnly.Checked = false;
        chbSelection.Checked = false;
        chbFemale2.Checked = false;
        chbMale2.Checked = false;
    }

    internal DialogResult ShowDialog(int personNr)
    {
        return ShowDialog(personNr, "", 0);
    }

    internal void Show(int famInArb)
    {
        _viewModel.ShowNamensuchDlg("", 0, famInArb);
        MainProject.Forms.Namensuch.btnFamilySheet.PerformClick();
        Show();
    }
}
