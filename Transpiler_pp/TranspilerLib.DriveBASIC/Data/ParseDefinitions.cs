using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Models;

namespace TranspilerLib.DriveBASIC.Data;

/// <summary>
/// 
/// </summary>
/// <author>
/// C. Rosewich
/// </author>
public class ParseDefinitions
{
    public const string
        CCommand = "<COMMAND>",
        CToken = "<TOKEN>",
        CTToken = "<TEXT:TOKEN>",
        CExpression = "<EXPRESSION>";

    /// <stereotype>Data</stereotype>
    /// <author>C. Rosewich</author>
    /// <since>12.11.2009</since>
    public static readonly string[] ParseStrings = ["Label>", "Message>",
        "<Identifyer:Param1>", "<Identifyer:Param2>", "<Identifyer:Param3>",
        "<float:Param3>", "<Integer:Param1>","<Integer:Param2>", "<Integer:Param3>",
        "<" + "Axis:Param1>", "<W>", CTToken, CExpression, "<Variable1>",
        "<Variable2>", "<Variable3>"];

    /// <stereotype>Data</stereotype>
    /// <associates>unt_DriveCompiler.TParseDef</associates>
    /// <author>C. Rosewich</author>
    /// <since>12.06.2009</since>
    public static readonly ParseDef[] ParseDefBase = [
        new(){Token= EDriveToken.tt_Nop, SubToken= 0,  text= ""},
        new(){Token= EDriveToken.tt_Nop, SubToken= 1, text= "NOP" },
        new(){Token= EDriveToken.tt_Nop, SubToken= 2, text= "DECLARE <Variable3>"},
        new(){Token= EDriveToken.tt_Nop, SubToken= 3, text= "DEFINE <Integer:Param1>,\"<Text:Message>\""},
        new(){Token= EDriveToken.tt_Nop,    SubToken= 4, text= "DEF_LBL <Identifyer:Label>"},
        new(){Token= EDriveToken.tt_Nop,    SubToken= 5, text= "SUB <Identifyer:Label>"},
        new(){Token= EDriveToken.tt_goto, SubToken= 0,    text= "GOTO <Integer:Param1>"},
        new(){Token= EDriveToken.tt_goto, SubToken= 1,    text= "CALL <Integer:Param1>"},
        new(){Token= EDriveToken.tt_goto, SubToken= 2,    text= "GOTO <Identifyer:Label>"},
        new(){Token= EDriveToken.tt_goto, SubToken= 3,    text= "CALL <Identifyer:Label>"},
        new(){Token= EDriveToken.tte_goto2, SubToken= 0,    text= "ON " + CExpression},
        new(){Token= EDriveToken.tte_goto2, SubToken= 1,    text= "ONC " + CExpression},
        new(){Token= EDriveToken.tt_end, SubToken= 0, text= "RETURN"},
        new(){Token= EDriveToken.tt_end, SubToken= 1, text= "END"},
        new(){Token= EDriveToken.tt_end, SubToken= 2,    text= "STOP"},
        new(){Token= EDriveToken.tt_end, SubToken= 3, text= "PAUSE"},
        new(){Token= EDriveToken.tt_end,    SubToken= 4, text= "ENDIF"},
        new(){Token= EDriveToken.tt_end, SubToken= 5, text= "NEXT"},
        new(){Token= EDriveToken.tt_end, SubToken= 6, text= "WEND"},
        new(){Token= EDriveToken.tte_if, SubToken= 0,    text= "IF " + CExpression + " THEN"},
        new(){Token= EDriveToken.tte_if, SubToken= 1,    text= "ELSIF " + CExpression + " THEN"},
        new(){Token= EDriveToken.tt_else, SubToken= 0,    text= "ELSE"},
        new(){Token= EDriveToken.tte_while, SubToken= 0,    text= "WHILE " + CExpression},
        new(){Token= EDriveToken.tt_for, SubToken= 0,    text= "FOR <Variable1> := 0 TO <Integer:Param3>"},
        new(){Token= EDriveToken.tt_for,    SubToken= 0, text= "FOR <Variable1> := <Variable2> TO <Integer:Param3>"},
        new(){Token= EDriveToken.tt_for, SubToken= 1,    text= "FOR <Variable1> := <Variable2> TO <Variable3>"},
        new(){Token= EDriveToken.tt_for,    SubToken= 2, text= "FOR <Variable1> := <Integer:Param3> DOWNTO 0"},
        new(){Token= EDriveToken.tt_for, SubToken= 2,    text= "FOR <Variable1> := <Integer:Param3> DOWNTO <Variable2>"},
        new(){Token= EDriveToken.tt_for, SubToken= 3,    text= "FOR <Variable1> := <Variable3> DOWNTO <Variable2>"},
        new(){Token= EDriveToken.tte_let,    SubToken= 0, text= "LET <Variable1> := " + CExpression},
        new(){Token= EDriveToken.tte_let,    SubToken= 1, text= "<Variable1> := " + CExpression},
        new(){Token= EDriveToken.tte_wait,    SubToken= 0, text= "WAIT " + CExpression},
        new(){Token= EDriveToken.tt_Msg, SubToken= 0,    text= "(*<Text:Message>*)"},
        new(){Token= EDriveToken.tt_Msg, SubToken= 1,    text= "//<Text:Message>"},
        new(){Token= EDriveToken.tte_Msg2, SubToken= 1,    text= "ERROR " + CExpression},
        new(){Token= EDriveToken.tt_Msg, SubToken= 2,    text= "STATE \"<Text:Message>\""},
        new(){Token= EDriveToken.tte_Msg2, SubToken= 2,    text= "STATE " + CExpression},
        new(){Token= EDriveToken.tt_Msg, SubToken= 3,    text= "MSG<W>\"<Text:Message>\""},
        new(){Token= EDriveToken.tte_Msg2, SubToken= 3,    text= "MSG " + CExpression},
        new(){Token= EDriveToken.tt_Msg, SubToken= 6,    text= "ERROR \"<Text:Message>\""},
        new(){Token= EDriveToken.tt_Funct, SubToken= 0,    text= "CALLF <Integer:Param1>"},
        new(){Token= EDriveToken.tt_Funct, SubToken= 1,    text= "FUNC <Integer:Param1>"},
        new(){Token= EDriveToken.tte_funct2, SubToken= 0,    text= "CALLF <Integer:Param1>," + CExpression},
        new(){Token= EDriveToken.tte_funct2,    SubToken= 1, text= "FUNC <Integer:Param1>," + CExpression},
        new(){Token= EDriveToken.tt_Sync, SubToken= 0, text= "SYNC_#<Integer:Param1> OFF"},
        new(){Token= EDriveToken.tt_Sync, SubToken= 1, text= "INT_#<Integer:Param1> <Identifyer:Param2>"},
        new(){Token= EDriveToken.tte_sync2, SubToken= 0, text= "WHENEVER_#<Integer:Param1> " + CExpression},
        new(){Token= EDriveToken.tte_sync2, SubToken= 1, text= "WHEN_#<Integer:Param1> " + CExpression},
        // Beispiel fuer eine Erweiterung
        new(){Token= EDriveToken.tte_drive, SubToken= 0, text= "DRIVE " + CExpression},
        new(){Token= EDriveToken.tte_drive, SubToken= 1, text= "DRIVE_<Axis:Param1> " + CExpression},
        new(){Token= EDriveToken.tte_drive, SubToken= 2, text= "DRIVE_REL " + CExpression},
        new(){Token= EDriveToken.tte_drive, SubToken= 3, text= "DRIVE_REL_<Axis:Param1> " + CExpression},

        new(){Token= EDriveToken.tte_drive_via, SubToken= 0, text= "DRIVEVIA " + CExpression},
        new(){Token= EDriveToken.tte_drive_via, SubToken= 1, text= "DRIVEVIA_<Axis:Param1> " + CExpression},
        new(){Token= EDriveToken.tte_drive_via, SubToken= 2, text= "DRIVEVIA_REL " + CExpression},
        new(){Token= EDriveToken.tte_drive_via, SubToken= 3, text= "DRIVEVIA_REL_<Axis:Param1> " + CExpression},

        new(){Token= EDriveToken.tte_drive_async, SubToken= 0, text= "DRIVEASYNC " + CExpression},
        new(){Token= EDriveToken.tte_drive_async, SubToken= 1, text= "DRIVEASYNC_<Axis:Param1> " + CExpression},
        new(){Token= EDriveToken.tte_drive_async, SubToken= 2, text= "DRIVEASYNC_REL " + CExpression},
        new(){Token= EDriveToken.tte_drive_async, SubToken= 3, text= "DRIVEASYNC_REL_<Axis:Param1> " + CExpression}];

    /// <stereotype>Data</stereotype>
    /// <associates>unt_DriveCompiler.TParseDef2</associates>
    /// <author>C. Rosewich</author>
    /// <since>12.06.2009</since>
    public static readonly ParseDef2[] PlaceHolderDefBase = [
        new(){PlaceHolder= "<Program>", Number= 0, text= "<Commands>"},
        new(){PlaceHolder= "<Commands>", Number= 0,    text= CCommand + Environment.NewLine + "<Commands>"},
        new(){PlaceHolder= "<Commands>",Number= 0, text= CCommand},
        new(){PlaceHolder= CCommand, Number= 0,  text= "<Identifyer:Label>: "+CTToken},
        new(){PlaceHolder= CCommand,    Number= 0, text= CTToken},

        new(){PlaceHolder= CExpression, Number= 0,  text= "<Identifyer:Param2><Operator><Wert2>"},
        new(){PlaceHolder= CExpression, Number= 16, text= "<Float:Param3><Operator><Identifyer:Param2>"},
        new(){PlaceHolder= CExpression, Number= (int)EExpressionOperation.teo_indirect, text= "<Variable2>[<Wert2>]"},
        new(){PlaceHolder= CExpression, Number= (int)EExpressionOperation.teo_fct, text= "FNC<Integer:Param2>new(){<Wert2>}"},
        new(){PlaceHolder= CExpression, Number= 0, text= "<Wert2>"},
        new(){PlaceHolder= CExpression, Number= (int)EExpressionOperation.teo_minus+32, text= "-<Variable3>"},

        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_plus, text= "+"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_minus, text= "-"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_mult, text= "*"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_div, text= "/"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_and, text= " AND "},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_or, text= " OR "},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_xor, text= " XOR "},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_lt, text= "<"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_lte, text= "<="},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_eq, text= "="},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_neq, text= "<>"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_gt, text= ">"},
        new(){PlaceHolder= "<Operator>", Number= (int)EExpressionOperation.teo_gte, text= ">="},

        new(){PlaceHolder= "<Wert2>", Number= 0, text= "<Float:Param3>"},
        new(){PlaceHolder= "<Wert2>", Number= 32, text= "<Variable3>"},
        new(){PlaceHolder= "<Variable1>", Number= 0, text= "<Identifyer:Param1>"},
        new(){PlaceHolder= "<Variable2>", Number= 0, text= "<Identifyer:Param2>"},
        new(){PlaceHolder= "<Variable3>", Number= 0, text= "<Identifyer:Param3>"},
    // ,new(){PlaceHolder= "<IConst3>", Number= 0, text= "<Integer=Param3>"}
    ];
    //
    /// <stereotype>Data</stereotype>    
    /// <summary>
    /// Definiert die erlaubten Zeichen fuer die verschiedenen Platzhalter-Typen
    /// </summary>
    public static (string Placeholder, bool HasPointAsSep, bool MaybeEmpty, ISet<char> first, ISet<char>? inner, ISet<char>? last)[] SysPHCharset =
    [("<Integer:",false,false, CharSets.numbers.ToHashSet(), CharSets.numbers.ToHashSet(),null),
     ("<Float:",false,false, CharSets.numbers.Concat(['+','-']).ToHashSet(), CharSets.numbersExt.ToHashSet(),CharSets.numbers.ToHashSet()),
     ("<Identifyer:",true,false, CharSets.letters.ToHashSet(), CharSets.lettersAndNumbers.Concat(['_']).ToHashSet(), null),
     ("<Identifyer:",false,false, CharSets.letters.ToHashSet(), CharSets.lettersAndNumbers.Concat(['_']).ToHashSet(),new HashSet<char>(){'%', '&', '.' }),
     ("<Axis:",false,false, 'x'.To('z').Concat('a'.To('c')).Concat('1'.To('6')).ToHashSet(), null ,null ),
     ("<Text:",false,false, CharSets.allNormal, CharSets.allNormal,null),
     ("<W>",false,true, CharSets.whitespace.ToHashSet(), CharSets.whitespace.ToHashSet(),null)];


    public static (EDriveToken Token, int SubToken, EDriveToken ReferencingToken, int ReferencingSubtoken, bool Backward)[] ReferencingToken =
        [(EDriveToken.tt_end,4,EDriveToken.tte_if,0,true),
     (EDriveToken.tt_end,6,EDriveToken.tte_while,-1,true),
     (EDriveToken.tt_end,5,EDriveToken.tt_for,-1,true),
     (EDriveToken.tt_else,-1,EDriveToken.tt_end,4,false),
     (EDriveToken.tte_if,-1,EDriveToken.tte_if,1,false),
     (EDriveToken.tte_if,-1,EDriveToken.tt_else,-1,false),
     (EDriveToken.tte_if,-1,EDriveToken.tt_end,4,false),
     (EDriveToken.tte_while,-1,EDriveToken.tt_end,6,false)
        ];

    // Removed obsolete static TteTokens set now that DriveCompiler builds it dynamically
 }
