using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Core.Models;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class UserDocumentFactoryImplTests
{
    [TestMethod]
    public void CreateDocument_WhenOdtRequested_ThrowsNotSupportedException()
    {
        var factory = new UserDocumentFactoryImpl();

        var exception = Assert.ThrowsExactly<NotSupportedException>(
            () => factory.CreateDocument(OFBOutputFormat.Odt));

        StringAssert.Contains(exception.Message, "ODT output is not supported");
    }

    [TestMethod]
    public void CreateDocument_WhenUnknownFormatRequested_ThrowsArgumentException()
    {
        var factory = new UserDocumentFactoryImpl();

        Assert.ThrowsExactly<ArgumentException>(
            () => factory.CreateDocument((OFBOutputFormat)int.MaxValue));
    }
}
