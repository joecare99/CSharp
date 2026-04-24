    private void Button2_Click(object sender, EventArgs e)
    {
        //Discarded unreachable code: IL_0085
        int try0000_dispatch = -1;
        int num3 = default(int);
        int num2 = default(int);
        int num = default(int);
        byte b = default(byte);
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                switch (try0000_dispatch)
                {
                    default:
                        ProjectData.ClearProjectError();
                        num3 = 1;
                        goto IL_0007;
                    case 196:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 1:
                                    break;
                                default:
                                    goto end_IL_0000;
                            }
                            int num4 = num2 + 1;
                            num2 = 0;
                            switch (num4)
                            {
                                case 1:
                                    break;
                                case 2:
                                    goto IL_0007;
                                case 3:
                                    goto IL_0024;
                                case 6:
                                    goto IL_002b;
                                case 7:
                                    goto IL_0033;
                                case 4:
                                case 5:
                                case 8:
                                    goto IL_0048;
                                case 9:
                                    goto IL_0054;
                                case 10:
                                    goto end_IL_0000_2;
                                default:
                                    goto end_IL_0000;
                                case 11:
                                    goto end_IL_0000_3;
                            }
                            goto default;
                        }
                    IL_0033:
                        num = 7;
                        FileSystem.Input(99, ref COND.Aus[b]);
                        goto IL_0048;
                    IL_0007:
                        num = 2;
                        FileSystem.FileOpen(99, COND.GenPlu + "\\Init\\Druck_ini.dat", OpenMode.Input);
                        goto IL_0024;
                    IL_0024:
                        num = 3;
                        b = 0;
                        goto IL_0048;
                    IL_0048:
                        num = 5;
                        if (!FileSystem.EOF(99))
                        {
                            goto IL_002b;
                        }
                        goto IL_0054;
                    IL_0054:
                        num = 9;
                        FileSystem.FileClose(99);
                        break;
                    IL_002b:
                        num = 6;
                        b = checked((byte)(unchecked(b) + 1));
                        goto IL_0033;
                    end_IL_0000_2:
                        break;
                }
                num = 10;
                Process.Start(COND.GenPlu + "\\Hilfe\\TeilB.PDF");
                break;
            end_IL_0000:;
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 196;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
            continue;
        end_IL_0000_3:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }