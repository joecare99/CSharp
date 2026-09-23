/// <summary>
/// Demonstrates two switch cases that share an outer goto target.
/// The target does not immediately follow the switch, so neither goto can
/// be replaced with break or removed as a duplicate case terminator.
/// </summary>
private void Test18Dat(int state)
{
    switch (state)
    {
        case 1:
            goto end;
        default:
            goto end;
    }

    AfterSwitch();
end:
    return;
}
