public void Test16Dat()
{
    while (b1)
    {
        if (b2)
        {
            goto l1; // must not be removed, even if it is the last statement in the block
            // end of block
        }
        else
        {
            // some other code
        }
        // some comment
    }
    // some other comment
    goto l1;
l1: // must have 3 gotos to this label, even if it is the last statement in the method
    return;
}