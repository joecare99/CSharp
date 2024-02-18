using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVVM.ViewModel;
using System;

namespace MVVM.View.Extension.Tests
{
    [TestClass()]
    public class IoCTests:BaseTestViewModel
    {
        private Func<Type, object?>? _gsOld;
        private Func<Type, object>? _grsOld;
        private object GetReqSrv(Type arg)
        {
            DoLog($"GetReqSrv({arg})");
            return this;
        }

        private object? GetSrv(Type arg)
        {
            DoLog($"GetSrv({arg})");
            return null;
        }

        [TestInitialize]
        public void Init()
        {
            _gsOld = IoC.GetSrv;
            _grsOld = IoC.GetReqSrv;
            IoC.GetSrv = GetSrv;
            IoC.GetReqSrv = GetReqSrv;
        }

        [TestCleanup]
        public void CleanUp() {
            IoC.GetSrv= _gsOld!;
            IoC.GetReqSrv= _grsOld!;
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.ThrowsException<NotImplementedException>(() => _grsOld?.Invoke(typeof(object)));
            Assert.AreEqual(null, _gsOld?.Invoke(typeof(object)));
        }

        [TestMethod()]
        public void GetRequiredServiceTest()
        {
            Assert.AreEqual(this,IoC.GetRequiredService<Object>());
            Assert.AreEqual("GetReqSrv(System.Object)\r\n", DebugLog);
        }

        [TestMethod()]
        public void GetServiceTest()
        {
            Assert.AreEqual(null, IoC.GetService<Object>());
            Assert.AreEqual("GetSrv(System.Object)\r\n", DebugLog);
        }

        [TestMethod()]
        public void ProvideValueTest()
        {
            var ioc = new IoC
            {
                Type = typeof(Object)
            };
            Assert.AreEqual(this, ioc.ProvideValue(null!));
            Assert.AreEqual("GetReqSrv(System.Object)\r\n", DebugLog);
        }
    }
}