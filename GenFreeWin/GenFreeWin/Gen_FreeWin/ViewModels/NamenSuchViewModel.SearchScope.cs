using BaseLib.Helper;
using GenFree.Data;

namespace Gen_FreeWin.ViewModels;

public partial class NamenSuchViewModel
{
    /// <summary>
    /// Executes free search (t316) – calls Zeigfamanl() helper.
    /// </summary>
    private void ExecuteFreeSearch()
    {
        if (Text1_Text == "")
        {
            Text1_Text = "0";
        }
        Zeigfamanl();
    }

    /// <summary>
    /// Executes place family search (t317) – indexes family table by BeaDat and seeks.
    /// </summary>
    private void ExecutePlaceSearch()
    {
        DataModul.DB_FamilyTable.Index = nameof(FamilyIndex.BeaDat);
        if (Text1_Text == "")
        {
            Text1_Text = "0";
        }
        DataModul.DB_PersonTable.Index = nameof(PersonIndex.PerNr);
        DataModul.DB_PersonTable.Seek(">=", Text1_Text.AsInt());
        Zeigfamanl2();
    }

    /// <summary>
    /// Executes phonetic search (t318) – Koelner_Phonetic algorithm on DSB_SearchTable.
    /// </summary>
    private void ExecutePhoneticSearch()
    {
        DataModul.DSB_SearchTable.Index = "K_Phonsuch";
        DataModul.DSB_SearchTable.Seek(">=", Module2.Koelner_Phonetic(Text1_Text), 0);
        if (!OmitSpouse_Checked)
        {
            Zeigfam();
        }
        else
        {
            Zeig(DataModul.DSB_SearchTable);
        }
    }

    /// <summary>
    /// Executes SoundEx search (t320) – SoundEx algorithm on DSB_SearchTable.
    /// </summary>
    private void ExecuteSoundExSearch()
    {
        DataModul.DSB_SearchTable.Index = "Soundsuch";
        DataModul.DSB_SearchTable.Seek(">=", Module2.GetSoundEx(Text1_Text), 0);
        if (!OmitSpouse_Checked)
        {
            Zeigfam();
        }
        else
        {
            Zeig(DataModul.DSB_SearchTable);
        }
    }

    /// <summary>
    /// Executes Leit search (t321) – exact match on DSB_SearchTable by Leitsuch index.
    /// </summary>
    private void ExecuteLeitSearch()
    {
        DataModul.DSB_SearchTable.Index = "Leitsuch";
        DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
        if (!OmitSpouse_Checked)
        {
            Zeigfam();
        }
        else
        {
            Zeig(DataModul.DSB_SearchTable);
        }
    }

    /// <summary>
    /// Executes person number search (t315) – Persuch index on DSB_SearchTable.
    /// </summary>
    private void ExecutePersonSearch()
    {
        DataModul.DSB_SearchTable.Index = "Persuch";
        DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
        Zeigfamdat();
    }

    /// <summary>
    /// Executes object number search (t313) – by Nummer index on DSB_SearchTable.
    /// </summary>
    private void ExecuteNumberSearch()
    {
        DataModul.DSB_SearchTable.Index = "Nummer";
        if (Text1_Text == "")
        {
            Text1_Text = "0";
        }
        DataModul.DSB_SearchTable.Seek(">=", Text1_Text.AsInt());
        if (!OmitSpouse_Checked)
        {
            Zeigfam();
        }
        else
        {
            Zeig(DataModul.DSB_SearchTable);
        }
    }

    /// <summary>
    /// Executes alias name search (t319) – Aliassuch index on DSB_SearchTable.
    /// </summary>
    private void ExecuteAliasSearch()
    {
        DataModul.DSB_SearchTable.Index = "Aliassuch";
        DataModul.DSB_SearchTable.Seek(">=", Text1_Text, 0);
        if (!OmitSpouse_Checked)
        {
            Zeigfam();
        }
        else
        {
            Zeig(DataModul.DSB_SearchTable);
        }
    }
}
