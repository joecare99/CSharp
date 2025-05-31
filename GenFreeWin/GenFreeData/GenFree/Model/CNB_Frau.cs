//using DAO;
using GenFree.Interfaces.DB;
using GenFree.Interfaces.Model;
using GenFree.Model;

namespace GenFree.GenFree.Model;

public class CNB_Frau : CUsesRecordSet<int>, INB_Frau
{
    protected override string __keyIndex => throw new System.NotImplementedException();

    protected override IRecordset _db_Table => throw new System.NotImplementedException();

    public override IRecordset? Seek(int key, out bool xBreak)
    {
        throw new System.NotImplementedException();
    }

    protected override int GetID(IRecordset recordset)
    {
        throw new System.NotImplementedException();
    }
}