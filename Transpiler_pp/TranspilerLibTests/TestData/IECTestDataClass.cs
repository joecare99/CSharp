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

namespace TranspilerLibTests.TestData;

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

    private static object? ReadObject(byte[] JsonData) => new DataContractJsonSerializer(typeof(List<TokenData>)).ReadObject(new MemoryStream(JsonData));
    internal static object? ReadFileObject(string sFile) => new DataContractJsonSerializer(typeof(List<TokenData>)).ReadObject(new FileStream(sFile,FileMode.Open));
    internal static object? ReadBlockFileObject(string sFile) => new DataContractJsonSerializer(
        typeof(ICodeBlock), [typeof(CodeBlock),typeof(IECCodeBlock)]).ReadObject(new FileStream(sFile, FileMode.Open));
    #region Test Data for Tokenizer
    public static object TestDataList0() => ReadObject(Resources.IECTestDataList0)!;
    public static object TestDataList1() => ReadObject(Resources.IECTestDataList1)!;
    public static object TestDataList2() => ReadObject(Resources.IECTestDataList2)!;
    #endregion

    #region Expected code (unchanged)
    public const string cExpCode0 = @"    
    TYPE  udtVector :
         UNION  
             v  :  udtVectorData ;
  Koordinate  :  ARRAY  [  0  ..  1  ]  OF  LREAL ;
         END_UNION
    END_TYPE";

    public static string cExpCode1 { get; } = Resources.TestExpCode01;
    public static string cExpCode2 { get; } = Resources.TestExpCode02;
    #endregion

    #region Expected TestData
    public const string testDataExp0 = @"///Declaration MainBlock 0,0

///Block Block 1,0,0
TYPE
///Variable Variable 2,4,0
udtVector
///Operation Operation 2,1
:
///Block Block 2,2
UNION
///Variable Variable 3,0
v
///Operation Operation 3,1
:
///Variable Variable 3,2
udtVectorData
///Operation Operation 3,3
;
///Variable Variable 3,4
Koordinate
///Operation Operation 3,5
:
///Function Function 3,6
ARRAY
///Bracket Bracket 3,7
[
///Number Number 4,0
0
///Operation Operation 4,1
..
///Number Number 4,2
1
///Bracket Bracket 3,8
]
///Function Function 3,9
OF
///Function Function 3,10
LREAL
///Operation Operation 3,11
;
///Block Block 2,3
END_UNION
///Block Block 1,1
END_TYPE";
    public static string testDataExp1 { get; } = Resources.TestExpParse01;
    public static string testDataExp2 { get; } = Resources.TestExpParse02;
    #endregion
}
