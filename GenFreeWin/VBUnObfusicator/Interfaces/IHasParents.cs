namespace VBUnObfusicator.Interfaces
{
    public interface IHasParents<T> where T : class
    {
        T Parent { get; set; }
    }
}