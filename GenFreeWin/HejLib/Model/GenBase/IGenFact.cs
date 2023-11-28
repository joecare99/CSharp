namespace WinAhnenCls.Model.GenBase
{
    public interface IGenFact : IGenData
    {
        IGenData Source { get; set; }
        int FType { get; set; }

    }
}
