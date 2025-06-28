using BaseLib.Helper;
using GenFree.Interfaces.VB;
using System;
namespace GenFree;

public class CVBConversions : IVBConversions
{
    public string ErrorToString()
    {
        // Gibt die aktuelle Fehlermeldung zurück, ähnlich wie VB's ErrorToString().
        // In C# gibt es kein globales Error-Objekt wie in VB, daher geben wir einen leeren String zurück.
        // Optional: Integration mit Thread-Local Exception Handling möglich.
        return string.Empty;
    }

    public double Int(double v) => Math.Floor(v);

    public string Str(object v) => v.AsString();

    public bool ToBoolean(object v) => v.AsBool();

    public double ToDouble(object v) => v.AsDouble();

    public int ToInteger(object v) => v.AsInt();

    public string ToString(object v) => v.AsString();

    public double Val(object v) => v.AsDouble();
}