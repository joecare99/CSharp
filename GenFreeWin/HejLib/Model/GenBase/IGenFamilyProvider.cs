using WinAhnenCls.Model.HejInd;

namespace WinAhnenCls.Model.GenBase
{
    public interface IGenFamilyProvider
    {
        IGenFamily GetGenFamily(object idx, EGenListType lType);
        void SetGenFamily(object idx, EGenListType lType, IGenFamily value);
    }
}