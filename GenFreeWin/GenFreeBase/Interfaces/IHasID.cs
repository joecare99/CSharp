namespace GenFree.Interfaces
{
    public interface IHasID<T>
    {
        T ID { get; } // This is the primary key
    }
}