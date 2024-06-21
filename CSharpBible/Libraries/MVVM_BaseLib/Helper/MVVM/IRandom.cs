namespace BaseLib.Helper.MVVM;

public interface IRandom
{
    int Next(int v1, int v2);
    double NextDouble();
    int NextInt();
    void Seed(int value);
}
