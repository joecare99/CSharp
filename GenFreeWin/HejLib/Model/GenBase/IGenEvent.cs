namespace WinAhnenCls.Model.GenBase
{
    public interface IGenEvent : IGenFact
    {
        string Date { get; set; }
        string Place { get; set; }
        EEventType EventType { get; set; }
    }
}
