using GenFree.Data;

namespace GenFree.Interfaces.Data;

public interface ICitationData: IHasIRecordset , IHasID<(short , int , EEventArt , short )>, IHasPropEnum<ESourceLinkProp>
{
    int iQuNr { get; set; } // Source ID
    short iLfdNr { get; set; } // Link number, used for multiple links to the same source
    short iLinkType { get; set; } // Kind of the Source
    string sSourceTitle { get; set; } // Source title 
    string sPage { get; set; } // Page number or identifier
    string sEntry { get; set; } // Citation entry-title or description
    string sOriginalText { get; set; } // Citation text
    string sComment { get; set; } // Notes or comments about the citation
    EEventArt eArt { get; set; }
    int iPerFamNr { get; set; }

    void Clear(); // Method to clear the citation data

    void Commit(int iPerFamNr, EEventArt eArt, short lfNR);
}
