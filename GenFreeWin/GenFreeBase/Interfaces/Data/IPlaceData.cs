//using DAO;

//using DAO;
using GenFree.Data;

namespace GenFree.Interfaces.Data;

public interface IPlaceData : IHasPropEnum<EPlaceProp>, IHasID<int>, IHasIRecordset
{
    int iLand { get; set; }
    int iStaat { get; set; }
    int iKreis { get; set; }
    int iOrt { get; set; }
    int iOrtsteil { get; set; }
    string sStaatk { get; set; }
    string sPLZ { get; set; }
    string sTerr { get; set; }
    string sLoc { get; set; }
    string sL { get; set; }
    string sB { get; set; }
    string sBem { get; set; }
    string sZusatz { get; set; }
    string sGOV { get; set; }
    string sPolName { get; set; }
    int ig { get; }
    string sOrt { get; }
    string sOrtsteil { get; }
    string sKreis { get; }
    string sLand { get; }
    string sStaat { get; }
}