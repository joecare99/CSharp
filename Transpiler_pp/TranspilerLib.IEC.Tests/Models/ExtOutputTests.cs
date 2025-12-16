using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Xml;
using TranspilerLib.Models;

namespace TranspilerLib.IEC.Models.Tests;

[TestClass]
public class ExtOutputTests
{
    private static string RunExtOutput(string xml, out string debug)
    {
        var sb = new StringBuilder();
        var db = new StringBuilder();
        using var sr = new StringReader(xml);
        using var xr = XmlReader.Create(sr, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = false });
        var reader = new IECReader(xr);
        var output = new ExtOutput();
        output.Output(reader, s => sb.Append(s), s => db.Append(s));
        debug = db.ToString();
        return sb.ToString();
    }

    [TestMethod]
    public void Output_Function_With_ReturnType_Interface_InputVars_And_Body_ST()
    {
        var xml = """
<project>
  <contentHeader>ignored</contentHeader>
  <types>
    <pous>
      <pou name="Add" pouType="function">
        <interface>
          <returnType>
            <type>
              <derived name="INT" />
            </type>
          </returnType>
          <inputVars>
            <variable name="a">
              <type><derived name="INT" /></type>
            </variable>
            <variable name="b">
              <type><derived name="INT" /></type>
            </variable>
          </inputVars>
        </interface>
        <body>
          <ST>a := a + b;</ST>
        </body>
      </pou>
    </pous>
  </types>
</project>
""";

        var result = RunExtOutput(xml, out _);
        var nl = Environment.NewLine;
        var expected = new StringBuilder()
            .Append($"FUNCTION Add : ")
            .Append($"INT; {nl}")
            .Append($"interface;{nl}")
            .Append($"VAR_INPUT{nl}")
            .Append($"    a : INT; {nl}")
            .Append($"    b : INT; {nl}")
            .Append($"END_VAR{nl}")
            .Append($"BEGIN{nl}")
            .Append($"a := a + b;{nl}")
            .Append($"END_FUNCTION{nl}")
            .ToString();

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Output_FunctionBlock_With_Locals_And_Body_ST()
    {
        var xml = """
<project>
  <types>
    <pous>
      <pou name="Main" pouType="functionBlock">
        <interface>
          <localVars>
            <variable name="x">
              <type><derived name="INT" /></type>
            </variable>
          </localVars>
        </interface>
        <body>
          <ST>x := 5;</ST>
        </body>
      </pou>
    </pous>
  </types>
</project>
""";

        var result = RunExtOutput(xml, out _);
        var nl = Environment.NewLine;
        var expected = new StringBuilder()
            .Append($"FUNCTIONBLOCK Main;")
            .Append($"interface;{nl}")
            .Append($"VAR{nl}")
            .Append($"    x : INT; {nl}")
            .Append($"END_VAR{nl}")
            .Append($"BEGIN{nl}")
            .Append($"x := 5;{nl}")
            .Append($"END_FUNCTION_BLOCK{nl}")
            .ToString();

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Output_Union_Type_With_Variables()
    {
        var xml = """
<project>
  <addData>
    <data>
      <union name="MyUnion">
        <variable name="A">
          <type><derived name="INT" /></type>
        </variable>
        <variable name="B">
          <type><derived name="BOOL" /></type>
        </variable>
      </union>
    </data>
  </addData>
</project>
""";

        var result = RunExtOutput(xml, out _);
        var nl = Environment.NewLine;
        var expected = new StringBuilder()
            .Append($"TYPE MyUnion :{nl}  UNION{nl}")
            .Append($"    A : INT; {nl}")
            .Append($"    B : BOOL; {nl}")
            .Append($"  END_UNION{nl}END_TYPE{nl}")
            .ToString();

        Assert.AreEqual(expected, result);
    }
}
