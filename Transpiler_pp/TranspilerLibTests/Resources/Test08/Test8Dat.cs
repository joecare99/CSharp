﻿public void Test8()
{
    if (true)
        goto IL_109;
    else
        goto IL_1003;
    IL_109:
    num = 209;
    i++;
    goto IL_10002;
IL_1003:
    num = 210;
    i--;
    goto IL_10002;
IL_10002:
    num = 212;
    goto IL_100001;
IL_100001:
    num = 213;
    return;
}