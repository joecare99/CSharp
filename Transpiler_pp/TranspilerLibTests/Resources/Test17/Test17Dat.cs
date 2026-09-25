/// <summary>
/// Demonstrates a switch branch that exits to a label outside the switch.
/// Replacing the goto with a switch-local break preserves the control flow
/// because execution continues at the statement immediately after the switch.
/// </summary>
private void Test17Dat(int state)
{
    switch (state)
    {
        case 1:
            goto end;
        default:
            break;
    }

end:
    return;
}
