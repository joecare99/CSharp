using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLibTests.Properties;

namespace TranspilerLibTests.TestData;

public class TestCSDataClass
{
    #region Raw TestData
    public const string testData0 = @"public const string TestData()
{
    // Discarded unreachable code: IL_0085
    string test = ""test"";
    goto IL_0001;
    /* Only one IL_0001 is allowed */
IL_0001: 
    Test(test: ""Some Test"");
    switch (test)
    {
        case ""test"":
        case ""test2"":
            break;
        default:
            goto IL_0002;
    }
    goto IL_0002;
IL_0002:
    return test;
}";
    public static string test1Data { get; } = Resources.Test1Dat_cs;
    public static string test2Data { get; } = Resources.Test2Dat_cs;
    public const string testData3 = @"public void Test3(){
Modul1.UbgT = (Text2[0].Text).Trim();
                            goto IL_105c;
                        IL_105c:
                            num = 209;
                            if (Modul1.UbgT == """")
                            {
                                goto IL_107c;
                            }
                            else
                            {
                                test = @""""""Test"""""";
                                test2 = $""\""{test}\"""";
                                test3 = $""{{test}}"";    
                                goto IL_108d;
                            }
                        //=================
                        IL_107c:
                            num = 210;
                            Modul1.UbgT = ""\"""";
                            goto IL_108d;
                        IL_108d:
                            num = 212;
                            DataModul.DB_PersonTable.Seek("">"", Modul1.UbgT, Modul1.PersInArb);
}";
    public const string testData4 = @"public void Test4(){
                            goto IL_105c;
                        IL_105c:
                            num = 209;
                            Test(test: ""Some Test"");
                            goto IL_107c;
                        //=================
                        IL_107c:
                            num = 210;
                            Modul1.UbgT = ""\"""";
                            goto IL_108d;
                        IL_108d:
                            num = 212;
}";
    public const string testData5 = @"public void Test5(){
    test = ""Some unended string ... ;
    test2 = $""Some {(test.Length()>5?@""\"""":""7"")} nested string ..."";
    test3 = ""Some broken""
    /* some comment */
        +"" string ..."";
    test3 = ""Another broken""
    // some other comment 
        +"" string ..."";
}";
    public const string testData6 = @"public void Test6(){
        if (true)
        {
            goto IL_105c;
        }
        else
            goto IL_107c;
    IL_105c:
        goto IL_107c;
    IL_107c:
        return;
}";
    public const string testData7 = @"public void Test7(){
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
}";
    public static readonly string test8Data = Resources.Test8Dat_cs;
    public static readonly string test9Data = Resources.Test9Dat_cs;
    public static readonly string test10Data = Resources.Test10Dat_cs;
    public static readonly string test11Data = Resources.Test11Dat_cs;
    public static readonly string test12Data = Resources.Test12Dat_cs;
    public static readonly string test13Data = Resources.Test13Dat_cs;

    #endregion

    #region Expected TestData
    public const string testDataExp0 = @"///Declaration MainBlock 0,0
public const string TestData()
///BlockStart Block 1,0
{
///Comment LComment 1,1
// Discarded unreachable code: IL_0085
///Operation Operation 1,2
string test = ""test"";
///Goto Goto 1,3 Dest:OK
goto IL_0001;
///Comment Comment 1,4
/* Only one IL_0001 is allowed */
///Label Label 1,5 1
IL_0001:
///Operation Operation 1,6
Test(test: ""Some Test"");
///Operation Operation 1,7
switch (test)
///BlockStart Block 2,0
{
///Operation Label 2,1
case ""test"":
///Operation Label 2,2
case ""test2"":
///Operation Operation 2,3
break;
///Label Label 2,4
default:
///Goto Goto 2,5 Dest:OK
goto IL_0002;
///BlockEnd Block 2,6
}
///Goto Goto 1,8 Dest:OK
goto IL_0002;
///Label Label 1,9 2
IL_0002:
///Operation Operation 1,10
return test;
///BlockEnd Block 1,11
}";
    public static string testDataExp1 { get; } = Resources.Test1ExpParse;
    public static string testDataExp2 { get; } = Resources.Test2ExpParse;
    public const string testDataExp3 = @"///Declaration MainBlock 0,0
public void Test3()
///BlockStart Block 1,0
{
///Operation Operation 1,1
Modul1.UbgT = (Text2[0].Text).Trim();
///Goto Goto 1,2 Dest:OK
goto IL_105c;
///Label Label 1,3 1
IL_105c:
///Operation Operation 1,4
num = 209;
///Operation Operation 1,5
if (Modul1.UbgT == """")
///BlockStart Block 2,0
{
///Goto Goto 2,1 Dest:OK
goto IL_107c;
///BlockEnd Block 2,2
}
///Operation Operation 1,6
else
///BlockStart Block 2,0
{
///Operation Operation 2,1
test = @""""""Test"""""";
///Operation Operation 2,2
test2 = $""\""{test}\"""";
///Operation Operation 2,3
test3 = $""{{test}}"";
///Goto Goto 2,4 Dest:OK
goto IL_108d;
///BlockEnd Block 2,5
}
///Comment LComment 1,7
//=================
///Label Label 1,8 1
IL_107c:
///Operation Operation 1,9
num = 210;
///Operation Operation 1,10
Modul1.UbgT = ""\"""";
///Goto Goto 1,11 Dest:OK
goto IL_108d;
///Label Label 1,12 2
IL_108d:
///Operation Operation 1,13
num = 212;
///Operation Operation 1,14
DataModul.DB_PersonTable.Seek("">"", Modul1.UbgT, Modul1.PersInArb);
///BlockEnd Block 1,15
}";
    public const string testDataExp4 = @"///Declaration MainBlock 0,0
public void Test4()
///BlockStart Block 1,0
{
///Goto Goto 1,1 Dest:OK
goto IL_105c;
///Label Label 1,2 1
IL_105c:
///Operation Operation 1,3
num = 209;
///Operation Operation 1,4
Test(test: ""Some Test"");
///Goto Goto 1,5 Dest:OK
goto IL_107c;
///Comment LComment 1,6
//=================
///Label Label 1,7 1
IL_107c:
///Operation Operation 1,8
num = 210;
///Operation Operation 1,9
Modul1.UbgT = ""\"""";
///Goto Goto 1,10 Dest:OK
goto IL_108d;
///Label Label 1,11 1
IL_108d:
///Operation Operation 1,12
num = 212;
///BlockEnd Block 1,13
}";
    public const string testDataExp5 = @"///Declaration MainBlock 0,0
public void Test5()
///BlockStart Block 1,0
{
///Operation Operation 1,1
test = ""Some unended string ... ;
///Operation Operation 1,2
test2 = $""Some {(test.Length()>5?@""\"""":""7"")} nested string ..."";
///Operation Operation 1,3
test3 = ""Some broken""
///Comment Comment 1,4
/* some comment */
///Operation Operation 1,5
+ "" string ..."";
///Operation Operation 1,6
test3 = ""Another broken""
///Comment LComment 1,7
// some other comment
///Operation Operation 1,8
+ "" string ..."";
///BlockEnd Block 1,9
}";
    public const string testDataExp6 = @"///Declaration MainBlock 0,0
public void Test6()
///BlockStart Block 1,0
{
///Operation Operation 1,1
if (true)
///BlockStart Block 2,0
{
///Goto Goto 2,1 Dest:OK
goto IL_105c;
///BlockEnd Block 2,2
}
///Operation Operation 1,2
else
///Goto Goto 1,3 Dest:OK
goto IL_107c;
///Label Label 1,4 1
IL_105c:
///Goto Goto 1,5 Dest:OK
goto IL_107c;
///Label Label 1,6 2
IL_107c:
///Operation Operation 1,7
return;
///BlockEnd Block 1,8
}";
    public const string testDataExp7 = @"///Declaration MainBlock 0,0
public void Test7()
///BlockStart Block 1,0
{
///Operation Operation 1,1
if (true)
///Goto Goto 1,2 Dest:OK
goto IL_105c;
///Operation Operation 1,3
else
///Goto Goto 1,4 Dest:OK
goto IL_107c;
///Label Label 1,5 1
IL_105c:
///Operation Operation 1,6
num = 209;
///Operation Operation 1,7
i++;
///Goto Goto 1,8 Dest:OK
goto IL_108c;
///Label Label 1,9 1
IL_107c:
///Operation Operation 1,10
num = 210;
///Operation Operation 1,11
i--;
///Goto Goto 1,12 Dest:OK
goto IL_108c;
///Label Label 1,13 2
IL_108c:
///Operation Operation 1,14
num = 212;
///Operation Operation 1,15
return;
///BlockEnd Block 1,16
}";
    public static readonly string test8DataExp = Resources.Test8ExpParse;
    public static readonly string test9DataExp = Resources.Test9ExpParse;
    public static readonly string test10DataExp = Resources.Test10ExpParse;
    public static readonly string test11DataExp = Resources.Test11ExpParse;
    public static readonly string test12DataExp = Resources.Test12ExpParse;
    public static readonly string test13DataExp = Resources.Test13ExpParse;
    //====================================================================================================================================================================

    public const string testDataMoveExp0 = @"";
    public const string testDataMoveExp = @"///Declaration MainBlock 0,0
private void Befehl_Click(object eventSender, EventArgs eventArgs)
///BlockStart Block 1,0
{
///Operation Operation 1,1
int try0000_dispatch = -1;
///Operation Operation 1,2
int num = default(int);
///Operation Operation 1,3
short index = default(short);
///Operation Operation 1,4
int num2 = default(int);
///Operation Operation 1,5
int num3 = default(int);
///Operation Operation 1,6
int number = default(int);
///Operation Operation 1,7
string prompt = default(string);
///Operation Operation 1,8
while (true)
///BlockStart Block 2,0
{
///Operation Operation 2,1
try
///BlockStart Block 3,0
{
///Comment Comment 3,1
/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
///Operation Operation 3,2
;
///Operation Operation 3,3
int num4;
///Operation Operation 3,4
string text;
///Operation Operation 3,5
switch (try0000_dispatch)
///BlockStart Block 4,0
{
///Label Label 4,1 1
default:
///Operation Operation 4,2
num = 1;
///Operation Operation 4,3
index = Befehl.GetIndex((Button)eventSender);
///Goto Goto 4,4 Dest:OK
goto IL_0015;
///Label Label 4,5
case 1043:
///BlockStart Block 5,0
{
///Operation Operation 5,1
num2 = num;
///Operation Operation 5,2
switch (num3)
///BlockStart Block 6,0
{
///Label Label 6,1
case 2:
///Operation Operation 6,2
break;
///Label Label 6,3
case 1:
///Goto Goto 6,4 Dest:OK
goto IL_0319;
///Label Label 6,5
default:
///Goto Goto 6,6 Dest:OK
goto end_IL_0000;
///BlockEnd Block 6,7
}
///Goto Goto 5,3 Dest:OK
goto IL_0248;
///BlockEnd Block 5,4
}
///Label Label 4,6 2
IL_0248:
///Operation Operation 4,7
num = 36;
///Operation Operation 4,8
number = Information.Err().Number;
///Goto Goto 4,9 Dest:OK
goto IL_0258;
///Label Label 4,10 1
IL_0319:
///Operation Operation 4,11
num4 = num2 + 1;
///Goto Goto 4,12 Dest:OK
goto IL_031d;
///Label Label 4,13 2
IL_0258:
///Operation Operation 4,14
num = 39;
///Operation Operation 4,15
if (number == 25)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_0262;
///BlockEnd Block 5,2
}
///Goto Goto 4,16 Dest:OK
goto IL_029b;
///Label Label 4,17 2
IL_029b:
///Operation Operation 4,18
num = 46;
///Operation Operation 4,19
if (number == 55)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_02a5;
///BlockEnd Block 5,2
}
///Goto Goto 4,20 Dest:OK
goto IL_02d0;
///Label Label 4,21 2
IL_02d0:
///Operation Operation 4,22
num = 52;
///Operation Operation 4,23
if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Operation Operation 5,1
ProjectData.EndApp();
///BlockEnd Block 5,2
}
///Goto Goto 4,24 Dest:OK
goto IL_02f6;
///Label Label 4,25 2
IL_00ad:
///Operation Operation 4,26
num = 9;
///Operation Operation 4,27
RichTextBox1.LoadFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,28 Dest:OK
goto IL_00cc;
///Label Label 4,29 2
IL_02f6:
///Operation Operation 4,30
num = 55;
///Operation Operation 4,31
ProjectData.ClearProjectError();
///Operation Operation 4,32
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,33 Dest:OK
goto IL_0315;
///Label Label 4,34 2
IL_00cc:
///Operation Operation 4,35
num = 10;
///Operation Operation 4,36
Interaction.Shell(COND.Aus[7] + "" "" + COND.Verz1 + ""Temp\\Text2.RTF"", AppWinStyle.MaximizedFocus);
///Goto Goto 4,37 Dest:OK
goto end_IL_0000_2;
///Label Label 4,38 3
IL_0315:
///Operation Operation 4,39
num4 = num2;
///Goto Goto 4,40 Dest:OK
goto IL_031d;
///Label Label 4,41 2
IL_031d:
///Operation Operation 4,42
num2 = 0;
///Operation Operation 4,43
switch (num4)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_0015;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_001d;
///Label Label 5,7
case 4:
///Goto Goto 5,8 Dest:OK
goto IL_0073;
///Label Label 5,9
case 6:
///Label Label 5,10
case 8:
///Goto Goto 5,11 Dest:OK
goto IL_008f;
///Label Label 5,12
case 9:
///Goto Goto 5,13 Dest:OK
goto IL_00ad;
///Label Label 5,14
case 10:
///Goto Goto 5,15 Dest:OK
goto IL_00cc;
///Label Label 5,16
case 12:
///Label Label 5,17
case 13:
///Goto Goto 5,18 Dest:OK
goto IL_00f9;
///Label Label 5,19
case 14:
///Goto Goto 5,20 Dest:OK
goto IL_010d;
///Label Label 5,21
case 15:
///Goto Goto 5,22 Dest:OK
goto IL_0117;
///Label Label 5,23
case 18:
///Label Label 5,24
case 19:
///Goto Goto 5,25 Dest:OK
goto IL_0134;
///Label Label 5,26
case 20:
///Goto Goto 5,27 Dest:OK
goto IL_0151;
///Label Label 5,28
case 21:
///Goto Goto 5,29 Dest:OK
goto IL_0178;
///Label Label 5,30
case 22:
///Goto Goto 5,31 Dest:OK
goto IL_0191;
///Label Label 5,32
case 23:
///Goto Goto 5,33 Dest:OK
goto IL_01aa;
///Label Label 5,34
case 24:
///Goto Goto 5,35 Dest:OK
goto IL_01d0;
///Label Label 5,36
case 26:
///Label Label 5,37
case 28:
///Goto Goto 5,38 Dest:OK
goto IL_01f9;
///Label Label 5,39
case 30:
///Label Label 5,40
case 31:
///Goto Goto 5,41 Dest:OK
goto IL_021f;
///Label Label 5,42
case 36:
///Goto Goto 5,43 Dest:OK
goto IL_0248;
///Label Label 5,44
case 38:
///Label Label 5,45
case 39:
///Goto Goto 5,46 Dest:OK
goto IL_0258;
///Label Label 5,47
case 40:
///Goto Goto 5,48 Dest:OK
goto IL_0262;
///Label Label 5,49
case 41:
///Goto Goto 5,50 Dest:OK
goto IL_026c;
///Label Label 5,51
case 42:
///Goto Goto 5,52 Dest:OK
goto IL_027f;
///Label Label 5,53
case 46:
///Goto Goto 5,54 Dest:OK
goto IL_029b;
///Label Label 5,55
case 47:
///Goto Goto 5,56 Dest:OK
goto IL_02a5;
///Label Label 5,57
case 48:
///Goto Goto 5,58 Dest:OK
goto IL_02b4;
///Label Label 5,59
case 51:
///Label Label 5,60
case 52:
///Goto Goto 5,61 Dest:OK
goto IL_02d0;
///Label Label 5,62
case 53:
///Label Label 5,63
case 55:
///Goto Goto 5,64 Dest:OK
goto IL_02f6;
///Label Label 5,65
default:
///Goto Goto 5,66 Dest:OK
goto end_IL_0000;
///Label Label 5,67
case 5:
///Label Label 5,68
case 11:
///Label Label 5,69
case 16:
///Label Label 5,70
case 17:
///Label Label 5,71
case 25:
///Label Label 5,72
case 29:
///Label Label 5,73
case 32:
///Label Label 5,74
case 33:
///Label Label 5,75
case 34:
///Label Label 5,76
case 35:
///Label Label 5,77
case 37:
///Label Label 5,78
case 43:
///Label Label 5,79
case 45:
///Label Label 5,80
case 49:
///Label Label 5,81
case 50:
///Label Label 5,82
case 56:
///Label Label 5,83
case 57:
///Label Label 5,84
case 58:
///Goto Goto 5,85 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,86
}
///Goto Goto 4,44 Dest:OK
goto default;
///Label Label 4,45 2
IL_02a5:
///Operation Operation 4,46
num = 47;
///Operation Operation 4,47
FileSystem.FileClose();
///Goto Goto 4,48 Dest:OK
goto IL_02b4;
///Label Label 4,49 2
IL_02b4:
///Operation Operation 4,50
num = 48;
///Operation Operation 4,51
ProjectData.ClearProjectError();
///Operation Operation 4,52
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,53 Dest:OK
goto IL_0315;
///Label Label 4,54 2
IL_008f:
///Operation Operation 4,55
num = 8;
///Operation Operation 4,56
RichTextBox1.SaveFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,57 Dest:OK
goto IL_00ad;
///Label Label 4,58 2
IL_0262:
///Operation Operation 4,59
num = 40;
///Operation Operation 4,60
prompt = ""Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen."";
///Goto Goto 4,61 Dest:OK
goto IL_026c;
///Label Label 4,62 2
IL_026c:
///Operation Operation 4,63
num = 41;
///Operation Operation 4,64
if (Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, ""Fehler"") == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,65 Dest:OK
goto IL_027f;
///Label Label 4,66 2
IL_027f:
///Operation Operation 4,67
num = 42;
///Operation Operation 4,68
ProjectData.ClearProjectError();
///Operation Operation 4,69
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,70 Dest:OK
goto IL_0315;
///Label Label 4,71 2
IL_0117:
///Operation Operation 4,72
num = 15;
///Operation Operation 4,73
MyProject.Forms.Druck.Show();
///Goto Goto 4,74 Dest:OK
goto end_IL_0000_2;
///Label Label 4,75 2
IL_0015:
///Operation Operation 4,76
ProjectData.ClearProjectError();
///Operation Operation 4,77
num3 = 2;
///Goto Goto 4,78 Dest:OK
goto IL_001d;
///Label Label 4,79 2
IL_001d:
///Operation Operation 4,80
num = 3;
///Operation Operation 4,81
text = ""Datum "" + Strings.Mid(DateAndTime.DateString, 4, 2) + ""."" + DateAndTime.DateString.Left( 2) + ""."" + Strings.Mid(DateAndTime.DateString, 7, 4);
///Goto Goto 4,82 Dest:OK
goto IL_0073;
///Label Label 4,83 2
IL_0073:
///Operation Operation 4,84
num = 4;
///Operation Operation 4,85
switch (index)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_00f9;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_0134;
///Label Label 5,7
default:
///Goto Goto 5,8 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,9
}
///Goto Goto 4,86 Dest:OK
goto IL_008f;
///Label Label 4,87 2
IL_0134:
///Operation Operation 4,88
num = 19;
///Operation Operation 4,89
MyProject.Forms.Hinter.CommonDialog1Save.Filter = ""Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF"";
///Goto Goto 4,90 Dest:OK
goto IL_0151;
///Label Label 4,91 2
IL_0151:
///Operation Operation 4,92
num = 20;
///Operation Operation 4,93
MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = COND.GenPlu + ""list\\"";
///Goto Goto 4,94 Dest:OK
goto IL_0178;
///Label Label 4,95 2
IL_0178:
///Operation Operation 4,96
num = 21;
///Operation Operation 4,97
MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
///Goto Goto 4,98 Dest:OK
goto IL_0191;
///Label Label 4,99 2
IL_0191:
///Operation Operation 4,100
num = 22;
///Operation Operation 4,101
MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
///Goto Goto 4,102 Dest:OK
goto IL_01aa;
///Label Label 4,103 2
IL_01aa:
///Operation Operation 4,104
num = 23;
///Operation Operation 4,105
if (MyProject.Forms.Hinter.CommonDialog1Save.FileName == """")
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,106 Dest:OK
goto IL_01d0;
///Label Label 4,107 2
IL_01d0:
///Operation Operation 4,108
num = 24;
///Operation Operation 4,109
switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_021f;
///Label Label 5,5
default:
///Goto Goto 5,6 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,7
}
///Goto Goto 4,110 Dest:OK
goto IL_01f9;
///Label Label 4,111 2
IL_021f:
///Operation Operation 4,112
num = 31;
///Operation Operation 4,113
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
///Goto Goto 4,114 Dest:OK
goto end_IL_0000_2;
///Label Label 4,115 2
IL_01f9:
///Operation Operation 4,116
num = 28;
///Operation Operation 4,117
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
///Goto Goto 4,118 Dest:OK
goto end_IL_0000_2;
///Label Label 4,119 2
IL_00f9:
///Operation Operation 4,120
num = 13;
///Operation Operation 4,121
RichTextBox1.Text = """";
///Goto Goto 4,122 Dest:OK
goto IL_010d;
///Label Label 4,123 2
IL_010d:
///Operation Operation 4,124
num = 14;
///Operation Operation 4,125
Close();
///Goto Goto 4,126 Dest:OK
goto IL_0117;
///Label Label 4,127 2
end_IL_0000:
///Operation Operation 4,128
break;
///BlockEnd Block 4,129
}
///BlockEnd Block 3,6
}
///Operation Operation 2,2
catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
///BlockStart Block 3,0
{
///Operation Operation 3,1
ProjectData.SetProjectError(obj);
///Operation Operation 3,2
try0000_dispatch = 1043;
///Operation Operation 3,3
continue;
///BlockEnd Block 3,4
}
///Operation Operation 2,3
throw ProjectData.CreateProjectError(-2146828237);
///Operation Operation 2,4
continue;
///Label Label 2,5 9
end_IL_0000_2:
///Operation Operation 2,6
break;
///BlockEnd Block 2,7
}
///Operation Operation 1,9
if (num2 != 0)
///BlockStart Block 2,0
{
///Operation Operation 2,1
ProjectData.ClearProjectError();
///BlockEnd Block 2,2
}
///BlockEnd Block 1,10
}";
    public const string testDataMoveExp2 = @"";
    public const string testDataDeleteExp = @"///Declaration MainBlock 0,0
private void Befehl_Click(object eventSender, EventArgs eventArgs)
///BlockStart Block 1,0
{
///Operation Operation 1,1
int try0000_dispatch = -1;
///Operation Operation 1,2
int num = default(int);
///Operation Operation 1,3
short index = default(short);
///Operation Operation 1,4
int num2 = default(int);
///Operation Operation 1,5
int num3 = default(int);
///Operation Operation 1,6
int number = default(int);
///Operation Operation 1,7
string prompt = default(string);
///Operation Operation 1,8
while (true)
///BlockStart Block 2,0
{
///Operation Operation 2,1
try
///BlockStart Block 3,0
{
///Comment Comment 3,1
/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
///Operation Operation 3,2
;
///Operation Operation 3,3
int num4;
///Operation Operation 3,4
string text;
///Operation Operation 3,5
switch (try0000_dispatch)
///BlockStart Block 4,0
{
///Label Label 4,1 1
default:
///Operation Operation 4,2
num = 1;
///Operation Operation 4,3
index = Befehl.GetIndex((Button)eventSender);
///Goto Goto 4,4 Dest:OK
goto IL_0015;
///Label Label 4,5
case 1043:
///BlockStart Block 5,0
{
///Operation Operation 5,1
num2 = num;
///Operation Operation 5,2
switch (num3)
///BlockStart Block 6,0
{
///Label Label 6,1
case 2:
///Operation Operation 6,2
break;
///Label Label 6,3
case 1:
///Goto Goto 6,4
goto IL_0319;
///Label Label 6,5
default:
///Goto Goto 6,6 Dest:OK
goto end_IL_0000;
///BlockEnd Block 6,7
}
///Goto Goto 5,3 Dest:OK
goto IL_0248;
///BlockEnd Block 5,4
}
///Label Label 4,6 2
IL_0248:
///Operation Operation 4,7
num = 36;
///Operation Operation 4,8
number = Information.Err().Number;
///Goto Goto 4,9 Dest:OK
goto IL_0258;
///Label Label 4,10 2
IL_0258:
///Operation Operation 4,11
num = 39;
///Operation Operation 4,12
if (number == 25)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_0262;
///BlockEnd Block 5,2
}
///Goto Goto 4,13 Dest:OK
goto IL_029b;
///Label Label 4,14 2
IL_029b:
///Operation Operation 4,15
num = 46;
///Operation Operation 4,16
if (number == 55)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_02a5;
///BlockEnd Block 5,2
}
///Goto Goto 4,17 Dest:OK
goto IL_02d0;
///Label Label 4,18 2
IL_02d0:
///Operation Operation 4,19
num = 52;
///Operation Operation 4,20
if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Operation Operation 5,1
ProjectData.EndApp();
///BlockEnd Block 5,2
}
///Goto Goto 4,21 Dest:OK
goto IL_02f6;
///Label Label 4,22 2
IL_00ad:
///Operation Operation 4,23
num = 9;
///Operation Operation 4,24
RichTextBox1.LoadFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,25 Dest:OK
goto IL_00cc;
///Label Label 4,26 2
IL_02f6:
///Operation Operation 4,27
num = 55;
///Operation Operation 4,28
ProjectData.ClearProjectError();
///Operation Operation 4,29
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,30 Dest:OK
goto IL_0315;
///Label Label 4,31 2
IL_00cc:
///Operation Operation 4,32
num = 10;
///Operation Operation 4,33
Interaction.Shell(COND.Aus[7] + "" "" + COND.Verz1 + ""Temp\\Text2.RTF"", AppWinStyle.MaximizedFocus);
///Goto Goto 4,34 Dest:OK
goto end_IL_0000_2;
///Label Label 4,35 3
IL_0315:
///Operation Operation 4,36
num4 = num2;
///Goto Goto 4,37 Dest:OK
goto IL_031d;
///Label Label 4,38 1
IL_031d:
///Operation Operation 4,39
num2 = 0;
///Operation Operation 4,40
switch (num4)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_0015;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_001d;
///Label Label 5,7
case 4:
///Goto Goto 5,8 Dest:OK
goto IL_0073;
///Label Label 5,9
case 6:
///Label Label 5,10
case 8:
///Goto Goto 5,11 Dest:OK
goto IL_008f;
///Label Label 5,12
case 9:
///Goto Goto 5,13 Dest:OK
goto IL_00ad;
///Label Label 5,14
case 10:
///Goto Goto 5,15 Dest:OK
goto IL_00cc;
///Label Label 5,16
case 12:
///Label Label 5,17
case 13:
///Goto Goto 5,18 Dest:OK
goto IL_00f9;
///Label Label 5,19
case 14:
///Goto Goto 5,20 Dest:OK
goto IL_010d;
///Label Label 5,21
case 15:
///Goto Goto 5,22 Dest:OK
goto IL_0117;
///Label Label 5,23
case 18:
///Label Label 5,24
case 19:
///Goto Goto 5,25 Dest:OK
goto IL_0134;
///Label Label 5,26
case 20:
///Goto Goto 5,27 Dest:OK
goto IL_0151;
///Label Label 5,28
case 21:
///Goto Goto 5,29 Dest:OK
goto IL_0178;
///Label Label 5,30
case 22:
///Goto Goto 5,31 Dest:OK
goto IL_0191;
///Label Label 5,32
case 23:
///Goto Goto 5,33 Dest:OK
goto IL_01aa;
///Label Label 5,34
case 24:
///Goto Goto 5,35 Dest:OK
goto IL_01d0;
///Label Label 5,36
case 26:
///Label Label 5,37
case 28:
///Goto Goto 5,38 Dest:OK
goto IL_01f9;
///Label Label 5,39
case 30:
///Label Label 5,40
case 31:
///Goto Goto 5,41 Dest:OK
goto IL_021f;
///Label Label 5,42
case 36:
///Goto Goto 5,43 Dest:OK
goto IL_0248;
///Label Label 5,44
case 38:
///Label Label 5,45
case 39:
///Goto Goto 5,46 Dest:OK
goto IL_0258;
///Label Label 5,47
case 40:
///Goto Goto 5,48 Dest:OK
goto IL_0262;
///Label Label 5,49
case 41:
///Goto Goto 5,50 Dest:OK
goto IL_026c;
///Label Label 5,51
case 42:
///Goto Goto 5,52 Dest:OK
goto IL_027f;
///Label Label 5,53
case 46:
///Goto Goto 5,54 Dest:OK
goto IL_029b;
///Label Label 5,55
case 47:
///Goto Goto 5,56 Dest:OK
goto IL_02a5;
///Label Label 5,57
case 48:
///Goto Goto 5,58 Dest:OK
goto IL_02b4;
///Label Label 5,59
case 51:
///Label Label 5,60
case 52:
///Goto Goto 5,61 Dest:OK
goto IL_02d0;
///Label Label 5,62
case 53:
///Label Label 5,63
case 55:
///Goto Goto 5,64 Dest:OK
goto IL_02f6;
///Label Label 5,65
default:
///Goto Goto 5,66 Dest:OK
goto end_IL_0000;
///Label Label 5,67
case 5:
///Label Label 5,68
case 11:
///Label Label 5,69
case 16:
///Label Label 5,70
case 17:
///Label Label 5,71
case 25:
///Label Label 5,72
case 29:
///Label Label 5,73
case 32:
///Label Label 5,74
case 33:
///Label Label 5,75
case 34:
///Label Label 5,76
case 35:
///Label Label 5,77
case 37:
///Label Label 5,78
case 43:
///Label Label 5,79
case 45:
///Label Label 5,80
case 49:
///Label Label 5,81
case 50:
///Label Label 5,82
case 56:
///Label Label 5,83
case 57:
///Label Label 5,84
case 58:
///Goto Goto 5,85 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,86
}
///Goto Goto 4,41 Dest:OK
goto default;
///Label Label 4,42 2
IL_02a5:
///Operation Operation 4,43
num = 47;
///Operation Operation 4,44
FileSystem.FileClose();
///Goto Goto 4,45 Dest:OK
goto IL_02b4;
///Label Label 4,46 2
IL_02b4:
///Operation Operation 4,47
num = 48;
///Operation Operation 4,48
ProjectData.ClearProjectError();
///Operation Operation 4,49
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,50 Dest:OK
goto IL_0315;
///Label Label 4,51 2
IL_008f:
///Operation Operation 4,52
num = 8;
///Operation Operation 4,53
RichTextBox1.SaveFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,54 Dest:OK
goto IL_00ad;
///Label Label 4,55 2
IL_0262:
///Operation Operation 4,56
num = 40;
///Operation Operation 4,57
prompt = ""Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen."";
///Goto Goto 4,58 Dest:OK
goto IL_026c;
///Label Label 4,59 2
IL_026c:
///Operation Operation 4,60
num = 41;
///Operation Operation 4,61
if (Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, ""Fehler"") == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,62 Dest:OK
goto IL_027f;
///Label Label 4,63 2
IL_027f:
///Operation Operation 4,64
num = 42;
///Operation Operation 4,65
ProjectData.ClearProjectError();
///Operation Operation 4,66
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,67 Dest:OK
goto IL_0315;
///Label Label 4,68 2
IL_0117:
///Operation Operation 4,69
num = 15;
///Operation Operation 4,70
MyProject.Forms.Druck.Show();
///Goto Goto 4,71 Dest:OK
goto end_IL_0000_2;
///Label Label 4,72 2
IL_0015:
///Operation Operation 4,73
ProjectData.ClearProjectError();
///Operation Operation 4,74
num3 = 2;
///Goto Goto 4,75 Dest:OK
goto IL_001d;
///Label Label 4,76 2
IL_001d:
///Operation Operation 4,77
num = 3;
///Operation Operation 4,78
text = ""Datum "" + Strings.Mid(DateAndTime.DateString, 4, 2) + ""."" + DateAndTime.DateString.Left( 2) + ""."" + Strings.Mid(DateAndTime.DateString, 7, 4);
///Goto Goto 4,79 Dest:OK
goto IL_0073;
///Label Label 4,80 2
IL_0073:
///Operation Operation 4,81
num = 4;
///Operation Operation 4,82
switch (index)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_00f9;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_0134;
///Label Label 5,7
default:
///Goto Goto 5,8 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,9
}
///Goto Goto 4,83 Dest:OK
goto IL_008f;
///Label Label 4,84 2
IL_0134:
///Operation Operation 4,85
num = 19;
///Operation Operation 4,86
MyProject.Forms.Hinter.CommonDialog1Save.Filter = ""Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF"";
///Goto Goto 4,87 Dest:OK
goto IL_0151;
///Label Label 4,88 2
IL_0151:
///Operation Operation 4,89
num = 20;
///Operation Operation 4,90
MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = COND.GenPlu + ""list\\"";
///Goto Goto 4,91 Dest:OK
goto IL_0178;
///Label Label 4,92 2
IL_0178:
///Operation Operation 4,93
num = 21;
///Operation Operation 4,94
MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
///Goto Goto 4,95 Dest:OK
goto IL_0191;
///Label Label 4,96 2
IL_0191:
///Operation Operation 4,97
num = 22;
///Operation Operation 4,98
MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
///Goto Goto 4,99 Dest:OK
goto IL_01aa;
///Label Label 4,100 2
IL_01aa:
///Operation Operation 4,101
num = 23;
///Operation Operation 4,102
if (MyProject.Forms.Hinter.CommonDialog1Save.FileName == """")
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,103 Dest:OK
goto IL_01d0;
///Label Label 4,104 2
IL_01d0:
///Operation Operation 4,105
num = 24;
///Operation Operation 4,106
switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_021f;
///Label Label 5,5
default:
///Goto Goto 5,6 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,7
}
///Goto Goto 4,107 Dest:OK
goto IL_01f9;
///Label Label 4,108 2
IL_021f:
///Operation Operation 4,109
num = 31;
///Operation Operation 4,110
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
///Goto Goto 4,111 Dest:OK
goto end_IL_0000_2;
///Label Label 4,112 2
IL_01f9:
///Operation Operation 4,113
num = 28;
///Operation Operation 4,114
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
///Goto Goto 4,115 Dest:OK
goto end_IL_0000_2;
///Label Label 4,116 2
IL_00f9:
///Operation Operation 4,117
num = 13;
///Operation Operation 4,118
RichTextBox1.Text = """";
///Goto Goto 4,119 Dest:OK
goto IL_010d;
///Label Label 4,120 2
IL_010d:
///Operation Operation 4,121
num = 14;
///Operation Operation 4,122
Close();
///Goto Goto 4,123 Dest:OK
goto IL_0117;
///Label Label 4,124 2
end_IL_0000:
///Operation Operation 4,125
break;
///BlockEnd Block 4,126
}
///BlockEnd Block 3,6
}
///Operation Operation 2,2
catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
///BlockStart Block 3,0
{
///Operation Operation 3,1
ProjectData.SetProjectError(obj);
///Operation Operation 3,2
try0000_dispatch = 1043;
///Operation Operation 3,3
continue;
///BlockEnd Block 3,4
}
///Operation Operation 2,3
throw ProjectData.CreateProjectError(-2146828237);
///Operation Operation 2,4
continue;
///Label Label 2,5 9
end_IL_0000_2:
///Operation Operation 2,6
break;
///BlockEnd Block 2,7
}
///Operation Operation 1,9
if (num2 != 0)
///BlockStart Block 2,0
{
///Operation Operation 2,1
ProjectData.ClearProjectError();
///BlockEnd Block 2,2
}
///BlockEnd Block 1,10
}";
    public const string testDataDelete2Exp = @"///Declaration MainBlock 0,0
private void Befehl_Click(object eventSender, EventArgs eventArgs)
///BlockStart Block 1,0
{
///Operation Operation 1,1
int try0000_dispatch = -1;
///Operation Operation 1,2
int num = default(int);
///Operation Operation 1,3
short index = default(short);
///Operation Operation 1,4
int num2 = default(int);
///Operation Operation 1,5
int num3 = default(int);
///Operation Operation 1,6
int number = default(int);
///Operation Operation 1,7
string prompt = default(string);
///Operation Operation 1,8
while (true)
///BlockStart Block 2,0
{
///Operation Operation 2,1
try
///BlockStart Block 3,0
{
///Comment Comment 3,1
/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/
///Operation Operation 3,2
;
///Operation Operation 3,3
int num4;
///Operation Operation 3,4
string text;
///Operation Operation 3,5
switch (try0000_dispatch)
///BlockStart Block 4,0
{
///Label Label 4,1 1
default:
///Operation Operation 4,2
num = 1;
///Operation Operation 4,3
index = Befehl.GetIndex((Button)eventSender);
///Goto Goto 4,4 Dest:OK
goto IL_0015;
///Label Label 4,5
case 1043:
///BlockStart Block 5,0
{
///Operation Operation 5,1
num2 = num;
///Operation Operation 5,2
switch (num3)
///BlockStart Block 6,0
{
///Label Label 6,1
case 2:
///Operation Operation 6,2
break;
///Label Label 6,3
case 1:
///Goto Goto 6,4 Dest:OK
goto IL_0319;
///Label Label 6,5
default:
///Goto Goto 6,6 Dest:OK
goto end_IL_0000;
///BlockEnd Block 6,7
}
///Goto Goto 5,3
goto IL_0248;
///BlockEnd Block 5,4
}
///Label Label 4,6 1
IL_0319:
///Operation Operation 4,7
num4 = num2 + 1;
///Goto Goto 4,8 Dest:OK
goto IL_031d;
///Label Label 4,9 1
IL_0258:
///Operation Operation 4,10
num = 39;
///Operation Operation 4,11
if (number == 25)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_0262;
///BlockEnd Block 5,2
}
///Goto Goto 4,12 Dest:OK
goto IL_029b;
///Label Label 4,13 2
IL_029b:
///Operation Operation 4,14
num = 46;
///Operation Operation 4,15
if (number == 55)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto IL_02a5;
///BlockEnd Block 5,2
}
///Goto Goto 4,16 Dest:OK
goto IL_02d0;
///Label Label 4,17 2
IL_02d0:
///Operation Operation 4,18
num = 52;
///Operation Operation 4,19
if (Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Operation Operation 5,1
ProjectData.EndApp();
///BlockEnd Block 5,2
}
///Goto Goto 4,20 Dest:OK
goto IL_02f6;
///Label Label 4,21 2
IL_00ad:
///Operation Operation 4,22
num = 9;
///Operation Operation 4,23
RichTextBox1.LoadFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,24 Dest:OK
goto IL_00cc;
///Label Label 4,25 2
IL_02f6:
///Operation Operation 4,26
num = 55;
///Operation Operation 4,27
ProjectData.ClearProjectError();
///Operation Operation 4,28
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,29 Dest:OK
goto IL_0315;
///Label Label 4,30 2
IL_00cc:
///Operation Operation 4,31
num = 10;
///Operation Operation 4,32
Interaction.Shell(COND.Aus[7] + "" "" + COND.Verz1 + ""Temp\\Text2.RTF"", AppWinStyle.MaximizedFocus);
///Goto Goto 4,33 Dest:OK
goto end_IL_0000_2;
///Label Label 4,34 3
IL_0315:
///Operation Operation 4,35
num4 = num2;
///Goto Goto 4,36 Dest:OK
goto IL_031d;
///Label Label 4,37 2
IL_031d:
///Operation Operation 4,38
num2 = 0;
///Operation Operation 4,39
switch (num4)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_0015;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_001d;
///Label Label 5,7
case 4:
///Goto Goto 5,8 Dest:OK
goto IL_0073;
///Label Label 5,9
case 6:
///Label Label 5,10
case 8:
///Goto Goto 5,11 Dest:OK
goto IL_008f;
///Label Label 5,12
case 9:
///Goto Goto 5,13 Dest:OK
goto IL_00ad;
///Label Label 5,14
case 10:
///Goto Goto 5,15 Dest:OK
goto IL_00cc;
///Label Label 5,16
case 12:
///Label Label 5,17
case 13:
///Goto Goto 5,18 Dest:OK
goto IL_00f9;
///Label Label 5,19
case 14:
///Goto Goto 5,20 Dest:OK
goto IL_010d;
///Label Label 5,21
case 15:
///Goto Goto 5,22 Dest:OK
goto IL_0117;
///Label Label 5,23
case 18:
///Label Label 5,24
case 19:
///Goto Goto 5,25 Dest:OK
goto IL_0134;
///Label Label 5,26
case 20:
///Goto Goto 5,27 Dest:OK
goto IL_0151;
///Label Label 5,28
case 21:
///Goto Goto 5,29 Dest:OK
goto IL_0178;
///Label Label 5,30
case 22:
///Goto Goto 5,31 Dest:OK
goto IL_0191;
///Label Label 5,32
case 23:
///Goto Goto 5,33 Dest:OK
goto IL_01aa;
///Label Label 5,34
case 24:
///Goto Goto 5,35 Dest:OK
goto IL_01d0;
///Label Label 5,36
case 26:
///Label Label 5,37
case 28:
///Goto Goto 5,38 Dest:OK
goto IL_01f9;
///Label Label 5,39
case 30:
///Label Label 5,40
case 31:
///Goto Goto 5,41 Dest:OK
goto IL_021f;
///Label Label 5,42
case 36:
///Goto Goto 5,43
goto IL_0248;
///Label Label 5,44
case 38:
///Label Label 5,45
case 39:
///Goto Goto 5,46 Dest:OK
goto IL_0258;
///Label Label 5,47
case 40:
///Goto Goto 5,48 Dest:OK
goto IL_0262;
///Label Label 5,49
case 41:
///Goto Goto 5,50 Dest:OK
goto IL_026c;
///Label Label 5,51
case 42:
///Goto Goto 5,52 Dest:OK
goto IL_027f;
///Label Label 5,53
case 46:
///Goto Goto 5,54 Dest:OK
goto IL_029b;
///Label Label 5,55
case 47:
///Goto Goto 5,56 Dest:OK
goto IL_02a5;
///Label Label 5,57
case 48:
///Goto Goto 5,58 Dest:OK
goto IL_02b4;
///Label Label 5,59
case 51:
///Label Label 5,60
case 52:
///Goto Goto 5,61 Dest:OK
goto IL_02d0;
///Label Label 5,62
case 53:
///Label Label 5,63
case 55:
///Goto Goto 5,64 Dest:OK
goto IL_02f6;
///Label Label 5,65
default:
///Goto Goto 5,66 Dest:OK
goto end_IL_0000;
///Label Label 5,67
case 5:
///Label Label 5,68
case 11:
///Label Label 5,69
case 16:
///Label Label 5,70
case 17:
///Label Label 5,71
case 25:
///Label Label 5,72
case 29:
///Label Label 5,73
case 32:
///Label Label 5,74
case 33:
///Label Label 5,75
case 34:
///Label Label 5,76
case 35:
///Label Label 5,77
case 37:
///Label Label 5,78
case 43:
///Label Label 5,79
case 45:
///Label Label 5,80
case 49:
///Label Label 5,81
case 50:
///Label Label 5,82
case 56:
///Label Label 5,83
case 57:
///Label Label 5,84
case 58:
///Goto Goto 5,85 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,86
}
///Goto Goto 4,40 Dest:OK
goto default;
///Label Label 4,41 2
IL_02a5:
///Operation Operation 4,42
num = 47;
///Operation Operation 4,43
FileSystem.FileClose();
///Goto Goto 4,44 Dest:OK
goto IL_02b4;
///Label Label 4,45 2
IL_02b4:
///Operation Operation 4,46
num = 48;
///Operation Operation 4,47
ProjectData.ClearProjectError();
///Operation Operation 4,48
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,49 Dest:OK
goto IL_0315;
///Label Label 4,50 2
IL_008f:
///Operation Operation 4,51
num = 8;
///Operation Operation 4,52
RichTextBox1.SaveFile(COND.Verz1 + ""TEMP\\Text2.RTF"", RichTextBoxStreamType.RichText);
///Goto Goto 4,53 Dest:OK
goto IL_00ad;
///Label Label 4,54 2
IL_0262:
///Operation Operation 4,55
num = 40;
///Operation Operation 4,56
prompt = ""Das angegebene Gerät ist nicht bereit.\rBitte einschalten oder abbrechen."";
///Goto Goto 4,57 Dest:OK
goto IL_026c;
///Label Label 4,58 2
IL_026c:
///Operation Operation 4,59
num = 41;
///Operation Operation 4,60
if (Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel, ""Fehler"") == MsgBoxResult.Cancel)
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,61 Dest:OK
goto IL_027f;
///Label Label 4,62 2
IL_027f:
///Operation Operation 4,63
num = 42;
///Operation Operation 4,64
ProjectData.ClearProjectError();
///Operation Operation 4,65
if (num2 == 0)
///BlockStart Block 5,0
{
///Operation Operation 5,1
throw ProjectData.CreateProjectError(-2146828268);
///BlockEnd Block 5,2
}
///Goto Goto 4,66 Dest:OK
goto IL_0315;
///Label Label 4,67 2
IL_0117:
///Operation Operation 4,68
num = 15;
///Operation Operation 4,69
MyProject.Forms.Druck.Show();
///Goto Goto 4,70 Dest:OK
goto end_IL_0000_2;
///Label Label 4,71 2
IL_0015:
///Operation Operation 4,72
ProjectData.ClearProjectError();
///Operation Operation 4,73
num3 = 2;
///Goto Goto 4,74 Dest:OK
goto IL_001d;
///Label Label 4,75 2
IL_001d:
///Operation Operation 4,76
num = 3;
///Operation Operation 4,77
text = ""Datum "" + Strings.Mid(DateAndTime.DateString, 4, 2) + ""."" + DateAndTime.DateString.Left( 2) + ""."" + Strings.Mid(DateAndTime.DateString, 7, 4);
///Goto Goto 4,78 Dest:OK
goto IL_0073;
///Label Label 4,79 2
IL_0073:
///Operation Operation 4,80
num = 4;
///Operation Operation 4,81
switch (index)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_00f9;
///Label Label 5,5
case 3:
///Goto Goto 5,6 Dest:OK
goto IL_0134;
///Label Label 5,7
default:
///Goto Goto 5,8 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,9
}
///Goto Goto 4,82 Dest:OK
goto IL_008f;
///Label Label 4,83 2
IL_0134:
///Operation Operation 4,84
num = 19;
///Operation Operation 4,85
MyProject.Forms.Hinter.CommonDialog1Save.Filter = ""Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF"";
///Goto Goto 4,86 Dest:OK
goto IL_0151;
///Label Label 4,87 2
IL_0151:
///Operation Operation 4,88
num = 20;
///Operation Operation 4,89
MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = COND.GenPlu + ""list\\"";
///Goto Goto 4,90 Dest:OK
goto IL_0178;
///Label Label 4,91 2
IL_0178:
///Operation Operation 4,92
num = 21;
///Operation Operation 4,93
MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;
///Goto Goto 4,94 Dest:OK
goto IL_0191;
///Label Label 4,95 2
IL_0191:
///Operation Operation 4,96
num = 22;
///Operation Operation 4,97
MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();
///Goto Goto 4,98 Dest:OK
goto IL_01aa;
///Label Label 4,99 2
IL_01aa:
///Operation Operation 4,100
num = 23;
///Operation Operation 4,101
if (MyProject.Forms.Hinter.CommonDialog1Save.FileName == """")
///BlockStart Block 5,0
{
///Goto Goto 5,1 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,2
}
///Goto Goto 4,102 Dest:OK
goto IL_01d0;
///Label Label 4,103 2
IL_01d0:
///Operation Operation 4,104
num = 24;
///Operation Operation 4,105
switch (MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)
///BlockStart Block 5,0
{
///Label Label 5,1
case 1:
///Operation Operation 5,2
break;
///Label Label 5,3
case 2:
///Goto Goto 5,4 Dest:OK
goto IL_021f;
///Label Label 5,5
default:
///Goto Goto 5,6 Dest:OK
goto end_IL_0000_2;
///BlockEnd Block 5,7
}
///Goto Goto 4,106 Dest:OK
goto IL_01f9;
///Label Label 4,107 2
IL_021f:
///Operation Operation 4,108
num = 31;
///Operation Operation 4,109
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);
///Goto Goto 4,110 Dest:OK
goto end_IL_0000_2;
///Label Label 4,111 2
IL_01f9:
///Operation Operation 4,112
num = 28;
///Operation Operation 4,113
RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);
///Goto Goto 4,114 Dest:OK
goto end_IL_0000_2;
///Label Label 4,115 2
IL_00f9:
///Operation Operation 4,116
num = 13;
///Operation Operation 4,117
RichTextBox1.Text = """";
///Goto Goto 4,118 Dest:OK
goto IL_010d;
///Label Label 4,119 2
IL_010d:
///Operation Operation 4,120
num = 14;
///Operation Operation 4,121
Close();
///Goto Goto 4,122 Dest:OK
goto IL_0117;
///Label Label 4,123 2
end_IL_0000:
///Operation Operation 4,124
break;
///BlockEnd Block 4,125
}
///BlockEnd Block 3,6
}
///Operation Operation 2,2
catch (Exception obj) when (obj is not null && num3 != 0 && num2 == 0)
///BlockStart Block 3,0
{
///Operation Operation 3,1
ProjectData.SetProjectError(obj);
///Operation Operation 3,2
try0000_dispatch = 1043;
///Operation Operation 3,3
continue;
///BlockEnd Block 3,4
}
///Operation Operation 2,3
throw ProjectData.CreateProjectError(-2146828237);
///Operation Operation 2,4
continue;
///Label Label 2,5 9
end_IL_0000_2:
///Operation Operation 2,6
break;
///BlockEnd Block 2,7
}
///Operation Operation 1,9
if (num2 != 0)
///BlockStart Block 2,0
{
///Operation Operation 2,1
ProjectData.ClearProjectError();
///BlockEnd Block 2,2
}
///BlockEnd Block 1,10
}";
    //  public static string TestDataExpReorder0 =@"";
    public static string testDataExpReorder1 { get; } = Resources.Test1ExpParseSL;

    public static string testDataExpReorder2 { get; } = Resources.Test2ExpParseSL;
    public static string testDataExpReorder3 { get; } = @"///Declaration MainBlock 0,0
public void Test3()
///BlockStart Block 1,0
{
///Operation Operation 1,1
Modul1.UbgT = (Text2[0].Text).Trim();
///Goto Goto 1,2 Dest:OK
goto IL_105c;
///Label Label 1,3 1
IL_105c:
///Operation Operation 1,4
num = 209;
///Operation Operation 1,5
if (Modul1.UbgT == """")
///BlockStart Block 2,0
{
///Goto Goto 2,1 Dest:OK
goto IL_107c;
///BlockEnd Block 2,2
}
///Operation Operation 1,6
else
///BlockStart Block 2,0
{
///Operation Operation 2,1
test = @""""""Test"""""";
///Operation Operation 2,2
test2 = $""\""{test}\"""";
///Operation Operation 2,3
test3 = $""{{test}}"";
///Goto Goto 2,4 Dest:OK
goto IL_108d;
///BlockEnd Block 2,5
}
///Comment LComment 1,7
//=================
///Label Label 1,8 1
IL_107c:
///Operation Operation 1,9
num = 210;
///Operation Operation 1,10
Modul1.UbgT = ""\"""";
///Goto Goto 1,11 Dest:OK
goto IL_108d;
///Label Label 1,12 2
IL_108d:
///Operation Operation 1,13
num = 212;
///Operation Operation 1,14
DataModul.DB_PersonTable.Seek("">"", Modul1.UbgT, Modul1.PersInArb);
///BlockEnd Block 1,15
}";
    public static string testDataExpReorder4 { get; } = @"///Declaration MainBlock 0,0
public void Test4()
///BlockStart Block 1,0
{
///Goto Goto 1,1 Dest:OK
goto IL_105c;
///Label Label 1,2 1
IL_105c:
///Operation Operation 1,3
num = 209;
///Operation Operation 1,4
Test(test: ""Some Test"");
///Goto Goto 1,5 Dest:OK
goto IL_107c;
///Comment LComment 1,6
//=================
///Label Label 1,7 1
IL_107c:
///Operation Operation 1,8
num = 210;
///Operation Operation 1,9
Modul1.UbgT = ""\"""";
///Goto Goto 1,10 Dest:OK
goto IL_108d;
///Label Label 1,11 1
IL_108d:
///Operation Operation 1,12
num = 212;
///BlockEnd Block 1,13
}";
    public static string testDataExpReorder7 { get; } = @"///Declaration MainBlock 0,0
public void Test4()
///BlockStart Block 1,0
{
///Goto Goto 1,1 Dest:OK
goto IL_105c;
///Label Label 1,2 1
IL_105c:
///Operation Operation 1,3
num = 209;
///Operation Operation 1,4
Test(test: ""Some Test"");
///Goto Goto 1,5 Dest:OK
goto IL_107c;
///Comment LComment 1,6
//=================
///Label Label 1,7 1
IL_107c:
///Operation Operation 1,8
num = 210;
///Operation Operation 1,9
Modul1.UbgT = ""\"""";
///Goto Goto 1,10 Dest:OK
goto IL_108d;
///Label Label 1,11 1
IL_108d:
///Operation Operation 1,12
num = 212;
///BlockEnd Block 1,13
}";

    public static string testDataExpReorder9 { get; } = Resources.Test9ExpParseSL;


    public static string testDataExpRemoveL0 { get; } = @"///Declaration MainBlock 0,0
public const string TestData()
///BlockStart Block 1,0
{
///Comment LComment 1,1
// Discarded unreachable code: IL_0085
///Operation Operation 1,2
string test = ""test"";
///Operation Operation 1,3
Test(test: ""Some Test"");
///Operation Operation 1,4
switch (test)
///BlockStart Block 2,0
{
///Operation Label 2,1
case ""test"":
///Operation Label 2,2
case ""test2"":
///Operation Operation 2,3
break;
///Label Label 2,4
default:
///Goto Goto 2,5 Dest:OK
goto IL_0002;
///BlockEnd Block 2,6
}
///Goto Goto 1,5 Dest:OK
goto IL_0002;
///Label Label 1,6 2
IL_0002:
///Operation Operation 1,7
return test;
///BlockEnd Block 1,8
}";
    public static string testDataExpRemoveL1 { get; } = Resources.Test1ExpParseRL;
    public static string testDataExpRemoveL2 { get; } = Resources.Test2ExpParseRL;
    public static string testDataExpRemoveL3 { get; } = @"///Declaration MainBlock 0,0
public void Test3()
///BlockStart Block 1,0
{
///Operation Operation 1,1
Modul1.UbgT = (Text2[0].Text).Trim();
///Operation Operation 1,2
if (Modul1.UbgT == """")
///BlockStart Block 2,0
{
///Operation Operation 2,1
Modul1.UbgT = ""\"""";
///BlockEnd Block 2,2
}
///Operation Operation 1,3
else
///BlockStart Block 2,0
{
///Operation Operation 2,1
test = @""""""Test"""""";
///Operation Operation 2,2
test2 = $""\""{test}\"""";
///Operation Operation 2,3
test3 = $""{{test}}"";
///BlockEnd Block 2,4
}
///Operation Operation 1,4
DataModul.DB_PersonTable.Seek("">"", Modul1.UbgT, Modul1.PersInArb);
///BlockEnd Block 1,5
}";
    public static string testDataExpRemoveL4 { get; } = @"///Declaration MainBlock 0,0
public void Test4()
///BlockStart Block 1,0
{
///Operation Operation 1,1
Test(test: ""Some Test"");
///Operation Operation 1,2
Modul1.UbgT = ""\"""";
///BlockEnd Block 1,3
}";
    public static string testDataExpRemoveL7 { get; } = @"///Declaration MainBlock 0,0
public void Test7()
///BlockStart Block 1,0
{
///Operation Operation 1,1
if (true)
///Start Block 2,0
{
///Operation Operation 2,1
i++;
///End Block 2,2
}
///Operation Operation 1,2
else
///Start Block 2,0
{
///Operation Operation 2,1
i--;
///End Block 2,2
}
///Operation Operation 1,3
return;
///BlockEnd Block 1,4
}";
    public static string testDataExpRemoveL9 { get; } = Resources.Test9ExpParseRL;
    public static string testDataExpRemoveL12 { get; } = Resources.Test12ExpParseRL;
    public static string testDataExpRemoveL13 { get; } = Resources.Test13ExpParseRL;
    #endregion

    #region Expected data for Tokenizer
    public const string cExpLog0 = @"T:Operation,0,public const string TestData()
T:Block,1,{
T:Operation,1,
T:LComment,1,// Discarded unreachable code: IL_0085
T:Operation,1,string test =
T:String,1,""test""
T:Operation,1,;
T:Goto,1,goto IL_0001;
T:Operation,1,
T:Comment,1,/* Only one IL_0001 is allowed */
T:Label,1,IL_0001:
T:Label,1,Test(test:
T:Operation,1,
T:String,1,""Some Test""
T:Operation,1,);
T:Operation,1,switch
T:Operation,1,(test)
T:Block,2,{
T:Operation,2,case
T:String,2,""test""
T:Label,2,:
T:Operation,2,case
T:String,2,""test2""
T:Label,2,:
T:Operation,2,break;
T:Label,2,default:
T:Goto,2,goto IL_0002;
T:Block,2,}
T:Goto,1,goto IL_0002;
T:Label,1,IL_0002:
T:Operation,1,return
T:Operation,1,test;
T:Block,1,}
";
    public static string cExp1Log { get; } = Resources.Test1ExpTokenize;
    public static string cExp2Log { get; } = Resources.Test2ExpTokenize;
    public const string cExpLog3 = @"T:Operation,0,public void Test3()
T:Block,1,{
T:Operation,1,Modul1.UbgT = (Text2[0].Text).Trim();
T:Goto,1,goto IL_105c;
T:Label,1,IL_105c:
T:Operation,1,num = 209;
T:Operation,1,if
T:Operation,1,(Modul1.UbgT ==
T:String,1,""""
T:Operation,1,)
T:Block,2,{
T:Goto,2,goto IL_107c;
T:Block,2,}
T:Operation,1,else
T:Block,2,{
T:Operation,2,test =
T:String,2,@""""""Test""""""
T:Operation,2,;
T:Operation,2,test2 =
T:String,2,$""\""{test}\""""
T:Operation,2,;
T:Operation,2,test3 =
T:String,2,$""{{test}}""
T:Operation,2,;
T:Goto,2,goto IL_108d;
T:Block,2,}
T:Operation,1,
T:LComment,1,//=================
T:Label,1,IL_107c:
T:Operation,1,num = 210;
T:Operation,1,Modul1.UbgT =
T:String,1,""\""""
T:Operation,1,;
T:Goto,1,goto IL_108d;
T:Label,1,IL_108d:
T:Operation,1,num = 212;
T:Operation,1,DataModul.DB_PersonTable.Seek(
T:String,1,"">""
T:Operation,1,, Modul1.UbgT, Modul1.PersInArb);
T:Block,1,}
";
    public const string cExpLog4 = @"T:Operation,0,public void Test4()
T:Block,1,{
T:Goto,1,goto IL_105c;
T:Label,1,IL_105c:
T:Operation,1,num = 209;
T:Label,1,Test(test:
T:Operation,1,
T:String,1,""Some Test""
T:Operation,1,);
T:Goto,1,goto IL_107c;
T:Operation,1,
T:LComment,1,//=================
T:Label,1,IL_107c:
T:Operation,1,num = 210;
T:Operation,1,Modul1.UbgT =
T:String,1,""\""""
T:Operation,1,;
T:Goto,1,goto IL_108d;
T:Label,1,IL_108d:
T:Operation,1,num = 212;
T:Block,1,}
";
    public const string cExpLog5 = @"T:Operation,0,public void Test5()
T:Block,1,{
T:Operation,1,test =
T:String,1,""Some unended string ... ;
T:Operation,1,test2 =
T:String,1,$""Some {(test.Length()>5?@""\"""":""7"")} nested string ...""
T:Operation,1,;
T:Operation,1,test3 =
T:String,1,""Some broken""
T:Operation,1,
T:Comment,1,/* some comment */
T:Operation,1,+
T:String,1,"" string ...""
T:Operation,1,;
T:Operation,1,test3 =
T:String,1,""Another broken""
T:Operation,1,
T:LComment,1,// some other comment
T:Operation,1,+
T:String,1,"" string ...""
T:Operation,1,;
T:Block,1,}
";
    public const string cExpLog6 = @"T:Operation,0,public void Test6()
T:Block,1,{
T:Operation,1,if
T:Operation,1,(true)
T:Block,2,{
T:Goto,2,goto IL_105c;
T:Block,2,}
T:Operation,1,else
T:Goto,1,goto IL_107c;
T:Label,1,IL_105c:
T:Goto,1,goto IL_107c;
T:Label,1,IL_107c:
T:Operation,1,return;
T:Block,1,}
";
    public const string cExpLog7 = @"T:Operation,0,public void Test7()
T:Block,1,{
T:Operation,1,if
T:Operation,1,(true)
T:Goto,1,goto IL_105c;
T:Operation,1,else
T:Goto,1,goto IL_107c;
T:Label,1,IL_105c:
T:Operation,1,num = 209;
T:Operation,1,i++;
T:Goto,1,goto IL_108c;
T:Label,1,IL_107c:
T:Operation,1,num = 210;
T:Operation,1,i--;
T:Goto,1,goto IL_108c;
T:Label,1,IL_108c:
T:Operation,1,num = 212;
T:Operation,1,return;
T:Block,1,}
";
    public static readonly string cExp8Log = Resources.Test8ExpTokenize;
    public static readonly string cExp9Log = Resources.Test9ExpTokenize;
    public static readonly string cExp10Log = Resources.Test10ExpTokenize;
    public static readonly string cExp11Log = Resources.Test11ExpTokenize;
    public static readonly string cExp12Log = Resources.Test12ExpTokenize;
    public static readonly string cExp13Log = Resources.Test13ExpTokenize;

    #endregion

    #region Expected code (unchanged)
    public const string cExpCode0 = @"    public const string TestData()
    {
        // Discarded unreachable code: IL_0085
        string test = ""test"";
        goto IL_0001;
        /* Only one IL_0001 is allowed */
    IL_0001:
        Test(test: ""Some Test"");
        switch (test)
        {
        case ""test"":
        case ""test2"":
            break;
        default:
            goto IL_0002;
        }
        goto IL_0002;
    IL_0002:
        return test;
    }";
    public static string cExpCode1 { get; } = Resources.Test1ExpCode;
    public static string cExpCode2 { get; } = Resources.Test2ExpCode;
    public const string cExpCode3 = @"    public void Test3()
    {
        Modul1.UbgT = (Text2[0].Text).Trim();
        goto IL_105c;
    IL_105c:
        num = 209;
        if (Modul1.UbgT == """")
        {
            goto IL_107c;
        }
        else
        {
            test = @""""""Test"""""";
            test2 = $""\""{test}\"""";
            test3 = $""{{test}}"";
            goto IL_108d;
        }
        //=================
    IL_107c:
        num = 210;
        Modul1.UbgT = ""\"""";
        goto IL_108d;
    IL_108d:
        num = 212;
        DataModul.DB_PersonTable.Seek("">"", Modul1.UbgT, Modul1.PersInArb);
    }";
    public const string cExpCode4 = @"    public void Test4()
    {
        goto IL_105c;
    IL_105c:
        num = 209;
        Test(test: ""Some Test"");
        goto IL_107c;
        //=================
    IL_107c:
        num = 210;
        Modul1.UbgT = ""\"""";
        goto IL_108d;
    IL_108d:
        num = 212;
    }";
    public const string cExpCode5 = @"    public void Test5()
    {
        test = ""Some unended string ... ;
        test2 = $""Some {(test.Length()>5?@""\"""":""7"")} nested string ..."";
        test3 = ""Some broken""
        /* some comment */
        + "" string ..."";
        test3 = ""Another broken""
        // some other comment
        + "" string ..."";
    }";
    public const string cExpCode6 = @"    public void Test6()
    {
        if (true)
        {
            goto IL_105c;
        }
        else
        goto IL_107c;
    IL_105c:
        goto IL_107c;
    IL_107c:
        return;
    }";
    public const string cExpCode7 = @"    public void Test7()
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
    }";
    public static string cExpCode8 { get; } = Resources.Test8ExpCode;
    public static string cExpCode9 { get; } = Resources.Test9ExpCode;
    public static string cExpCode10 { get; } = Resources.Test10ExpCode;
    public static string cExpCode11 { get; } = Resources.Test11ExpCode;
    public static string cExpCode12 { get; } = Resources.Test12ExpCode;
    public static string cExpCode13 { get; } = Resources.Test13ExpCode;
    #endregion
    private static object? ReadObject(byte[] JsonData) => new DataContractJsonSerializer(typeof(List<TokenData>)).ReadObject(new MemoryStream(JsonData));

    #region Intermediste token-list
    public static object TestDataList0() => new List<TokenData>(){
    ("public const string TestData()", CodeBlockType.Operation, 0),
    ("{", CodeBlockType.Block, 1),
    ("", CodeBlockType.Operation, 1),
    ("// Discarded unreachable code: IL_0085", CodeBlockType.LComment, 1),
    ("string test =", CodeBlockType.Operation, 1),
    ("\"test\"", CodeBlockType.String, 1),
    (";", CodeBlockType.Operation, 1),
    ("goto IL_0001;", CodeBlockType.Goto, 1),
    ("", CodeBlockType.Operation, 1),
    ("/* Only one IL_0001 is allowed */", CodeBlockType.Comment, 1),
    ("IL_0001:", CodeBlockType.Label, 1),
    ("Test(test:", CodeBlockType.Label, 1),
    ("", CodeBlockType.Operation, 1),
    ("\"Some Test\"", CodeBlockType.String, 1),
    (");", CodeBlockType.Operation, 1),
    ("switch", CodeBlockType.Operation, 1),
("(test)", CodeBlockType.Operation, 1),
    ("{", CodeBlockType.Block, 2),
    ("case", CodeBlockType.Operation, 2),
    ("\"test\"", CodeBlockType.String, 2),
    (":", CodeBlockType.Label, 2),
    ("case", CodeBlockType.Operation, 2),
    ("\"test2\"", CodeBlockType.String, 2),
    (":", CodeBlockType.Label, 2),
    ("break;", CodeBlockType.Operation, 2),
    ("default:", CodeBlockType.Label, 2),
    ("goto IL_0002;", CodeBlockType.Goto, 2),
    ("}", CodeBlockType.Block, 2),
    ("goto IL_0002;", CodeBlockType.Goto, 1),
    ("IL_0002:", CodeBlockType.Label, 1),
    ("return", CodeBlockType.Operation, 1),
("test;", CodeBlockType.Operation, 1),
    ("}", CodeBlockType.Block, 1),};
    public static object TestDataList1_org() => new List<TokenData>() {
    ("private void Befehl_Click(object eventSender, EventArgs eventArgs)", CodeBlockType.Operation, 0),
    ("{", CodeBlockType.Block, 1),
    ("int try0000_dispatch = -1;", CodeBlockType.Operation, 1),
    ("int num = default(int);", CodeBlockType.Operation, 1),
    ("short index = default(short);", CodeBlockType.Operation, 1),
    ("int num2 = default(int);", CodeBlockType.Operation, 1),
    ("int num3 = default(int);", CodeBlockType.Operation, 1),
    ("int number = default(int);", CodeBlockType.Operation, 1),
    ("string prompt = default(string);", CodeBlockType.Operation, 1),
    ("while", CodeBlockType.Operation, 1),
("(true)", CodeBlockType.Operation, 1),
    ("{", CodeBlockType.Block, 2),
    ("try", CodeBlockType.Operation, 2),
    ("{", CodeBlockType.Block, 3),
    ("", CodeBlockType.Operation, 3),
    ("/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/", CodeBlockType.Comment, 3),
    (";", CodeBlockType.Operation, 3),
    ("int num4;", CodeBlockType.Operation, 3),
    ("string text;", CodeBlockType.Operation, 3),
    ("switch", CodeBlockType.Operation, 3),
("(try0000_dispatch)", CodeBlockType.Operation, 3),
    ("{", CodeBlockType.Block, 4),
    ("default:", CodeBlockType.Label, 4),
    ("num = 1;", CodeBlockType.Operation, 4),
    ("index = Befehl.GetIndex((Button)eventSender);", CodeBlockType.Operation, 4),
    ("goto IL_0015;", CodeBlockType.Goto, 4),
    ("case 1043:", CodeBlockType.Label, 4),
    ("{", CodeBlockType.Block, 5),
    ("num2 = num;", CodeBlockType.Operation, 5),
    ("switch", CodeBlockType.Operation, 5),
("(num3)", CodeBlockType.Operation, 5),
    ("{", CodeBlockType.Block, 6),
    ("case 2:", CodeBlockType.Label, 6),
    ("break;", CodeBlockType.Operation, 6),
    ("case 1:", CodeBlockType.Label, 6),
    ("goto IL_0319;", CodeBlockType.Goto, 6),
    ("default:", CodeBlockType.Label, 6),
    ("goto end_IL_0000;", CodeBlockType.Goto, 6),
    ("}", CodeBlockType.Block, 6),
    ("goto IL_0248;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("IL_0319:", CodeBlockType.Label, 4),
    ("num4 = num2 + 1;", CodeBlockType.Operation, 4),
    ("goto IL_031d;", CodeBlockType.Goto, 4),
    ("IL_0248:", CodeBlockType.Label, 4),
    ("num = 36;", CodeBlockType.Operation, 4),
    ("number = Information.Err().Number;", CodeBlockType.Operation, 4),
    ("goto IL_0258;", CodeBlockType.Goto, 4),
    ("IL_0258:", CodeBlockType.Label, 4),
    ("num = 39;", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(number == 25)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("goto IL_0262;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_029b;", CodeBlockType.Goto, 4),
    ("IL_029b:", CodeBlockType.Label, 4),
    ("num = 46;", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(number == 55)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("goto IL_02a5;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_02d0;", CodeBlockType.Goto, 4),
    ("IL_02d0:", CodeBlockType.Label, 4),
    ("num = 52;", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(Interaction.MsgBox(Conversion.ErrorToString(), MsgBoxStyle.OkCancel, (Information.Err().Number).AsString()) == MsgBoxResult.Cancel)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("ProjectData.EndApp();", CodeBlockType.Operation, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_02f6;", CodeBlockType.Goto, 4),
    ("IL_00ad:", CodeBlockType.Label, 4),
    ("num = 9;", CodeBlockType.Operation, 4),
    ("RichTextBox1.LoadFile(COND.Verz1 +", CodeBlockType.Operation, 4),
    ("\"TEMP\\\\Text2.RTF\"", CodeBlockType.String, 4),
    (", RichTextBoxStreamType.RichText);", CodeBlockType.Operation, 4),
    ("goto IL_00cc;", CodeBlockType.Goto, 4),
    ("IL_02f6:", CodeBlockType.Label, 4),
    ("num = 55;", CodeBlockType.Operation, 4),
    ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(num2 == 0)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("throw", CodeBlockType.Operation, 5),
("ProjectData.CreateProjectError(-2146828268);", CodeBlockType.Operation, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_0315;", CodeBlockType.Goto, 4),
    ("IL_00cc:", CodeBlockType.Label, 4),
    ("num = 10;", CodeBlockType.Operation, 4),
    ("Interaction.Shell(COND.Aus[7] +", CodeBlockType.Operation, 4),
    ("\" \"", CodeBlockType.String, 4),
    ("+ COND.Verz1 +", CodeBlockType.Operation, 4),
    ("\"Temp\\\\Text2.RTF\"", CodeBlockType.String, 4),
    (", AppWinStyle.MaximizedFocus);", CodeBlockType.Operation, 4),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 4),
    ("IL_0315:", CodeBlockType.Label, 4),
    ("num4 = num2;", CodeBlockType.Operation, 4),
    ("goto IL_031d;", CodeBlockType.Goto, 4),
    ("IL_031d:", CodeBlockType.Label, 4),
    ("num2 = 0;", CodeBlockType.Operation, 4),
    ("switch", CodeBlockType.Operation, 4),
("(num4)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("case 1:", CodeBlockType.Label, 5),
    ("break;", CodeBlockType.Operation, 5),
    ("case 2:", CodeBlockType.Label, 5),
    ("goto IL_0015;", CodeBlockType.Goto, 5),
    ("case 3:", CodeBlockType.Label, 5),
    ("goto IL_001d;", CodeBlockType.Goto, 5),
    ("case 4:", CodeBlockType.Label, 5),
    ("goto IL_0073;", CodeBlockType.Goto, 5),
    ("case 6:", CodeBlockType.Label, 5),
    ("case 8:", CodeBlockType.Label, 5),
    ("goto IL_008f;", CodeBlockType.Goto, 5),
    ("case 9:", CodeBlockType.Label, 5),
    ("goto IL_00ad;", CodeBlockType.Goto, 5),
    ("case 10:", CodeBlockType.Label, 5),
    ("goto IL_00cc;", CodeBlockType.Goto, 5),
    ("case 12:", CodeBlockType.Label, 5),
    ("case 13:", CodeBlockType.Label, 5),
    ("goto IL_00f9;", CodeBlockType.Goto, 5),
    ("case 14:", CodeBlockType.Label, 5),
    ("goto IL_010d;", CodeBlockType.Goto, 5),
    ("case 15:", CodeBlockType.Label, 5),
    ("goto IL_0117;", CodeBlockType.Goto, 5),
    ("case 18:", CodeBlockType.Label, 5),
    ("case 19:", CodeBlockType.Label, 5),
    ("goto IL_0134;", CodeBlockType.Goto, 5),
    ("case 20:", CodeBlockType.Label, 5),
    ("goto IL_0151;", CodeBlockType.Goto, 5),
    ("case 21:", CodeBlockType.Label, 5),
    ("goto IL_0178;", CodeBlockType.Goto, 5),
    ("case 22:", CodeBlockType.Label, 5),
    ("goto IL_0191;", CodeBlockType.Goto, 5),
    ("case 23:", CodeBlockType.Label, 5),
    ("goto IL_01aa;", CodeBlockType.Goto, 5),
    ("case 24:", CodeBlockType.Label, 5),
    ("goto IL_01d0;", CodeBlockType.Goto, 5),
    ("case 26:", CodeBlockType.Label, 5),
    ("case 28:", CodeBlockType.Label, 5),
    ("goto IL_01f9;", CodeBlockType.Goto, 5),
    ("case 30:", CodeBlockType.Label, 5),
    ("case 31:", CodeBlockType.Label, 5),
    ("goto IL_021f;", CodeBlockType.Goto, 5),
    ("case 36:", CodeBlockType.Label, 5),
    ("goto IL_0248;", CodeBlockType.Goto, 5),
    ("case 38:", CodeBlockType.Label, 5),
    ("case 39:", CodeBlockType.Label, 5),
    ("goto IL_0258;", CodeBlockType.Goto, 5),
    ("case 40:", CodeBlockType.Label, 5),
    ("goto IL_0262;", CodeBlockType.Goto, 5),
    ("case 41:", CodeBlockType.Label, 5),
    ("goto IL_026c;", CodeBlockType.Goto, 5),
    ("case 42:", CodeBlockType.Label, 5),
    ("goto IL_027f;", CodeBlockType.Goto, 5),
    ("case 46:", CodeBlockType.Label, 5),
    ("goto IL_029b;", CodeBlockType.Goto, 5),
    ("case 47:", CodeBlockType.Label, 5),
    ("goto IL_02a5;", CodeBlockType.Goto, 5),
    ("case 48:", CodeBlockType.Label, 5),
    ("goto IL_02b4;", CodeBlockType.Goto, 5),
    ("case 51:", CodeBlockType.Label, 5),
    ("case 52:", CodeBlockType.Label, 5),
    ("goto IL_02d0;", CodeBlockType.Goto, 5),
    ("case 53:", CodeBlockType.Label, 5),
    ("case 55:", CodeBlockType.Label, 5),
    ("goto IL_02f6;", CodeBlockType.Goto, 5),
    ("default:", CodeBlockType.Label, 5),
    ("goto end_IL_0000;", CodeBlockType.Goto, 5),
    ("case 5:", CodeBlockType.Label, 5),
    ("case 11:", CodeBlockType.Label, 5),
    ("case 16:", CodeBlockType.Label, 5),
    ("case 17:", CodeBlockType.Label, 5),
    ("case 25:", CodeBlockType.Label, 5),
    ("case 29:", CodeBlockType.Label, 5),
    ("case 32:", CodeBlockType.Label, 5),
    ("case 33:", CodeBlockType.Label, 5),
    ("case 34:", CodeBlockType.Label, 5),
    ("case 35:", CodeBlockType.Label, 5),
    ("case 37:", CodeBlockType.Label, 5),
    ("case 43:", CodeBlockType.Label, 5),
    ("case 45:", CodeBlockType.Label, 5),
    ("case 49:", CodeBlockType.Label, 5),
    ("case 50:", CodeBlockType.Label, 5),
    ("case 56:", CodeBlockType.Label, 5),
    ("case 57:", CodeBlockType.Label, 5),
    ("case 58:", CodeBlockType.Label, 5),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto default;", CodeBlockType.Goto, 4),
    ("IL_02a5:", CodeBlockType.Label, 4),
    ("num = 47;", CodeBlockType.Operation, 4),
    ("FileSystem.FileClose();", CodeBlockType.Operation, 4),
    ("goto IL_02b4;", CodeBlockType.Goto, 4),
    ("IL_02b4:", CodeBlockType.Label, 4),
    ("num = 48;", CodeBlockType.Operation, 4),
    ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(num2 == 0)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("throw", CodeBlockType.Operation, 5),
("ProjectData.CreateProjectError(-2146828268);", CodeBlockType.Operation, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_0315;", CodeBlockType.Goto, 4),
    ("IL_008f:", CodeBlockType.Label, 4),
    ("num = 8;", CodeBlockType.Operation, 4),
    ("RichTextBox1.SaveFile(COND.Verz1 +", CodeBlockType.Operation, 4),
    ("\"TEMP\\\\Text2.RTF\"", CodeBlockType.String, 4),
    (", RichTextBoxStreamType.RichText);", CodeBlockType.Operation, 4),
    ("goto IL_00ad;", CodeBlockType.Goto, 4),
    ("IL_0262:", CodeBlockType.Label, 4),
    ("num = 40;", CodeBlockType.Operation, 4),
    ("prompt =", CodeBlockType.Operation, 4),
    ("\"Das angegebene Gerät ist nicht bereit.\\rBitte einschalten oder abbrechen.\"", CodeBlockType.String, 4),
    (";", CodeBlockType.Operation, 4),
    ("goto IL_026c;", CodeBlockType.Goto, 4),
    ("IL_026c:", CodeBlockType.Label, 4),
    ("num = 41;", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(Interaction.MsgBox(prompt, MsgBoxStyle.OkCancel,", CodeBlockType.Operation, 4),
    ("\"Fehler\"", CodeBlockType.String, 4),
    (") == MsgBoxResult.Cancel)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_027f;", CodeBlockType.Goto, 4),
    ("IL_027f:", CodeBlockType.Label, 4),
    ("num = 42;", CodeBlockType.Operation, 4),
    ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(num2 == 0)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("throw", CodeBlockType.Operation, 5),
("ProjectData.CreateProjectError(-2146828268);", CodeBlockType.Operation, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_0315;", CodeBlockType.Goto, 4),
    ("IL_0117:", CodeBlockType.Label, 4),
    ("num = 15;", CodeBlockType.Operation, 4),
    ("MyProject.Forms.Druck.Show();", CodeBlockType.Operation, 4),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 4),
    ("IL_0015:", CodeBlockType.Label, 4),
    ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 4),
    ("num3 = 2;", CodeBlockType.Operation, 4),
    ("goto IL_001d;", CodeBlockType.Goto, 4),
    ("IL_001d:", CodeBlockType.Label, 4),
    ("num = 3;", CodeBlockType.Operation, 4),
    ("text =", CodeBlockType.Operation, 4),
    ("\"Datum \"", CodeBlockType.String, 4),
    ("+ Strings.Mid(DateAndTime.DateString, 4, 2) +", CodeBlockType.Operation, 4),
    ("\".\"", CodeBlockType.String, 4),
    ("+ DateAndTime.DateString.Left( 2) +", CodeBlockType.Operation, 4),
    ("\".\"", CodeBlockType.String, 4),
    ("+ Strings.Mid(DateAndTime.DateString, 7, 4);", CodeBlockType.Operation, 4),
    ("goto IL_0073;", CodeBlockType.Goto, 4),
    ("IL_0073:", CodeBlockType.Label, 4),
    ("num = 4;", CodeBlockType.Operation, 4),
    ("switch", CodeBlockType.Operation, 4),
("(index)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("case 1:", CodeBlockType.Label, 5),
    ("break;", CodeBlockType.Operation, 5),
    ("case 2:", CodeBlockType.Label, 5),
    ("goto IL_00f9;", CodeBlockType.Goto, 5),
    ("case 3:", CodeBlockType.Label, 5),
    ("goto IL_0134;", CodeBlockType.Goto, 5),
    ("default:", CodeBlockType.Label, 5),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_008f;", CodeBlockType.Goto, 4),
    ("IL_0134:", CodeBlockType.Label, 4),
    ("num = 19;", CodeBlockType.Operation, 4),
    ("MyProject.Forms.Hinter.CommonDialog1Save.Filter =", CodeBlockType.Operation, 4),
    ("\"Text (*.txt)|*.txt|Formartierter Text (*.RTF)|*.RTF\"", CodeBlockType.String, 4),
    (";", CodeBlockType.Operation, 4),
    ("goto IL_0151;", CodeBlockType.Goto, 4),
    ("IL_0151:", CodeBlockType.Label, 4),
    ("num = 20;", CodeBlockType.Operation, 4),
    ("MyProject.Forms.Hinter.CommonDialog1Save.InitialDirectory = COND.GenPlu +", CodeBlockType.Operation, 4),
    ("\"list\\\\\"", CodeBlockType.String, 4),
    (";", CodeBlockType.Operation, 4),
    ("goto IL_0178;", CodeBlockType.Goto, 4),
    ("IL_0178:", CodeBlockType.Label, 4),
    ("num = 21;", CodeBlockType.Operation, 4),
    ("MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex = 2;", CodeBlockType.Operation, 4),
    ("goto IL_0191;", CodeBlockType.Goto, 4),
    ("IL_0191:", CodeBlockType.Label, 4),
    ("num = 22;", CodeBlockType.Operation, 4),
    ("MyProject.Forms.Hinter.CommonDialog1Save.ShowDialog();", CodeBlockType.Operation, 4),
    ("goto IL_01aa;", CodeBlockType.Goto, 4),
    ("IL_01aa:", CodeBlockType.Label, 4),
    ("num = 23;", CodeBlockType.Operation, 4),
    ("if", CodeBlockType.Operation, 4),
("(Operators.CompareString(MyProject.Forms.Hinter.CommonDialog1Save.FileName,", CodeBlockType.Operation, 4),
    ("\"\"", CodeBlockType.String, 4),
    (", TextCompare:", CodeBlockType.Label, 4),
    ("false) == 0)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_01d0;", CodeBlockType.Goto, 4),
    ("IL_01d0:", CodeBlockType.Label, 4),
    ("num = 24;", CodeBlockType.Operation, 4),
    ("switch", CodeBlockType.Operation, 4),
("(MyProject.Forms.Hinter.CommonDialog1Save.FilterIndex)", CodeBlockType.Operation, 4),
    ("{", CodeBlockType.Block, 5),
    ("case 1:", CodeBlockType.Label, 5),
    ("break;", CodeBlockType.Operation, 5),
    ("case 2:", CodeBlockType.Label, 5),
    ("goto IL_021f;", CodeBlockType.Goto, 5),
    ("default:", CodeBlockType.Label, 5),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 5),
    ("}", CodeBlockType.Block, 5),
    ("goto IL_01f9;", CodeBlockType.Goto, 4),
    ("IL_021f:", CodeBlockType.Label, 4),
    ("num = 31;", CodeBlockType.Operation, 4),
    ("RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.RichText);", CodeBlockType.Operation, 4),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 4),
    ("IL_01f9:", CodeBlockType.Label, 4),
    ("num = 28;", CodeBlockType.Operation, 4),
    ("RichTextBox1.SaveFile(MyProject.Forms.Hinter.CommonDialog1Save.FileName, RichTextBoxStreamType.PlainText);", CodeBlockType.Operation, 4),
    ("goto end_IL_0000_2;", CodeBlockType.Goto, 4),
    ("IL_00f9:", CodeBlockType.Label, 4),
    ("num = 13;", CodeBlockType.Operation, 4),
    ("RichTextBox1.Text =", CodeBlockType.Operation, 4),
    ("\"\"", CodeBlockType.String, 4),
    (";", CodeBlockType.Operation, 4),
    ("goto IL_010d;", CodeBlockType.Goto, 4),
    ("IL_010d:", CodeBlockType.Label, 4),
    ("num = 14;", CodeBlockType.Operation, 4),
    ("Close();", CodeBlockType.Operation, 4),
    ("goto IL_0117;", CodeBlockType.Goto, 4),
    ("end_IL_0000:", CodeBlockType.Label, 4),
    ("break;", CodeBlockType.Operation, 4),
    ("}", CodeBlockType.Block, 4),
    ("}", CodeBlockType.Block, 3),
    ("catch", CodeBlockType.Operation, 2),
    ("(Exception obj)", CodeBlockType.Operation, 2),
    ("when (obj is not null && num3 != 0 && num2 == 0)", CodeBlockType.Operation, 2),
    ("{", CodeBlockType.Block, 3),
    ("ProjectData.SetProjectError(obj);", CodeBlockType.Operation, 3),
    ("try0000_dispatch = 1043;", CodeBlockType.Operation, 3),
    ("continue;", CodeBlockType.Operation, 3),
    ("}", CodeBlockType.Block, 3),
    ("throw", CodeBlockType.Operation, 2),
("ProjectData.CreateProjectError(-2146828237);", CodeBlockType.Operation, 2),
    ("continue;", CodeBlockType.Operation, 2),
    ("end_IL_0000_2:", CodeBlockType.Label, 2),
    ("break;", CodeBlockType.Operation, 2),
    ("}", CodeBlockType.Block, 2),
    ("if", CodeBlockType.Operation, 1),
("(num2 != 0)", CodeBlockType.Operation, 1),
    ("{", CodeBlockType.Block, 2),
    ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 2),
    ("}", CodeBlockType.Block, 2),
    ("}", CodeBlockType.Block, 1),
    };
    public static object? TestDataList1() => ReadObject(Resources.Test1DataList);

    public static object TestDataList2_org() => new List<TokenData>() {
        ("private void Button2_Click(object sender, EventArgs e)", CodeBlockType.Operation, 0),
        ("{", CodeBlockType.Block, 1),
        ("", CodeBlockType.Operation, 1),
        ("//Discarded unreachable code: IL_0085", CodeBlockType.LComment, 1),
        ("int try0000_dispatch = -1;", CodeBlockType.Operation, 1),
        ("int num3 = default(int);", CodeBlockType.Operation, 1),
        ("int num2 = default(int);", CodeBlockType.Operation, 1),
        ("int num = default(int);", CodeBlockType.Operation, 1),
        ("byte b = default(byte);", CodeBlockType.Operation, 1),
        ("while", CodeBlockType.Operation, 1),
("(true)", CodeBlockType.Operation, 1),
        ("{", CodeBlockType.Block, 2),
        ("try", CodeBlockType.Operation, 2),
        ("{", CodeBlockType.Block, 3),
        ("", CodeBlockType.Operation, 3),
        ("/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/", CodeBlockType.Comment, 3),
        (";", CodeBlockType.Operation, 3),
        ("switch", CodeBlockType.Operation, 3),
("(try0000_dispatch)", CodeBlockType.Operation, 3),
        ("{", CodeBlockType.Block, 4),
        ("default:", CodeBlockType.Label, 4),
        ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 4),
        ("num3 = 1;", CodeBlockType.Operation, 4),
        ("goto IL_0007;", CodeBlockType.Goto, 4),
        ("case 196:", CodeBlockType.Label, 4),
        ("{", CodeBlockType.Block, 5),
        ("num2 = num;", CodeBlockType.Operation, 5),
        ("switch", CodeBlockType.Operation, 5),
("(num3)", CodeBlockType.Operation, 5),
        ("{", CodeBlockType.Block, 6),
        ("case 1:", CodeBlockType.Label, 6),
        ("break;", CodeBlockType.Operation, 6),
        ("default:", CodeBlockType.Label, 6),
        ("goto end_IL_0000;", CodeBlockType.Goto, 6),
        ("}", CodeBlockType.Block, 6),
        ("int num4 = num2 + 1;", CodeBlockType.Operation, 5),
        ("num2 = 0;", CodeBlockType.Operation, 5),
        ("switch", CodeBlockType.Operation, 5),
("(num4)", CodeBlockType.Operation, 5),
        ("{", CodeBlockType.Block, 6),
        ("case 1:", CodeBlockType.Label, 6),
        ("break;", CodeBlockType.Operation, 6),
        ("case 2:", CodeBlockType.Label, 6),
        ("goto IL_0007;", CodeBlockType.Goto, 6),
        ("case 3:", CodeBlockType.Label, 6),
        ("goto IL_0024;", CodeBlockType.Goto, 6),
        ("case 6:", CodeBlockType.Label, 6),
        ("goto IL_002b;", CodeBlockType.Goto, 6),
        ("case 7:", CodeBlockType.Label, 6),
        ("goto IL_0033;", CodeBlockType.Goto, 6),
        ("case 4:", CodeBlockType.Label, 6),
        ("case 5:", CodeBlockType.Label, 6),
        ("case 8:", CodeBlockType.Label, 6),
        ("goto IL_0048;", CodeBlockType.Goto, 6),
        ("case 9:", CodeBlockType.Label, 6),
        ("goto IL_0054;", CodeBlockType.Goto, 6),
        ("case 10:", CodeBlockType.Label, 6),
        ("goto end_IL_0000_2;", CodeBlockType.Goto, 6),
        ("default:", CodeBlockType.Label, 6),
        ("goto end_IL_0000;", CodeBlockType.Goto, 6),
        ("case 11:", CodeBlockType.Label, 6),
        ("goto end_IL_0000_3;", CodeBlockType.Goto, 6),
        ("}", CodeBlockType.Block, 6),
        ("goto default;", CodeBlockType.Goto, 5),
        ("}", CodeBlockType.Block, 5),
        ("IL_0033:", CodeBlockType.Label, 4),
        ("num = 7;", CodeBlockType.Operation, 4),
        ("FileSystem.Input(99, ref COND.Aus[b]);", CodeBlockType.Operation, 4),
        ("goto IL_0048;", CodeBlockType.Goto, 4),
        ("IL_0007:", CodeBlockType.Label, 4),
        ("num = 2;", CodeBlockType.Operation, 4),
        ("FileSystem.FileOpen(99, COND.GenPlu +", CodeBlockType.Operation, 4),
        ("\"\\\\Init\\\\Druck_ini.dat\"", CodeBlockType.String, 4),
        (", OpenMode.Input);", CodeBlockType.Operation, 4),
        ("goto IL_0024;", CodeBlockType.Goto, 4),
        ("IL_0024:", CodeBlockType.Label, 4),
        ("num = 3;", CodeBlockType.Operation, 4),
        ("b = 0;", CodeBlockType.Operation, 4),
        ("goto IL_0048;", CodeBlockType.Goto, 4),
        ("IL_0048:", CodeBlockType.Label, 4),
        ("num = 5;", CodeBlockType.Operation, 4),
        ("if", CodeBlockType.Operation, 4),
("(!FileSystem.EOF(99))", CodeBlockType.Operation, 4),
        ("{", CodeBlockType.Block, 5),
        ("goto IL_002b;", CodeBlockType.Goto, 5),
        ("}", CodeBlockType.Block, 5),
        ("goto IL_0054;", CodeBlockType.Goto, 4),
        ("IL_0054:", CodeBlockType.Label, 4),
        ("num = 9;", CodeBlockType.Operation, 4),
        ("FileSystem.FileClose(99);", CodeBlockType.Operation, 4),
        ("break;", CodeBlockType.Operation, 4),
        ("IL_002b:", CodeBlockType.Label, 4),
        ("num = 6;", CodeBlockType.Operation, 4),
        ("b = checked((byte)(unchecked(b) + 1));", CodeBlockType.Operation, 4),
        ("goto IL_0033;", CodeBlockType.Goto, 4),
        ("end_IL_0000_2:", CodeBlockType.Label, 4),
        ("break;", CodeBlockType.Operation, 4),
        ("}", CodeBlockType.Block, 4),
        ("num = 10;", CodeBlockType.Operation, 3),
        ("Process.Start(COND.GenPlu +", CodeBlockType.Operation, 3),
        ("\"\\\\Hilfe\\\\TeilB.PDF\"", CodeBlockType.String, 3),
        (");", CodeBlockType.Operation, 3),
        ("break;", CodeBlockType.Operation, 3),
        ("end_IL_0000:", CodeBlockType.Label, 3),
        (";", CodeBlockType.Operation, 3),
        ("}", CodeBlockType.Block, 3),
        ("catch", CodeBlockType.Operation, 2),
        ("(Exception obj)", CodeBlockType.Operation, 2),
        ("when (obj is not null && num3 != 0 && num2 == 0)", CodeBlockType.Operation, 2),
        ("{", CodeBlockType.Block, 3),
        ("ProjectData.SetProjectError(obj);", CodeBlockType.Operation, 3),
        ("try0000_dispatch = 196;", CodeBlockType.Operation, 3),
        ("continue;", CodeBlockType.Operation, 3),
        ("}", CodeBlockType.Block, 3),
        ("throw", CodeBlockType.Operation, 2),
("ProjectData.CreateProjectError(-2146828237);", CodeBlockType.Operation, 2),
        ("continue;", CodeBlockType.Operation, 2),
        ("end_IL_0000_3:", CodeBlockType.Label, 2),
        ("break;", CodeBlockType.Operation, 2),
        ("}", CodeBlockType.Block, 2),
        ("if", CodeBlockType.Operation, 1),
("(num2 != 0)", CodeBlockType.Operation, 1),
        ("{", CodeBlockType.Block, 2),
        ("ProjectData.ClearProjectError();", CodeBlockType.Operation, 2),
        ("}", CodeBlockType.Block, 2),
        ("}", CodeBlockType.Block, 1),
    };

    public static object? TestDataList2() => ReadObject(Resources.Test2DataList);

    public static object TestDataList3() => new List<TokenData>(){
        ("public void Test3()", CodeBlockType.Operation, 0),
        ("{", CodeBlockType.Block, 1),
        ("Modul1.UbgT = (Text2[0].Text).Trim();", CodeBlockType.Operation, 1),
        ("goto IL_105c;", CodeBlockType.Goto, 1),
        ("IL_105c:", CodeBlockType.Label, 1),
        ("num = 209;", CodeBlockType.Operation, 1),
        ("if", CodeBlockType.Operation, 1),
("(Modul1.UbgT ==", CodeBlockType.Operation, 1),
        ("\"\"", CodeBlockType.String, 1),
        (")", CodeBlockType.Operation, 1),
        ("{", CodeBlockType.Block, 2),
        ("goto IL_107c;", CodeBlockType.Goto, 2),
        ("}", CodeBlockType.Block, 2),
        ("else", CodeBlockType.Operation, 1),
        ("{", CodeBlockType.Block, 2),
        ("test =", CodeBlockType.Operation, 2),
        ("@\"\"\"Test\"\"\"", CodeBlockType.String, 2),
        (";", CodeBlockType.Operation, 2),
        ("test2 =", CodeBlockType.Operation, 2),
        ("$\"\\\"{test}\\\"\"", CodeBlockType.String, 2),
        (";", CodeBlockType.Operation, 2),
        ("test3 =", CodeBlockType.Operation, 2),
        ("$\"{{test}}\"", CodeBlockType.String, 2),
        (";", CodeBlockType.Operation, 2),
        ("goto IL_108d;", CodeBlockType.Goto, 2),
        ("}", CodeBlockType.Block, 2),
        ("", CodeBlockType.Operation, 1),
        ("//=================", CodeBlockType.LComment, 1),
        ("IL_107c:", CodeBlockType.Label, 1),
        ("num = 210;", CodeBlockType.Operation, 1),
        ("Modul1.UbgT =", CodeBlockType.Operation, 1),
        ("\"\\\"\"", CodeBlockType.String, 1),
        (";", CodeBlockType.Operation, 1),
        ("goto IL_108d;", CodeBlockType.Goto, 1),
        ("IL_108d:", CodeBlockType.Label, 1),
        ("num = 212;", CodeBlockType.Operation, 1),
        ("DataModul.DB_PersonTable.Seek(", CodeBlockType.Operation, 1),
        ("\">\"", CodeBlockType.String, 1),
        (", Modul1.UbgT, Modul1.PersInArb);", CodeBlockType.Operation, 1),
        ("}", CodeBlockType.Block, 1),};
    public static object TestDataList4() => new List<TokenData>(){
     ("public void Test4()", CodeBlockType.Operation, 0),
("{", CodeBlockType.Block, 1),
("goto IL_105c;", CodeBlockType.Goto, 1),
("IL_105c:", CodeBlockType.Label, 1),
("num = 209;", CodeBlockType.Operation, 1),
("Test(test:", CodeBlockType.Label, 1),
("", CodeBlockType.Operation, 1),
("\"Some Test\"", CodeBlockType.String, 1),
(");", CodeBlockType.Operation, 1),
("goto IL_107c;", CodeBlockType.Goto, 1),
("", CodeBlockType.Operation, 1),
("//=================", CodeBlockType.LComment, 1),
("IL_107c:", CodeBlockType.Label, 1),
("num = 210;", CodeBlockType.Operation, 1),
("Modul1.UbgT =", CodeBlockType.Operation, 1),
("\"\\\"\"", CodeBlockType.String, 1),
(";", CodeBlockType.Operation, 1),
("goto IL_108d;", CodeBlockType.Goto, 1),
("IL_108d:", CodeBlockType.Label, 1),
("num = 212;", CodeBlockType.Operation, 1),
("}", CodeBlockType.Block, 1),
 };
    public static object TestDataList5() => new List<TokenData>(){
        ("public void Test5()", CodeBlockType.Operation, 0),
        ("{", CodeBlockType.Block, 1),
        ("test =", CodeBlockType.Operation, 1),
        ("\"Some unended string ... ;", CodeBlockType.String, 1),
        ("test2 =", CodeBlockType.Operation, 1),
        ("$\"Some {(test.Length()>5?@\"\\\"\":\"7\")} nested string ...\"", CodeBlockType.String, 1),
        (";", CodeBlockType.Operation, 1),
        ("test3 =", CodeBlockType.Operation, 1),
        ("\"Some broken\"", CodeBlockType.String, 1),
        ("", CodeBlockType.Operation, 1),
        ("/* some comment */", CodeBlockType.Comment, 1),
        ("+", CodeBlockType.Operation, 1),
        ("\" string ...\"", CodeBlockType.String, 1),
        (";", CodeBlockType.Operation, 1),
        ("test3 =", CodeBlockType.Operation, 1),
        ("\"Another broken\"", CodeBlockType.String, 1),
        ("", CodeBlockType.Operation, 1),
        ("// some other comment", CodeBlockType.LComment, 1),
        ("+", CodeBlockType.Operation, 1),
        ("\" string ...\"", CodeBlockType.String, 1),
        (";", CodeBlockType.Operation, 1),
        ("}", CodeBlockType.Block, 1),
    };
    public static object TestDataList6() => new List<TokenData>() {
        ("public void Test6()", CodeBlockType.Operation, 0),
        ("{", CodeBlockType.Block, 1),
        ("if", CodeBlockType.Operation, 1),
        ("(true)", CodeBlockType.Operation, 1),
        ("{", CodeBlockType.Block, 2),
        ("goto IL_105c;", CodeBlockType.Goto, 2),
        ("}", CodeBlockType.Block, 2),
        ("else", CodeBlockType.Operation, 1),
        ("goto IL_107c;", CodeBlockType.Goto, 1),
        ("IL_105c:", CodeBlockType.Label, 1),
        ("goto IL_107c;", CodeBlockType.Goto, 1),
        ("IL_107c:", CodeBlockType.Label, 1),
        ("return;", CodeBlockType.Operation, 1),
        ("}", CodeBlockType.Block, 1),
    };
    public static object TestDataList7() => new List<TokenData>() {
    ("public void Test7()", CodeBlockType.Operation, 0),
    ("{", CodeBlockType.Block, 1),
    ("if", CodeBlockType.Operation, 1),
    ("(true)", CodeBlockType.Operation, 1),
    ("goto IL_105c;", CodeBlockType.Goto, 1),
    ("else", CodeBlockType.Operation, 1),
    ("goto IL_107c;", CodeBlockType.Goto, 1),
    ("IL_105c:", CodeBlockType.Label, 1),
    ("num = 209;", CodeBlockType.Operation, 1),
    ("i++;", CodeBlockType.Operation, 1),
    ("goto IL_108c;", CodeBlockType.Goto, 1),
    ("IL_107c:", CodeBlockType.Label, 1),
    ("num = 210;", CodeBlockType.Operation, 1),
    ("i--;", CodeBlockType.Operation, 1),
    ("goto IL_108c;", CodeBlockType.Goto, 1),
    ("IL_108c:", CodeBlockType.Label, 1),
    ("num = 212;", CodeBlockType.Operation, 1),
    ("return;", CodeBlockType.Operation, 1),
    ("}", CodeBlockType.Block, 1),
    };
    public static object? TestDataList8() => ReadObject(Resources.Test8DataList);
    public static object? TestDataList9() => ReadObject(Resources.Test9DataList);
    public static object? TestDataList10() => ReadObject(Resources.Test10DataList);
    public static object? TestDataList11() => ReadObject(Resources.Test11DataList);
    public static object? TestDataList12() => ReadObject(Resources.Test12DataList);
    public static object? TestDataList13() => ReadObject(Resources.Test13DataList);
    #endregion
}
