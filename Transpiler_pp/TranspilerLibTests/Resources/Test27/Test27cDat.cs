private void Test27c(bool b1, bool b2)
{
    if (b1)
    {
        // Comment differences do not make equivalent bodies different.
        SomeFunction();
    }
    else if (b2)
    {
        SomeFunction();
        // Both comments must remain visible after merging.
        // Additional note from the second alternative.
    }
}
