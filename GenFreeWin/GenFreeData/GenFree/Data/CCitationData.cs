using BaseLib.Helper;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.GenFree.Data;

public class CCitationData : ICitationData
{
    public string this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int iSourceId { get => field; set => field = value; }
    public short iSourceKnd { get => field; set => field = value; }
    public string sSourceTitle { get => field; set => field = value; }
    public string sPage { get => field; set => field = value; }
    public string sEntry { get => field; set => field = value; }
    public string sOriginalText { get => field; set => field = value; }
    public string sComment { get => field; set => field = value; }
  
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        iSourceId = 0;
        iSourceKnd = 0;
        sSourceTitle = string.Empty;
        sPage = string.Empty;
        sEntry = string.Empty;
        sOriginalText = string.Empty;
        sComment = string.Empty;
    }
    /// <summary>
    /// Commits the data. 
    /// </summary>
    /// <param name="iPerFamNr">The i per fam nr.</param>
    /// <param name="eArt">The e art.</param>
    /// <param name="lfNR">The lf nr.</param>
    public void Commit(int iPerFamNr, EEventArt eArt, short lfNR)
    {
        IRecordset DB_SourceLinkTable = DataModul.DB_SourceLinkTable;
        if (iSourceKnd < 3)
        {
            DB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab21);
            DB_SourceLinkTable.Seek("=", iSourceKnd, iPerFamNr, iSourceId);
        }
        else
        {
            DB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab23);
            DB_SourceLinkTable.Seek("=", 3, iPerFamNr, iSourceId, eArt, lfNR);
        }
        if (!DB_SourceLinkTable.NoMatch)
        {
            DB_SourceLinkTable.Edit();
            DB_SourceLinkTable.Fields[SourceLinkFields._3.AsFld()].Value = (sEntry.Trim() + " ").Left(DB_SourceLinkTable.Fields[SourceLinkFields._3.AsFld()].Size);
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Aus)].Value = sPage;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Orig)].Value = sOriginalText;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Kom)].Value = sComment;
            DB_SourceLinkTable.Update();
        }
        else
        {
            DB_SourceLinkTable.AddNew();
            DB_SourceLinkTable.Fields[SourceLinkFields._0.AsFld()].Value = iSourceKnd;
            DB_SourceLinkTable.Fields[SourceLinkFields._1.AsFld()].Value = iPerFamNr;
            DB_SourceLinkTable.Fields[SourceLinkFields._2.AsFld()].Value = iSourceId;
            DB_SourceLinkTable.Fields[SourceLinkFields._3.AsFld()].Value = (sEntry.Trim() + " ").Left(DB_SourceLinkTable.Fields[SourceLinkFields._3.AsFld()].Size);
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Art)].Value = eArt;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.LfNr)].Value = lfNR;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Aus)].Value = sPage;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Orig)].Value = sOriginalText;
            DB_SourceLinkTable.Fields[nameof(SourceLinkFields.Kom)].Value = sComment;
            DB_SourceLinkTable.Update();
        }
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public void FillData(IRecordset dB_Table)
    {
        if (dB_Table == null) throw new ArgumentNullException(nameof(dB_Table), "dB_Table cannot be null.");
        iSourceKnd = (short)dB_Table.Fields[SourceLinkFields._0.AsFld()].AsInt();
        iSourceId = dB_Table.Fields[SourceLinkFields._2.AsFld()].AsInt();
        sEntry = dB_Table.Fields[SourceLinkFields._3.AsFld()].AsString();
        sPage = dB_Table.Fields[nameof(SourceLinkFields.Aus)].AsString();
        sOriginalText = dB_Table.Fields[nameof(SourceLinkFields.Orig)].AsString();
        sComment = dB_Table.Fields[nameof(SourceLinkFields.Kom)].AsString();
    }

    public void SetDBValue(IRecordset dB_Table, Enum[]? asProps)
    {
        if (dB_Table == null) throw new ArgumentNullException(nameof(dB_Table), "dB_Table cannot be null.");
        if (asProps == null || asProps.Length == 0) return;
        foreach (var prop in asProps)
        {
            switch (prop)
            {
                case SourceLinkFields._0:
                    dB_Table.Fields[SourceLinkFields._0.AsFld()].Value = iSourceKnd;
                    break;
                case SourceLinkFields._1:
                    dB_Table.Fields[SourceLinkFields._1.AsFld()].Value = iSourceId;
                    break;
                case SourceLinkFields._2:
                    dB_Table.Fields[SourceLinkFields._2.AsFld()].Value = sEntry;
                    break;
                case SourceLinkFields._3:
                    dB_Table.Fields[SourceLinkFields._3.AsFld()].Value = sPage;
                    break;
                case SourceLinkFields.Aus:
                    dB_Table.Fields[nameof(SourceLinkFields.Aus)].Value = sOriginalText;
                    break;
                case SourceLinkFields.Orig:
                    dB_Table.Fields[nameof(SourceLinkFields.Orig)].Value = sComment;
                    break;
                case SourceLinkFields.Kom:
                    dB_Table.Fields[nameof(SourceLinkFields.Kom)].Value = sComment;
                    break;
            }
        }
    }
}
