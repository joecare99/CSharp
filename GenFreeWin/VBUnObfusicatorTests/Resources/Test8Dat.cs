public void Test7()
{
    if (true)
        goto IL_105c;
    else
        goto IL_107c;
    IL_105c:
    num = 209;
    i++;
    goto IL_108c;
IL_107c:
    num = 210;
    i--;
    goto IL_108c;
IL_108c:
    num = 212;
    return;
}