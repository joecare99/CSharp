namespace GenFree.Interfaces
{
    public interface IHasID<T>
    {
        T ID { get; } // This is the primary key

        void NewID(); // Method to generate a new ID, if needed
    }
}