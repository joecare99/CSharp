/// <summary>
/// Demonstrates a switch branch that exits to a label outside the switch.
/// The goto must be preserved because replacing it with a switch-local break
/// would change the control flow.
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
