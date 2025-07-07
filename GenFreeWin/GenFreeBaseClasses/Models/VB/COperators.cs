using GenFree.Interfaces.VB;
using System;

namespace Gen_FreeWin.Main;

public class COperators : IOperators
{
    public object AddObject(object value1, object value2)
    {
       return value1 switch
        {
            int i1 when value2 is int i2 => i1 + i2,
            string s1 when value2 is string s2 => s1 + s2,
            _ => throw new System.NotImplementedException("Unsupported types for addition")
        };
    }

    public bool AndObject(bool v1, bool v2)
    {
        return v1 && v2;
    }

    public int CompareString(string v1, string v2, bool TextCompare)
    {
        if (TextCompare)
        {
            return string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
        }
        return string.Compare(v1, v2);
    }

    public object ConcatenateObject(object verz, object value)
    {
        return verz switch
        {
            string s1 when value is string s2 => s1 + s2,
            _ => throw new System.NotImplementedException("Unsupported types for concatenation")
        };
    }

    public bool ConditionalCompareObjectEqual(object v1, object v2, bool TextCompare)
    {
        if (v1 is string str1 && v2 is string str2)
        {
            return TextCompare ? string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase) : str1 == str2;
        }
        return v1.Equals(v2);

    }

    public bool ConditionalCompareObjectGreater(int v1, string v2, bool TextCompare)
    {
        if (v2 is string str2)
        {
            return TextCompare ? string.Compare(v1.ToString(), str2, StringComparison.OrdinalIgnoreCase) > 0 : v1.ToString().CompareTo(str2) > 0;
        }
        throw new System.NotImplementedException("Unsupported types for comparison");
    }

    public bool ConditionalCompareObjectLess(object v1, object v2, bool TextCompare)
    {
        throw new System.NotImplementedException();
    }

    public bool ConditionalCompareObjectNotEqual(object v1, string v2, bool TextCompare)
    {
        throw new System.NotImplementedException();
    }

    public bool OrObject(bool v1, bool v2)
    {
        throw new System.NotImplementedException();
    }
}