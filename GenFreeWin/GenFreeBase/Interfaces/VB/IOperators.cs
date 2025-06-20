namespace GenFree.Interfaces.VB;

public interface IOperators
{
    object AddObject(object value1, object value2);
    bool AndObject(bool v1, bool v2);
    object ConcatenateObject(object verz, object value);
    bool ConditionalCompareObjectEqual(object v1, object v2, bool TextCompare);
    bool ConditionalCompareObjectGreater(int v1, string v2, bool TextCompare);
    bool ConditionalCompareObjectLess(object v1, object v2, bool TextCompare);
    bool ConditionalCompareObjectNotEqual(object v1, string v2, bool TextCompare);
    bool OrObject(bool v1, bool v2);
}