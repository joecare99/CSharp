using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase
{
    public interface IGenIndProvider
    {
        IGenIndividual GetGenIndivid(object idx, EGenListType lType);
        void SetGenIndivid(object idx, EGenListType lType, IGenIndividual value);
    }
}