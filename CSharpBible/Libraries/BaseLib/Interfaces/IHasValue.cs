namespace BaseLib.Interfaces;

public interface IHasValue : IHasValue<object>
{
}
public interface IHasValue<T>
{
    T? Value { get; }
}
