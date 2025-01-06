using System;
using System.Collections.Generic;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Data;
using TranspilerLibTests.TestData;
using BaseLib.Helper;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using static TranspilerLib.Helper.TestHelper;

namespace TranspilerLib.Models.Scanner.Tests
{
    [TestClass()]
    public class IECCodeBuilderTests
    {
        private const string TokensFilePath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Tokens{0}.json";
        private const string BlocksFilePath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Blocks{0}.json";
        private const string BlocksTextPath = ".\\Resources\\IECCodeBuilder\\Test{0}\\Blocks{0}.txt";

        public static IEnumerable<object?[]> OnIECTokenTestData => [
              ["0" , new List<TokenData>() { new("-", CodeBlockType.Operation, 1, 1), new("2.0", CodeBlockType.Number, 1, 3) }, GetBuildCommands("00")],
              ["1" , GetTokenlist("01"), GetBuildCommands("01") ],
              ["2" , GetTokenlist("02"), GetBuildCommands("02") ],
              ["3" , GetTokenlist("03"), GetBuildCommands("03") ],
              ["4" , GetTokenlist("04"), GetBuildCommands("04") ],
              ["5" , GetTokenlist("05"), GetBuildCommands("05") ],
              ["6" , GetTokenlist("06"), GetBuildCommands("06") ],
              ["7" , GetTokenlist("07"), GetBuildCommands("07") ],
            ];

        private static string[] GetBuildCommands(string sFNStumb)
        {
            var sFile = BlocksTextPath.Format(sFNStumb.PadLeft(2, '0'));
            if (File.Exists(sFile))
                try
                {
                    return File.ReadAllText(sFile).Split(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            else
            {
                return null;
            }
        }

        private static object? GetTokenlist(string sFNStumb)
        {
            var sFile = TokensFilePath.Format(sFNStumb.PadLeft(2, '0'));
            if (File.Exists(sFile))
            {
                return IECTestDataClass.ReadFileObject(sFile);
            }
            else
            {
                return null;
            }
        }

        private IECCodeBuilder testClass;

        [TestInitialize]
        public void Init() { 
            testClass = new IECCodeBuilder();
        }

        [TestMethod()]
        public void IECCodeBuilderTest()
        {
            Assert.IsNotNull(testClass);
            Assert.IsInstanceOfType(testClass, typeof(ICodeBuilder));
            Assert.IsInstanceOfType(testClass, typeof(IECCodeBuilder));
            var data = GetTokenlist("01");
            Assert.IsNotNull(data);
            data = GetTokenlist("2");
        }

        [DataTestMethod()]
        [DynamicData(nameof(OnIECTokenTestData))]
        public void OnTokenTest(string sFNStumb, List<TokenData> tokens, string[] codes)
        {
            // Arrange
            ICodeBlock codeBlock = new CodeBlock() { Name = "Declaration", Code = "", Parent = null,SourcePos=-1 };
            var data = testClass.NewData(codeBlock);

            // Act
            if (tokens != null)
            foreach (var token in tokens)
            {
                testClass.OnToken(token, data);
            }

            var sFile = BlocksFilePath.Format(sFNStumb.PadLeft(2, '0'))+"_";
            using var ms = new FileStream(sFile, FileMode.Create);
            new DataContractJsonSerializer(
                type: typeof(List<ICodeBlock>),
                knownTypes: [typeof(IECCodeBlock)])
            .WriteObject(ms, codeBlock.SubBlocks);
            ms.Close();

            sFile = BlocksTextPath.Format(sFNStumb.PadLeft(2, '0')) + "_";
            using var fs2 = new FileStream(sFile, FileMode.Create);
            using var sw2 = new StreamWriter(fs2);
            sw2.Write(codeBlock.ToString());
            sw2.Close();

            // Assert
            Debug.WriteLine("CodeBlocks:");
            Debug.WriteLine(codeBlock.ToString());

            Debug.WriteLine("Code:");
            Debug.WriteLine(codeBlock.ToCode(2));            
            AssertAreEqual(string.Join(Environment.NewLine, codes??[""]), codeBlock.ToString());
        }
    }
}