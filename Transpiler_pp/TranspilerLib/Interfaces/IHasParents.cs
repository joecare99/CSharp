namespace TranspilerLib.Interfaces
{
    public interface IHasParents<T> where T : class
    {
        T Parent { get; set; }
    }
}