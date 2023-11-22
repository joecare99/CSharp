using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase
{
    public interface IGenEventProvider
    {
        IGenEvent GetGenEvent(object idx, EGenListType lType);
        void SetGenEvent(object idx, EGenListType lType, IGenEvent value);
    }
}