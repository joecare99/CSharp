using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCAMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCAMS.Core.Tests
{
    [TestClass()]
    public class TDebugHelpersTests
    {
#if NET6_0_OR_GREATER
        private readonly string cExp= @".Invoke.ExecuteInternal.ExecuteWithAbortSafety.<ExecuteInternal>b__0.InvokeAsSynchronousTask.Invoke.Invoke.InvokeMethod";
#else
        private readonly string cExp= @".Execute.Invoke.ExecuteInternal.ExecuteWithAbortSafety.InvokeAsSynchronousTask.Invoke.UnsafeInvokeInternal.InvokeMethod";
#endif
        [TestMethod()]
        public void GetMethodStackTest()
        {
            Assert.AreEqual(cExp,TDebugHelpers.GetMethodStack());
        }
    }
}