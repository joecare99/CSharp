namespace GenFree.Interfaces.VB;

public interface IVBConversions
{
    string ErrorToString();
    double Int(double v);
    string Str(object v);
    bool ToBoolean(object v);
    double ToDouble(object v);
    int ToInteger(object v);
    string ToString(object v);
    double Val(object v);
}