using BaseLib.Helper;
using GenFree.Data;
using GenFree.Helper;
using GenFree.Interfaces.Data;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Sys;
using GenFree.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data;

public class CCitationData : CSourceLinkData, ICitationData
{
    public CCitationData(IRecordset db_Table, bool xNoInit = false) : base(db_Table, xNoInit)
    {
    }

    public string sSourceTitle { get => field; set => field = value; } = "";


    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        iQuNr = 0;
        iLinkType = 0;
        sSourceTitle = string.Empty;
        sPage = string.Empty;
        eArt = EEventArt.eA_Unknown;
        sEntry = string.Empty;
        sOriginalText = string.Empty;
        sComment = string.Empty;
        ClearChangedProps();
    }
    /// <summary>
    /// Commits the data. 
    /// </summary>
    /// <param name="iPerFamNr">The i per fam nr.</param>
    /// <param name="eArt">The e art.</param>
    /// <param name="lfNR">The lf nr.</param>
    public void Commit(int iPerFamNr, EEventArt eArt, short lfNR)
    {
        IRecordset DB_SourceLinkTable = _db_Table;
        if (iLinkType < 3)
        {
            DB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab21);
            DB_SourceLinkTable.Seek("=", iLinkType, iPerFamNr, iQuNr);
        }
        else
        {
            DB_SourceLinkTable.Index = nameof(SourceLinkIndex.Tab23);
            DB_SourceLinkTable.Seek("=", 3, iPerFamNr, iQuNr, eArt, lfNR);
        }
        if (!DB_SourceLinkTable.NoMatch)
        {
            DB_SourceLinkTable.Edit();
            DB_SourceLinkTable.Fields[SourceLinkFields._4].Value = (sEntry.Trim() + " ").Left(DB_SourceLinkTable.Fields[SourceLinkFields._3].Size);
            DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value = sPage;
            DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value = sOriginalText;
            DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value = sComment;
            DB_SourceLinkTable.Update();
        }
        else
        {
            DB_SourceLinkTable.AddNew();
            DB_SourceLinkTable.Fields[SourceLinkFields._1].Value = iLinkType;
            DB_SourceLinkTable.Fields[SourceLinkFields._2].Value = iPerFamNr;
            DB_SourceLinkTable.Fields[SourceLinkFields._3].Value = iQuNr;
            DB_SourceLinkTable.Fields[SourceLinkFields._4].Value = (sEntry.Trim() + " ").Left(DB_SourceLinkTable.Fields[SourceLinkFields._3].Size);
            DB_SourceLinkTable.Fields[SourceLinkFields.Art].Value = eArt;
            DB_SourceLinkTable.Fields[SourceLinkFields.LfNr].Value = lfNR;
            DB_SourceLinkTable.Fields[SourceLinkFields.Aus].Value = sPage;
            DB_SourceLinkTable.Fields[SourceLinkFields.Orig].Value = sOriginalText;
            DB_SourceLinkTable.Fields[SourceLinkFields.Kom].Value = sComment;
            DB_SourceLinkTable.Update();
        }
    }


    public override Type GetPropType(ESourceLinkProp prop)
    {
        return prop switch
        {
            ESourceLinkProp.sSourceTitle => typeof(string),
            _ => base.GetPropType(prop),
        };
    }

    public override object? GetPropValue(ESourceLinkProp prop)
    {
        return prop switch
        {
            ESourceLinkProp.sSourceTitle => sSourceTitle,
            _ => base.GetPropValue(prop),
        };
    }

    public override void SetPropValue(ESourceLinkProp prop, object value)
    {
        if (prop == ESourceLinkProp.sSourceTitle)
        {
            sSourceTitle = value.AsString();
        }
        else
            base.SetPropValue(prop, value);
    }
}
