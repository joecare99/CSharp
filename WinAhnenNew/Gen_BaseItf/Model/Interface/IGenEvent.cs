using Gen_BaseItf.Model.Data;

namespace Gen_BaseItf.Model.Interface;

public interface IGenEvent : IGenFact
{
    string Date { get; set; }
    string Place { get; set; }
    EEventType EventType { get; set; }
}
