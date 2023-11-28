namespace WinAhnenCls.Model.GenBase
{
    public interface IGenEntity : IGenData
    {
        public interface _IEvents
        {
            IGenEvent this[object Idx] { get; set; }
        }

        int EventCount { get; }
        _IEvents Events { get; }
    }

}
