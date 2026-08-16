using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic;

namespace GenFree.Helper.Tests
{
    [TestClass]
    public class DynProxyTests
    {
        private sealed class SampleTarget
        {
            public string? Name { get; set; }

            public int Count { get; set; }

            public string Describe(string prefix) => $"{prefix}:{Name}";

            public int Increment() => ++Count;
        }

        [TestMethod]
        public void TryGetMember_And_TrySetMember_ForwardValues()
        {
            // Arrange
            dynamic proxy = new DynProxy(typeof(SampleTarget));

            // Act
            proxy.Name = "Ada";
            proxy.Count = 2;

            // Assert
            Assert.AreEqual("Ada", proxy.Name);
            Assert.AreEqual(2, proxy.Count);
        }

        [TestMethod]
        public void TryInvokeMember_InvokesPublicMethod()
        {
            // Arrange
            dynamic proxy = new DynProxy(typeof(SampleTarget));
            proxy.Name = "Ada";
            proxy.Count = 1;

            // Act
            var result = proxy.Increment();

            // Assert
            Assert.AreEqual(2, proxy.Count);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TryInvokeMember_UsesMethodArguments()
        {
            // Arrange
            dynamic proxy = new DynProxy(typeof(SampleTarget));
            proxy.Name = "Ada";

            // Act
            var result = proxy.Describe("Hello");

            // Assert
            Assert.AreEqual("Hello:Ada", result);
        }

        [TestMethod]
        public void TryGetMember_OnMissingMember_ThrowsRuntimeBinderException()
        {
            // Arrange
            dynamic proxy = new DynProxy(typeof(SampleTarget));

            // Act / Assert
            Assert.ThrowsException<RuntimeBinderException>(() => _ = proxy.MissingMember);
        }

        [TestMethod]
        public void Constructor_WithNullType_ThrowsArgumentNullException()
        {
            // Act / Assert
            Assert.ThrowsException<ArgumentNullException>(() => new DynProxy(null!));
        }
    }
}
