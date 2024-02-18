namespace GenFree.Interfaces.Model
{
    public interface INB_Family : IUsesRecordset<int>, IUsesID<int>
    {
        void Append(int famInArb, bool xAppenWitt = true);
    }
}
