private void Test14Dat()
{
    switch (sTest)
    {
        case 0:
            goto IL_0001;
        default:
            break;
        IL_0001:
            v = 0;
            goto IL_0002;
        IL_0002:
            K[v] = 0;
            goto IL_0003;
        IL_0003:
            v++;
            if (v < 13)
            {
                goto IL_0002;
            }
            goto IL_0004;
        IL_0004:
            v = 0;
            goto IL_0005;
        IL_0005:
            K[v] = 0;
            goto IL_0006;
        IL_0006:
            if (v < 16)
            {
                v++;
                goto IL_0005;
            }
        goto IL_0007;
            IL_0007:
            v++;
            goto IL_0008;
            IL_0008:
            if (v < 18)
            {
                goto IL_0005;
            }

    }
}
