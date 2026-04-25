namespace GenFree.Helper
{
    public interface IListItem<T>
    {
        T? ItemData { get; }
        string ItemString { get; }
        string ToString();
    }
}