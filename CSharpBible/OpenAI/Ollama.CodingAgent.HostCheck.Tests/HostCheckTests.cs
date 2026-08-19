using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ollama.CodingAgent.HostCheck.Tests;

[TestClass]
public sealed class HostCheckTests
{
    [TestMethod]
    public void ArgumentReader_ReturnsValueAndRejectsMissingValue()
    {
        MethodInfo reader = Assembly.Load("Ollama.CodingAgent.HostCheck")
            .GetType("Ollama.CodingAgent.HostCheck.Program", throwOnError: true)!
            .GetMethod("ReadNextValue", BindingFlags.NonPublic | BindingFlags.Static)!;

        object[] arguments = [new string[] { "--prompt", "value" }, 0, "--prompt"];
        Assert.AreEqual("value", reader.Invoke(null, arguments));
        Assert.AreEqual(1, arguments[1]);
        TargetInvocationException exception = Assert.ThrowsExactly<TargetInvocationException>(
            () => reader.Invoke(null, [new string[] { "--prompt" }, 0, "--prompt"]));
        Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
    }
}
