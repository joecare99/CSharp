using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using TranspilerLib.Data;
using TranspilerLibTests.Properties;
using System.IO;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;
using TranspilerLib.IEC.Models.Scanner;

namespace TranspilerLib.IEC.TestData;

public class IECTestDataClass
{
    #region Raw TestData
    public const string testData0 = @"TYPE udtVector :
  UNION
    v : udtVectorData;
    Koordinate : ARRAY[0..1] OF LREAL;
  END_UNION
END_TYPE";
    public static readonly string testData1 = Resources.TestCode01;
    public static readonly string testData2 = Resources.TestCode02;
    #endregion

    #region Expected data for Tokenizer
    public const string cExpLog0 = @"T:Block,0,0,TYPE
T:Variable,4,1,udtVector
T:Operation,14,1,:
T:Block,16,1,UNION
T:Variable,25,2,v
T:Operation,32,2,:
T:Variable,34,2,udtVectorData
T:Operation,48,2,;
T:Variable,49,2,Koordinate
T:Operation,65,2,:
T:Function,67,2,ARRAY
T:Bracket,73,2,[
T:Number,74,3,0
T:Operation,75,3,..
T:Number,77,3,1
T:Bracket,78,2,]
T:Function,79,2,OF
T:Function,82,2,LREAL
T:Operation,88,2,;
T:Block,89,1,END_UNION
T:Block,102,0,END_TYPE
";
    public static readonly string cExpLog1 = Resources.ExpLog01;
    public static readonly string cExpLog2 = Resources.ExpLog02;
    #endregion

    private static object? ReadObject(byte[] JsonData)
        => RemapLegacyTokenData(new DataContractJsonSerializer(typeof(List<TokenData>)).ReadObject(new MemoryStream(JsonData)) as List<TokenData>);

    internal static object? ReadFileObject(string sFile)
        => RemapLegacyTokenData(new DataContractJsonSerializer(typeof(List<TokenData>)).ReadObject(new FileStream(sFile,FileMode.Open)) as List<TokenData>);

    private static List<TokenData>? RemapLegacyTokenData(List<TokenData>? tokenData)
    {
        if (tokenData == null)
        {
            return null;
        }

        for (var i = 0; i < tokenData.Count; i++)
        {
            tokenData[i] = tokenData[i] with { type = MapLegacyCodeBlockType(tokenData[i].type) };
        }

        return tokenData;
    }

    private static CodeBlockType MapLegacyCodeBlockType(CodeBlockType type)
        => (int)type switch
        {
            3 => CodeBlockType.Variable,
            4 => CodeBlockType.Function,
            5 => CodeBlockType.Declaration,
            7 => CodeBlockType.Operation,
            12 => CodeBlockType.LComment,
            13 => CodeBlockType.FLComment,
            17 => CodeBlockType.Block,
            18 => CodeBlockType.Number,
            19 => CodeBlockType.Bracket,
            _ => type,
        };

    internal static object? ReadBlockFileObject(string sFile) => new DataContractJsonSerializer(
        typeof(ICodeBlock), [typeof(CodeBlock),typeof(IECCodeBlock)]).ReadObject(new FileStream(sFile, FileMode.Open));
    #region Test Data for Tokenizer
    public static object TestDataList0() => ReadObject(Resources.IECTestDataList0)!;
    public static object TestDataList1() => ReadObject(Resources.IECTestDataList1)!;
    public static object TestDataList2() => ReadObject(Resources.IECTestDataList2)!;
    #endregion

    #region Expected code (unchanged)
    public const string cExpCode0 = @"    
    TYPE
 udtVector :
       UNION
 v :
         ;  Koordinate  ARRAY[ : 0 : 1 : ] : OF
 LREAL             ;       END_UNION
    END_TYPE";

    public static string cExpCode1 { get; } = @"    
        // Dies ist ein Kommentar zum Typ ST_01

    TYPE
 ST_01 :
       STRUCT
            // Dies ist ein Kommentar zum Element a
  a  INT := INT#1 :          ;             // Dies ist ein 2. Kommentar zum Element a
             // Dies ist ein Kommentar zum Element b
  b  LREAL := LREAL#2.7e-2 :          ;             // Dies ist ein 2. Kommentar zum Element b
       END_STRUCT
    END_TYPE";
    public static string cExpCode2 { get; } = @"    
    FUNCTION
 fbCalcWinkelvVector :
       VAR_INPUT
 Vec :
         ;       END_VAR       VAR_OUTPUT
 Len  LREAL :          ;       END_VAR       VAR
 ZwErg  LREAL :          ;       END_VAR
    BEGIN
      IF
       THEN
 ZwErg := SEL(
 := ATAN(
 Vec.v.y / vec.v.x  ) := ATAN(
 Vec.v.y / vec.v.x  ) + pi , ) :=          ;  len := Vec. := v. := x  COS(
 ZwErg  ) /          ;  fbCalcWinkelvVector := ZwErg  pi * 2.0 - DINT_TO_REAL(
 TRUNC(
 Zwerg / pi * 0.5 + 1.5  )  ) * pi * 2.0 -          ;       ELSIF
       THEN
 fbCalcWinkelvVector := SEL(
 := ATAN( - Vec.v.x / vec.v.y - ) + 0.5 * pi , ATAN( - Vec.v.x / vec.v.y - ) - 0.5 * pi - ) ,              ;  len := Vec. := v. := y  SIN(
 fbCalcWinkelvVector  ) /          ;       else
 fbCalcWinkelvVector := 0.0         ;  Len := 0.0         ;       end_if       ;
    END_FUNCTION";
    #endregion

    #region Expected TestData
    public const string testDataExp0 = @"///Declaration MainBlock 0,0

///Block Block 1,0,0
TYPE
///Variable Variable 2,4,0
udtVector
///Operation Operation 2,14,1
:
///Block Block 2,16,2
UNION
///Variable Variable 3,25,0
v
///Operation Operation 3,32,1
:
///Variable Variable 4,34,0
udtVectorData
///Block Block 3,48,2
;
///Variable Variable 3,49,3
Koordinate
///Operation Operation 3,65,4
:
///Function Function 4,67,0
ARRAY[
///Number Number 4,74,1
0
///Operation Operation 4,75,2
..
///Number Number 5,77,0
1
///Bracket Bracket 4,78,3
]
///Function Function 4,79,4
OF
///Function Function 5,82,0
LREAL
///Block Block 5,88,1
;
///Block Block 2,89,3
END_UNION
///Block Block 1,102,1
END_TYPE";
    public static string testDataExp1 { get; } = @"///Declaration MainBlock 0,0

///FLComment FLComment 1,0,0
// Dies ist ein Kommentar zum Typ ST_01
///Block Block 1,40,1
TYPE
///Variable Variable 2,45,0
ST_01
///Operation Operation 2,51,1
:
///Block Block 2,53,2
STRUCT
///FLComment FLComment 3,67,0
// Dies ist ein Kommentar zum Element a
///Variable Variable 3,107,1
a
///Operation Operation 3,111,2
:
///Assignment Assignment 4,116,0
:=
///Function Variable 5,112,0
INT
///Number Number 5,119,1
INT#1
///Block Block 4,125,1
;
///LComment LComment 3,127,3
// Dies ist ein 2. Kommentar zum Element a
///FLComment FLComment 3,173,4
// Dies ist ein Kommentar zum Element b
///Variable Variable 3,213,5
b
///Operation Operation 3,217,6
:
///Assignment Assignment 4,224,0
:=
///Function Variable 5,218,0
LREAL
///Number Number 5,227,1
LREAL#2.7e-2
///Block Block 4,240,1
;
///LComment LComment 3,242,7
// Dies ist ein 2. Kommentar zum Element b
///Block Block 2,285,3
END_STRUCT
///Block Block 1,297,2
END_TYPE";
    public static string testDataExp2 { get; } = @"///Declaration MainBlock 0,0

///Block Block 1,0,0
FUNCTION
///Variable Variable 2,8,0
fbCalcWinkelvVector
///Operation Operation 2,28,1
:
///Function Function 3,30,0
LREAL
///Block Block 3,36,1
;
///Block Block 2,37,2
VAR_INPUT
///Variable Variable 3,48,0
Vec
///Operation Operation 3,57,1
:
///Variable Variable 4,59,0
udtVector
///Block Block 3,69,2
;
///Block Block 2,70,3
END_VAR
///Block Block 2,79,4
VAR_OUTPUT
///Function Function 3,91,0
Len
///Operation Operation 3,100,1
:
///Function Function 4,102,0
LREAL
///Block Block 4,108,1
;
///Block Block 2,109,5
END_VAR
///Block Block 2,118,6
VAR
///Variable Variable 3,123,0
ZwErg
///Operation Operation 3,134,1
:
///Function Function 4,136,0
LREAL
///Block Block 4,142,1
;
///Block Block 2,143,7
END_VAR
///Block Block 1,152,1
BEGIN
///Block Block 2,159,0
IF
///Operation Operation 3,176,0
>
///Function Function 4,163,0
ABS(
///Variable Variable 5,168,0
Vec.v.x
///Bracket Bracket 5,175,1
)
///Function Function 4,178,1
ABS(
///Variable Variable 5,183,0
Vec.v.y
///Bracket Bracket 5,190,1
)
///Block Block 2,191,1
THEN
///Assignment Assignment 3,206,0
:=
///Variable Variable 4,196,0
ZwErg
///Function Function 4,209,1
SEL(
///Operation Operation 5,220,0
<
///Variable Variable 6,213,0
Vec.v.x
///Number Number 6,221,1
0.0
///Operation Operation 4,224,2
,
///Function Function 5,225,0
ATAN(
///Operation Operation 6,237,0
/
///Variable Variable 7,230,0
Vec.v.y
///Variable Variable 7,238,1
vec.v.x
///Bracket Bracket 6,245,1
)
///Operation Operation 4,246,3
,
///Operation Operation 5,268,0
+
///Function Function 6,247,0
ATAN(
///Operation Operation 7,259,0
/
///Variable Variable 8,252,0
Vec.v.y
///Variable Variable 8,260,1
vec.v.x
///Bracket Bracket 7,267,1
)
///Variable Variable 6,269,1
pi
///Bracket Bracket 5,271,1
)
///Block Block 4,272,4
;
///Assignment Assignment 3,281,1
:=
///Function Variable 4,273,0
len
///Variable Variable 4,283,1
Vec.
///Variable Variable 4,287,2
v.
///Variable Variable 4,289,3
x
///Operation Operation 3,290,2
/
///Function Function 4,291,0
COS(
///Variable Variable 5,295,0
ZwErg
///Bracket Bracket 5,300,1
)
///Block Block 4,301,1
;
///Assignment Assignment 3,326,3
:=
///Variable Variable 4,302,0
fbCalcWinkelvVector
///Variable Variable 4,329,1
ZwErg
///Operation Operation 3,342,4
-
///Operation Operation 4,335,0
+
///Operation Operation 5,338,0
*
///Variable Variable 6,336,0
pi
///Number Number 6,339,1
2.0
///Operation Operation 4,383,1
*
///Operation Operation 5,380,0
*
///Function Function 6,343,0
DINT_TO_REAL(
///Function Function 7,356,0
TRUNC(
///Operation Operation 8,374,0
+
///Operation Operation 9,370,0
*
///Operation Operation 10,367,0
/
///Variable Variable 11,362,0
Zwerg
///Variable Variable 11,368,1
pi
///Number Number 10,371,1
0.5
///Number Number 9,375,1
1.5
///Bracket Bracket 8,378,1
)
///Bracket Bracket 7,379,1
)
///Variable Variable 6,381,1
pi
///Number Number 5,384,1
2.0
///Block Block 4,387,2
;
///Block Block 2,388,2
ELSIF
///Operation Operation 3,408,0
>
///Function Function 4,395,0
ABS(
///Variable Variable 5,400,0
Vec.v.y
///Bracket Bracket 5,407,1
)
///Variable Variable 4,410,1
epsilon
///Block Block 2,418,3
THEN
///Assignment Assignment 3,448,0
:=
///Variable Variable 4,423,0
fbCalcWinkelvVector
///Function Function 4,451,1
SEL(
///Operation Operation 5,463,0
<
///Variable Variable 6,456,0
Vec.v.y
///Number Number 6,465,1
0.0
///Operation Operation 4,469,2
,
///Operation Operation 5,492,0
+
///Operation Operation 6,475,0
-
///Function Function 7,470,0
ATAN(
///Operation Operation 7,483,1
/
///Variable Variable 8,476,0
Vec.v.x
///Variable Variable 8,484,1
vec.v.y
///Bracket Bracket 7,491,2
)
///Operation Operation 6,496,1
*
///Number Number 7,493,0
0.5
///Variable Variable 7,497,1
pi
///Operation Operation 5,499,1
,
///Operation Operation 6,522,0
-
///Operation Operation 7,505,0
-
///Function Function 8,500,0
ATAN(
///Operation Operation 8,513,1
/
///Variable Variable 9,506,0
Vec.v.x
///Variable Variable 9,514,1
vec.v.y
///Bracket Bracket 8,521,2
)
///Operation Operation 7,526,1
*
///Number Number 8,523,0
0.5
///Variable Variable 8,527,1
pi
///Bracket Bracket 7,529,2
)
///Block Block 6,530,1
;
///Assignment Assignment 3,539,1
:=
///Function Variable 4,531,0
len
///Variable Variable 4,541,1
Vec.
///Variable Variable 4,545,2
v.
///Variable Variable 4,547,3
y
///Operation Operation 3,548,2
/
///Function Function 4,549,0
SIN(
///Variable Variable 5,553,0
fbCalcWinkelvVector
///Bracket Bracket 5,572,1
)
///Block Block 4,573,1
;
///Block Block 2,574,4
else
///Assignment Assignment 3,604,0
:=
///Variable Variable 4,580,0
fbCalcWinkelvVector
///Number Number 4,607,1
0.0
///Block Block 3,610,1
;
///Assignment Assignment 3,619,2
:=
///Function Variable 4,611,0
Len
///Number Number 4,622,1
0.0
///Block Block 3,626,3
;
///Block Block 2,627,5
end_if
///Block Block 2,635,6
;
///Block Block 1,636,2
END_FUNCTION";
    #endregion
}
