using BaseLib.Helper;
using GenFreeWin.Models;
using GenFreeWin.ViewModels.Interfaces;
using GenFreeWin.Views;
using GenFree;
using GenFree.Interfaces.Sys;
using GenFreeWin.Views;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace GenFreeWin;

[DesignerGenerated]
public class Besitz : Form
{
    private static List<WeakReference> __ENCList = new();
    private readonly IBesitzViewModel _viewModel;
    private IModul1 Modul1 => _Modul1.Instance;

    [AccessedThroughProperty(nameof(ListBox1))]
    private ListBox _ListBox1;


    [AccessedThroughProperty(nameof(ListBox2))]
    private ListBox _ListBox2;


    [AccessedThroughProperty(nameof(Button3))]
    private Button _Button3;

    [AccessedThroughProperty(nameof(Button4))]
    private Button _Button4;

    [AccessedThroughProperty(nameof(Button5))]
    private Button _Button5;


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual ListBox ListBox1
    {
        [DebuggerNonUserCode]
        get => _ListBox1;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = ListBox1_SelectedIndexChanged;
            EventHandler value3 = ListBox1_DoubleClick;
            if (_ListBox1 != null)
            {
                _ListBox1.SelectedIndexChanged -= value2;
                _ListBox1.DoubleClick -= value3;
            }
            _ListBox1 = value;
            if (_ListBox1 != null)
            {
                _ListBox1.SelectedIndexChanged += value2;
                _ListBox1.DoubleClick += value3;
            }
        }
    }

    internal Label Label2;
    internal Label Label3;
    internal Label Label4;
    internal Label Label5;
    internal Label Label6;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal virtual ListBox ListBox2
    {
        [DebuggerNonUserCode]
        get => _ListBox2;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = ListBox2_DoubleClick;
            if (_ListBox2 != null)
            {
                _ListBox2.DoubleClick -= value2;
            }
            _ListBox2 = value;
            if (_ListBox2 != null)
            {
                _ListBox2.DoubleClick += value2;
            }
        }
    }

    internal Label Label7;
    internal Label Label8;
    internal Label Label9;
    internal Label Label10;
    internal Label Label11;
    internal Label Label12;
    internal Label Label13;
    internal Label Label14;
    internal Label Label15;
    internal Label Label16;
    internal Label Label19;
    internal virtual Button Button3
    {
        [DebuggerNonUserCode]
        get => _Button3;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button3_Click;
            if (_Button3 != null)
            {
                _Button3.Click -= value2;
            }
            _Button3 = value;
            if (_Button3 != null)
            {
                _Button3.Click += value2;
            }
        }
    }

    internal virtual Button Button4
    {
        [DebuggerNonUserCode]
        get => _Button4;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button4_Click;
            if (_Button4 != null)
            {
                _Button4.Click -= value2;
            }
            _Button4 = value;
            if (_Button4 != null)
            {
                _Button4.Click += value2;
            }
        }
    }

    internal virtual Button Button5
    {
        [DebuggerNonUserCode]
        get => _Button5;
        [MethodImpl(MethodImplOptions.Synchronized)]
        [DebuggerNonUserCode]
        set
        {
            EventHandler value2 = Button5_Click;
            if (_Button5 != null)
            {
                _Button5.Click -= value2;
            }
            _Button5 = value;
            if (_Button5 != null)
            {
                _Button5.Click += value2;
            }
        }
    }

    internal Label Label20;
    internal Label Label17;
    internal Label Label21;
    internal Label Label18;
    internal Label Label1;
    internal Label Label23;
    internal Label Label22;
    internal Label Label24;
    internal Label Label25;
    private string Modul1_Mldg;

    [DebuggerNonUserCode]
    public Besitz() : this(IoC.GetRequiredService<IBesitzViewModel>())
    {
    }

    [DebuggerNonUserCode]
    internal Besitz(IBesitzViewModel viewModel)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        Load += Besitz_Load;
        _viewModel.RequestClose = Close;
        _viewModel.ShowInformation = message => _ = Menue.Default.MsgBox(message, title: string.Empty, icon: MessageBoxIcon.Information);
        _viewModel.PropertyChanged += BesitzViewModel_PropertyChanged;
        _viewModel.AkteItems.CollectionChanged += AkteItems_CollectionChanged;
        _viewModel.EntryItems.CollectionChanged += EntryItems_CollectionChanged;
        lock (__ENCList)
        {
            __ENCList.Add(new WeakReference(this));
        }
        InitializeComponent();
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing)
            {
                Load -= Besitz_Load;
                _viewModel.PropertyChanged -= BesitzViewModel_PropertyChanged;
                _viewModel.AkteItems.CollectionChanged -= AkteItems_CollectionChanged;
                _viewModel.EntryItems.CollectionChanged -= EntryItems_CollectionChanged;
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    [DebuggerStepThrough]
    private void InitializeComponent()
    {
        ListBox1 = new ListBox();
        Label2 = new Label();
        Label3 = new Label();
        Label4 = new Label();
        Label5 = new Label();
        Label6 = new Label();
        ListBox2 = new ListBox();
        Label7 = new Label();
        Label8 = new Label();
        Label9 = new Label();
        Label10 = new Label();
        Label11 = new Label();
        Label12 = new Label();
        Label13 = new Label();
        Label14 = new Label();
        Label15 = new Label();
        Label16 = new Label();
        Label19 = new Label();
        Button3 = new Button();
        Button4 = new Button();
        Button5 = new Button();
        Label20 = new Label();
        Label17 = new Label();
        Label21 = new Label();
        Label18 = new Label();
        Label1 = new Label();
        Label23 = new Label();
        Label22 = new Label();
        Label24 = new Label();
        Label25 = new Label();
        SuspendLayout();
        ListBox1.FormattingEnabled = true;
        ListBox1.ItemHeight = 17;
        ListBox1.Location = new Point(15, 146);
        ListBox1.Name = "ListBox1";
        ListBox1.Size = new Size(488, 208);
        ListBox1.Sorted = true;
        ListBox1.TabIndex = 3;
        ListBox1.Visible = false;
        Label2.BackColor = Color.FromArgb(224, 224, 224);
        Label2.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label2.Location = new Point(70, 0);
        Label2.Name = "lblState";
        Label2.Size = new Size(96, 21);
        Label2.TabIndex = 4;
        Label3.BackColor = Color.FromArgb(224, 224, 224);
        Label3.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label3.Location = new Point(306, 0);
        Label3.Name = "lblDisplayHint";
        Label3.Size = new Size(420, 21);
        Label3.TabIndex = 5;
        Label4.BackColor = Color.FromArgb(224, 224, 224);
        Label4.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label4.Location = new Point(131, 30);
        Label4.Name = "lblSearch";
        Label4.Size = new Size(361, 21);
        Label4.TabIndex = 6;
        Label4.TextAlign = ContentAlignment.MiddleLeft;
        Label5.BackColor = Color.FromArgb(224, 224, 224);
        Label5.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label5.Location = new Point(814, 0);
        Label5.Name = "lblSorting";
        Label5.Size = new Size(166, 22);
        Label5.TabIndex = 7;
        Label5.TextAlign = ContentAlignment.MiddleLeft;
        Label6.AutoSize = true;
        Label6.Location = new Point(1184, 9);
        Label6.Name = "Label6";
        Label6.Size = new Size(51, 17);
        Label6.TabIndex = 9;
        Label6.Text = "Label6";
        ListBox2.FormattingEnabled = true;
        ListBox2.ItemHeight = 17;
        ListBox2.Location = new Point(12, 146);
        ListBox2.Name = "ListBox2";
        ListBox2.Size = new Size(489, 208);
        ListBox2.TabIndex = 10;
        ListBox2.Visible = false;
        Label7.BackColor = Color.FromArgb(224, 224, 224);
        Label7.Location = new Point(13, -2);
        Label7.Name = "lblRemark";
        Label7.Size = new Size(51, 21);
        Label7.TabIndex = 11;
        Label7.Text = "Akte:";
        Label7.TextAlign = ContentAlignment.MiddleRight;
        Label8.BackColor = Color.FromArgb(224, 224, 224);
        Label8.ForeColor = Color.Black;
        Label8.Location = new Point(172, 0);
        Label8.Name = "Label8";
        Label8.Size = new Size(128, 21);
        Label8.TabIndex = 12;
        Label8.Text = "Verwaltungsort:";
        Label8.TextAlign = ContentAlignment.MiddleRight;
        Label9.BackColor = Color.FromArgb(224, 224, 224);
        Label9.ForeColor = Color.Black;
        Label9.Location = new Point(13, 30);
        Label9.Name = "Label9";
        Label9.Size = new Size(112, 21);
        Label9.TabIndex = 13;
        Label9.Text = "Bauernschaft:";
        Label9.TextAlign = ContentAlignment.MiddleRight;
        Label10.BackColor = Color.FromArgb(224, 224, 224);
        Label10.ForeColor = Color.Black;
        Label10.Location = new Point(732, 0);
        Label10.Name = "Label10";
        Label10.Size = new Size(76, 21);
        Label10.TabIndex = 14;
        Label10.Text = "Hofklasse:";
        Label10.TextAlign = ContentAlignment.MiddleRight;
        Label11.BackColor = Color.FromArgb(224, 224, 224);
        Label11.ForeColor = Color.Black;
        Label11.Location = new Point(12, 60);
        Label11.Name = "Label11";
        Label11.Size = new Size(52, 21);
        Label11.TabIndex = 15;
        Label11.Text = "Jahr:";
        Label11.TextAlign = ContentAlignment.MiddleRight;
        Label12.BackColor = Color.FromArgb(224, 224, 224);
        Label12.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label12.Location = new Point(70, 60);
        Label12.Name = "Label12";
        Label12.Size = new Size(65, 21);
        Label12.TabIndex = 16;
        Label12.TextAlign = ContentAlignment.MiddleLeft;
        Label13.BackColor = Color.FromArgb(224, 224, 224);
        Label13.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label13.Location = new Point(198, 59);
        Label13.Name = "lblPredicate";
        Label13.Size = new Size(65, 21);
        Label13.TabIndex = 17;
        Label13.TextAlign = ContentAlignment.MiddleLeft;
        Label14.BackColor = Color.FromArgb(224, 224, 224);
        Label14.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label14.Location = new Point(355, 59);
        Label14.Name = "lblNickname";
        Label14.Size = new Size(71, 21);
        Label14.TabIndex = 18;
        Label14.TextAlign = ContentAlignment.MiddleLeft;
        Label15.BackColor = Color.FromArgb(224, 224, 224);
        Label15.Location = new Point(269, 60);
        Label15.Name = "Label15";
        Label15.Size = new Size(80, 21);
        Label15.TabIndex = 19;
        Label15.Text = "Abgängig:";
        Label15.TextAlign = ContentAlignment.MiddleRight;
        Label16.BackColor = Color.FromArgb(224, 224, 224);
        Label16.Location = new Point(141, 59);
        Label16.Name = "Label16";
        Label16.Size = new Size(51, 21);
        Label16.TabIndex = 20;
        Label16.Text = "Erbaut:";
        Label16.TextAlign = ContentAlignment.MiddleRight;
        Label19.BackColor = Color.FromArgb(224, 224, 224);
        Label19.ForeColor = Color.Black;
        Label19.Location = new Point(12, 91);
        Label19.Name = "Label19";
        Label19.Size = new Size(480, 21);
        Label19.TabIndex = 24;
        Label19.Text = "Name:";
        Label19.TextAlign = ContentAlignment.MiddleCenter;
        Button3.Location = new Point(510, 329);
        Button3.Name = "btnReenter";
        Button3.Size = new Size(105, 23);
        Button3.TabIndex = 26;
        Button3.Text = "löschen";
        Button3.UseVisualStyleBackColor = true;
        Button3.Visible = false;
        Button4.Location = new Point(621, 329);
        Button4.Name = "btnClose";
        Button4.Size = new Size(105, 23);
        Button4.TabIndex = 27;
        Button4.Text = "abbrechen";
        Button4.UseVisualStyleBackColor = true;
        Button5.Location = new Point(826, 331);
        Button5.Name = "btnHometown";
        Button5.Size = new Size(105, 23);
        Button5.TabIndex = 28;
        Button5.Text = "fertig";
        Button5.UseVisualStyleBackColor = true;
        Button5.Visible = false;
        Label20.BackColor = Color.FromArgb(224, 224, 224);
        Label20.ForeColor = Color.Black;
        Label20.Location = new Point(507, 81);
        Label20.Name = "Label20";
        Label20.Size = new Size(480, 21);
        Label20.TabIndex = 25;
        Label20.Text = "Art des Gebäudes:";
        Label20.TextAlign = ContentAlignment.MiddleCenter;
        Label17.BackColor = Color.Red;
        Label17.Location = new Point(-1, 52);
        Label17.Name = "Label17";
        Label17.Size = new Size(1000, 2);
        Label17.TabIndex = 29;
        Label21.AutoSize = true;
        Label21.Location = new Point(639, 59);
        Label21.Name = "Label21";
        Label21.Size = new Size(59, 17);
        Label21.TabIndex = 30;
        Label21.Text = "Label21";
        Label21.Visible = false;
        Label18.BackColor = Color.FromArgb(224, 224, 224);
        Label18.Location = new Point(12, 112);
        Label18.Name = "Label18";
        Label18.Size = new Size(480, 197);
        Label18.TabIndex = 32;
        Label1.BackColor = Color.FromArgb(224, 224, 224);
        Label1.Location = new Point(507, 102);
        Label1.Name = "Label1";
        Label1.Size = new Size(480, 197);
        Label1.TabIndex = 33;
        Label23.AutoSize = true;
        Label23.BackColor = Color.Yellow;
        Label23.Location = new Point(28, 124);
        Label23.Name = "Label23";
        Label23.Size = new Size(146, 17);
        Label23.TabIndex = 34;
        Label23.Text = "Eintrag (Jahr) wählen";
        Label23.Visible = false;
        Label22.AutoSize = true;
        Label22.BackColor = Color.Yellow;
        Label22.Location = new Point(28, 124);
        Label22.Name = "Label22";
        Label22.Size = new Size(235, 17);
        Label22.TabIndex = 35;
        Label22.Text = "Vorhandene Akten; Bitte auswählen";
        Label22.Visible = false;
        Label24.BackColor = Color.FromArgb(224, 224, 224);
        Label24.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label24.Location = new Point(507, 31);
        Label24.Name = "Label24";
        Label24.Size = new Size(203, 21);
        Label24.TabIndex = 36;
        Label24.TextAlign = ContentAlignment.MiddleLeft;
        Label25.BackColor = Color.FromArgb(224, 224, 224);
        Label25.Font = new Font("Arial", 8.5f, FontStyle.Bold, GraphicsUnit.Point, 0);
        Label25.Location = new Point(716, 30);
        Label25.Name = "Label25";
        Label25.Size = new Size(264, 21);
        Label25.TabIndex = 37;
        Label25.TextAlign = ContentAlignment.MiddleLeft;
        AutoScaleDimensions = new SizeF(8f, 17f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(992, 357);
        Controls.Add(Label25);
        Controls.Add(Label24);
        Controls.Add(Label22);
        Controls.Add(Label23);
        Controls.Add(Label1);
        Controls.Add(Label21);
        Controls.Add(Label17);
        Controls.Add(Label20);
        Controls.Add(Button5);
        Controls.Add(Button4);
        Controls.Add(Button3);
        Controls.Add(ListBox1);
        Controls.Add(ListBox2);
        Controls.Add(Label19);
        Controls.Add(Label16);
        Controls.Add(Label15);
        Controls.Add(Label14);
        Controls.Add(Label13);
        Controls.Add(Label12);
        Controls.Add(Label11);
        Controls.Add(Label10);
        Controls.Add(Label9);
        Controls.Add(Label8);
        Controls.Add(Label7);
        Controls.Add(Label6);
        Controls.Add(Label5);
        Controls.Add(Label4);
        Controls.Add(Label3);
        Controls.Add(Label2);
        Controls.Add(Label18);
        Font = new Font("Arial", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Margin = new Padding(4);
        Name = "Besitz";
        SizeGripStyle = SizeGripStyle.Hide;
        StartPosition = FormStartPosition.Manual;
        Text = "Besitz";
        ResumeLayout(false);
        PerformLayout();
    }

    private void BesitzViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ApplyViewModel();
    }

    private void AkteItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SynchronizeListBoxItems(ListBox1, _viewModel.AkteItems);
    }

    private void EntryItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SynchronizeListBoxItems(ListBox2, _viewModel.EntryItems);
    }

    private void Besitz_Load(object sender, EventArgs e)
    {
        ApplyFonts();
        Top = Personen.Instance.btnProperty.Top;
        _viewModel.Load(Modul1.Ubg, Personen.Instance.PersonNr);
        ApplyViewModel();
    }

    private void ApplyFonts()
    {
        Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label3.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label24.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label25.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label4.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label5.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label12.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label13.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        Label14.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        ListBox1.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
        ListBox2.Font = new Font("Arial", Modul1.FontSize, FontStyle.Regular);
    }

    private void ApplyViewModel()
    {
        Label2.Text = _viewModel.AkteText;
        Label3.Text = _viewModel.VerwaltungsortText;
        Label4.Text = _viewModel.BauernschaftText;
        Label5.Text = _viewModel.HofklasseText;
        Label6.Text = _viewModel.AkteText;
        Label12.Text = _viewModel.JahrText;
        Label13.Text = _viewModel.ErbautText;
        Label14.Text = _viewModel.AbgaengigText;
        Label18.Text = _viewModel.NameText;
        Label1.Text = _viewModel.GebaeudeartText;
        Label24.Text = _viewModel.FlurText;
        Label25.Text = _viewModel.ParzelleText;
        Label22.Text = _viewModel.AkteHintText;
        Label23.Text = _viewModel.EntryHintText;

        ListBox1.Visible = _viewModel.AkteSelectionVisible;
        ListBox2.Visible = _viewModel.EntrySelectionVisible;
        Label22.Visible = _viewModel.AkteHintVisible;
        Label23.Visible = _viewModel.EntryHintVisible;
        Button3.Visible = _viewModel.DeleteVisible;
        Button4.Visible = _viewModel.CancelVisible;
        Button5.Visible = _viewModel.ConfirmVisible;

        SynchronizeListBoxItems(ListBox1, _viewModel.AkteItems);
        SynchronizeListBoxItems(ListBox2, _viewModel.EntryItems);
    }

    private static void SynchronizeListBoxItems<T>(ListBox listBox, IList<T> source) where T : class
    {
        if (source.Count == 0)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.Items.Clear();
            }

            return;
        }

        for (int index = listBox.Items.Count - 1; index >= 0; index--)
        {
            if (listBox.Items[index] is not T existingItem || !source.Contains(existingItem))
            {
                listBox.Items.RemoveAt(index);
            }
        }

        foreach (var item in source)
        {
            if (!listBox.Items.Contains(item))
            {
                listBox.Items.Add(item);
            }
        }
    }

    private void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        _viewModel.SelectAkte(ListBox1.SelectedItem as BesitzAkteListItem);
        ApplyViewModel();
    }

    private void ListBox2_DoubleClick(object sender, EventArgs e)
    {
        _viewModel.SelectEntry(ListBox2.SelectedItem as BesitzEntryListItem);
        ApplyViewModel();
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        _viewModel.ConfirmSelection();
    }

    private void Button4_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Button3_Click(object sender, EventArgs e)
    {
        _viewModel.RemoveSelection();
    }

    private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    internal DialogResult ShowDialog(int v)
    {
        throw new NotImplementedException();
    }
}
