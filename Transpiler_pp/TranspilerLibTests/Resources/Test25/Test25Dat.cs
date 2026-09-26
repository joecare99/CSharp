private void Test25(bool b1, bool b2)
{
    if (b1)
    {
        // Documentation from the first equivalent branch.
        SomeFunction();
    }
    else if (b2)
    {
        SomeFunction();
        // Documentation from the second equivalent branch.
    }
}
