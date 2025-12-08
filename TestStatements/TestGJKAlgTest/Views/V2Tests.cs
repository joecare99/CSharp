namespace TestGJKAlg.Views.Tests;

[TestClass()]
public class V2Tests
{
    static void Assert_AreEqual(V2 vExp, V2 vAct, string Msg = "")
    {
        Assert.AreEqual(vExp.X, vAct.X, $"{Msg}.X");
        Assert.AreEqual(vExp.Y, vAct.Y, $"{Msg}.Y");
        Assert.AreEqual(vExp.Z, vAct.Z, $"{Msg}.Z");
    }

    [DataRow(new float[]{0,0}, new float[] { 0, 0 })]
    [DataRow(new float[]{1,0}, new float[] { -1, 0 })]
    [DataRow(new float[]{0,1}, new float[] { 0, -1 })]
    public void InverseTest(float[] Act1, float[] Exp)
    {
        Assert_AreEqual(new V2(Exp[0], Exp[1]), new V2(Act1[0], Act1[1]).Inverse(), "Inverse");
    }

    [TestMethod()]
    public void V2Test()
    {
        Assert_AreEqual(new V2(0, 0), new V2(), "Zero1");
        Assert_AreEqual(new V2(0, 0), V2.Zero, "Zero2");
        Assert_AreEqual(new V2(0, 0), new V2(0,0), "Zero3");
    }

    [TestMethod()]
    [DataRow(new float[] { 0, 0 }, new float[] { 0, 0 })]
    [DataRow(new float[] { 1, 0 }, new float[] { 1, 0 })]
    [DataRow(new float[] { 0, 1 }, new float[] { 0, 1 })]
    public void V2Test1(float[] Act1, float[] Exp)
    {
        Assert_AreEqual(new V2(Exp[0], Exp[1]), new V2(Act1[0], Act1[1]), "V2Test1");
    }

    [TestMethod()]
    [DataRow(new float[] { 0, 0 }, new float[] { 0, 0 },  0 )]
    [DataRow(new float[] { 1, 0 }, new float[] { 1, 0 },  1 )]
    [DataRow(new float[] { 0, 1 }, new float[] { 1, 0 },  0 )]
    [DataRow(new float[] { 1, 0 }, new float[] { 0, 1 }, 1)]
    [DataRow(new float[] { 0, 1 }, new float[] { 0, 1 }, 0)]
    [DataRow(new float[] { 1, 1 }, new float[] { -1, 1 }, 0)]
    public void DotTest(float[] Act1, float[] Act2, float Exp)
    {
        V2 v1 = new V2(Act1[0], Act1[1]);
        V2 v2 = new V2(Act2[0], Act2[1]);
        Assert.AreEqual(Exp, V2.Dot(v1, v2), "Dot");
    }

    [TestMethod()]
    public void LengthTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void CrossTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void InverseTest1()
    {
        Assert.Fail();
    }
}