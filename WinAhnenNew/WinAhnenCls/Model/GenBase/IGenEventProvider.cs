using GenInterfaces.Interfaces.Genealogic;
using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase;

public interface IGenEventProvider
{
    IGenFact GetGenEvent(object idx, EGenListType lType);
    void SetGenEvent(object idx, EGenListType lType, IGenFact value);
}