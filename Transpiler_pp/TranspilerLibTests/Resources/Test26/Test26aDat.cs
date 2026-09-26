private void Test26a(bool b1, bool b2, bool b3)
{
    if (b1)
    {
        SomeFunction();
    }
    else if (b2)
    {
        SomeOtherFunction();
    }
    else if (b3)
    {
        FallbackFunction();
    }
}
