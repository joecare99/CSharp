private void Test28(bool b1, bool b2)
{
    if (b1)
    {
        D();
    }
    else
    {
        // Keep this line comment immediately before else-if.
        if (b2)
        {
            E();
        }
        else
        {
            F();
        }
    }
}
