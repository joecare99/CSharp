using GenFree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Interfaces.Data;

public interface ICitationData: IHasIRecordset 
{
    [Obsolete("use named properties")]
    string this[int index] { get; set; } // Indexer to access citation data by index
    int iSourceId { get; set; } // Source ID
    short iSourceKnd { get; set; } // Kind of the Source
    string sSourceTitle { get; set; } // Source title 
    string sPage { get; set; } // Page number or identifier
    string sEntry { get; set; } // Citation entry-title or description
    string sOriginalText { get; set; } // Citation text
    string sComment { get; set; } // Notes or comments about the citation

    void Clear(); // Method to clear the citation data

    void Commit(int iPerFamNr, EEventArt eArt, short lfNR);
}
