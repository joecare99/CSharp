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
l1: // nop
    return;
}