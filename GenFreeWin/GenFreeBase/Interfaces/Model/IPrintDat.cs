using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Model;

/// <summary>
/// Interface for print-specific data and methods used only in Druck project.
/// Accessed via IModul1.PrintDat property.
/// </summary>
public interface IPrintDat
{
    /// <summary>Gets or sets the superscript offset for citations.</summary>
    int Hoch { get; set; }

    /// <summary>Gets or sets the count of remarks/citations.</summary>
    int BemZahl { get; set; }

    /// <summary>Gets the remarks/citations container.</summary>
    IList<string> KontBem { get; }

    /// <summary>Gets the temporary storage for Kont values.</summary>
    IList<string> KontSP { get; }

    /// <summary>Gets the temporary storage for Kont1 values.</summary>
    IList<string> KontSP1 { get; }

    /// <summary>Gets or sets the family save variable.</summary>
    int FamSp { get; set; }

    /// <summary>Gets or sets the godparent flag.</summary>
    bool Pat { get; set; }

    /// <summary>Gets or sets the flag switch for printing.</summary>
    int Flagsch { get; set; }

    /// <summary>Gets or sets the ancestor number length.</summary>
    byte KonLen { get; set; }

    /// <summary>Gets or sets general yes/no flag.</summary>
    int Ja { get; set; }

    /// <summary>Gets or sets the sequence number.</summary>
    byte LfNR { get; set; }

    /// <summary>Removes trailing newlines and spaces.</summary>
    string Retweg(string sText);

    /// <summary>Adds person to name index.</summary>
    void Namenindex(long lAhne);

    /// <summary>Reads person source/citation data.</summary>
    void PerQu(ref int iFamPer);

    /// <summary>Reads source data for date.</summary>
    void QuellenDatum(ref int iNr, EEventArt eArt, ref short iLfNR);

    /// <summary>Reads source data with dot notation.</summary>
    void QuelledotnDatum(ref int iNr, EEventArt eArt, ref short iLfNR);

    /// <summary>Reads family residence data.</summary>
    void FWohn(EEventArt eArt, ref short iListart);

    /// <summary>Reads witness data for person.</summary>
    void Zeuge_Bei(int iPersInArb, ref short iListart);
}
