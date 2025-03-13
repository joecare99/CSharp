using GenFree2Base.Interfaces;

namespace BaseGenClasses.Helper;

public interface IGenILBuilder
{
    IIndexedList<T> NewList<T, T2>();
}