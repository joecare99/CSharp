﻿    private void Befehl_Click(object eventSender, EventArgs eventArgs)
    {
        int try0000_dispatch = -1;
        int num = default(int);
        short index = default(short);
        int num2 = default(int);
        int num3 = default(int);
        int number = default(int);
        string prompt = default(string);
        while (true)
        {
            try
            {
                /*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
                ;
                int num4;
                string text;
                switch (try0000_dispatch)
                {
                    default:
                        num = 1;
                        index = Befehl.GetIndex((Button)eventSender);
                        goto IL_0015;
                    case 1043:
                        {
                            num2 = num;
                            switch (num3)
                            {
                                case 2:
                                    break;
                                case 1:
                                    goto IL_0319;
                                default:
                                    goto end_IL_0000;
                            }
                            goto IL_0248;
                        }
                    IL_0319:
                        num4 = num2 + 1;
                        goto IL_031d;
                    IL_0248:
                        num = 36;
                        number = Information.Err().Number;
                        goto IL_0258;
                    IL_0258:
                        num = 39;
                        if (number == 25)
                        {
                            goto IL_0262;
                        }
                        goto IL_029b;
                    IL_029b:
                        num = 46;
                        if (number == 55)
                        {
                            goto IL_02a5;
                        }
                        goto IL_02d0;
                    IL_02d0:
                        num = 52;
                        if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)
                        {
                            ProjectData.EndApp();
                        }
                        goto IL_02f6;
                    IL_00ad:
                        num = 9;
                        RichTextBox1.LoadFile(COND.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        goto IL_00cc;
                    IL_02f6:
                        num = 55;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0315;
                    IL_00cc:
                        num = 10;
                        Interaction.Shell(COND.Aus[7] + " " + COND.Verz1 + "Temp\\Text2.RTF", AppWinStyle.MaximizedFocus);
                        goto end_IL_0000_2;
                    IL_0315:
                        num4 = num2;
                        goto IL_031d;
                    IL_031d:
                        num2 = 0;
                        switch (num4)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_0015;
                            case 3:
                                goto IL_001d;
                            case 4:
                                goto IL_0073;
                            case 6:
                            case 8:
                                goto IL_008f;
                            case 9:
                                goto IL_00ad;
                            case 10:
                                goto IL_00cc;
                            case 12:
                            case 13:
                                goto IL_00f9;
                            case 14:
                                goto IL_010d;
                            case 15:
                                goto IL_0117;
                            case 18:
                            case 19:
                                goto IL_0134;
                            case 20:
                                goto IL_0151;
                            case 21:
                                goto IL_0178;
                            case 22:
                                goto IL_0191;
                            case 23:
                                goto IL_01aa;
                            case 24:
                                goto IL_01d0;
                            case 26:
                            case 28:
                                goto IL_01f9;
                            case 30:
                            case 31:
                                goto IL_021f;
                            case 36:
                                goto IL_0248;
                            case 38:
                            case 39:
                                goto IL_0258;
                            case 40:
                                goto IL_0262;
                            case 41:
                                goto IL_026c;
                            case 42:
                                goto IL_027f;
                            case 46:
                                goto IL_029b;
                            case 47:
                                goto IL_02a5;
                            case 48:
                                goto IL_02b4;
                            case 51:
                            case 52:
                                goto IL_02d0;
                            case 53:
                            case 55:
                                goto IL_02f6;
                            default:
                                goto end_IL_0000;
                            case 5:
                            case 11:
                            case 16:
                            case 17:
                            case 25:
                            case 29:
                            case 32:
                            case 33:
                            case 34:
                            case 35:
                            case 37:
                            case 43:
                            case 45:
                            case 49:
                            case 50:
                            case 56:
                            case 57:
                            case 58:
                                goto end_IL_0000_2;
                        }
                        goto default;
                    IL_02a5:
                        num = 47;
                        FileSystem.FileClose();
                        goto IL_02b4;
                    IL_02b4:
                        num = 48;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0315;
                    IL_008f:
                        num = 8;
                        RichTextBox1.SaveFile(COND.Verz1 + "TEMP\\Text2.RTF", RichTextBoxStreamType.RichText);
                        goto IL_00ad;
                    IL_0262:
                        num = 40;
                        prompt = "Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen.";
                        goto IL_026c;
                    IL_026c:
                        num = 41;
                        if (Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, "Fehler") == MsgBoxResult.Cancel)
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_027f;
                    IL_027f:
                        num = 42;
                        ProjectData.ClearProjectError();
                        if (num2 == 0)
                        {
                            throw ProjectData.CreateProjectError(-2146828268);
                        }
                        goto IL_0315;
                    IL_0117:
                        num = 15;
                        MyProject.Forms.Druck.Show();
                        goto end_IL_0000_2;
                    IL_0015:
                        ProjectData.ClearProjectError();
                        num3 = 2;
                        goto IL_001d;
                    IL_001d:
                        num = 3;
                        text = "Datum " + Strings.Mid(DateAndTime.DateString, 4, 2) + "." + DateAndTime.DateString.Left( 2) + "." + Strings.Mid(DateAndTime.DateString, 7, 4);
                        goto IL_0073;
                    IL_0073:
                        num = 4;
                        switch (index)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_00f9;
                            case 3:
                                goto IL_0134;
                            default:
                                goto end_IL_0000_2;
                        }
                        goto IL_008f;
                    IL_0134:
                        num = 19;
                        MyProject.Forms.Hinter.CommonDialog1Save.Filter = "Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF";
                        goto IL_0151;
                    IL_0151:
                        num = 20;
                        MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = COND.GenPlu + "list\\";
                        goto IL_0178;
                    IL_0178:
                        num = 21;
                        MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
                        goto IL_0191;
                    IL_0191:
                        num = 22;
                        MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
                        goto IL_01aa;
                    IL_01aa:
                        num = 23;
                        if (MyProject.Forms.Hinter.CommonDialog1Save.FileName ==  "")
                        {
                            goto end_IL_0000_2;
                        }
                        goto IL_01d0;
                    IL_01d0:
                        num = 24;
                        switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
                        {
                            case 1:
                                break;
                            case 2:
                                goto IL_021f;
                            default:
                                goto end_IL_0000_2;
                        }
                        goto IL_01f9;
                    IL_021f:
                        num = 31;
                        RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
                        goto end_IL_0000_2;
                    IL_01f9:
                        num = 28;
                        RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
                        goto end_IL_0000_2;
                    IL_00f9:
                        num = 13;
                        RichTextBox1.Text = "";
                        goto IL_010d;
                    IL_010d:
                        num = 14;
                        Close();
                        goto IL_0117;
                    end_IL_0000:
                        break;
                }
            }
            catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
            {
                ProjectData.SetProjectError(obj);
                try0000_dispatch = 1043;
                continue;
            }
            throw ProjectData.CreateProjectError(-2146828237);
            continue;
        end_IL_0000_2:
            break;
        }
        if (num2 != 0)
        {
            ProjectData.ClearProjectError();
        }
    }