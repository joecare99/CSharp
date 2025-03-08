using GenInterfaces.Interfaces.Genealogic;
using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase;

public interface IGenIndProvider
{
    IGenPerson GetGenIndivid(object idx, EGenListType lType);
    void SetGenIndivid(object idx, EGenListType lType, IGenPerson value);
}