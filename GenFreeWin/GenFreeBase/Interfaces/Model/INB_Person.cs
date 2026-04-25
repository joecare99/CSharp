namespace GenFree.Interfaces.Model
{
    public interface INB_Person : IUsesRecordset<int>, IUsesID<int>
    {
        int MinID { get; }

        void Append(int persInArb, bool xAppenWitt = true);
    }
}
