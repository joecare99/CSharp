using BaseLib.Helper;
using Gen_FreeWin;
using GenFree.Data;
using GenFree.Helper;
using GenFree.ViewModels.Interfaces;
using MVVM.ViewModel;
using System;
using System.Collections;
using System.Windows.Forms;

namespace GenFree.ViewModels;

public partial class FrmQuellSrchViewModel:BaseViewModelCT, IFrmQuellSrchViewModel
{
    IQuellVerwViewModel _parent;
    IInteraction Interaction => _parent.Interaction;
    Quellverw View => (Quellverw)_parent.View;
    public void btnClose_Click(object sender, EventArgs e)
    {
        View.LagerFrame.Visible = false;
        View.frmSrch_edtSearch.Text = "";
        View.frmSrch_Label5.Text = "";
        View.frmSrch_ListBox1.Items.Clear();
       
        long Satznr = View._Label1_13.Tag.AsInt();
        _parent.Les1(Satznr, Rich: false);
    }

    public void ListBox1_DoubleClick(object sender, EventArgs e)
    {
        MyListItem myListItem = (MyListItem)View.frmSrch_ListBox1.SelectedItem;
        long Satznr = View._Label1_13.Tag.AsInt();
        DataModul.DB_RepoTab.Index = "Dop";
        DataModul.DB_RepoTab.Seek("=", Satznr, myListItem.ItemData.ToString());
        if (DataModul.DB_RepoTab.NoMatch)
        {
            DataModul.DB_RepoTab.AddNew();
            DataModul.DB_RepoTab.Fields["Repo"].Value = myListItem.ItemData.ToString();
            DataModul.DB_RepoTab.Fields["Quelle"].Value = Satznr;
            DataModul.DB_RepoTab.Update();
        }
        View.LagerFrame.Visible = false;
        View.frmSrch_edtSearch.Text = "";
        View.frmSrch_Label5.Text = "";
        View.frmSrch_ListBox1.Items.Clear();
        _parent.Les1(Satznr, Rich: false);
    }

    public void edtSearch_TextChanged(object sender, EventArgs e)
    {
        IList listBox = View.frmSrch_ListBox1.Items;
        GenFree.Interfaces.DB.IRecordset dB_RepoTable = DataModul.DB_RepoTable;
     
        listBox.Clear();
        dB_RepoTable.Index = "Name";
        dB_RepoTable.Seek(">=", View.frmSrch_edtSearch.Text);
        while (!dB_RepoTable.EOF)
        {
            var Repo_sName = dB_RepoTable.Fields[RepoFields.Name].AsString();
            int Repo_iNr = dB_RepoTable.Fields[RepoFields.Nr].AsInt();
            string Repo_sOrt = dB_RepoTable.Fields[RepoFields.Ort].AsString();
            listBox.Add(new ListItem<int>((Repo_sName + " " + Repo_sOrt), Repo_iNr));
            dB_RepoTable.MoveNext();
        }
    }

    public void btnDeleteEntry_Click(object sender, EventArgs e)
    {
        MyListItem myListItem = (MyListItem)View.ComboBox1.SelectedItem;
        DataModul.DB_RepoTab.Index = "Dop";
        DataModul.DB_RepoTab.Seek("=", View._Label1_13.Tag, View.ComboBox1.Tag);
        if (!DataModul.DB_RepoTab.NoMatch)
        {
            if (Interaction.MsgBox("Verbindung der Quelle zu dieser Lageradresse wirklich löschen?", title: "Warnung", mb: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Warning) == DialogResult.Cancel)
            {
                return;
            }
            DataModul.DB_RepoTab.Delete();
        }
        View.frmSrch_Label5.Visible = false;
        View.frmSrch_btnDeleteEntry.Visible = false;
    }
}
