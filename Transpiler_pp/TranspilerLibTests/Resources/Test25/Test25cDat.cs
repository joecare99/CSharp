private void Test25c(bool b1, bool b2)
{
    if (b1)
    {
        SomeFunction();
    }
    else
    {
        BeforeNestedIf();
        if (b2)
        {
            SomeFunction();
        }
    }
}
