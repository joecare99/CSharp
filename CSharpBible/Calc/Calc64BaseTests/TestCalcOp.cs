using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc64Base.Tests
{
    public class TestCalcOp : CalcOperation
    {
        #region Properties
        public int iExpLength { get; set; }
#if NET5_0_OR_GREATER
        public object[]? oExpData { get; set; }
        public object[]? oSetData { get; set; }
#else
        public object[] oExpData { get; set; }
        public object[] oSetData { get; set; }
#endif
        public bool xSetResult { get; set; }

        public bool SetNeedAccumulator { set => NeedAccumulator = value; }
        public bool SetNeedRegister { set => NeedRegister = value; }
        public bool SetNeedMemory { set => NeedMemory = value; }
        #endregion

        #region Methods
        public TestCalcOp() : base("?", "SomeQuest",1) { }
        public override bool Execute(ref object[] o)
        {
            Assert.AreEqual(iExpLength, o.Length);
            for (var i=0; i< o.Length;i++)
                Assert.AreEqual(oExpData?[i], o[i]);
            for (var i = 0; i < o.Length; i++)
                o[i]= oSetData?[i] ?? 0L;
            return xSetResult;
        }
        #endregion
    }
}