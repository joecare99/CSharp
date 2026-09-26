private void Test15Dat()
{
    if (b1)
    {
        if (b2)
        {
            goto l1; // can be removed, because the next unconditional jump is the same as this one
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
l2: // some other code
    goto l1;
l1: // nop
    return;
}