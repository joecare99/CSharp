private void Test23Dat()
{
    string UbgT = "";
    string text2 = "";
    string[] array = new string[2];
    string Satzend = "";
    string[] aus = new string[76];
    var OrtTable = new OrtTable();
    int try0000_dispatch = -1;
    int num2 = 0;
    int num3 = 0;
    int num4 = 0;
    int num = 0;
    int lErl = 0;
    switch (try0000_dispatch)
    {
        default:
            goto end_IL_0000;
        case 5761:
        {
            num2 = num;
            if (num2 != 0)
            {
                num4 = num2 + 1;
            }
            switch (num4)
            {
                case 42:
                case 208:
                    goto IL_0437;
                case 52:
                case 53:
                    goto IL_0539;
                case 40:
                case 45:
                case 54:
                case 57:
                case 58:
                case 61:
                case 62:
                case 63:
                case 64:
                    goto IL_05f8;
                default:
                    goto end_IL_0000;
            }
        }
    IL_0437:
        num = 42;
        lErl = 10;
        if (UbgT != "")
        {
            UbgT = UbgT + " " + Strings.RTrim(array[1]) + Strings.RTrim(array[2]);
            goto IL_05f8;
        }
        if (aus[75] == "1")
        {
            text2 = "in";
            if (!Information.IsDBNull(RuntimeHelpers.GetObjectValue(OrtTable.Fields["Zusatz"].Value)))
            {
                if (Strings.Trim(OrtTable.Fields["Zusatz"].Value.AsString()) != "")
                {
                    text2 = Strings.Trim(OrtTable.Fields["Zusatz"].Value.AsString());
                }
                goto IL_0539;
            }
            UbgT = text2 + " " + Strings.RTrim(array[1]) + Strings.RTrim(array[2]) + Satzend;
            goto IL_05f8;
        }
        UbgT = Strings.RTrim(array[1]) + Strings.RTrim(array[2]) + Satzend;
        goto IL_05f8;
    IL_0539:
        num = 53;
        UbgT = text2 + " " + Strings.RTrim(array[1]) + Strings.RTrim(array[2]) + Satzend;
        goto IL_05f8;
    IL_05f8:
        num = 64;
        if (UbgT == "")
        {
            goto end_IL_0000;
        }
        goto end_IL_0000;
    }
end_IL_0000:
    return;
}
