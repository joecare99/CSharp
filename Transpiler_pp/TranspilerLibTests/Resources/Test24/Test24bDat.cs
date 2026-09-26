private void Test24b(bool b1, bool b2)
{
    if (b1)
    {
        BeforeNestedIf();
        if (b2)
        {
            SomeFunction();
        }
    }
}
