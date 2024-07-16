namespace Gen_BaseItf.Model.Interface;

public interface IGenFact : IGenData
{
    public interface _ILinks
    {
        IGenEntity this[object Idx] { get; set; }
    } 

    IGenData Source { get; set; }
    int FType { get; set; }

    _ILinks Links { get; }
}
