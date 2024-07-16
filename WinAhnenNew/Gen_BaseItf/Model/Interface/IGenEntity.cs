namespace Gen_BaseItf.Model.Interface;

public interface IGenEntity : IGenData
{
    public interface _IEvents
    {
        IGenEvent this[object Idx] { get; set; }
    }

    int EventCount { get; }
    _IEvents Events { get; }
}
